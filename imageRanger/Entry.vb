Public Class Entry

    Public Event SelectedChenged(ByVal sender As Object, ByVal e As EventArgs)

    Public Property Color() As Brush
        Get
            If Selected Then
                Return New SolidColorBrush(Configuration.Theme.Background)
            Else
                If IsLink Then
                    If IsMissingLink Then
                        Return New SolidColorBrush(Configuration.Theme.Red)
                    Else
                        Return New SolidColorBrush(Configuration.Theme.Cyan)
                    End If
                ElseIf IsDir Then
                    Return New SolidColorBrush(Configuration.Theme.Blue)
                ElseIf IsExe Then
                    Return New SolidColorBrush(Configuration.Theme.Green)
                Else
                    Return New SolidColorBrush(Configuration.Theme.Foreground)
                End If
            End If
        End Get
        Set(ByVal value As Brush)
        End Set
    End Property

    Public Property BgColor() As Brush
        Get
            If Selected Then
                If IsLink Then
                    If IsMissingLink Then
                        Return New SolidColorBrush(Configuration.Theme.Red)
                    Else
                        Return New SolidColorBrush(Configuration.Theme.Cyan)
                    End If
                ElseIf IsDir Then
                    Return New SolidColorBrush(Configuration.Theme.Blue)
                ElseIf IsExe Then
                    Return New SolidColorBrush(Configuration.Theme.Green)
                Else
                    Return New SolidColorBrush(Configuration.Theme.Foreground)
                End If
            Else
                Return New SolidColorBrush(Configuration.Theme.Background)
            End If
        End Get
        Set(ByVal value As Brush)
        End Set
    End Property

    Public Property Control As EntryControl
    Public Property IsDir As Boolean = False
    Public Property IsLink As Boolean = False
    Public Property IsMissingLink As Boolean = False
    Public Property IsExe As Boolean = False
    Public Property Index As Integer = -1

    Private _Selected As Boolean = False
    Public Property Selected As Boolean
        Get
            Return _Selected
        End Get
        Set(value As Boolean)
            If value <> _Selected Then
                _Selected = value
                RaiseEvent SelectedChenged(Me, New EventArgs)
            End If
        End Set
    End Property

    Public ReadOnly Property Order As String
        Get
            If IsDir Then
                Return "1 " & DisplayName
            Else
                Return "2 " & DisplayName
            End If
        End Get
    End Property

    Friend FileInfo As IO.FileInfo
    Friend DirInfo As IO.DirectoryInfo
    Friend LnkInfo As LnkReader.Lnk
    Friend LinkLocation As IO.FileInfo
    Friend Attributes As FileAttribute
    Private isEmpty As Boolean = False

    Public Property ChildEntries As Entry() = {}
    Public Property Parent As Entry

    Public Property DisplayName As String
        Get
            If IsDir Then
                Return DirInfo.Name
            Else
                Return FileInfo.Name
            End If
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property DisplayInfo As String
        Get
            If IsDir Then
                Return IIf(IsLink, "-> ", "") & ChildEntries.Length
            Else
                Return IIf(IsLink, "-> ", "") & FileSizeStr()
            End If
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property Path() As String
        Get
            If IsDir Then
                Return "" & DirInfo?.FullName
            End If
            Return "" & FileInfo?.FullName
        End Get
        Set(ByVal value As String)
        End Set
    End Property

    Public Property EntryInfos As String
        Get
            If IsDir Then
                Return $"{ChildEntries.Length} {DirInfo.CreationTime:yyyy-MM-dd HH:mm}" & IIf(IsLink, " -> " + DirInfo.FullName, "")
            Else
                Return $"{FileSizeStr()} C:{FileInfo.CreationTime:yyyy-MM-dd HH:mm} W:{FileInfo.LastWriteTime:yyyy-MM-dd HH:mm} A:{FileInfo.LastAccessTime:yyyy-MM-dd HH:mm}" & IIf(IsLink, " -> " + FileInfo.FullName, "")
            End If
        End Get
        Set(value As String)
        End Set
    End Property

    Private Function FileSizeStr() As String
        Dim size As Double
        If FileInfo.Exists Then
            size = FileInfo.Length
        End If

        Dim Unit As String = "B"

        If size > 1000 Then
            size /= 1000
            Unit = "kB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "MB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "GB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "TB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "PB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "EB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "ZB"
        End If

        If size > 1000 Then
            size /= 1000
            Unit = "YB"
        End If

        Dim sizeStr As String = Math.Round(size, 2).ToString()

        If ((1.7).ToString()(1) = ","c) Then
            sizeStr = sizeStr.Replace(",", ".")
        End If

        Return sizeStr & Unit
    End Function

    Public Sub New(FI As IO.FileInfo)
        If FI.Extension = ".lnk" Then
            LnkInfo = LnkReader.Lnk.OpenLnk(FI.FullName)
            IsLink = True
            LinkLocation = FI
            If IO.File.Exists(LnkInfo.BasePath) Or IO.Directory.Exists(LnkInfo.BasePath) Then
                Attributes = IO.File.GetAttributes(LnkInfo.BasePath)
                If (Attributes & FileAttribute.Directory) = FileAttribute.Directory Then
                    IsDir = True
                    DirInfo = New IO.DirectoryInfo(LnkInfo.BasePath)
                Else
                    FileInfo = New IO.FileInfo(LnkInfo.BasePath)
                End If
            Else
                FileInfo = FI
                Attributes = IO.File.GetAttributes(FI.FullName)
            End If
        Else
            If FI.Extension = ".exe" Then
                IsExe = True
            End If
            FileInfo = FI
            Attributes = IO.File.GetAttributes(FI.FullName)
        End If
    End Sub

    <DebuggerHidden>
    Public Sub New(DI As IO.DirectoryInfo)
        IsDir = True
        DirInfo = DI
        Attributes = IO.File.GetAttributes(DI.FullName)
    End Sub

    <DebuggerHidden>
    Public Sub New()
        IsDir = True
        DirInfo = Nothing
        isEmpty = True
    End Sub

    <DebuggerHidden>
    Public Overrides Function ToString() As String
        Dim str As String = ""
        If IsDir Then
            str = "DIR "
        End If
        str &= DisplayName
        If IsLink Then
            str &= " ->"
        End If
        Return str
    End Function

    <DebuggerHidden>
    Public Function IsPath(path As String) As Boolean
        If isEmpty Then Return False
        Dim A As String = IO.Path.GetFullPath(path)
        Dim B As String = IO.Path.GetFullPath(Me.Path)
        If Configuration.IgnoreCaseInPath Then
            A = A.ToLower
            B = B.ToLower
        End If
        Return A.Equals(B)
    End Function

    Public Function CreateParent() As Entry
        If Parent IsNot Nothing Then Return Parent
        Dim Path As String
        If IsLink Then
            Path = LinkLocation.Directory.Parent.FullName
        ElseIf IsDir Then
            If DirInfo?.Parent Is Nothing Then Return Nothing
            Path = DirInfo.Parent.FullName
        Else
            Path = FileInfo.Directory.Parent.FullName
        End If
        Parent = New Entry(New IO.DirectoryInfo(Path))
        Return Parent
    End Function

End Class
