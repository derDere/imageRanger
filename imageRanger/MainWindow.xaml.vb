Class MainWindow

    Public Property BackgroundBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.Background)
        End Get
        Set(value As Brush)
        End Set
    End Property

    Public Property ForegroundBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.Foreground)
        End Get
        Set(value As Brush)
        End Set
    End Property

    Public Property LineBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.LightBlack)
        End Get
        Set(value As Brush)
        End Set
    End Property

    Public Property MashineDataBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.Green)
        End Get
        Set(value As Brush)
        End Set
    End Property

    Public Property CurrentPathBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.Blue)
        End Get
        Set(value As Brush)
        End Set
    End Property

    Public Property CurrentEntryBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.LightAsh)
        End Get
        Set(value As Brush)
        End Set
    End Property

    Public Property MashineData As String
        Get
            Return $"{Environment.UserName}@{Environment.MachineName}:"
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property CurrentPath As String
        Get
            Dim path As String = "" & CurrentDir?.Path
            If Not path.EndsWith("\") Then path &= "\"
            Return path
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property CurrentEntryName As String
        Get
            If Index < 0 Or Entries.Length <= 0 Then Return ""
            Return Entries(Index).DisplayName
        End Get
        Set(value As String)
        End Set
    End Property

    'Private _Entries As New List(Of Entry)
    Public Property Entries() As Entry()
        Get
            If CurrentDir Is Nothing Then Return {}
            Return CurrentDir.ChildEntries
            'Return _Entries.ToArray
        End Get
        Set(ByVal value As Entry())
            '_Entries = value.ToList
        End Set
    End Property

    Private _ParentEntries As Entry()
    Public Property ParentEntries() As Entry()
        Get
            Return _ParentEntries
        End Get
        Set(ByVal value As Entry())
            _ParentEntries = value
        End Set
    End Property

    Public Property ChildEntries() As Entry()
        Get
            If Index < 0 Or Entries.Length <= 0 Then Return {}
            If Entries(Index).IsDir Then
                Return Entries(Index).ChildEntries
            End If
            Return {}
        End Get
        Set(ByVal value As Entry())
        End Set
    End Property

    Public Property CurrentDir As Entry

    Private _Index As Integer = -1
    Public Property Index As Integer
        Get
            Return _Index
        End Get
        Set(value As Integer)
            If value < 0 Then value = 0
            If value >= Entries.Length Then value = Entries.Length - 1
            If value <> _Index Then
                If _Index >= 0 Then _
                    Entries(_Index).Selected = False
                _Index = value
                Entries(_Index).Selected = True
                ShowSelectionContent()
            End If
        End Set
    End Property

    Private Sub AddDirEntryToList(DI As IO.DirectoryInfo, L As List(Of Entry), loadChildren As Boolean)
        If CurrentDir Is Nothing Then Exit Sub

        Dim entry As Entry
        If CurrentDir.IsPath(DI.FullName) Then
            entry = CurrentDir
            entry.Selected = True
            entry.ChildEntries = LoadEntries(entry.DirInfo, entry, True)
        Else
            entry = New Entry(DI)
            If loadChildren Then
                entry.ChildEntries = LoadEntries(entry.DirInfo, entry, False)
            End If
        End If

        L.Add(entry)
    End Sub

    Private Function LoadEntries(rootDir As IO.DirectoryInfo, parent As Entry, loadChildren As Boolean) As Entry()
        Dim L As New List(Of Entry)
        Try
            If rootDir Is Nothing And CurrentDir Is Nothing Then
                CurrentDir = New Entry()
                CurrentDir.ChildEntries = LoadEntries(Nothing, Nothing, True)
            ElseIf rootDir Is Nothing Then
                For Each drive As IO.DriveInfo In IO.DriveInfo.GetDrives()
                    If drive.IsReady Then
                        AddDirEntryToList(drive.RootDirectory, L, loadChildren)
                    End If
                Next
            Else
                For Each dir As String In IO.Directory.GetDirectories(rootDir.FullName)
                    If dir = "." Or dir = ".." Then Continue For
                    Dim DI As New IO.DirectoryInfo(dir)
                    AddDirEntryToList(DI, L, loadChildren)
                Next
                For Each file As String In IO.Directory.GetFiles(rootDir.FullName)
                    Dim FI As New IO.FileInfo(file)
                    L.Add(New Entry(FI))
                Next
            End If
        Catch ex As Exception
            If Debugger.IsAttached Then
                Console.WriteLine(ex.ToString())
            End If
        End Try
        L = L.OrderBy(Function(e) e.Order).ToList
        For i = 0 To L.Count - 1
            L(i).Index = i
        Next
        Return L.ToArray
    End Function

    Private NewIndex As Integer = 0

    Private Sub LoadAll()
        Dim openDI As IO.DirectoryInfo = Nothing
        If CurrentDir IsNot Nothing Then openDI = New IO.DirectoryInfo(CurrentDir.Path)
        Dim parentDI As IO.DirectoryInfo = openDI?.Parent
        ParentEntries = LoadEntries(parentDI, Nothing, False)
        _Index = -1
        Index = NewIndex
        LeftList.GetBindingExpression(DataContextProperty).UpdateTarget()
        MiddleList.GetBindingExpression(DataContextProperty).UpdateTarget()
        MashineRun.GetBindingExpression(Run.TextProperty).UpdateTarget()
        CurrentPathRun.GetBindingExpression(Run.TextProperty).UpdateTarget()
        CurrentEntryRun.GetBindingExpression(Run.TextProperty).UpdateTarget()
        RightList.GetBindingExpression(DataContextProperty).UpdateTarget()
    End Sub

    Public Shared ReadOnly IMG_FORMATS As String() = {
        ".ANI", ".CLP", ".CMP", ".CMW", ".CUR", ".DIC", ".EPS", ".EXIF", ".FLC", ".GIF", ".HDP", ".ICO", ".IFF", ".JPG", ".JP2", ".MRC", ".PBM", ".PCX", ".PNG", ".PSD", ".RAS", ".SGI", ".TGA", ".WMF", ".WPG", ".XPM", ".XWD", ".ABC", ".CAL", ".CMP", ".ICA", ".IMG", ".ITG", ".JB2", ".MAC", ".XPS", ".MNG", ".MSP", ".SMP", ".TIFF", ".EMF", ".XBM"
    }

    Private Sub ShowSelectionContent()
        Dim entry As Entry = Entries(Index)

        PreviewImg.Visibility = Visibility.Collapsed
        RightList.Visibility = Visibility.Collapsed
        TextView.Visibility = Visibility.Collapsed

        If entry.IsDir Then
            RightList.Visibility = Visibility.Visible
        Else
            Dim Binary As Boolean = Configuration.BinaryFiles.Contains(entry.FileInfo.Extension.ToUpper)
            Dim Img As Boolean = IMG_FORMATS.Contains(entry.FileInfo.Extension.ToUpper)
            Dim ForcedText As Boolean = Configuration.ForcedTextFiles.Contains(entry.FileInfo.Extension.ToUpper)

            If Img And Not ForcedText Then
                PreviewImg.Visibility = Visibility.Visible
                PreviewImg.Source = New BitmapImage(New Uri(entry.Path))
            ElseIf Binary And Not ForcedText Then
            Else
                TextView.Visibility = Visibility.Visible
                TextView.Text = IO.File.ReadAllText(entry.Path)
            End If
        End If

        RightList.GetBindingExpression(DataContextProperty).UpdateTarget()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        CurrentDir = New Entry(New IO.DirectoryInfo("../../../../../.."))
        LoadAll()
    End Sub

    Private Sub InputTxb_KeyDown(sender As Object, e As KeyEventArgs) Handles InputTxb.KeyDown, InputTxb.PreviewKeyDown
        e.Handled = True
        Select Case e.Key
            Case Key.Up, Key.K
                Index -= 1

            Case Key.Down, Key.J
                Index += 1

            Case Key.Left, Key.H
                If ParentEntries.Length > 0 Then
                    NewIndex = CurrentDir.Index
                    CurrentDir = CurrentDir.CreateParent
                    LoadAll()
                End If

            Case Key.Right, Key.L
                If Entries(Index).IsDir Then
                    NewIndex = 0
                    CurrentDir = Entries(Index)
                    LoadAll()
                End If

            Case Key.Q
                Me.Close()
            Case Else

        End Select
        'MiddleList.GetBindingExpression(DataContextProperty).UpdateTarget()
        CurrentEntryRun.GetBindingExpression(Run.TextProperty).UpdateTarget()
        InputTxb.Text = ""
    End Sub

    Private Sub MainWindow_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        InputTxb.Focus()
    End Sub

    Private Sub InputTxb_LostFocus(sender As Object, e As RoutedEventArgs) Handles InputTxb.LostFocus
        e.Handled = True
        InputTxb.Focus()
    End Sub

End Class
