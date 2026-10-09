Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports Microsoft.Win32

''' <summary>
''' Shared look of the application: fonts, colors (light and dark), spacing, button styles and glyph icons.
''' Keep every visual constant here so forms don't repeat styling. The dark palette is applied to existing
''' controls by <see cref="ThemeApplier"/>.
''' </summary>
Public Module UiTheme

#Region "Fonts"
    Public ReadOnly BaseFont As New Font("Segoe UI", 9.0F)
    Public ReadOnly TitleFont As New Font("Segoe UI Semibold", 12.0F)
    Public ReadOnly SectionFont As New Font("Segoe UI Semibold", 9.0F)
#End Region

#Region "Light / dark"
    Private _IsDark As Boolean?

    ''' <summary>
    ''' True when the dark palette is in use: "Theme" in appsettings.json is "Dark", or it is "Auto"
    ''' (default) and Windows is set to dark mode for apps. Decided once, at startup.
    ''' </summary>
    Public ReadOnly Property IsDark() As Boolean
        Get
            If Not _IsDark.HasValue Then _IsDark = DetectDark()
            Return _IsDark.Value
        End Get
    End Property

    Private Function DetectDark() As Boolean
        Select Case AppConfiguration.Theme
            Case "dark" : Return True
            Case "light" : Return False
        End Select

        Try
            Dim oValue As Object = Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", Nothing)
            Return TypeOf oValue Is Integer AndAlso CInt(oValue) = 0
        Catch ex As Exception
            Console.WriteLine("UiTheme.DetectDark: " + ex.Message)
            Return False
        End Try
    End Function
#End Region

#Region "Colors"
    Private Const FallbackAccent As Integer = &H67C0 'RGB(0, 103, 192)

    'Status colors for text: the light-mode ones are too dark to read on the dark surface
    Public ReadOnly Property SuccessColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(108, 203, 95), Color.FromArgb(&HF, &H7B, &HF))
        End Get
    End Property
    Public ReadOnly Property ErrorColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(255, 140, 150), Color.FromArgb(&HC4, &H2B, &H1C))
        End Get
    End Property

    'Semantic palette. In light mode these are the system colors the app always used.
    ''' <summary>Window and content background.</summary>
    Public ReadOnly Property SurfaceColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(32, 32, 32), SystemColors.Window)
        End Get
    End Property
    ''' <summary>Secondary areas: status bar, disabled buttons.</summary>
    Public ReadOnly Property PanelColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(43, 43, 43), SystemColors.Control)
        End Get
    End Property
    ''' <summary>Fill of text boxes, combo boxes and secondary buttons.</summary>
    Public ReadOnly Property InputColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(45, 45, 45), SystemColors.Window)
        End Get
    End Property
    Public ReadOnly Property BorderColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(105, 105, 105), SystemColors.ControlDark)
        End Get
    End Property
    ''' <summary>Dividing lines and disabled borders.</summary>
    Public ReadOnly Property DividerColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(62, 62, 62), SystemColors.ControlLight)
        End Get
    End Property
    Public ReadOnly Property TextColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(243, 243, 243), SystemColors.ControlText)
        End Get
    End Property
    Public ReadOnly Property MutedTextColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(168, 168, 168), SystemColors.GrayText)
        End Get
    End Property
    Public ReadOnly Property HoverColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(58, 58, 58), SystemColors.ControlLight)
        End Get
    End Property
    Public ReadOnly Property PressedColor() As Color
        Get
            Return If(IsDark, Color.FromArgb(76, 76, 76), SystemColors.ControlDark)
        End Get
    End Property

    ''' <summary>Windows accent color (DWM colorization), or a blue fallback when it can't be read.</summary>
    Public ReadOnly Property AccentColor() As Color
        Get
            Try
                Dim oValue As Object = Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", Nothing)
                If TypeOf oValue Is Integer Then
                    Dim oAbgr As Integer = CInt(oValue) 'stored as 0xAABBGGRR
                    Return Color.FromArgb(oAbgr And &HFF, (oAbgr >> 8) And &HFF, (oAbgr >> 16) And &HFF)
                End If
            Catch ex As Exception
                Console.WriteLine("UiTheme.AccentColor: " + ex.Message)
            End Try
            Return Color.FromArgb(255, Color.FromArgb(FallbackAccent))
        End Get
    End Property

    ''' <summary>
    ''' The accent color, darkened as little as needed so white text on it reaches the WCAG AA
    ''' contrast ratio (4.5:1). Keeps the hue while guaranteeing readable button text.
    ''' </summary>
    Public ReadOnly Property AccentForWhiteText() As Color
        Get
            Dim oColor As Color = AccentColor
            Dim oSteps As Integer = 0
            While ContrastRatio(oColor, Color.White) < 4.5 AndAlso oSteps < 20
                oColor = Shade(oColor, -0.05)
                oSteps += 1
            End While
            Return oColor
        End Get
    End Property

    ''' <summary>
    ''' The accent color for text, links, icons and progress on top of <see cref="SurfaceColor"/>. Same as
    ''' <see cref="AccentForWhiteText"/> in light mode; lightened in dark mode until it reads well on dark.
    ''' </summary>
    Public ReadOnly Property AccentOnSurface() As Color
        Get
            If Not IsDark Then Return AccentForWhiteText

            Dim oColor As Color = AccentColor
            Dim oSteps As Integer = 0
            While ContrastRatio(oColor, SurfaceColor) < 5.5 AndAlso oSteps < 20
                oColor = Shade(oColor, 0.07)
                oSteps += 1
            End While
            Return oColor
        End Get
    End Property

    ''' <summary>Soft accent-tinted background (badges) with its readable text color.</summary>
    Public ReadOnly Property AccentTintColor() As Color
        Get
            Return If(IsDark, Blend(SurfaceColor, AccentColor, 0.3), Shade(AccentForWhiteText, 0.88))
        End Get
    End Property
    Public ReadOnly Property AccentTintTextColor() As Color
        Get
            Return If(IsDark, TextColor, AccentForWhiteText)
        End Get
    End Property

    Public Function ContrastRatio(ByVal pFirst As Color, ByVal pSecond As Color) As Double
        Dim oLight As Double = Math.Max(Luminance(pFirst), Luminance(pSecond))
        Dim oDark As Double = Math.Min(Luminance(pFirst), Luminance(pSecond))
        Return (oLight + 0.05) / (oDark + 0.05)
    End Function

    Private Function Luminance(ByVal pColor As Color) As Double
        Return 0.2126 * Linear(pColor.R) + 0.7152 * Linear(pColor.G) + 0.0722 * Linear(pColor.B)
    End Function

    Private Function Linear(ByVal pChannel As Integer) As Double
        Dim oValue As Double = pChannel / 255.0
        Return If(oValue <= 0.03928, oValue / 12.92, Math.Pow((oValue + 0.055) / 1.055, 2.4))
    End Function

    Public Function Shade(ByVal pColor As Color, ByVal pFactor As Double) As Color
        'pFactor < 0 darkens, > 0 lightens
        Dim oTarget As Integer = If(pFactor < 0, 0, 255)
        Dim oAmount As Double = Math.Abs(pFactor)
        Return Color.FromArgb(pColor.A,
                              CInt(pColor.R + (oTarget - pColor.R) * oAmount),
                              CInt(pColor.G + (oTarget - pColor.G) * oAmount),
                              CInt(pColor.B + (oTarget - pColor.B) * oAmount))
    End Function

    ''' <summary>Mixes <paramref name="pFrom"/> towards <paramref name="pTo"/> (0 = pFrom, 1 = pTo).</summary>
    Public Function Blend(ByVal pFrom As Color, ByVal pTo As Color, ByVal pAmount As Double) As Color
        Return Color.FromArgb(CInt(pFrom.R + (CInt(pTo.R) - pFrom.R) * pAmount),
                              CInt(pFrom.G + (CInt(pTo.G) - pFrom.G) * pAmount),
                              CInt(pFrom.B + (CInt(pTo.B) - pFrom.B) * pAmount))
    End Function
