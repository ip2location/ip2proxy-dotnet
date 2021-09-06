Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public NotInheritable Class ComponentWebService
    Private _APIKey As String = ""
    Private _Package As String = ""
    Private _UseSSL As Boolean = True
    Private ReadOnly _RegexAPIKey As New Regex("^[\dA-Z]{10}$")
    Private ReadOnly _RegexPackage As New Regex("^PX\d+$")
    Private Const BASE_URL As String = "api.ip2proxy.com/"

    ' Description: Initialize
    Public Sub New()
    End Sub

    ' Description: Set the API key and package for the queries
    Public Sub Open(ByVal APIKey As String, ByVal Package As String, ByVal Optional UseSSL As Boolean = True)
        _APIKey = APIKey
        _Package = Package
        _UseSSL = UseSSL

        CheckParams()
    End Sub

    ' Description: Validate API key and package
    Private Sub CheckParams()
        If Not _RegexAPIKey.IsMatch(_APIKey) AndAlso _APIKey <> "demo" Then
            Throw New Exception("Invalid API key.")
        End If

        If Not _RegexPackage.IsMatch(_Package) Then
            Throw New Exception("Invalid package name.")
        End If
    End Sub

    ' Description: Query web service to get location information by IP address
    Public Function IPQuery(ByVal IP As String) As JObject
        CheckParams() ' check here in case user haven't called Open yet

        Dim url As String
        Dim protocol As String = If(_UseSSL, "https", "http")
        url = protocol & "://" & BASE_URL & "?key=" & _APIKey & "&package=" & _Package & "&ip=" & Net.WebUtility.UrlEncode(IP)
        Dim request As New Http
        Dim rawjson As String
        rawjson = request.GetMethod(url)
        Dim results = JsonConvert.DeserializeObject(Of Object)(rawjson)

        Return results
    End Function

    ' Description: Check web service credit balance
    Public Function GetCredit() As JObject
        CheckParams() ' check here in case user haven't called Open yet

        Dim url As String
        Dim protocol As String = If(_UseSSL, "https", "http")
        url = protocol & "://" & BASE_URL & "?key=" & _APIKey & "&check=true"
        Dim request As New Http
        Dim rawjson As String
        rawjson = request.GetMethod(url)
        Dim results = JsonConvert.DeserializeObject(Of Object)(rawjson)

        Return results
    End Function

End Class
