# Quickstart

## Requirements

Microsoft .NET 4.72 framework or later.
Compatible with .NET Core 2.x/3.x SDK.
Compatible with .NET 5/6/7/8.

## Dependencies

This library requires IP2Proxy BIN database to function. You may download the BIN database at

-   IP2Proxy LITE BIN Data (Free): <https://lite.ip2location.com>
-   IP2Proxy Commercial BIN Data (Comprehensive):
    <https://www.ip2location.com>

## Sample Codes

### Query proxy information from BIN database

You can query the proxy information from the IP2Proxy BIN database as below:

```vb.net
Imports System.IO
Imports IP2Proxy

Dim proxy As New Component
Dim all As ProxyResult

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

If proxy.Open("C:\data\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP-DOMAIN-USAGETYPE-ASN-LASTSEEN-THREAT-RESIDENTIAL-PROVIDER.BIN", Component.IOModes.IP2PROXY_MEMORY_MAPPED) = 0 Then
	Console.WriteLine("GetModuleVersion: " & Component.GetModuleVersion())
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

### Query proxy information using a stream and async IP query

You can query the proxy information using a stream and async IP query as below:

```vb.net
Imports System.IO
Imports IP2Proxy

Dim proxy As New Component
Dim all As ProxyResult

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

Using myStream = New FileStream("C:\data\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP-DOMAIN-USAGETYPE-ASN-LASTSEEN-THREAT-RESIDENTIAL-PROVIDER.BIN", FileMode.Open, FileAccess.Read, FileShare.Read)
	If proxy.Open(myStream) = 0 Then
		Console.WriteLine("GetModuleVersion: " & Component.GetModuleVersion())
		Console.WriteLine("GetPackageVersion: " & proxy.GetPackageVersion())
		Console.WriteLine("GetDatabaseVersion: " & proxy.GetDatabaseVersion())

		' reading all available fields
		all = proxy.GetAllAsync(ip).Result
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
		isproxy = proxy.IsProxyAsync(ip).Result
		Console.WriteLine("Is_Proxy: " & isproxy.ToString())

		proxytype = proxy.GetProxyTypeAsync(ip).Result
		Console.WriteLine("Proxy_Type: " & proxytype)

		countryshort = proxy.GetCountryShortAsync(ip).Result
		Console.WriteLine("Country_Short: " & countryshort)

		countrylong = proxy.GetCountryLongAsync(ip).Result
		Console.WriteLine("Country_Long: " & countrylong)

		region = proxy.GetRegionAsync(ip).Result
		Console.WriteLine("Region: " & region)

		city = proxy.GetCityAsync(ip).Result
		Console.WriteLine("City: " & city)

		isp = proxy.GetISPAsync(ip).Result
		Console.WriteLine("ISP: " & isp)

		domain = proxy.GetDomainAsync(ip).Result
		Console.WriteLine("Domain: " & domain)

		usagetype = proxy.GetUsageTypeAsync(ip).Result
		Console.WriteLine("Usage_Type: " & usagetype)

		asn = proxy.GetASNAsync(ip).Result
		Console.WriteLine("ASN: " & asn)

		[as] = proxy.GetASAsync(ip).Result
		Console.WriteLine("AS: " & [as])

		lastseen = proxy.GetLastSeenAsync(ip).Result
		Console.WriteLine("Last_Seen: " & lastseen)

		threat = proxy.GetThreatAsync(ip).Result
		Console.WriteLine("Threat: " & threat)

		provider = proxy.GetProviderAsync(ip).Result
		Console.WriteLine("Provider: " & provider)
	Else
		Console.WriteLine("Error reading BIN file.")
	End If
	proxy.Close()
End Using
```