#End Region

#Region "Spacing"
    Public Const SpaceS As Integer = 8
    Public Const SpaceM As Integer = 12
    Public Const SpaceL As Integer = 16
    Public Const SpaceXL As Integer = 24

    ''' <summary>Scales a 96-DPI pixel value to the DPI of the monitor where <paramref name="pControl"/> is.</summary>
    Public Function Dp(ByVal pControl As Control, ByVal pPixels As Integer) As Integer
        Return CInt(Math.Round(pPixels * pControl.DeviceDpi / 96.0))
    End Function
#End Region

#Region "Buttons"
    ''' <summary>
    ''' Windows draws the text of a disabled flat button with its own gray, ignoring ForeColor, which is
    ''' nearly invisible on the dark palette. In dark mode the disabled state is painted by hand instead.
    ''' </summary>
    Private Sub HookDisabledPaint(ByVal pButton As ButtonBase)
        If IsDark Then AddHandler pButton.Paint, AddressOf DisabledButton_Paint
    End Sub

    Private Sub DisabledButton_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim oButton As ButtonBase = DirectCast(sender, ButtonBase)
        If oButton.Enabled Then Exit Sub

        Dim oRadio As RadioButton = TryCast(oButton, RadioButton)
        Dim oIsSelected As Boolean = Not oRadio Is Nothing AndAlso oRadio.Checked
        Dim oFill As Color = If(oIsSelected, oButton.FlatAppearance.CheckedBackColor, oButton.BackColor)
        Dim oTextColor As Color = If(oIsSelected, TextColor, MutedTextColor)
        Dim oFlags As TextFormatFlags = TextFormatFlags.NoPadding Or TextFormatFlags.HidePrefix Or TextFormatFlags.SingleLine

        Dim oInner As Rectangle = oButton.ClientRectangle
        oInner.Inflate(-2, -2) 'keep the 1px border the system already drew
        Using oBrush As New SolidBrush(oFill)
            e.Graphics.FillRectangle(oBrush, oInner)
        End Using

        Dim oTextSize As Size = TextRenderer.MeasureText(e.Graphics, oButton.Text, oButton.Font, Size.Empty, oFlags)
        Dim oImage As Image = oButton.Image
        Dim oImageWidth As Integer = If(oImage Is Nothing, 0, oImage.Width)
        Dim oGap As Integer = If(oImage Is Nothing, 0, Dp(oButton, 6))
        Dim oLeft As Integer = Math.Max(oInner.Left, (oButton.Width - (oImageWidth + oGap + oTextSize.Width)) \ 2)

        If Not oImage Is Nothing Then
            Dim oDim As New Imaging.ColorMatrix With {.Matrix33 = 0.5F}
            Using oAttributes As New Imaging.ImageAttributes
                oAttributes.SetColorMatrix(oDim)
                e.Graphics.DrawImage(oImage, New Rectangle(oLeft, (oButton.Height - oImage.Height) \ 2, oImage.Width, oImage.Height),
                                     0, 0, oImage.Width, oImage.Height, GraphicsUnit.Pixel, oAttributes)
            End Using
        End If

        TextRenderer.DrawText(e.Graphics, oButton.Text, oButton.Font,
                              New Point(oLeft + oImageWidth + oGap, (oButton.Height - oTextSize.Height) \ 2), oTextColor, oFlags)
    End Sub

    Public Sub StylePrimaryButton(ByVal pButton As Button)
        Dim oAccent As Color = AccentForWhiteText
        HookDisabledPaint(pButton)
        pButton.FlatStyle = FlatStyle.Flat
        pButton.UseVisualStyleBackColor = False
        pButton.ForeColor = Color.White
        pButton.Font = SectionFont
        pButton.Cursor = Cursors.Hand
        pButton.FlatAppearance.BorderSize = 1
        pButton.FlatAppearance.MouseOverBackColor = Shade(oAccent, -0.1)
        pButton.FlatAppearance.MouseDownBackColor = Shade(oAccent, -0.2)
        UpdatePrimaryButton(pButton)
        AddHandler pButton.EnabledChanged, AddressOf PrimaryButton_EnabledChanged
    End Sub
    Private Sub PrimaryButton_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs)
        UpdatePrimaryButton(DirectCast(sender, Button))
    End Sub
    Private Sub UpdatePrimaryButton(ByVal pButton As Button)
        'Disabled flat buttons draw gray text, unreadable on the accent fill: switch to a neutral fill
        Dim oBack As Color = If(pButton.Enabled, AccentForWhiteText, DividerColor)
        pButton.BackColor = oBack
        pButton.FlatAppearance.BorderColor = oBack
    End Sub

    Public Sub StyleSecondaryButton(ByVal pButton As Button)
        HookDisabledPaint(pButton)
        pButton.FlatStyle = FlatStyle.Flat
        pButton.UseVisualStyleBackColor = False
        pButton.ForeColor = TextColor
        pButton.Font = BaseFont
        pButton.Cursor = Cursors.Hand
        pButton.FlatAppearance.BorderSize = 1
        pButton.FlatAppearance.MouseOverBackColor = HoverColor
        pButton.FlatAppearance.MouseDownBackColor = PressedColor
        UpdateSecondaryButton(pButton)
        AddHandler pButton.EnabledChanged, AddressOf SecondaryButton_EnabledChanged
    End Sub
    Private Sub SecondaryButton_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs)
        UpdateSecondaryButton(DirectCast(sender, Button))
    End Sub
    Private Sub UpdateSecondaryButton(ByVal pButton As Button)
        'A disabled flat button otherwise looks almost the same as an enabled one
        pButton.BackColor = If(pButton.Enabled, InputColor, PanelColor)
        pButton.FlatAppearance.BorderColor = If(pButton.Enabled, BorderColor, DividerColor)
    End Sub

    ''' <summary>Styles a RadioButton as one segment of a segmented control (checked = accent fill).</summary>
    Public Sub StyleToggleButton(ByVal pButton As RadioButton)
        HookDisabledPaint(pButton)
        pButton.Appearance = Appearance.Button
        pButton.FlatStyle = FlatStyle.Flat
        pButton.UseVisualStyleBackColor = False
        pButton.TextAlign = ContentAlignment.MiddleCenter
        pButton.Cursor = Cursors.Hand
        pButton.FlatAppearance.BorderSize = 1
        UpdateToggleButton(pButton)
        AddHandler pButton.CheckedChanged, AddressOf ToggleButton_StateChanged
        AddHandler pButton.EnabledChanged, AddressOf ToggleButton_StateChanged
    End Sub
    Private Sub ToggleButton_StateChanged(ByVal sender As Object, ByVal e As EventArgs)
        UpdateToggleButton(DirectCast(sender, RadioButton))
    End Sub
    Private Sub UpdateToggleButton(ByVal pButton As RadioButton)
        If pButton.Enabled Then
            pButton.BackColor = InputColor
            pButton.FlatAppearance.CheckedBackColor = AccentForWhiteText
            pButton.FlatAppearance.BorderColor = BorderColor
            pButton.ForeColor = If(pButton.Checked, Color.White, TextColor)
        Else
            'Keep the selection visible but clearly inactive
            pButton.BackColor = PanelColor
            pButton.FlatAppearance.CheckedBackColor = If(IsDark, Blend(SurfaceColor, AccentForWhiteText, 0.45), Shade(AccentForWhiteText, 0.8))
            pButton.FlatAppearance.BorderColor = DividerColor
            pButton.ForeColor = If(pButton.Checked, If(IsDark, TextColor, AccentForWhiteText), MutedTextColor)
        End If
        pButton.FlatAppearance.MouseOverBackColor = If(pButton.Checked, Shade(AccentForWhiteText, -0.1), HoverColor)
        pButton.FlatAppearance.MouseDownBackColor = If(pButton.Checked, Shade(AccentForWhiteText, -0.2), PressedColor)
    End Sub

    ''' <summary>Gives a LinkLabel the accent color (readable on the current surface) in every link state.</summary>
    Public Sub StyleLink(ByVal pLink As LinkLabel)
        pLink.LinkColor = AccentOnSurface
        pLink.VisitedLinkColor = AccentOnSurface
        pLink.ActiveLinkColor = Shade(AccentOnSurface, If(IsDark, 0.25, -0.2))
        pLink.DisabledLinkColor = MutedTextColor
    End Sub

    ''' <summary>Puts a glyph icon before the text of a button, matching its text color.</summary>
    Public Sub SetGlyph(ByVal pButton As Button, ByVal pCode As Integer)
        If pButton.AutoSize Then pButton.AutoSizeMode = AutoSizeMode.GrowAndShrink 'fit the content, not the designer size
        pButton.Image = GlyphIcon(pCode, Dp(pButton, 16), pButton.ForeColor)
        pButton.ImageAlign = ContentAlignment.MiddleLeft
        pButton.TextImageRelation = TextImageRelation.ImageBeforeText
        pButton.TextAlign = ContentAlignment.MiddleCenter
    End Sub
