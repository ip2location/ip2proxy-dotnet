Imports System
Imports System.IO
Imports System.IO.MemoryMappedFiles
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports System.Numerics
Imports System.Text.RegularExpressions
Imports System.Globalization

Public Structure ProxyResult
    Public Is_Proxy As Integer
    Public Proxy_Type As String
    Public Country_Short As String
    Public Country_Long As String
    Public Region As String
    Public City As String
    Public ISP As String
    Public Domain As String
    Public Usage_Type As String
    Public ASN As String
    Public [AS] As String
    Public Last_Seen As String
    Public Threat As String
End Structure

Public Class Component
    Private _DBFilePath As String = ""
    Private _MMF As MemoryMappedFile = Nothing
    Private _IndexArrayIPv4(65535, 1) As Integer
    Private _IndexArrayIPv6(65535, 1) As Integer
    Private _IPv4Accessor As MemoryMappedViewAccessor = Nothing
    Private _IPv4Offset As Integer = 0
    Private _IPv6Accessor As MemoryMappedViewAccessor = Nothing
    Private _IPv6Offset As Integer = 0
    Private _MapDataAccessor As MemoryMappedViewAccessor = Nothing
    Private _MapDataOffset As Integer = 0
    Private _OutlierCase1 As Regex = New Regex("^:(:[\dA-F]{1,4}){7}$", RegexOptions.IgnoreCase)
    Private _OutlierCase2 As Regex = New Regex("^:(:[\dA-F]{1,4}){5}:(\d{1,3}\.){3}\d{1,3}$", RegexOptions.IgnoreCase)
    Private _OutlierCase3 As Regex = New Regex("^\d+$")
    Private _OutlierCase4 As Regex = New Regex("^([\dA-F]{1,4}:){6}(0\d+\.|.*?\.0\d+).*$")
    Private _OutlierCase5 As Regex = New Regex("^(\d+\.){1,2}\d+$")
    Private _IPv4MappedRegex As Regex = New Regex("^(.*:)((\d+\.){3}\d+)$")
    Private _IPv4MappedRegex2 As Regex = New Regex("^.*((:[\dA-F]{1,4}){2})$")
    Private _IPv4CompatibleRegex As Regex = New Regex("^::[\dA-F]{1,4}$", RegexOptions.IgnoreCase)
    Private _UseMemoryMappedFile As Boolean = False
    Private _IPv4ColumnSize As Integer = 0
    Private _IPv6ColumnSize As Integer = 0
    Private _MapFileName As String = "MyProxyBIN" ' If running multiple websites with different application pools, every website must have different mapped file name

    Private _BaseAddr As Integer = 0
    Private _DBCount As Integer = 0
    Private _DBColumn As Integer = 0
    Private _DBType As Integer = 0
    Private _DBDay As Integer = 1
    Private _DBMonth As Integer = 1
    Private _DBYear As Integer = 1
    Private _BaseAddrIPv6 As Integer = 0
    Private _DBCountIPv6 As Integer = 0
    Private _IndexBaseAddr As Integer = 0
    Private _IndexBaseAddrIPv6 As Integer = 0

    Private _FromBI As New BigInteger(281470681743360)
    Private _ToBI As New BigInteger(281474976710655)
    Private _FromBI2 As BigInteger = BigInteger.Parse("42545680458834377588178886921629466624")
    Private _ToBI2 As BigInteger = BigInteger.Parse("42550872755692912415807417417958686719")
    Private _FromBI3 As BigInteger = BigInteger.Parse("42540488161975842760550356425300246528")
    Private _ToBI3 As BigInteger = BigInteger.Parse("42540488241204005274814694018844196863")
    Private _DivBI As BigInteger = New BigInteger(4294967295)

    Private Const MAX_IPV4_RANGE As Long = 4294967295
    Private MAX_IPV6_RANGE As BigInteger = BigInteger.Pow(2, 128) - 1
    Private Const MSG_NOT_SUPPORTED As String = "NOT SUPPORTED"
    Private Const MSG_INVALID_IP As String = "INVALID IP ADDRESS"
    Private Const MSG_MISSING_FILE As String = "MISSING FILE"
    Private Const MSG_IPV6_UNSUPPORTED As String = "IPV6 ADDRESS MISSING IN IPV4 BIN"

    Public Enum IOModes
        IP2PROXY_FILE_IO = 1
        IP2PROXY_MEMORY_MAPPED = 2
    End Enum

    Public Enum Modes
        COUNTRY_SHORT = 1
        COUNTRY_LONG = 2
        REGION = 3
        CITY = 4
        ISP = 5
        PROXY_TYPE = 6
        IS_PROXY = 7
        DOMAIN = 8
        USAGE_TYPE = 9
        ASN = 10
        [AS] = 11
        LAST_SEEN = 12
        THREAT = 13
        ALL = 100
    End Enum

    Private COUNTRY_POSITION() As Byte = {0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3}
    Private REGION_POSITION() As Byte = {0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4}
    Private CITY_POSITION() As Byte = {0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5}
    Private ISP_POSITION() As Byte = {0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6}
    Private PROXYTYPE_POSITION() As Byte = {0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2}
    Private DOMAIN_POSITION() As Byte = {0, 0, 0, 0, 0, 7, 7, 7, 7, 7, 7}
    Private USAGETYPE_POSITION() As Byte = {0, 0, 0, 0, 0, 0, 8, 8, 8, 8, 8}
    Private ASN_POSITION() As Byte = {0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 9}
    Private AS_POSITION() As Byte = {0, 0, 0, 0, 0, 0, 0, 10, 10, 10, 10}
    Private LASTSEEN_POSITION() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 11}
    Private THREAT_POSITION() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 12}

    Private COUNTRY_POSITION_OFFSET As Integer = 0
    Private REGION_POSITION_OFFSET As Integer = 0
    Private CITY_POSITION_OFFSET As Integer = 0
    Private ISP_POSITION_OFFSET As Integer = 0
    Private PROXYTYPE_POSITION_OFFSET As Integer = 0
    Private DOMAIN_POSITION_OFFSET As Integer = 0
    Private USAGETYPE_POSITION_OFFSET As Integer = 0
    Private ASN_POSITION_OFFSET As Integer = 0
    Private AS_POSITION_OFFSET As Integer = 0
    Private LASTSEEN_POSITION_OFFSET As Integer = 0
    Private THREAT_POSITION_OFFSET As Integer = 0

    Private COUNTRY_ENABLED As Boolean = False
    Private REGION_ENABLED As Boolean = False
    Private CITY_ENABLED As Boolean = False
    Private ISP_ENABLED As Boolean = False
    Private PROXYTYPE_ENABLED As Boolean = False
    Private DOMAIN_ENABLED As Boolean = False
    Private USAGETYPE_ENABLED As Boolean = False
    Private ASN_ENABLED As Boolean = False
    Private AS_ENABLED As Boolean = False
    Private LASTSEEN_ENABLED As Boolean = False
    Private THREAT_ENABLED As Boolean = False

    'Description: Returns the module version
    Public Function GetModuleVersion() As String
        Dim Ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version()
        Return Ver.Major & "." & Ver.Minor & "." & Ver.Build
    End Function

    'Description: Returns the package version
    Public Function GetPackageVersion() As String
        Return _DBType.ToString()
    End Function

    'Description: Returns the IP database version
    Public Function GetDatabaseVersion() As String
        If _DBYear = 0 Then
            Return ""
        Else
            Return "20" & _DBYear.ToString(CultureInfo.CurrentCulture()) & "." & _DBMonth.ToString(CultureInfo.CurrentCulture()) & "." & _DBDay.ToString(CultureInfo.CurrentCulture())
        End If
    End Function

    'Description: Returns an integer to state if is proxy
    Public Function IsProxy(IP As String) As Integer
        ' -1 is error
        '  0 is not a proxy
        '  1 is proxy except DCH and SES
        '  2 is proxy and (DCH or SES)
        Return ProxyQuery(IP, Modes.IS_PROXY).Is_Proxy
    End Function

    'Description: Returns a string for the country code
    Public Function GetCountryShort(IP As String) As String
        Return ProxyQuery(IP, Modes.COUNTRY_SHORT).Country_Short
    End Function

    'Description: Returns a string for the country name
    Public Function GetCountryLong(IP As String) As String
        Return ProxyQuery(IP, Modes.COUNTRY_LONG).Country_Long
    End Function

    'Description: Returns a string for the region name
    Public Function GetRegion(IP As String) As String
        Return ProxyQuery(IP, Modes.REGION).Region
    End Function

    'Description: Returns a string for the city name
    Public Function GetCity(IP As String) As String
        Return ProxyQuery(IP, Modes.CITY).City
    End Function

    'Description: Returns a string for the ISP name
    Public Function GetISP(IP As String) As String
        Return ProxyQuery(IP, Modes.ISP).ISP
    End Function

    'Description: Returns a string for the proxy type
    Public Function GetProxyType(IP As String) As String
        Return ProxyQuery(IP, Modes.PROXY_TYPE).Proxy_Type
    End Function

    'Description: Returns a string for the domain
    Public Function GetDomain(IP As String) As String
        Return ProxyQuery(IP, Modes.DOMAIN).Domain
    End Function

    'Description: Returns a string for the usage type
    Public Function GetUsageType(IP As String) As String
        Return ProxyQuery(IP, Modes.USAGE_TYPE).Usage_Type
    End Function

    'Description: Returns a string for the ASN
    Public Function GetASN(IP As String) As String
        Return ProxyQuery(IP, Modes.ASN).ASN
    End Function

    'Description: Returns a string for the AS
    Public Function GetAS(IP As String) As String
        Return ProxyQuery(IP, Modes.AS).AS
    End Function

    'Description: Returns a string for the last seen
    Public Function GetLastSeen(IP As String) As String
        Return ProxyQuery(IP, Modes.LAST_SEEN).Last_Seen
    End Function

    'Description: Returns a string for the threat
    Public Function GetThreat(IP As String) As String
        Return ProxyQuery(IP, Modes.THREAT).Threat
    End Function

    'Description: Returns all results
    Public Function GetAll(IP As String) As ProxyResult
        Return ProxyQuery(IP)
    End Function

    ' Description: Create memory mapped file
    Private Sub CreateMemoryMappedFile()
        If _MMF Is Nothing Then
            Try
                _MMF = MemoryMappedFile.OpenExisting(_MapFileName, MemoryMappedFileRights.Read)
            Catch ex As Exception
                Try
                    _MMF = MemoryMappedFile.CreateFromFile(_DBFilePath, FileMode.Open, _MapFileName, New FileInfo(_DBFilePath).Length, MemoryMappedFileAccess.Read)
                Catch ex2 As Exception
                    Try
                        Dim len As Long = New FileInfo(_DBFilePath).Length
                        _MMF = MemoryMappedFile.CreateNew(_MapFileName, len, MemoryMappedFileAccess.ReadWrite)
                        Using stream As MemoryMappedViewStream = _MMF.CreateViewStream()
                            Using writer As BinaryWriter = New BinaryWriter(stream)
                                Using fs As FileStream = New FileStream(_DBFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                                    Dim buff(len) As Byte
                                    fs.Read(buff, 0, buff.Length)
                                    writer.Write(buff, 0, buff.Length)
                                End Using
                            End Using
                        End Using
                    Catch ex3 As Exception
                        ' this part onwards trying Linux specific stuff (no named map)
                        Try
                            _MMF = MemoryMappedFile.OpenExisting(Nothing, MemoryMappedFileRights.Read)
                        Catch ex4 As Exception
                            Try
                                _MMF = MemoryMappedFile.CreateFromFile(_DBFilePath, FileMode.Open, Nothing, New FileInfo(_DBFilePath).Length, MemoryMappedFileAccess.Read)
                            Catch ex5 As Exception
                                Try
                                    Dim len As Long = New FileInfo(_DBFilePath).Length
                                    _MMF = MemoryMappedFile.CreateNew(Nothing, len, MemoryMappedFileAccess.ReadWrite)
                                    Using stream As MemoryMappedViewStream = _MMF.CreateViewStream()
                                        Using writer As BinaryWriter = New BinaryWriter(stream)
                                            Using fs As FileStream = New FileStream(_DBFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                                                Dim buff(len) As Byte
                                                fs.Read(buff, 0, buff.Length)
                                                writer.Write(buff, 0, buff.Length)
                                            End Using
                                        End Using
                                    End Using
                                Catch ex6 As Exception
                                    ErrLog(ex6.Message & "---" & ex6.StackTrace)
                                End Try
                            End Try
                        End Try
                    End Try
                End Try
            End Try
        End If
    End Sub

    ' Description: Destroy memory mapped file
    Private Sub DestroyMemoryMappedFile()
        If Not _MMF Is Nothing Then
            _MMF.Dispose()
            _MMF = Nothing
        End If
    End Sub

    ' Description: Create memory accessors
    Private Sub CreateAccessors()
        If _IPv4Accessor Is Nothing Then
            Dim IPv4Bytes As Integer
            IPv4Bytes = _IPv4ColumnSize * _DBCount ' 4 bytes per column
            _IPv4Offset = _BaseAddr - 1
            _IPv4Accessor = _MMF.CreateViewAccessor(_IPv4Offset, IPv4Bytes, MemoryMappedFileAccess.Read) ' assume MMF created
            _MapDataOffset = _IPv4Offset + IPv4Bytes
        End If

        If _DBCountIPv6 > 0 AndAlso _IPv6Accessor Is Nothing Then
            Dim IPv6Bytes As Integer
            IPv6Bytes = _IPv6ColumnSize * _DBCountIPv6 ' 4 bytes per column but IPFrom 16 bytes
            _IPv6Offset = _BaseAddrIPv6 - 1
            _IPv6Accessor = _MMF.CreateViewAccessor(_IPv6Offset, IPv6Bytes, MemoryMappedFileAccess.Read) ' assume MMF created
            _MapDataOffset = _IPv6Offset + IPv6Bytes
        End If

        If _MapDataAccessor Is Nothing Then
            _MapDataAccessor = _MMF.CreateViewAccessor(_MapDataOffset, 0, MemoryMappedFileAccess.Read) ' read from offset till EOF
        End If
    End Sub

    ' Description: Destroy memory accessors
    Private Sub DestroyAccessors()
        If Not _IPv4Accessor Is Nothing Then
            _IPv4Accessor.Dispose()
            _IPv4Accessor = Nothing
        End If

        If Not _IPv6Accessor Is Nothing Then
            _IPv6Accessor.Dispose()
            _IPv6Accessor = Nothing
        End If

        If Not _MapDataAccessor Is Nothing Then
            _MapDataAccessor.Dispose()
            _MapDataAccessor = Nothing
        End If
    End Sub

    ' Description: Read BIN file into memory mapped file and create accessors
    Private Function LoadBIN() As Boolean
        Dim LoadOK As Boolean = False
        Try
            If _DBFilePath <> "" Then
                CreateMemoryMappedFile()

                If _MMF IsNot Nothing Then
                    ' below use temp accessor as we only need once to read meta data (use this even when in filestream mode)
                    Using _MetaAccessor As MemoryMappedViewAccessor = _MMF.CreateViewAccessor(0, 64, MemoryMappedFileAccess.Read) ' 64 bytes header
                        _DBType = _MetaAccessor.ReadByte(0)
                        _DBColumn = _MetaAccessor.ReadByte(1)
                        _DBYear = _MetaAccessor.ReadByte(2)
                        _DBMonth = _MetaAccessor.ReadByte(3)
                        _DBDay = _MetaAccessor.ReadByte(4)
                        _DBCount = _MetaAccessor.ReadInt32(5) '4 bytes
                        _BaseAddr = _MetaAccessor.ReadInt32(9) '4 bytes
                        _DBCountIPv6 = _MetaAccessor.ReadInt32(13) '4 bytes
                        _BaseAddrIPv6 = _MetaAccessor.ReadInt32(17) '4 bytes
                        _IndexBaseAddr = _MetaAccessor.ReadInt32(21) '4 bytes
                        _IndexBaseAddrIPv6 = _MetaAccessor.ReadInt32(25) '4 bytes

                        _IPv4ColumnSize = _DBColumn << 2 ' 4 bytes each column
                        _IPv6ColumnSize = 16 + ((_DBColumn - 1) << 2) ' 4 bytes each column, except IPFrom column which is 16 bytes

                        ' since both IPv4 and IPv6 use 4 bytes for the below columns, can just do it once here
                        'COUNTRY_POSITION_OFFSET = If(COUNTRY_POSITION(_DBType) <> 0, (COUNTRY_POSITION(_DBType) - 1) << 2, 0)
                        'REGION_POSITION_OFFSET = If(REGION_POSITION(_DBType) <> 0, (REGION_POSITION(_DBType) - 1) << 2, 0)
                        'CITY_POSITION_OFFSET = If(CITY_POSITION(_DBType) <> 0, (CITY_POSITION(_DBType) - 1) << 2, 0)
                        'ISP_POSITION_OFFSET = If(ISP_POSITION(_DBType) <> 0, (ISP_POSITION(_DBType) - 1) << 2, 0)
                        'PROXYTYPE_POSITION_OFFSET = If(PROXYTYPE_POSITION(_DBType) <> 0, (PROXYTYPE_POSITION(_DBType) - 1) << 2, 0)
                        'DOMAIN_POSITION_OFFSET = If(DOMAIN_POSITION(_DBType) <> 0, (DOMAIN_POSITION(_DBType) - 1) << 2, 0)
                        'USAGETYPE_POSITION_OFFSET = If(USAGETYPE_POSITION(_DBType) <> 0, (USAGETYPE_POSITION(_DBType) - 1) << 2, 0)
                        'ASN_POSITION_OFFSET = If(ASN_POSITION(_DBType) <> 0, (ASN_POSITION(_DBType) - 1) << 2, 0)
                        'AS_POSITION_OFFSET = If(AS_POSITION(_DBType) <> 0, (AS_POSITION(_DBType) - 1) << 2, 0)
                        'LASTSEEN_POSITION_OFFSET = If(LASTSEEN_POSITION(_DBType) <> 0, (LASTSEEN_POSITION(_DBType) - 1) << 2, 0)
                        'THREAT_POSITION_OFFSET = If(THREAT_POSITION(_DBType) <> 0, (THREAT_POSITION(_DBType) - 1) << 2, 0)

                        ' slightly different offset for reading by row
                        COUNTRY_POSITION_OFFSET = If(COUNTRY_POSITION(_DBType) <> 0, (COUNTRY_POSITION(_DBType) - 2) << 2, 0)
                        REGION_POSITION_OFFSET = If(REGION_POSITION(_DBType) <> 0, (REGION_POSITION(_DBType) - 2) << 2, 0)
                        CITY_POSITION_OFFSET = If(CITY_POSITION(_DBType) <> 0, (CITY_POSITION(_DBType) - 2) << 2, 0)
                        ISP_POSITION_OFFSET = If(ISP_POSITION(_DBType) <> 0, (ISP_POSITION(_DBType) - 2) << 2, 0)
                        PROXYTYPE_POSITION_OFFSET = If(PROXYTYPE_POSITION(_DBType) <> 0, (PROXYTYPE_POSITION(_DBType) - 2) << 2, 0)
                        DOMAIN_POSITION_OFFSET = If(DOMAIN_POSITION(_DBType) <> 0, (DOMAIN_POSITION(_DBType) - 2) << 2, 0)
                        USAGETYPE_POSITION_OFFSET = If(USAGETYPE_POSITION(_DBType) <> 0, (USAGETYPE_POSITION(_DBType) - 2) << 2, 0)
                        ASN_POSITION_OFFSET = If(ASN_POSITION(_DBType) <> 0, (ASN_POSITION(_DBType) - 2) << 2, 0)
                        AS_POSITION_OFFSET = If(AS_POSITION(_DBType) <> 0, (AS_POSITION(_DBType) - 2) << 2, 0)
                        LASTSEEN_POSITION_OFFSET = If(LASTSEEN_POSITION(_DBType) <> 0, (LASTSEEN_POSITION(_DBType) - 2) << 2, 0)
                        THREAT_POSITION_OFFSET = If(THREAT_POSITION(_DBType) <> 0, (THREAT_POSITION(_DBType) - 2) << 2, 0)

                        COUNTRY_ENABLED = If(COUNTRY_POSITION(_DBType) <> 0, True, False)
                        REGION_ENABLED = If(REGION_POSITION(_DBType) <> 0, True, False)
                        CITY_ENABLED = If(CITY_POSITION(_DBType) <> 0, True, False)
                        ISP_ENABLED = If(ISP_POSITION(_DBType) <> 0, True, False)
                        PROXYTYPE_ENABLED = If(PROXYTYPE_POSITION(_DBType) <> 0, True, False)
                        DOMAIN_ENABLED = If(DOMAIN_POSITION(_DBType) <> 0, True, False)
                        USAGETYPE_ENABLED = If(USAGETYPE_POSITION(_DBType) <> 0, True, False)
                        ASN_ENABLED = If(ASN_POSITION(_DBType) <> 0, True, False)
                        AS_ENABLED = If(AS_POSITION(_DBType) <> 0, True, False)
                        LASTSEEN_ENABLED = If(LASTSEEN_POSITION(_DBType) <> 0, True, False)
                        THREAT_ENABLED = If(THREAT_POSITION(_DBType) <> 0, True, False)
                    End Using

                    Using _IndexAccessor As MemoryMappedViewAccessor = _MMF.CreateViewAccessor(_IndexBaseAddr - 1, _BaseAddr - _IndexBaseAddr, MemoryMappedFileAccess.Read) ' reading indexes
                        Dim Pointer As Integer = 0

                        ' read IPv4 index
                        For x As Integer = _IndexArrayIPv4.GetLowerBound(0) To _IndexArrayIPv4.GetUpperBound(0)
                            _IndexArrayIPv4(x, 0) = _IndexAccessor.ReadInt32(Pointer) '4 bytes for from row
                            _IndexArrayIPv4(x, 1) = _IndexAccessor.ReadInt32(Pointer + 4) '4 bytes for to row
                            Pointer += 8
                        Next

                        If _IndexBaseAddrIPv6 > 0 Then
                            ' read IPv6 index
                            For x As Integer = _IndexArrayIPv6.GetLowerBound(0) To _IndexArrayIPv6.GetUpperBound(0)
                                _IndexArrayIPv6(x, 0) = _IndexAccessor.ReadInt32(Pointer) '4 bytes for from row
                                _IndexArrayIPv6(x, 1) = _IndexAccessor.ReadInt32(Pointer + 4) '4 bytes for to row
                                Pointer += 8
                            Next
                        End If
                    End Using

                    If _UseMemoryMappedFile Then
                        CreateAccessors()
                    Else
                        DestroyMemoryMappedFile()
                    End If
                    LoadOK = True
                End If
            End If
        Catch Ex As Exception
            ErrLog(Ex.Message)
        End Try

        Return LoadOK
    End Function

    ' Description: Reverse the bytes if system is little endian
    Private Sub LittleEndian(ByRef ByteArr() As Byte)
        If System.BitConverter.IsLittleEndian Then
            Dim ByteList As New List(Of Byte)(ByteArr)
            ByteList.Reverse()
            ByteArr = ByteList.ToArray()
        End If
    End Sub

    ' Description: Initialize the component with the BIN file path and mode
    Public Function Open(ByVal DatabasePath As String, Optional ByVal IOMode As IOModes = IOModes.IP2PROXY_FILE_IO) As Integer
        If _DBType = 0 Then
            _DBFilePath = DatabasePath

            If IOMode = IOModes.IP2PROXY_MEMORY_MAPPED Then
                _UseMemoryMappedFile = True
            End If

            If Not LoadBIN() Then ' problems reading BIN
                Return -1
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    ' Description: Query database to get proxy information by IP address
    Private Function ProxyQuery(ByVal IPAddress As String, Optional ByVal Mode As Modes = Modes.ALL) As ProxyResult
        Dim Result As ProxyResult
        Dim StrIP As String
        Dim IPType As Integer = 0
        Dim DBType As Integer = 0
        Dim BaseAddr As Integer = 0
        Dim DBColumn As Integer = 0
        Dim Accessor As MemoryMappedViewAccessor = Nothing
        Dim FS As FileStream = Nothing

        Dim CountryPos As Long = 0
        Dim Low As Long = 0
        Dim High As Long = 0
        Dim Mid As Long = 0
        Dim IPFrom As BigInteger = 0
        Dim IPTo As BigInteger = 0
        Dim IPNum As BigInteger = 0
        Dim IndexAddr As Long = 0
        Dim MAX_IP_RANGE As BigInteger = 0
        Dim RowOffset As Long = 0
        Dim RowOffset2 As Long = 0
        Dim ColumnSize As Integer = 0
        Dim OverCapacity As Boolean = False

        Try
            If IPAddress = "" OrElse IPAddress Is Nothing Then
                With Result
                    .Is_Proxy = -1
                    .Proxy_Type = MSG_INVALID_IP
                    .Country_Short = MSG_INVALID_IP
                    .Country_Long = MSG_INVALID_IP
                    .Region = MSG_INVALID_IP
                    .City = MSG_INVALID_IP
                    .ISP = MSG_INVALID_IP
                    .Domain = MSG_INVALID_IP
                    .Usage_Type = MSG_INVALID_IP
                    .ASN = MSG_INVALID_IP
                    .AS = MSG_INVALID_IP
                    .Last_Seen = MSG_INVALID_IP
                    .Threat = MSG_INVALID_IP
                End With
                Return Result
            End If

            StrIP = Me.VerifyIP(IPAddress, IPType, IPNum)
            If StrIP <> "Invalid IP" Then
                IPAddress = StrIP
            Else
                With Result
                    .Is_Proxy = -1
                    .Proxy_Type = MSG_INVALID_IP
                    .Country_Short = MSG_INVALID_IP
                    .Country_Long = MSG_INVALID_IP
                    .Region = MSG_INVALID_IP
                    .City = MSG_INVALID_IP
                    .ISP = MSG_INVALID_IP
                    .Domain = MSG_INVALID_IP
                    .Usage_Type = MSG_INVALID_IP
                    .ASN = MSG_INVALID_IP
                    .AS = MSG_INVALID_IP
                    .Last_Seen = MSG_INVALID_IP
                    .Threat = MSG_INVALID_IP
                End With
                Return Result
            End If

            ' Read BIN if haven't done so
            If _DBType = 0 Then
                If Not LoadBIN() Then ' problems reading BIN
                    With Result
                        .Is_Proxy = -1
                        .Proxy_Type = MSG_MISSING_FILE
                        .Country_Short = MSG_MISSING_FILE
                        .Country_Long = MSG_MISSING_FILE
                        .Region = MSG_MISSING_FILE
                        .City = MSG_MISSING_FILE
                        .ISP = MSG_MISSING_FILE
                        .Domain = MSG_MISSING_FILE
                        .Usage_Type = MSG_MISSING_FILE
                        .ASN = MSG_MISSING_FILE
                        .AS = MSG_MISSING_FILE
                        .Last_Seen = MSG_MISSING_FILE
                        .Threat = MSG_MISSING_FILE
                    End With
                    Return Result
                End If
            End If

            If _UseMemoryMappedFile Then
                CreateMemoryMappedFile()
                CreateAccessors()
            Else
                DestroyAccessors()
                DestroyMemoryMappedFile()
                FS = New FileStream(_DBFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            End If

            Select Case IPType
                Case 4
                    ' IPv4
                    MAX_IP_RANGE = MAX_IPV4_RANGE
                    High = _DBCount
                    If _UseMemoryMappedFile Then
                        Accessor = _IPv4Accessor
                    Else
                        BaseAddr = _BaseAddr
                    End If
                    ColumnSize = _IPv4ColumnSize

                    IndexAddr = IPNum >> 16

                    Low = _IndexArrayIPv4(IndexAddr, 0)
                    High = _IndexArrayIPv4(IndexAddr, 1)
                Case 6
                    ' IPv6
                    If _DBCountIPv6 = 0 Then
                        With Result
                            .Is_Proxy = -1
                            .Proxy_Type = MSG_IPV6_UNSUPPORTED
                            .Country_Short = MSG_IPV6_UNSUPPORTED
                            .Country_Long = MSG_IPV6_UNSUPPORTED
                            .Region = MSG_IPV6_UNSUPPORTED
                            .City = MSG_IPV6_UNSUPPORTED
                            .ISP = MSG_IPV6_UNSUPPORTED
                            .Domain = MSG_IPV6_UNSUPPORTED
                            .Usage_Type = MSG_IPV6_UNSUPPORTED
                            .ASN = MSG_IPV6_UNSUPPORTED
                            .AS = MSG_IPV6_UNSUPPORTED
                            .Last_Seen = MSG_IPV6_UNSUPPORTED
                            .Threat = MSG_IPV6_UNSUPPORTED
                        End With
                        Return Result
                    End If
                    MAX_IP_RANGE = MAX_IPV6_RANGE
                    High = _DBCountIPv6
                    If _UseMemoryMappedFile Then
                        Accessor = _IPv6Accessor
                    Else
                        BaseAddr = _BaseAddrIPv6
                    End If
                    ColumnSize = _IPv6ColumnSize

                    If _IndexBaseAddrIPv6 > 0 Then
                        IndexAddr = IPNum >> 112

                        Low = _IndexArrayIPv6(IndexAddr, 0)
                        High = _IndexArrayIPv6(IndexAddr, 1)
                    End If
            End Select

            If IPNum >= MAX_IP_RANGE Then
                IPNum = MAX_IP_RANGE - 1
            End If

            While (Low <= High)
                Mid = CInt((Low + High) / 2)

                RowOffset = BaseAddr + (Mid * ColumnSize)
                RowOffset2 = RowOffset + ColumnSize

                If _UseMemoryMappedFile Then
                    OverCapacity = (RowOffset2 >= Accessor.Capacity)
                End If

                IPFrom = Read32Or128(RowOffset, IPType, Accessor, FS)
                IPTo = If(OverCapacity, BigInteger.Zero, Read32Or128(RowOffset2, IPType, Accessor, FS))

                If IPNum >= IPFrom AndAlso IPNum < IPTo Then
                    Dim Is_Proxy As Integer = -1
                    Dim Proxy_Type As String = MSG_NOT_SUPPORTED
                    Dim Country_Short As String = MSG_NOT_SUPPORTED
                    Dim Country_Long As String = MSG_NOT_SUPPORTED
                    Dim Region As String = MSG_NOT_SUPPORTED
                    Dim City As String = MSG_NOT_SUPPORTED
                    Dim ISP As String = MSG_NOT_SUPPORTED
                    Dim Domain As String = MSG_NOT_SUPPORTED
                    Dim Usage_Type As String = MSG_NOT_SUPPORTED
                    Dim ASN As String = MSG_NOT_SUPPORTED
                    Dim [AS] As String = MSG_NOT_SUPPORTED
                    Dim Last_Seen As String = MSG_NOT_SUPPORTED
                    Dim Threat As String = MSG_NOT_SUPPORTED

                    Dim FirstCol As Integer = 4 ' for IPv4, IP From is 4 bytes
                    If IPType = 6 Then ' IPv6
                        FirstCol = 16 ' 16 bytes for IPv6
                        'RowOffset = RowOffset + 12 ' coz below is assuming all columns are 4 bytes, so got 12 left to go to make 16 bytes total
                    End If

                    ' read the row here after the IP From column (remaining columns are all 4 bytes)
                    Dim Row() As Byte = ReadRow(RowOffset + FirstCol, ColumnSize - FirstCol, Accessor, FS)

                    If PROXYTYPE_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.PROXY_TYPE OrElse Mode = Modes.IS_PROXY Then
                            'Proxy_Type = ReadStr(Read32(RowOffset + PROXYTYPE_POSITION_OFFSET, Accessor, FS), FS)
                            Proxy_Type = ReadStr(Read32_Row(Row, PROXYTYPE_POSITION_OFFSET), FS)
                        End If
                    End If
                    If COUNTRY_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.COUNTRY_SHORT OrElse Mode = Modes.COUNTRY_LONG OrElse Mode = Modes.IS_PROXY Then
                            'CountryPos = Read32(RowOffset + COUNTRY_POSITION_OFFSET, Accessor, FS)
                            CountryPos = Read32_Row(Row, COUNTRY_POSITION_OFFSET)
                        End If
                        If Mode = Modes.ALL OrElse Mode = Modes.COUNTRY_SHORT OrElse Mode = Modes.IS_PROXY Then
                            Country_Short = ReadStr(CountryPos, FS)
                        End If
                        If Mode = Modes.ALL OrElse Mode = Modes.COUNTRY_LONG Then
                            Country_Long = ReadStr(CountryPos + 3, FS)
                        End If
                    End If
                    If REGION_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.REGION Then
                            'Region = ReadStr(Read32(RowOffset + REGION_POSITION_OFFSET, Accessor, FS), FS)
                            Region = ReadStr(Read32_Row(Row, REGION_POSITION_OFFSET), FS)
                        End If
                    End If
                    If CITY_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.CITY Then
                            'City = ReadStr(Read32(RowOffset + CITY_POSITION_OFFSET, Accessor, FS), FS)
                            City = ReadStr(Read32_Row(Row, CITY_POSITION_OFFSET), FS)
                        End If
                    End If
                    If ISP_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.ISP Then
                            'ISP = ReadStr(Read32(RowOffset + ISP_POSITION_OFFSET, Accessor, FS), FS)
                            ISP = ReadStr(Read32_Row(Row, ISP_POSITION_OFFSET), FS)
                        End If
                    End If
                    If DOMAIN_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.DOMAIN Then
                            'Domain = ReadStr(Read32(RowOffset + DOMAIN_POSITION_OFFSET, Accessor, FS), FS)
                            Domain = ReadStr(Read32_Row(Row, DOMAIN_POSITION_OFFSET), FS)
                        End If
                    End If
                    If USAGETYPE_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.USAGE_TYPE Then
                            'Usage_Type = ReadStr(Read32(RowOffset + USAGETYPE_POSITION_OFFSET, Accessor, FS), FS)
                            Usage_Type = ReadStr(Read32_Row(Row, USAGETYPE_POSITION_OFFSET), FS)
                        End If
                    End If
                    If ASN_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.ASN Then
                            'ASN = ReadStr(Read32(RowOffset + ASN_POSITION_OFFSET, Accessor, FS), FS)
                            ASN = ReadStr(Read32_Row(Row, ASN_POSITION_OFFSET), FS)
                        End If
                    End If
                    If AS_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.AS Then
                            '[AS] = ReadStr(Read32(RowOffset + AS_POSITION_OFFSET, Accessor, FS), FS)
                            [AS] = ReadStr(Read32_Row(Row, AS_POSITION_OFFSET), FS)
                        End If
                    End If
                    If LASTSEEN_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.LAST_SEEN Then
                            'Last_Seen = ReadStr(Read32(RowOffset + LASTSEEN_POSITION_OFFSET, Accessor, FS), FS)
                            Last_Seen = ReadStr(Read32_Row(Row, LASTSEEN_POSITION_OFFSET), FS)
                        End If
                    End If
                    If THREAT_ENABLED Then
                        If Mode = Modes.ALL OrElse Mode = Modes.THREAT Then
                            'Threat = ReadStr(Read32(RowOffset + THREAT_POSITION_OFFSET, Accessor, FS), FS)
                            Threat = ReadStr(Read32_Row(Row, THREAT_POSITION_OFFSET), FS)
                        End If
                    End If

                    If Country_Short = "-" OrElse Proxy_Type = "-" Then
                        Is_Proxy = 0
                    Else
                        If Proxy_Type = "DCH" OrElse Proxy_Type = "SES" Then
                            Is_Proxy = 2
                        Else
                            Is_Proxy = 1
                        End If
                    End If

                    With Result
                        .Is_Proxy = Is_Proxy
                        .Proxy_Type = Proxy_Type
                        .Country_Short = Country_Short
                        .Country_Long = Country_Long
                        .Region = Region
                        .City = City
                        .ISP = ISP
                        .Domain = Domain
                        .Usage_Type = Usage_Type
                        .ASN = ASN
                        .AS = [AS]
                        .Last_Seen = Last_Seen
                        .Threat = Threat
                    End With
                    Return Result
                Else
                    If IPNum < IPFrom Then
                        High = Mid - 1
                    Else
                        Low = Mid + 1
                    End If
                End If
            End While

            With Result
                .Is_Proxy = -1
                .Proxy_Type = MSG_INVALID_IP
                .Country_Short = MSG_INVALID_IP
                .Country_Long = MSG_INVALID_IP
                .Region = MSG_INVALID_IP
                .City = MSG_INVALID_IP
                .ISP = MSG_INVALID_IP
                .Domain = MSG_INVALID_IP
                .Usage_Type = MSG_INVALID_IP
                .ASN = MSG_INVALID_IP
                .AS = MSG_INVALID_IP
                .Last_Seen = MSG_INVALID_IP
                .Threat = MSG_INVALID_IP
            End With
            Return Result
        Catch ex As Exception
            ErrLog(ex.Message)
            Throw
        Finally
            Result = Nothing
            If Not FS Is Nothing Then
                FS.Close()
                FS.Dispose()
            End If
        End Try
    End Function

    ' Read whole row into array of bytes
    Private Function ReadRow(ByVal _Pos As Long, ByVal MyLen As UInt32, ByRef MyAccessor As MemoryMappedViewAccessor, ByRef MyFilestream As FileStream) As Byte()
        Dim row(MyLen - 1) As Byte

        If _UseMemoryMappedFile Then
            MyAccessor.ReadArray(Of Byte)(_Pos, row, 0, MyLen)
        Else
            MyFilestream.Seek(_Pos - 1, SeekOrigin.Begin)
            MyFilestream.Read(row, 0, MyLen)
        End If
        Return row
    End Function

    Private Function Read32Or128(ByVal _Pos As Long, ByVal _MyIPType As Integer, ByRef MyAccessor As MemoryMappedViewAccessor, ByRef MyFilestream As FileStream) As BigInteger
        If _MyIPType = 4 Then
            Return Read32(_Pos, MyAccessor, MyFilestream)
        ElseIf _MyIPType = 6 Then
            Return Read128(_Pos, MyAccessor, MyFilestream) ' only IPv6 will run this
        Else
            Return 0
        End If
    End Function

    ' Read 128 bits in the encrypted database
    Private Function Read128(ByVal _Pos As Long, ByRef MyAccessor As MemoryMappedViewAccessor, ByRef MyFilestream As FileStream) As BigInteger
        Try
            Dim BigRetVal As BigInteger = 0

            If _UseMemoryMappedFile Then
                BigRetVal = MyAccessor.ReadUInt64(_Pos + 8)
                BigRetVal <<= 64
                BigRetVal += MyAccessor.ReadUInt64(_Pos)
            Else
                Dim _Byte(15) As Byte ' 16 bytes
                MyFilestream.Seek(_Pos - 1, SeekOrigin.Begin)
                MyFilestream.Read(_Byte, 0, 16)
                BigRetVal = System.BitConverter.ToUInt64(_Byte, 8)
                BigRetVal <<= 64
                BigRetVal += System.BitConverter.ToUInt64(_Byte, 0)
            End If

            Return BigRetVal
        Catch Ex As Exception
            Throw
            'ErrLog("Read128-" & Ex.Message)
        End Try
    End Function

    ' Read 32 bits in byte array
    Private Function Read32_Row(ByRef Row() As Byte, ByVal ByteOffset As Integer) As BigInteger
        Try
            Dim _Byte(3) As Byte ' 4 bytes
            Array.Copy(Row, ByteOffset, _Byte, 0, 4)

            Return System.BitConverter.ToUInt32(_Byte, 0)
        Catch ex As Exception
            Throw
            'ErrLog("Read32_Row-" & ex.Message)
        End Try
    End Function

    ' Read 32 bits in the encrypted database
    Private Function Read32(ByVal _Pos As Long, ByRef MyAccessor As MemoryMappedViewAccessor, ByRef MyFilestream As FileStream) As BigInteger
        Try
            If _UseMemoryMappedFile Then
                Return MyAccessor.ReadUInt32(_Pos)
            Else
                Dim _Byte(3) As Byte ' 4 bytes
                MyFilestream.Seek(_Pos - 1, SeekOrigin.Begin)
                MyFilestream.Read(_Byte, 0, 4)

                Return System.BitConverter.ToUInt32(_Byte, 0)
            End If
        Catch Ex As Exception
            Throw
            'ErrLog("Read32-" & Ex.Message)
        End Try
    End Function

    ' Read strings in the encrypted database
    Private Function ReadStr(ByVal _Pos As Long, ByRef Myfilestream As FileStream) As String
        Try
            If _UseMemoryMappedFile Then
                Dim _Byte1 As Byte
                Dim _Bytes() As Byte
                _Pos -= _MapDataOffset
                _Byte1 = _MapDataAccessor.ReadByte(_Pos)
                ReDim _Bytes(_Byte1 - 1)
                _MapDataAccessor.ReadArray(_Pos + 1, _Bytes, 0, _Byte1)
                Return System.Text.Encoding.Default.GetString(_Bytes)
            Else
                Dim _Bytes(0) As Byte
                Dim _Bytes2() As Byte
                Myfilestream.Seek(_Pos, SeekOrigin.Begin)
                Myfilestream.Read(_Bytes, 0, 1)
                ReDim _Bytes2(_Bytes(0) - 1)
                Myfilestream.Read(_Bytes2, 0, _Bytes(0))
                Return System.Text.Encoding.Default.GetString(_Bytes2)
            End If
        Catch Ex As Exception
            Throw
            'ErrLog("ReadStr-" & Ex.Message)
        End Try
    End Function

    ' Description: Write any errors to log
    Private Sub ErrLog(ByVal StrMessage As String)
        Dim FS As FileStream = New FileStream(System.AppDomain.CurrentDomain.BaseDirectory & "ip2proxy_errLog.txt", FileMode.Append)
        Dim ObjWriter As StreamWriter = New StreamWriter(FS)
        ObjWriter.WriteLine(DateTime.Now() & "," & StrMessage)
        ObjWriter.Close()
        FS.Close()
    End Sub

    ' Description: Initialize
    Public Sub New()
    End Sub

    ' Description: Remove memory accessors
    Protected Overrides Sub Finalize()
        Close()
        MyBase.Finalize()
    End Sub

    ' Description: Destroy memory accessors & memory mapped file
    Public Function Close() As Integer
        Try
            DestroyAccessors()
            DestroyMemoryMappedFile()
            _BaseAddr = 0
            _DBCount = 0
            _DBColumn = 0
            _DBType = 0
            _DBDay = 1
            _DBMonth = 1
            _DBYear = 1
            _BaseAddrIPv6 = 0
            _DBCountIPv6 = 0
            _IndexBaseAddr = 0
            _IndexBaseAddrIPv6 = 0
            Return 0
        Catch Ex As Exception
            Return -1
        End Try
    End Function

    ' Description: Validate the IP address input
    Private Function VerifyIP(ByVal StrParam As String, ByRef StrIPType As Integer, ByRef IPNum As BigInteger) As String
        Try
            Dim Address As IPAddress = Nothing
            Dim FinalIP As String = ""

            'do checks for outlier cases here
            If _OutlierCase1.IsMatch(StrParam) OrElse _OutlierCase2.IsMatch(StrParam) Then 'good ip list outliers
                StrParam = "0000" & StrParam.Substring(1)
            End If

            If Not _OutlierCase3.IsMatch(StrParam) AndAlso Not _OutlierCase4.IsMatch(StrParam) AndAlso Not _OutlierCase5.IsMatch(StrParam) AndAlso IPAddress.TryParse(StrParam, Address) Then
                Select Case Address.AddressFamily
                    Case System.Net.Sockets.AddressFamily.InterNetwork
                        ' we have IPv4
                        StrIPType = 4
                        'Return address.ToString()
                    Case System.Net.Sockets.AddressFamily.InterNetworkV6
                        ' we have IPv6
                        StrIPType = 6
                        'Return address.ToString()
                    Case Else
                        Return "Invalid IP"
                End Select

                FinalIP = Address.ToString().ToUpper()

                'get ip number
                IPNum = IPNo(Address)

                If StrIPType = 6 Then
                    If IPNum >= _FromBI AndAlso IPNum <= _ToBI Then
                        'ipv4-mapped ipv6 should treat as ipv4 and read ipv4 data section
                        StrIPType = 4
                        IPNum = IPNum - _FromBI

                        'expand ipv4-mapped ipv6
                        If _IPv4MappedRegex.IsMatch(FinalIP) Then
                            Dim Tmp As String = String.Join("", Enumerable.Repeat("0000:", 5).ToList)
                            FinalIP = FinalIP.Replace("::", Tmp)
                        ElseIf _IPv4MappedRegex2.IsMatch(FinalIP) Then
                            Dim MyMatch As RegularExpressions.Match = _IPv4MappedRegex2.Match(FinalIP)
                            Dim x As Integer = 0

                            Dim Tmp As String = MyMatch.Groups(1).ToString()
                            Dim TmpArr() As String = Tmp.Trim(":").Split(":")
                            Dim Len As Integer = TmpArr.Length - 1
                            For x = 0 To Len
                                TmpArr(x) = TmpArr(x).PadLeft(4, "0")
                            Next
                            Dim MyRear As String = String.Join("", TmpArr)
                            Dim Bytes As Byte()

                            Bytes = BitConverter.GetBytes(Convert.ToInt32("0x" & MyRear, 16))
                            FinalIP = FinalIP.Replace(Tmp, ":" & Bytes(3) & "." & Bytes(2) & "." & Bytes(1) & "." & Bytes(0))
                            Tmp = String.Join("", Enumerable.Repeat("0000:", 5).ToList)
                            FinalIP = FinalIP.Replace("::", Tmp)
                        End If
                    ElseIf IPNum >= _FromBI2 AndAlso IPNum <= _ToBI2 Then
                        '6to4 so need to remap to ipv4
                        StrIPType = 4

                        IPNum = IPNum >> 80
                        IPNum = IPNum And _DivBI ' get last 32 bits
                    ElseIf IPNum >= _FromBI3 AndAlso IPNum <= _ToBI3 Then
                        'Teredo so need to remap to ipv4
                        StrIPType = 4

                        IPNum = Not IPNum
                        IPNum = IPNum And _DivBI ' get last 32 bits
                    ElseIf IPNum <= MAX_IPV4_RANGE Then
                        'ipv4-compatible ipv6 (DEPRECATED BUT STILL SUPPORTED BY .NET)
                        StrIPType = 4

                        If _IPv4CompatibleRegex.IsMatch(FinalIP) Then
                            Dim Bytes As Byte() = BitConverter.GetBytes(Convert.ToInt32(FinalIP.Replace("::", "0x"), 16))
                            FinalIP = "::" & Bytes(3) & "." & Bytes(2) & "." & Bytes(1) & "." & Bytes(0)
                        ElseIf FinalIP = "::" Then
                            FinalIP = FinalIP & "0.0.0.0"
                        End If
                        Dim Tmp As String = String.Join("", Enumerable.Repeat("0000:", 5).ToList)
                        FinalIP = FinalIP.Replace("::", Tmp & "FFFF:")
                    Else
                        'expand ipv6 normal
                        Dim MyArr() As String = Regex.Split(FinalIP, "::")
                        Dim x As Integer = 0
                        Dim LeftSide As New List(Of String)
                        LeftSide.AddRange(MyArr(0).Split(":"))

                        If MyArr.Length > 1 Then
                            Dim RightSide As New List(Of String)
                            RightSide.AddRange(MyArr(1).Split(":"))

                            Dim MidArr As List(Of String)
                            MidArr = Enumerable.Repeat("0000", 8 - LeftSide.Count - RightSide.Count).ToList

                            RightSide.InsertRange(0, MidArr)
                            RightSide.InsertRange(0, LeftSide)

                            Dim RLen As Integer = RightSide.Count - 1
                            For x = 0 To RLen
                                RightSide.Item(x) = RightSide.Item(x).PadLeft(4, "0")
                            Next

                            FinalIP = String.Join(":", RightSide)
                        Else
                            Dim LLen As Integer = LeftSide.Count - 1
                            For x = 0 To LLen
                                LeftSide.Item(x) = LeftSide.Item(x).PadLeft(4, "0")
                            Next

                            FinalIP = String.Join(":", LeftSide)
                        End If
                    End If

                End If

                Return FinalIP
            Else
                Return "Invalid IP"
            End If
        Catch Ex As Exception
            Return "Invalid IP"
        End Try
    End Function

    ' Description: Convert either IPv4 or IPv6 into big integer
    Private Function IPNo(ByRef IPAddress As IPAddress) As BigInteger
        Try
            Dim AddrBytes() As Byte = IPAddress.GetAddressBytes()
            LittleEndian(AddrBytes)

            Dim Final As BigInteger

            If AddrBytes.Length > 8 Then
                'IPv6
                Final = System.BitConverter.ToUInt64(AddrBytes, 8)
                Final <<= 64
                Final += System.BitConverter.ToUInt64(AddrBytes, 0)
            Else
                'IPv4
                Final = System.BitConverter.ToUInt32(AddrBytes, 0)
            End If

            Return Final
        Catch Ex As Exception
            Return 0
        End Try
    End Function

End Class