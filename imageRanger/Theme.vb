Imports System.Text.RegularExpressions

Public Class Theme

    <Newtonsoft.Json.JsonIgnore>
    Private Shared ReadOnly DEFAULT_COLORS As String() = {"#000000", "#b80000", "#00b700", "#8c8c14", "#1c66ff", "#800080", "#008080", "#d4d4d4", "#606060", "#ff6363", "#72ff72", "#ffff00", "#648cff", "#c157c1", "#21d5d5", "#ffffff"}

    Public Enum ColorNames
        Black = 0
        Red = 1
        Green = 2
        Yellow = 3
        Blue = 4
        Magenta = 5
        Cyan = 6
        Ash = 7
        LightBlack = 8
        LightRed = 9
        LightGreen = 10
        LightYellow = 11
        LightBlue = 12
        LightMagenta = 13
        LightCyan = 14
        LightAsh = 15
    End Enum

    <Newtonsoft.Json.JsonIgnore>
    Default Public ReadOnly Property Item(ColorName As ColorNames) As Color
        Get
            Return Colors(ColorName)
        End Get
    End Property

#Region "Named Colors"
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Black() As Color
        Get
            Return Item(ColorNames.Black)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Red() As Color
        Get
            Return Item(ColorNames.Red)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Green() As Color
        Get
            Return Item(ColorNames.Green)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Yellow() As Color
        Get
            Return Item(ColorNames.Yellow)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Blue() As Color
        Get
            Return Item(ColorNames.Blue)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Magenta() As Color
        Get
            Return Item(ColorNames.Magenta)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Cyan() As Color
        Get
            Return Item(ColorNames.Cyan)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property Ash() As Color
        Get
            Return Item(ColorNames.Ash)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightBlack() As Color
        Get
            Return Item(ColorNames.LightBlack)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightRed() As Color
        Get
            Return Item(ColorNames.LightRed)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightGreen() As Color
        Get
            Return Item(ColorNames.LightGreen)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightYellow() As Color
        Get
            Return Item(ColorNames.LightYellow)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightBlue() As Color
        Get
            Return Item(ColorNames.LightBlue)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightMagenta() As Color
        Get
            Return Item(ColorNames.LightMagenta)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightCyan() As Color
        Get
            Return Item(ColorNames.LightCyan)
        End Get
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property LightAsh() As Color
        Get
            Return Item(ColorNames.LightAsh)
        End Get
    End Property
#End Region

    <Newtonsoft.Json.JsonProperty("name")>
    Public Property Name As String = "Default"

    <Newtonsoft.Json.JsonProperty("author")>
    Public Property Author As String = "Get more at ht" & "tp://terminal.sexy/"

    <Newtonsoft.Json.JsonIgnore>
    Public Property Colors As Color() = DEFAULT_COLORS.Select(Function(s) Str2Color(s)).ToArray()

    <DebuggerBrowsable(False)>
    <Newtonsoft.Json.JsonProperty("color")>
    Private Property JJColors() As String()
        Get
            Return Colors.Select(Function(c) Color2Str(c)).ToArray()
        End Get
        Set(ByVal value As String())
            value = value.Where(Function(s) StrIsColor(s)).ToArray()
            If value.Length <> 16 Then
                value = DEFAULT_COLORS
            End If
            Colors = value.Select(Function(s) Str2Color(s)).ToArray()
        End Set
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public Property Foreground As Color = Color.FromRgb(&HD4, &HD4, &HD4)

    <DebuggerBrowsable(False)>
    <Newtonsoft.Json.JsonProperty("foreground")>
    Private Property JJForeground As String
        Get
            Return Color2Str(Foreground)
        End Get
        Set(value As String)
            If StrIsColor(value) Then _
                Foreground = Str2Color(value)
        End Set
    End Property

    <Newtonsoft.Json.JsonIgnore>
    Public Property Background As Color = Color.FromRgb(0, 0, 0)

    <DebuggerBrowsable(False)>
    <Newtonsoft.Json.JsonProperty("background")>
    Private Property JJBackground As String
        Get
            Return Color2Str(Background)
        End Get
        Set(value As String)
            If StrIsColor(value) Then _
                Background = Str2Color(value)
        End Set
    End Property

#Region "Shared"
    <DebuggerBrowsable(False)>
    Private Shared ReadOnly ColorMatch As New Regex("^#[0-9a-f]{6}$", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    <DebuggerHidden>
    Private Shared Function StrIsColor(str As String) As Boolean
        Return ColorMatch.IsMatch(str)
    End Function

    <DebuggerHidden>
    Private Shared Function Str2Color(str As String) As Color
        If StrIsColor(str) Then
            Return Color.FromRgb(
                Byte.Parse(str.Substring(1, 2), Globalization.NumberStyles.HexNumber),
                Byte.Parse(str.Substring(3, 2), Globalization.NumberStyles.HexNumber),
                Byte.Parse(str.Substring(5, 2), Globalization.NumberStyles.HexNumber)
            )
        Else
            Console.WriteLine("Error! String is not a Color: " & str)
            Dim Rnd As New Random
            Return Color.FromRgb(Rnd.Next(0, 255), Rnd.Next(0, 255), Rnd.Next(0, 255))
        End If
    End Function

    <DebuggerHidden>
    Private Shared Function Color2Str(color As Color) As String
        Dim str As String = "#"
        str &= Hex(color.R).ToLower().PadLeft(2, "0"c)
        str &= Hex(color.G).ToLower().PadLeft(2, "0"c)
        str &= Hex(color.B).ToLower().PadLeft(2, "0"c)
        Return str
    End Function
#End Region

End Class

#Region "EXAMPLE JSON"
'{
'  "name": "",
'  "author": "",
'  "color": [
'    "#000000",
'    "#800000",
'    "#008000",
'    "#808000",
'    "#000080",
'    "#800080",
'    "#008080",
'    "#a0a0a0",
'    "#606060",
'    "#ff0000",
'    "#00ff00",
'    "#ffff00",
'    "#0000ff",
'    "#ff00ff",
'    "#00ffff",
'    "#ffffff"
'  ],
'  "foreground": "#c2c2c2",
'  "background": "#000000"
'}

'{
'  "name": "",
'  "author": "",
'  "color": [
'    "#000000",
'    "#b80000",
'    "#00b700",
'    "#8c8c14",
'    "#1c66ff",
'    "#800080",
'    "#008080",
'    "#d4d4d4",
'    "#606060",
'    "#ff6363",
'    "#72ff72",
'    "#ffff00",
'    "#648cff",
'    "#c157c1",
'    "#21d5d5",
'    "#ffffff"
'  ],
'  "foreground": "#d4d4d4",
'  "background": "#000000"
'}
#End Region