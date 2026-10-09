''' <summary>
''' Applies the dark palette of <see cref="UiTheme"/> to the controls a form already has. WinForms on .NET 8
''' has no dark mode of its own, so this recolors what the designer files set with system colors and asks
''' Windows for dark scroll bars, list headers and combo boxes. Buttons are skipped: they are styled by
''' UiTheme.StylePrimaryButton / StyleSecondaryButton / StyleToggleButton.
''' Only used in dark mode; light mode keeps the plain system look.
''' </summary>
Friend Module ThemeApplier
    Private Declare Unicode Function SetWindowTheme Lib "uxtheme.dll" (ByVal pHwnd As IntPtr, ByVal pSubAppName As String, ByVal pSubIdList As String) As Integer
    Private Declare Auto Function SendMessage Lib "user32.dll" (ByVal pHwnd As IntPtr, ByVal pMsg As Integer, ByVal pWParam As IntPtr, ByVal pLParam As IntPtr) As IntPtr

    Private Const LVM_GETHEADER As Integer = &H101F

    Friend Sub ApplyTo(ByVal pForm As Form)
        pForm.BackColor = UiTheme.SurfaceColor
        pForm.ForeColor = UiTheme.TextColor

        For Each oControl As Control In pForm.Controls
            ThemeControl(oControl)
        Next
    End Sub

    Private Sub ThemeControl(ByVal pControl As Control)
        'Parent first: children without their own color then inherit the new ambient colors
        If Not (TypeOf pControl Is ButtonBase AndAlso Not TypeOf pControl Is CheckBox) Then
            pControl.BackColor = MapBack(pControl)
            pControl.ForeColor = MapFore(pControl)
        End If

        Dim oTextBox As TextBoxBase = TryCast(pControl, TextBoxBase)
        Dim oCombo As ComboBox = TryCast(pControl, ComboBox)
        Dim oTree As TreeView = TryCast(pControl, TreeView)
        Dim oList As ListView = TryCast(pControl, ListView)
        Dim oLink As LinkLabel = TryCast(pControl, LinkLabel)
        Dim oCheck As CheckBox = TryCast(pControl, CheckBox)
        Dim oStrip As ToolStrip = TryCast(pControl, ToolStrip)

        If Not oTextBox Is Nothing Then
            oTextBox.BorderStyle = BorderStyle.FixedSingle 'the 3D border is drawn with light system colors
            If oTextBox.Multiline Then ApplyNativeTheme(pControl, "DarkMode_Explorer", False)
        ElseIf Not oCombo Is Nothing Then
            ApplyNativeTheme(pControl, "DarkMode_CFD", False)
        ElseIf Not oTree Is Nothing Then
            ApplyNativeTheme(pControl, "DarkMode_Explorer", False)
        ElseIf Not oList Is Nothing Then
            ApplyNativeTheme(pControl, "DarkMode_ItemsView", True)
            DrawListViewHeaderByHand(oList)
        ElseIf Not oLink Is Nothing Then
            UiTheme.StyleLink(oLink)
        ElseIf Not oCheck Is Nothing Then
            oCheck.FlatStyle = FlatStyle.Flat
        ElseIf Not oStrip Is Nothing Then
            ThemeToolStrip(oStrip)
        End If

        If Not pControl.ContextMenuStrip Is Nothing Then ThemeToolStrip(pControl.ContextMenuStrip)

        For Each oChild As Control In pControl.Controls
            ThemeControl(oChild)
        Next
    End Sub

    Private Function MapBack(ByVal pControl As Control) As Color
        Dim oColor As Color = pControl.BackColor
        If Not oColor.IsSystemColor Then Return oColor

        Select Case oColor.ToKnownColor()
            Case KnownColor.Window
                Return If(TypeOf pControl Is TextBoxBase OrElse TypeOf pControl Is ComboBox, UiTheme.InputColor, UiTheme.SurfaceColor)
            Case KnownColor.Control
                Return UiTheme.PanelColor
            Case KnownColor.ControlLight
                Return UiTheme.DividerColor
            Case KnownColor.ControlDark
                Return UiTheme.BorderColor
            Case Else
                Return oColor
        End Select
    End Function

    Private Function MapFore(ByVal pControl As Control) As Color
        Dim oColor As Color = pControl.ForeColor
        If Not oColor.IsSystemColor Then Return oColor

        Select Case oColor.ToKnownColor()
            Case KnownColor.ControlText, KnownColor.WindowText
                Return UiTheme.TextColor
            Case KnownColor.GrayText
                Return UiTheme.MutedTextColor
            Case Else
                Return oColor
        End Select
    End Function

#Region "Native dark theme"
    ''' <summary>
    ''' Asks Windows to draw a control's scroll bars, header and drop-down with its dark theme. Controls that
    ''' are not visible yet have no handle, so the call is repeated whenever the handle is (re)created.
    ''' </summary>
    Private Sub ApplyNativeTheme(ByVal pControl As Control, ByVal pThemeName As String, ByVal pThemeHeader As Boolean)
        AddHandler pControl.HandleCreated, Sub() SetNativeTheme(pControl, pThemeName, pThemeHeader)
        If pControl.IsHandleCreated Then SetNativeTheme(pControl, pThemeName, pThemeHeader)
    End Sub

    Private Sub SetNativeTheme(ByVal pControl As Control, ByVal pThemeName As String, ByVal pThemeHeader As Boolean)
        Try
            SetWindowTheme(pControl.Handle, pThemeName, Nothing)

            If pThemeHeader Then
                Dim oHeader As IntPtr = SendMessage(pControl.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero)
                If oHeader <> IntPtr.Zero Then SetWindowTheme(oHeader, "DarkMode_ItemsView", Nothing)
            End If
        Catch ex As Exception
            Console.WriteLine("ThemeApplier.SetNativeTheme: " + ex.Message)
        End Try
    End Sub
#End Region

#Region "List view header"
    ''' <summary>
    ''' The dark header theme keeps its text dark, so the header is drawn by hand. Items and sub-items are
    ''' still drawn by the control (DrawDefault), so selection, groups and sorting behave as before.
    ''' </summary>
    Private Sub DrawListViewHeaderByHand(ByVal pList As ListView)
        pList.OwnerDraw = True
        AddHandler pList.DrawColumnHeader, AddressOf ListView_DrawColumnHeader
        AddHandler pList.DrawItem, Sub(sender, e) e.DrawDefault = True
        AddHandler pList.DrawSubItem, Sub(sender, e) e.DrawDefault = True

        'Owner-drawn list views flicker on resize unless double buffered (the property is protected)
        GetType(Control).GetProperty("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance).SetValue(pList, True)
    End Sub

    Private Sub ListView_DrawColumnHeader(ByVal sender As Object, ByVal e As DrawListViewColumnHeaderEventArgs)
        Using oBack As New SolidBrush(UiTheme.PanelColor), oLine As New Pen(UiTheme.DividerColor)
            e.Graphics.FillRectangle(oBack, e.Bounds)
            e.Graphics.DrawLine(oLine, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1)
            e.Graphics.DrawLine(oLine, e.Bounds.Right - 1, e.Bounds.Top + 4, e.Bounds.Right - 1, e.Bounds.Bottom - 5)
        End Using
        TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                              New Rectangle(e.Bounds.X + 6, e.Bounds.Y, Math.Max(0, e.Bounds.Width - 12), e.Bounds.Height),
                              UiTheme.TextColor, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)
    End Sub
#End Region

#Region "Tool strips, menus and tool tips"
    Private Sub ThemeToolStrip(ByVal pStrip As ToolStrip)
        pStrip.Renderer = New ToolStripProfessionalRenderer(New DarkColorTable) With {.RoundedEdges = False}
        pStrip.BackColor = UiTheme.PanelColor
        pStrip.ForeColor = UiTheme.TextColor
        For Each oItem As ToolStripItem In pStrip.Items
            ThemeToolStripItem(oItem)
        Next
    End Sub

    Private Sub ThemeToolStripItem(ByVal pItem As ToolStripItem)
        pItem.ForeColor = UiTheme.TextColor

        Dim oDropDownItem As ToolStripDropDownItem = TryCast(pItem, ToolStripDropDownItem)
        If oDropDownItem Is Nothing Then Exit Sub
        For Each oChild As ToolStripItem In oDropDownItem.DropDownItems
            ThemeToolStripItem(oChild)
        Next
    End Sub

    ''' <summary>Owner-draws a ToolTip with the dark palette (the system tool tip stays light).</summary>
    Friend Sub ThemeToolTip(ByVal pToolTip As ToolTip)
        If Not UiTheme.IsDark Then Exit Sub

        pToolTip.OwnerDraw = True
        AddHandler pToolTip.Popup, AddressOf ToolTip_Popup
        AddHandler pToolTip.Draw, AddressOf ToolTip_Draw
    End Sub

    Private Sub ToolTip_Popup(ByVal sender As Object, ByVal e As PopupEventArgs)
        Dim oText As String = DirectCast(sender, ToolTip).GetToolTip(e.AssociatedControl)
        Dim oSize As Size = TextRenderer.MeasureText(oText, UiTheme.BaseFont)
        e.ToolTipSize = New Size(oSize.Width + 16, oSize.Height + 10)
    End Sub

    Private Sub ToolTip_Draw(ByVal sender As Object, ByVal e As DrawToolTipEventArgs)
        Using oBack As New SolidBrush(UiTheme.PanelColor), oBorder As New Pen(UiTheme.BorderColor)
            e.Graphics.FillRectangle(oBack, e.Bounds)
            e.Graphics.DrawRectangle(oBorder, New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1))
        End Using
        TextRenderer.DrawText(e.Graphics, e.ToolTipText, UiTheme.BaseFont,
                              New Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 16, e.Bounds.Height),
                              UiTheme.TextColor, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Class DarkColorTable
        Inherits ProfessionalColorTable

        Public Overrides ReadOnly Property ToolStripDropDownBackground() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientBegin() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientMiddle() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientEnd() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuBorder() As Color
            Get
                Return UiTheme.BorderColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemBorder() As Color
            Get
                Return UiTheme.HoverColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelected() As Color
            Get
                Return UiTheme.HoverColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelectedGradientBegin() As Color
            Get
                Return UiTheme.HoverColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelectedGradientEnd() As Color
            Get
                Return UiTheme.HoverColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemPressedGradientBegin() As Color
            Get
                Return UiTheme.PressedColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemPressedGradientEnd() As Color
            Get
                Return UiTheme.PressedColor
            End Get
        End Property
        Public Overrides ReadOnly Property SeparatorDark() As Color
            Get
                Return UiTheme.DividerColor
            End Get
        End Property
        Public Overrides ReadOnly Property SeparatorLight() As Color
            Get
                Return UiTheme.DividerColor
            End Get
        End Property
        Public Overrides ReadOnly Property StatusStripGradientBegin() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
        Public Overrides ReadOnly Property StatusStripGradientEnd() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripBorder() As Color
            Get
                Return UiTheme.PanelColor
            End Get
        End Property
    End Class
#End Region

End Module
