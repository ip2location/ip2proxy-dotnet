# IP2Proxy .NET Component

This component allows user to query an IP address if it was being used as open proxy, web proxy, VPN anonymizer and TOR exits. It lookup the proxy IP address from **IP2Proxy BIN Data** file. This data file can be downloaded at

* Free IP2Proxy BIN Data: https://lite.ip2location.com
* Commercial IP2Proxy BIN Data: https://www.ip2location.com/proxy-database

## Requirements

Visual Studio 2012 or later.
Microsoft .NET 4.0 framework or later.

## Methods
Below are the methods supported in this class.

|Method Name|Description|
|---|---|
|Open|Open the IP2Proxy BIN data for lookup. Please see the **Usage** section of the 2 modes supported to load the BIN data file.|
|Close|Close and clean up the file pointer.|
|GetPackageVersion|Get the package version (1 to 4 for PX1 to PX4 respectively).|
|GetModuleVersion|Get the module version.|
|GetDatabaseVersion|Get the database version.|
|IsProxy|Check whether if an IP address was a proxy. Returned value:<ul><li>-1 : errors</li><li>0 : not a proxy</li><li>1 : a proxy</li><li>2 : a data center IP address</li></ul>|
|GetAll|Return the proxy information in an object.|
|GetProxyType|Return the proxy type. Please visit <a href="https://www.ip2location.com/databases/px4-ip-proxytype-country-region-city-isp" target="_blank">IP2Location</a> for the list of proxy types supported|
|GetCountryShort|Return the ISO3166-1 country code (2-digits) of the proxy.|
|GetCountryLong|Return the ISO3166-1 country name of the proxy.|
|GetRegion|Return the ISO3166-2 region name of the proxy. Please visit <a href="https://www.ip2location.com/free/iso3166-2" target="_blank">ISO3166-2 Subdivision Code</a> for the information of ISO3166-2 supported|
|GetCity|Return the city name of the proxy.|
|GetISP|Return the ISP name of the proxy.|

## Usage

Open and read IP2Proxy binary database. There are 2 modes:

1. **IOModes.IP2PROXY_FILE_IO** - File I/O reading. Slower look, but low resource consuming. This is the default.
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

Dim ip As String = "221.121.146.0"

If proxy.Open("C:\samples\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP.SAMPLE.BIN", IP2Proxy.Component.IOModes.IP2PROXY_MEMORY_MAPPED) = 0 Then
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
Else
	Console.WriteLine("Error reading BIN file.")
End If
proxy.Close()

```
