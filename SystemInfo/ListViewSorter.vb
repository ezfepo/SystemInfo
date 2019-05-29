Imports Microsoft.VisualBasic

Public Class ListViewSorter
    Implements IComparer

    Private _ColumnNumber As Integer
    Private _SortOrder As SortOrder

    Public Sub New(ByVal pColumnNumber As Integer, ByVal pSortOrder As SortOrder)
        _ColumnNumber = pColumnNumber
        _SortOrder = pSortOrder
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim oListViewItemX As ListViewItem = CType(x, ListViewItem)
        Dim oListViewItemY As ListViewItem = CType(y, ListViewItem)

        Dim oX As String = String.Empty
        Dim oY As String = String.Empty

        If oListViewItemX.SubItems.Count > _ColumnNumber Then oX = oListViewItemX.SubItems(_ColumnNumber).Text
        If oListViewItemY.SubItems.Count > _ColumnNumber Then oY = oListViewItemY.SubItems(_ColumnNumber).Text

        If _SortOrder = SortOrder.Ascending Then
            If IsNumeric(oX) And IsNumeric(oY) Then
                Return Val(oX).CompareTo(Val(oY))
            ElseIf IsDate(oX) And IsDate(oY) Then
                Return DateTime.Parse(oX).CompareTo(DateTime.Parse(oY))
            Else
                Return String.Compare(oX, oY)
            End If
        Else
            If IsNumeric(oX) And IsNumeric(oY) Then
                Return Val(oY).CompareTo(Val(oX))
            ElseIf IsDate(oX) And IsDate(oY) Then
                Return DateTime.Parse(oY).CompareTo(DateTime.Parse(oX))
            Else
                Return String.Compare(oY, oX)
            End If
        End If
    End Function

    Public Shared Sub Sort(ByRef lvw As ListView, ByRef pSortingColumn As ColumnHeader, ByVal pClickedColumn As Integer)
        Dim oColumnHeader As ColumnHeader = lvw.Columns(pClickedColumn)
        Dim oSortOrder As SortOrder = SortOrder.Ascending

        If Not pSortingColumn Is Nothing Then
            If oColumnHeader.Equals(pSortingColumn) Then
                If pSortingColumn.Text.StartsWith("> ") Then oSortOrder = SortOrder.Descending
            End If

            pSortingColumn.Text = pSortingColumn.Text.Substring(2)
        End If

        pSortingColumn = oColumnHeader
        If oSortOrder = SortOrder.Ascending Then
            pSortingColumn.Text = "> " & pSortingColumn.Text
        Else
            pSortingColumn.Text = "< " & pSortingColumn.Text
        End If

        lvw.ListViewItemSorter = New ListViewSorter(pClickedColumn, oSortOrder)
        lvw.Sort()
    End Sub

    Public Shared Sub ClearSort(ByRef lvw As ListView, ByRef pSortingColumn As ColumnHeader)
        If Not pSortingColumn Is Nothing Then
            If pSortingColumn.Text.StartsWith("> ") Then pSortingColumn.Text = pSortingColumn.Text.Substring(2)
            If pSortingColumn.Text.StartsWith("< ") Then pSortingColumn.Text = pSortingColumn.Text.Substring(2)

            pSortingColumn = Nothing
        End If

        lvw.ListViewItemSorter = Nothing
    End Sub
End Class