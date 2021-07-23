Public Class EntryControl

    Private Entry As Entry

    Public Property HideInfo As Boolean
        Get
            Return InfoCC.Visibility = Visibility.Hidden
        End Get
        Set(value As Boolean)
            If value Then
                InfoCC.Visibility = Visibility.Hidden
                InfoCC.Width = 1
            Else
                InfoCC.Visibility = Visibility.Visible
                InfoCC.Width = Double.NaN
            End If
        End Set
    End Property

    Private Sub EntryControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If TypeOf DataContext Is Entry Then
            Entry = DataContext
            Entry.Control = Me

            AddHandler Entry.SelectedChenged, AddressOf Entry_SelectedChenged

            If Entry.Selected Then Me.BringIntoView()
        End If
    End Sub

    Private Sub Entry_SelectedChenged(sender As Object, e As EventArgs)
        Grid.GetBindingExpression(BackgroundProperty).UpdateTarget()
        NameCC.GetBindingExpression(ForegroundProperty).UpdateTarget()
        InfoCC.GetBindingExpression(ForegroundProperty).UpdateTarget()
        Me.BringIntoView()
    End Sub

End Class
