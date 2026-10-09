#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Prepares this computer to be scanned remotely by SystemInfo (WMI over DCOM, workgroup / no domain).

.DESCRIPTION
    Run once, in PowerShell as administrator, ON THE COMPUTER THAT WILL BE SCANNED.
      1. Optionally creates a local administrator account (default name: systeminfo) to scan with.
         This is the most reliable option: Microsoft accounts that sign in with a PIN often fail
         network logons. The account is hidden from the sign-in screen.
      2. Enables the built-in "Windows Management Instrumentation (WMI)" inbound firewall rules.
      3. Sets LocalAccountTokenFilterPolicy = 1 so a remote local administrator gets a full
         (non-filtered) token. Without it, UAC makes remote WMI fail with "Access denied".
         Note: this also allows remote administrative access with local accounts over other
         protocols (for example SMB admin shares). Only do this on networks you trust.

    The built-in rules only accept connections from the local subnet, so the computer running
    SystemInfo must be on the same subnet as this one.

    Use -Undo to revert everything, including the account.

.PARAMETER CreateUser
    Create the local administrator account (or reset its password if it already exists) without asking.

.PARAMETER UserName
    Name of the local account to create or remove. Default: systeminfo.

.PARAMETER Undo
    Disable the firewall rules, remove LocalAccountTokenFilterPolicy and delete the account.

.EXAMPLE
    .\enable-remote-wmi.ps1
    Applies the settings and asks whether to create the 'systeminfo' account.
.EXAMPLE
    .\enable-remote-wmi.ps1 -CreateUser
.EXAMPLE
    .\enable-remote-wmi.ps1 -Undo
#>
param(
    [switch]$CreateUser,
    [string]$UserName = 'systeminfo',
    [switch]$Undo
)

$ErrorActionPreference = 'Stop'

# Rule names (not display names) and the group SID, so this works on any Windows display language
$wmiRules = 'WMI-RPCSS-In-TCP', 'WMI-WINMGMT-In-TCP', 'WMI-ASYNC-In-TCP'
$policyPath = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System'
$policyName = 'LocalAccountTokenFilterPolicy'
$hiddenUsersPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList'
$administratorsSid = 'S-1-5-32-544'

if ($Undo) {
    Disable-NetFirewallRule -Name $wmiRules
    Write-Host 'Firewall: WMI inbound rules disabled.'

    Remove-ItemProperty -Path $policyPath -Name $policyName -ErrorAction SilentlyContinue
    Write-Host "Registry: $policyName removed."

    if (Get-LocalUser -Name $UserName -ErrorAction SilentlyContinue) {
        Remove-LocalUser -Name $UserName
        Write-Host "Account '$UserName' removed."
    }
    Remove-ItemProperty -Path $hiddenUsersPath -Name $UserName -ErrorAction SilentlyContinue

    Write-Host 'Remote WMI access for SystemInfo has been removed from this computer.'
    return
}

# 1) Local administrator account
$create = [bool]$CreateUser
if (-not $create) {
    $answer = Read-Host "Create a local administrator account '$UserName' for SystemInfo? (recommended) [Y/n]"
    $create = $answer -eq '' -or $answer -match '^(y|yes|s|si)$'
}

if ($create) {
    $password = Read-Host -AsSecureString "Password for '$UserName'"
    $confirm = Read-Host -AsSecureString 'Confirm password'
    $plainPassword = [Net.NetworkCredential]::new('', $password).Password
    if ($plainPassword.Length -eq 0) { throw 'The password cannot be empty.' }
    if ($plainPassword -ne [Net.NetworkCredential]::new('', $confirm).Password) { throw 'The passwords do not match.' }
    $plainPassword = $null

    if (Get-LocalUser -Name $UserName -ErrorAction SilentlyContinue) {
        Set-LocalUser -Name $UserName -Password $password -PasswordNeverExpires $true
        Write-Host "Account '$UserName' already existed: password updated."
    }
    else {
        New-LocalUser -Name $UserName -Password $password -PasswordNeverExpires -AccountNeverExpires `
            -Description 'Used by SystemInfo to scan this computer remotely' | Out-Null
        Write-Host "Account '$UserName' created."
    }

    try {
        Add-LocalGroupMember -SID $administratorsSid -Member $UserName -ErrorAction Stop
        Write-Host "Account '$UserName' added to the Administrators group."
    }
    catch {
        if ($_.FullyQualifiedErrorId -notlike 'MemberExists*') { throw }
    }

    # Keep the account off the sign-in screen
    New-Item -Path $hiddenUsersPath -Force | Out-Null
    New-ItemProperty -Path $hiddenUsersPath -Name $UserName -Value 0 -PropertyType DWord -Force | Out-Null
}

# 2) Firewall
Enable-NetFirewallRule -Name $wmiRules
Write-Host 'Firewall: WMI inbound rules enabled.'

# 3) Full token for remote local administrators
New-ItemProperty -Path $policyPath -Name $policyName -Value 1 -PropertyType DWord -Force | Out-Null
Write-Host "Registry: $policyName = 1."

$user = if ($create) { $UserName } else { 'a local administrator account' }
Write-Host "Done. In SystemInfo, scan '$env:COMPUTERNAME' with $user and its password."
