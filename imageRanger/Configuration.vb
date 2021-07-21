Public Class Configuration

#Region "Construction"
    Private Shared MySelf As Configuration

    Shared Sub New()
        Reload()
    End Sub

    Public Shared Sub Reload()
        Dim path As String = FileName()
        If Not IO.File.Exists(path) Then
            Save()
        Else
            Dim JJ As String = IO.File.ReadAllText(path)
            MySelf = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Configuration)(JJ)
        End If
    End Sub

    Public Shared Sub Save()
        If MySelf Is Nothing Then _
            MySelf = New Configuration
        Dim JJ As String = Newtonsoft.Json.JsonConvert.SerializeObject(MySelf, Newtonsoft.Json.Formatting.Indented)
        IO.File.WriteAllText(FileName, JJ)
    End Sub

    Private Const CONFIG_FOLDER_NAME As String = ".config"
    Private Const APP_CONFIG_FOLDER_NAME As String = "imageRanger"
    Private Const CONFIG_FILE_NAME As String = "config.json"
    <DebuggerHidden>
    Private Shared Function FileName() As String
        Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        path &= "\" & CONFIG_FOLDER_NAME
        If Not IO.Directory.Exists(path) Then IO.Directory.CreateDirectory(path)
        path &= "\" & APP_CONFIG_FOLDER_NAME
        If Not IO.Directory.Exists(path) Then IO.Directory.CreateDirectory(path)
        path &= "\" & CONFIG_FILE_NAME
        Return path
    End Function
#End Region

    <Newtonsoft.Json.JsonProperty("Theme")>
    Private _Theme As New Theme
    <Newtonsoft.Json.JsonIgnore>
    Public Shared Property Theme() As Theme
        Get
            Return MySelf._Theme
        End Get
        Set(ByVal value As Theme)
            MySelf._Theme = value
        End Set
    End Property

    <Newtonsoft.Json.JsonProperty("ShowHidden")>
    Private _ShowHidden As Boolean = False
    <Newtonsoft.Json.JsonIgnore>
    Public Shared Property ShowHidden() As Boolean
        Get
            Return MySelf._ShowHidden
        End Get
        Set(ByVal value As Boolean)
            MySelf._ShowHidden = value
        End Set
    End Property

End Class