#End Region

#Region "Glyph icons"
    ''' <summary>Segoe Fluent Icons code points (Segoe MDL2 Assets on Windows 10 uses the same ones).</summary>
    Public Class Glyphs
        Public Const Play As Integer = &HE768
        Public Const OpenFolder As Integer = &HED25
        Public Const Copy As Integer = &HE8C8
        Public Const Cancel As Integer = &HE711
        Public Const RemoteComputer As Integer = &HE839
        Public Const Computer As Integer = &HE7F4
        Public Const Theme As Integer = &HE793
    End Class

    Private _GlyphFontName As String

    Private ReadOnly Property GlyphFontName() As String
        Get
            If _GlyphFontName Is Nothing Then
                _GlyphFontName = If(IsFontInstalled("Segoe Fluent Icons"), "Segoe Fluent Icons", "Segoe MDL2 Assets")
            End If
            Return _GlyphFontName
        End Get
    End Property

    Private Function IsFontInstalled(ByVal pName As String) As Boolean
        Using oFonts As New InstalledFontCollection
            For Each oFamily As FontFamily In oFonts.Families
                If String.Equals(oFamily.Name, pName, StringComparison.OrdinalIgnoreCase) Then Return True
            Next
        End Using
        Return False
    End Function

    ''' <summary>Renders an icon-font glyph to a square bitmap of <paramref name="pSize"/> pixels.</summary>
    Public Function GlyphIcon(ByVal pCode As Integer, ByVal pSize As Integer, ByVal pColor As Color) As Bitmap
        Dim oBitmap As New Bitmap(pSize, pSize)
        Using oGraphics As Graphics = Graphics.FromImage(oBitmap),
              oFont As New Font(GlyphFontName, pSize * 0.8F, FontStyle.Regular, GraphicsUnit.Pixel),
              oBrush As New SolidBrush(pColor),
              oFormat As New StringFormat
            oGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            oFormat.Alignment = StringAlignment.Center
            oFormat.LineAlignment = StringAlignment.Center
            oGraphics.DrawString(ChrW(pCode), oFont, oBrush, New RectangleF(0, 0, pSize, pSize), oFormat)
        End Using
        Return oBitmap
    End Function
