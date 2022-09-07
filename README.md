# IP2Proxy .NET Component

This component allows user to query an IP address if it was being used as VPN anonymizer, open proxies, web proxies, Tor exits, data center, web hosting (DCH) range, search engine robots (SES) and residential (RES). It lookup the proxy IP address from **IP2Proxy BIN Data** file. This data file can be downloaded at

* Free IP2Proxy BIN Data: https://lite.ip2location.com
* Commercial IP2Proxy BIN Data: https://www.ip2location.com/database/ip2proxy

As an alternative, this component can also call the IP2Proxy Web Service. This requires an API key. If you don't have an existing API key, you can subscribe for one at the below:

https://www.ip2location.com/web-service/ip2proxy

## Requirements

Microsoft .NET 4.72 framework or later.
Compatible with .NET Core 2.x/3.x SDK.
Compatible with .NET 5/6.

## QUERY USING THE BIN FILE

## Methods
Below are the methods supported in this class.

|Method Name|Description|
|---|---|
|Open|Open the IP2Proxy BIN data for lookup. Please see the **Usage** section of the 2 modes supported to load the BIN data file.|
|Close|Close and clean up the file pointer.|
|GetPackageVersion|Get the package version (1 to 11 for PX1 to PX11 respectively).|
|GetModuleVersion|Get the module version.|
|GetDatabaseVersion|Get the database version.|
|IsProxy|Check whether if an IP address was a proxy. Returned value:<ul><li>-1 : errors</li><li>0 : not a proxy</li><li>1 : a proxy</li><li>2 : a data center IP address or search engine robot</li></ul>|
|GetAll|Return the proxy information in an object.|
|GetProxyType|Return the proxy type. Please visit <a href="https://www.ip2location.com/database/px10-ip-proxytype-country-region-city-isp-domain-usagetype-asn-lastseen-threat-residential" target="_blank">IP2Location</a> for the list of proxy types supported|
|GetCountryShort|Return the ISO3166-1 country code (2-digits) of the proxy.|
|GetCountryLong|Return the ISO3166-1 country name of the proxy.|
|GetRegion|Return the ISO3166-2 region name of the proxy. Please visit <a href="https://www.ip2location.com/free/iso3166-2" target="_blank">ISO3166-2 Subdivision Code</a> for the information of ISO3166-2 supported|
|GetCity|Return the city name of the proxy.|
|GetISP|Return the ISP name of the proxy.|
|GetDomain|Return the domain name of the proxy.|
|GetUsageType|Return the usage type classification of the proxy. Please visit <a href="https://www.ip2location.com/database/px10-ip-proxytype-country-region-city-isp-domain-usagetype-asn-lastseen-threat-residential" target="_blank">IP2Location</a> for the list of usage types supported.|
|GetASN|Return the autonomous system number of the proxy.|
|GetAS|Return the autonomous system name of the proxy.|
|GetLastSeen|Return the number of days that the proxy was last seen.|
|GetThreat|Return the threat type of the proxy.|
|GetProvider|Return the provider of the proxy.|

## Usage

Open and read IP2Proxy binary database. There are 2 modes:

1. **IOModes.IP2PROXY_FILE_IO** - File I/O reading. Slower lookup, but low resource consuming. This is the default.
2. **IOModes.IP2PROXY_MEMORY_MAPPED** - Stores whole IP2Proxy database into a memory-mapped file. Extremely resources consuming. Do not use this mode if your system do not have enough memory.

```vb.net
Dim proxy As New IP2Proxy.Component
Dim all As IP2Proxy.ProxyResult

Dim isproxy As Integer
Dim proxytype As String
Dim countryshort As String
Dim countrylong As String
Dim region As String
Dim city As String
Dim isp As String
Dim domain As String
Dim usagetype As String
Dim asn As String
Dim [as] As String
Dim lastseen As String
Dim threat As String
Dim provider As String

Dim ip As String = "221.121.146.0"

If proxy.Open("C:\data\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP-DOMAIN-USAGETYPE-ASN-LASTSEEN-THREAT-RESIDENTIAL-PROVIDER.BIN", IP2Proxy.Component.IOModes.IP2PROXY_MEMORY_MAPPED) = 0 Then
	Console.WriteLine("GetModuleVersion: " & proxy.GetModuleVersion())
	Console.WriteLine("GetPackageVersion: " & proxy.GetPackageVersion())
	Console.WriteLine("GetDatabaseVersion: " & proxy.GetDatabaseVersion())

	' reading all available fields
	all = proxy.GetAll(ip)
	Console.WriteLine("Is_Proxy: " & all.Is_Proxy.ToString())
	Console.WriteLine("Proxy_Type: " & all.Proxy_Type)
	Console.WriteLine("Country_Short: " & all.Country_Short)
	Console.WriteLine("Country_Long: " & all.Country_Long)
	Console.WriteLine("Region: " & all.Region)
	Console.WriteLine("City: " & all.City)
	Console.WriteLine("ISP: " & all.ISP)
	Console.WriteLine("Domain: " & all.Domain)
	Console.WriteLine("Usage_Type: " & all.Usage_Type)
	Console.WriteLine("ASN: " & all.ASN)
	Console.WriteLine("AS: " & all.AS)
	Console.WriteLine("Last_Seen: " & all.Last_Seen)
	Console.WriteLine("Threat: " & all.Threat)
	Console.WriteLine("Provider: " & all.Provider)

	' reading individual fields
	isproxy = proxy.IsProxy(ip)
	Console.WriteLine("Is_Proxy: " & isproxy.ToString())

	proxytype = proxy.GetProxyType(ip)
	Console.WriteLine("Proxy_Type: " & proxytype)

	countryshort = proxy.GetCountryShort(ip)
	Console.WriteLine("Country_Short: " & countryshort)

	countrylong = proxy.GetCountryLong(ip)
	Console.WriteLine("Country_Long: " & countrylong)

	region = proxy.GetRegion(ip)
	Console.WriteLine("Region: " & region)

	city = proxy.GetCity(ip)
	Console.WriteLine("City: " & city)

	isp = proxy.GetISP(ip)
	Console.WriteLine("ISP: " & isp)

	domain = proxy.GetDomain(ip)
	Console.WriteLine("Domain: " & domain)

	usagetype = proxy.GetUsageType(ip)
	Console.WriteLine("Usage_Type: " & usagetype)

	asn = proxy.GetASN(ip)
	Console.WriteLine("ASN: " & asn)

	[as] = proxy.GetAS(ip)
	Console.WriteLine("AS: " & [as])

	lastseen = proxy.GetLastSeen(ip)
	Console.WriteLine("Last_Seen: " & lastseen)

	threat = proxy.GetThreat(ip)
	Console.WriteLine("Threat: " & threat)

	provider = proxy.GetProvider(ip)
	Console.WriteLine("Provider: " & provider)
Else
	Console.WriteLine("Error reading BIN file.")
End If
proxy.Close()

```

