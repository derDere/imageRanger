Class MainWindow

    Private Property BackgroundBrush As Brush
        Get
            Return New SolidColorBrush(Configuration.Theme.Background)
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
            Return "LAL\ALA\"
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property CurrentEntryName As String
        Get
            Return "ENTRY"
        End Get
        Set(value As String)
        End Set
    End Property

    Private _Entries As Entry() = {}
    Public Property Entries() As Entry()
        Get
            Return _Entries
        End Get
        Set(ByVal value As Entry())
            _Entries = value
        End Set
    End Property

    Public Shared Function LoadEntries(rootDir As String) As Entry()
        Dim L As New List(Of Entry)
        For Each dir As String In IO.Directory.GetDirectories(rootDir)
            Dim DI As New IO.DirectoryInfo(dir)
            L.Add(New Entry(DI))
        Next
        For Each file As String In IO.Directory.GetFiles(rootDir)
            Dim FI As New IO.FileInfo(file)
            L.Add(New Entry(FI))
        Next
        L = L.OrderBy(Function(e) e.Order).ToList
        For index = 0 To L.Count - 1
            L(index).Index = index
        Next
        Return L.ToArray
    End Function

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Entries = LoadEntries("../../../../../..")
        MiddleList.GetBindingExpression(DataContextProperty).UpdateTarget()
    End Sub

End Class