#End Region

#Region "Window"
    Private Const DWMWA_WINDOW_CORNER_PREFERENCE As Integer = 33
    Private Const DWMWCP_ROUND As Integer = 2
    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20
    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19

    Private Declare Function DwmSetWindowAttribute Lib "dwmapi.dll" (ByVal pHwnd As IntPtr, ByVal pAttribute As Integer, ByRef pValue As Integer, ByVal pSize As Integer) As Integer

    ''' <summary>
    ''' Window-level styling, to call from the form's Load: rounded corners on Windows 11 and, in dark mode,
    ''' a dark title bar plus the dark palette on every control of the form.
    ''' </summary>
    Public Sub Apply(ByVal pForm As Form)
        ApplyRoundedCorners(pForm)
        If Not IsDark Then Exit Sub

        SetDarkTitleBar(pForm)
        ThemeApplier.ApplyTo(pForm)
    End Sub

    ''' <summary>Asks Windows 11 for rounded window corners; harmless no-op on Windows 10.</summary>
    Public Sub ApplyRoundedCorners(ByVal pForm As Form)
        Try
            Dim oPreference As Integer = DWMWCP_ROUND
            DwmSetWindowAttribute(pForm.Handle, DWMWA_WINDOW_CORNER_PREFERENCE, oPreference, Marshal.SizeOf(oPreference))
        Catch ex As Exception
            Console.WriteLine("UiTheme.ApplyRoundedCorners: " + ex.Message)
        End Try
    End Sub

    Private Sub SetDarkTitleBar(ByVal pForm As Form)
        Try
            Dim oEnabled As Integer = 1
            If DwmSetWindowAttribute(pForm.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, oEnabled, Marshal.SizeOf(oEnabled)) <> 0 Then
                DwmSetWindowAttribute(pForm.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, oEnabled, Marshal.SizeOf(oEnabled))
            End If
        Catch ex As Exception
            Console.WriteLine("UiTheme.SetDarkTitleBar: " + ex.Message)
        End Try
    End Sub
#End Region

End Module