## QUERY USING THE IP2PROXY PROXY DETECTION WEB SERVICE

## Methods
Below are the methods supported in this class.

|Method Name|Description|
|---|---|
|Open(ByVal APIKey As String, ByVal Package As String, ByVal Optional UseSSL As Boolean = True)| Expects 2 or 3 input parameters:<ol><li>IP2Proxy API Key.</li><li>Package (PX1 - PX11)</li></li><li>Use HTTPS or HTTP</li></ol> |
|IPQuery(ByVal IP As String)|Query IP address. This method returns a JObject containing the proxy info. <ul><li>countryCode</li><li>countryName</li><li>regionName</li><li>cityName</li><li>isp</li><li>domain</li><li>usageType</li><li>asn</li><li>as</li><li>lastSeen</li><li>threat</li><li>proxyType</li><li>isProxy</li><li>provider</li><ul>|
|GetCredit()|This method returns the web service credit balance in a JObject.|

## Usage

```vb.net
Imports System.IO

Module TestIP2Proxy

    Sub Main()
        Dim proxyws As New IP2Proxy.ComponentWebService()
        Dim ip = "221.121.146.0"
        Dim apikey = "YOUR_API_KEY"
        Dim package = "PX11"
        Dim ssl = True

        proxyws.Open(apikey, package, ssl)
        Dim myresult = proxyws.IPQuery(ip)

        If myresult.ContainsKey("response") Then
            If myresult("response").ToString <> "OK" Then
                Console.WriteLine("Error: " & myresult("response").ToString)
            Else
                Console.WriteLine("countryCode: " & If(myresult.ContainsKey("countryCode"), myresult("countryCode").ToString, ""))
                Console.WriteLine("countryName: " & If(myresult.ContainsKey("countryName"), myresult("countryName").ToString, ""))
                Console.WriteLine("regionName: " & If(myresult.ContainsKey("regionName"), myresult("regionName").ToString, ""))
                Console.WriteLine("cityName: " & If(myresult.ContainsKey("cityName"), myresult("cityName").ToString, ""))
                Console.WriteLine("isp: " & If(myresult.ContainsKey("isp"), myresult("isp").ToString, ""))
                Console.WriteLine("domain: " & If(myresult.ContainsKey("domain"), myresult("domain").ToString, ""))
                Console.WriteLine("usageType: " & If(myresult.ContainsKey("usageType"), myresult("usageType").ToString, ""))
                Console.WriteLine("asn: " & If(myresult.ContainsKey("asn"), myresult("asn").ToString, ""))
                Console.WriteLine("as: " & If(myresult.ContainsKey("as"), myresult("as").ToString, ""))
                Console.WriteLine("lastSeen: " & If(myresult.ContainsKey("lastSeen"), myresult("lastSeen").ToString, ""))
                Console.WriteLine("proxyType: " & If(myresult.ContainsKey("proxyType"), myresult("proxyType").ToString, ""))
                Console.WriteLine("threat: " & If(myresult.ContainsKey("threat"), myresult("threat").ToString, ""))
                Console.WriteLine("isProxy: " & If(myresult.ContainsKey("isProxy"), myresult("isProxy").ToString, ""))
                Console.WriteLine("provider: " & If(myresult.ContainsKey("provider"), myresult("provider").ToString, ""))
            End If
        End If

        myresult = proxyws.GetCredit()
        If myresult.ContainsKey("response") Then
            Console.WriteLine("Credit balance: " & If(myresult.ContainsKey("response"), myresult("response").ToString, ""))
        End If
    End Sub

End Module
```

### Proxy Type

|Proxy Type|Description|
|---|---|
|VPN|Anonymizing VPN services|
|TOR|Tor Exit Nodes|
|PUB|Public Proxies|
|WEB|Web Proxies|
|DCH|Hosting Providers/Data Center|
|SES|Search Engine Robots|
|RES|Residential Proxies [PX10+]|

### Usage Type

|Usage Type|Description|
|---|---|
|COM|Commercial|
|ORG|Organization|
|GOV|Government|
|MIL|Military|
|EDU|University/College/School|
|LIB|Library|
|CDN|Content Delivery Network|
|ISP|Fixed Line ISP|
|MOB|Mobile ISP|
|DCH|Data Center/Web Hosting/Transit|
|SES|Search Engine Spider|
|RSV|Reserved|

### Threat Type

|Threat Type|Description|
|---|---|
|SPAM|Spammer|
|SCANNER|Security Scanner or Attack|
|BOTNET|Spyware or Malware|
