Imports System.Drawing.Drawing2D

''' <summary>
''' Thin rounded progress bar in the accent color. Moves smoothly towards each new value and has an
''' indeterminate (sliding segment) mode for steps of unknown length, such as connecting. While
''' <see cref="Active"/>, a faint segment slides over the unfilled part so long steps don't look frozen.
''' </summary>
Public Class AccentProgressBar
    Inherits Control

    Private Const FrameInterval As Integer = 15
    Private Const EaseFactor As Double = 0.12
    Private Const MarqueeWidth As Double = 0.3
    Private Const MarqueeStep As Double = 0.012

    Private _Value As Double
    Private _DisplayedValue As Double
    Private _Indeterminate As Boolean
    Private _Active As Boolean
    Private _MarqueePosition As Double = -MarqueeWidth
    Private WithEvents _Timer As New Timer With {.Interval = FrameInterval}

    Public Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.SupportsTransparentBackColor, True)
        SetStyle(ControlStyles.Selectable, False)
        BackColor = Color.Transparent
        Size = New Size(200, 8)
    End Sub

    ''' <summary>Progress, 0-100.</summary>
    Public Property Value As Double
        Get
            Return _Value
        End Get
        Set(ByVal value As Double)
            _Value = Math.Max(0, Math.Min(100, value))
            If value < _DisplayedValue Then _DisplayedValue = _Value 'never animate backwards
            _Timer.Start()
        End Set
    End Property

    Public Property Indeterminate As Boolean
        Get
            Return _Indeterminate
        End Get
        Set(ByVal value As Boolean)
            _Indeterminate = value
            _MarqueePosition = -MarqueeWidth
            _Timer.Start()
            Invalidate()
        End Set
    End Property

    ''' <summary>Shows a faint sliding segment over the unfilled part of the bar (determinate mode only).</summary>
    Public Property Active As Boolean
        Get
            Return _Active
        End Get
        Set(ByVal value As Boolean)
            _Active = value
            If value Then _Timer.Start()
            Invalidate()
        End Set
    End Property

    ''' <summary>Jumps to 0 without animating.</summary>
    Public Sub Reset()
        _Value = 0
        _DisplayedValue = 0
        Invalidate()
    End Sub

    Private Sub _Timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles _Timer.Tick
        If _Indeterminate OrElse _Active Then
            _MarqueePosition += MarqueeStep
            If _MarqueePosition > 1 Then _MarqueePosition = -MarqueeWidth
        End If
        If Not _Indeterminate Then
            _DisplayedValue += (_Value - _DisplayedValue) * EaseFactor
            If Math.Abs(_Value - _DisplayedValue) < 0.05 Then
                _DisplayedValue = _Value
                If Not _Active Then _Timer.Stop()
            End If
        End If
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim oTrack As New RectangleF(0, 0, Width - 1, Height - 1)

        Using oTrackBrush As New SolidBrush(If(UiTheme.IsDark, UiTheme.PressedColor, UiTheme.Shade(UiTheme.AccentForWhiteText, 0.85))),
              oTrackPath As GraphicsPath = RoundedRectangle(oTrack)
            e.Graphics.FillPath(oTrackBrush, oTrackPath)
        End Using

        If _Indeterminate Then
            FillSegment(e.Graphics, Marquee(oTrack), UiTheme.AccentOnSurface)
            Exit Sub
        End If

        Dim oFilled As New RectangleF(0, 0, CSng(oTrack.Width * _DisplayedValue / 100), oTrack.Height)
        If _Active Then
            'Sliding hint only over the part still to do
            Dim oRemaining As New RectangleF(oFilled.Right, 0, oTrack.Right - oFilled.Right, oTrack.Height)
            Dim oHint As RectangleF = RectangleF.Intersect(oRemaining, Marquee(oTrack))
            FillSegment(e.Graphics, oHint, Color.FromArgb(110, UiTheme.AccentOnSurface))
        End If
        FillSegment(e.Graphics, oFilled, UiTheme.AccentOnSurface)
    End Sub

    Private Function Marquee(ByVal pTrack As RectangleF) As RectangleF
        Dim oLeft As Single = CSng(_MarqueePosition * pTrack.Width)
        Return RectangleF.Intersect(pTrack, New RectangleF(oLeft, 0, CSng(MarqueeWidth * pTrack.Width), pTrack.Height))
    End Function

    Private Shared Sub FillSegment(ByVal pGraphics As Graphics, ByVal pBounds As RectangleF, ByVal pColor As Color)
        If pBounds.Width < 1 Then Exit Sub
        pBounds.Width = Math.Max(pBounds.Width, pBounds.Height) 'keep the rounded ends round
        Using oBrush As New SolidBrush(pColor),
              oPath As GraphicsPath = RoundedRectangle(pBounds)
            pGraphics.FillPath(oBrush, oPath)
        End Using
    End Sub

    Private Shared Function RoundedRectangle(ByVal pBounds As RectangleF) As GraphicsPath
        Dim oDiameter As Single = Math.Min(pBounds.Height, pBounds.Width)
        Dim oPath As New GraphicsPath
        oPath.AddArc(pBounds.X, pBounds.Y, oDiameter, oDiameter, 90, 180)
        oPath.AddArc(pBounds.Right - oDiameter, pBounds.Y, oDiameter, oDiameter, 270, 180)
        oPath.CloseFigure()
        Return oPath
    End Function

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then _Timer.Dispose()
        MyBase.Dispose(disposing)
    End Sub
End Class
