Imports System.Net
Imports System.Threading

''' <summary>
''' Looks for computers on the local subnets and lets the user pick one to scan.
''' The search starts when the dialog opens and stops when it closes.
''' </summary>
Public Class frmNetworkScan
    Private _Cancel As CancellationTokenSource
    Private _Searching As Boolean

    ''' <summary>The computer picked (name, or IP address when it has no name); valid after the dialog returns OK.</summary>
    Public ReadOnly Property SelectedHost As String = String.Empty

    Private Sub frmNetworkScan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        UiTheme.Apply(Me)
        UiTheme.StylePrimaryButton(btnSelect)
        UiTheme.StyleSecondaryButton(btnSearch)
        UiTheme.StyleSecondaryButton(btnCancel)
    End Sub

    Private Sub frmNetworkScan_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Shown
        StartSearch()
    End Sub

    Private Async Sub StartSearch()
        If _Searching Then
            _Cancel.Cancel() 'the button reads "Stop" while searching
            Exit Sub
        End If

        lvwHosts.Items.Clear()
        btnSelect.Enabled = False

        Dim oDescription As String = String.Empty
        Dim oAddresses As List(Of IPAddress)
        Try
            oAddresses = NetworkDiscovery.GetCandidates(oDescription)
        Catch ex As Exception
            lblStatus.Text = "Could not read the network adapters: " + ex.Message
            lblStatus.ForeColor = UiTheme.ErrorColor
            Exit Sub
        End Try

        If oAddresses.Count = 0 Then
            lblStatus.Text = "This computer is not connected to a network."
            lblStatus.ForeColor = UiTheme.ErrorColor
            Exit Sub
        End If

        _Cancel = New CancellationTokenSource
        Dim oToken As CancellationToken = _Cancel.Token
        Dim oTotal As Integer = oAddresses.Count
        Dim oChecked As Integer = 0

        'Progress(Of T) created here posts back to the UI thread
        Dim oFound As New Progress(Of DiscoveredHost)(Sub(pHost) AddHost(pHost))
        Dim oProgress As New Progress(Of Integer)(Sub(pDone)
                                                      oChecked = Math.Max(oChecked, pDone)
                                                      ShowProgress(oDescription, oChecked, oTotal)
                                                  End Sub)

        SetSearching(True)
        ShowProgress(oDescription, 0, oTotal)
        Try
            Await NetworkDiscovery.ScanAsync(oAddresses, oFound, oProgress, oToken)
        Catch ex As OperationCanceledException
            'Stopped by the user: keep what was found
        Catch ex As Exception
            lblStatus.Text = "The search failed: " + ex.Message
            lblStatus.ForeColor = UiTheme.ErrorColor
            SetSearching(False)
            Exit Sub
        End Try

        If IsDisposed Then Exit Sub
        SetSearching(False)
        lblStatus.ForeColor = UiTheme.MutedTextColor
        lblStatus.Text = If(oToken.IsCancellationRequested, "Search stopped. ", "Search finished. ") +
            CountText() + " on " + oDescription + ". Computers that block TCP port 135 (Windows firewall) don't appear: they can't be scanned remotely either."
    End Sub

    Private Sub AddHost(ByVal pHost As DiscoveredHost)
        If IsDisposed Then Exit Sub
        Dim oItem As New ListViewItem(If(pHost.Name.Length > 0, pHost.Name, "(no name)"))
        oItem.SubItems.Add(pHost.Address)
        oItem.Tag = pHost
        lvwHosts.Items.Add(oItem)
        lvwHosts.Sort()
    End Sub

    Private Sub ShowProgress(ByVal pDescription As String, ByVal pChecked As Integer, ByVal pTotal As Integer)
        If IsDisposed OrElse Not _Searching Then Exit Sub
        lblStatus.ForeColor = UiTheme.MutedTextColor
        lblStatus.Text = "Searching " + pDescription + ChrW(8230) + " " + pChecked.ToString() + " of " + pTotal.ToString() + " addresses checked, " + CountText()
    End Sub

    Private Function CountText() As String
        Dim oCount As Integer = lvwHosts.Items.Count
        Return oCount.ToString() + If(oCount = 1, " computer found", " computers found")
    End Function

    Private Sub SetSearching(ByVal pSearching As Boolean)
        _Searching = pSearching
        btnSearch.Text = If(pSearching, "&Stop", "Search &again")
        Me.UseWaitCursor = False
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        StartSearch()
    End Sub

    Private Sub lvwHosts_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lvwHosts.SelectedIndexChanged
        btnSelect.Enabled = lvwHosts.SelectedItems.Count > 0
    End Sub

    Private Sub lvwHosts_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles lvwHosts.DoubleClick
        If lvwHosts.SelectedItems.Count > 0 Then btnSelect.PerformClick()
    End Sub

    Private Sub btnSelect_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelect.Click
        If lvwHosts.SelectedItems.Count = 0 Then Exit Sub
        _SelectedHost = DirectCast(lvwHosts.SelectedItems(0).Tag, DiscoveredHost).Host
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub frmNetworkScan_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _Cancel Is Nothing Then _Cancel.Cancel()
    End Sub
End Class
