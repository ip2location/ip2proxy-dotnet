Imports System.IO

Module TestIP2Proxy

    Sub Main()
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

        Dim ip As String = "46.101.133.66"

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


    End Sub

End Module
