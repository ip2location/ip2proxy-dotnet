namespace TestIP2ProxyComponent
{
    class TestIP2Proxy
    {
        static void Main(string[] args)
        {
            IP2Proxy.Component proxy = new IP2Proxy.Component();
            IP2Proxy.ProxyResult all;

            int isproxy;
            string proxytype;
            string countryshort;
            string countrylong;
            string region;
            string city;
            string isp;

            string ip = "221.121.146.0";

            if (proxy.Open("C:\\data\\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP.BIN", IP2Proxy.Component.IOModes.IP2PROXY_MEMORY_MAPPED) == 0)
            {
                System.Console.WriteLine("GetModuleVersion: " + proxy.GetModuleVersion());
                System.Console.WriteLine("GetPackageVersion: " + proxy.GetPackageVersion());
                System.Console.WriteLine("GetDatabaseVersion: " + proxy.GetDatabaseVersion());

                // reading all available fields
                all = proxy.GetAll(ip);
                System.Console.WriteLine("Is_Proxy: " + all.Is_Proxy.ToString());
                System.Console.WriteLine("Proxy_Type: " + all.Proxy_Type);
                System.Console.WriteLine("Country_Short: " + all.Country_Short);
                System.Console.WriteLine("Country_Long: " + all.Country_Long);
                System.Console.WriteLine("Region: " + all.Region);
                System.Console.WriteLine("City: " + all.City);
                System.Console.WriteLine("ISP: " + all.ISP);

                // reading individual fields
                isproxy = proxy.IsProxy(ip);
                System.Console.WriteLine("Is_Proxy: " + isproxy.ToString());

                proxytype = proxy.GetProxyType(ip);
                System.Console.WriteLine("Proxy_Type: " + proxytype);

                countryshort = proxy.GetCountryShort(ip);
                System.Console.WriteLine("Country_Short: " + countryshort);

                countrylong = proxy.GetCountryLong(ip);
                System.Console.WriteLine("Country_Long: " + countrylong);

                region = proxy.GetRegion(ip);
                System.Console.WriteLine("Region: " + region);

                city = proxy.GetCity(ip);
                System.Console.WriteLine("City: " + city);

                isp = proxy.GetISP(ip);
                System.Console.WriteLine("ISP: " + isp);
            }
            else
            {
                System.Console.WriteLine("Error reading BIN file.");
            }
            proxy.Close();

        }
    }
}
