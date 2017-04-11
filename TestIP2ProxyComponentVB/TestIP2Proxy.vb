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

        Dim ip As String = "221.121.146.0"

        If proxy.Open("C:\data\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP.BIN", IP2Proxy.Component.IOModes.IP2PROXY_MEMORY_MAPPED) = 0 Then
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


    End Sub

End Module
