Imports System.IO
Imports System.Windows.Forms

Public Class ConsoleRedir
    Inherits StringWriter

    Private _RichTextBox As RichTextBox

    <System.Diagnostics.DebuggerStepThroughAttribute()> _
    Public Sub New(ByRef pRichTextBox As RichTextBox)
        _RichTextBox = pRichTextBox
        Control.CheckForIllegalCrossThreadCalls = False
        Console.SetOut(Me)
    End Sub

    <System.Diagnostics.DebuggerStepThroughAttribute()> _
    Public Overrides Sub Write(ByVal value As String)
        _RichTextBox.AppendText(value)
        _RichTextBox.Refresh()
    End Sub

    <System.Diagnostics.DebuggerStepThroughAttribute()> _
    Public Overrides Sub WriteLine(ByVal value As String)
        Me.Write(value + Environment.NewLine)
    End Sub
End Class

