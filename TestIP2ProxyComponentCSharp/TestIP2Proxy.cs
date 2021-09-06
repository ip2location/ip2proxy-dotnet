using System;
using Newtonsoft.Json.Linq;

namespace TestIP2ProxyComponent
{
    class TestIP2Proxy
    {
        static void Main(string[] args)
        {
            // Query using BIN file
            IP2Proxy.Component proxy = new IP2Proxy.Component();
            IP2Proxy.ProxyResult all;

            int isproxy;
            string proxytype;
            string countryshort;
            string countrylong;
            string region;
            string city;
            string isp;
            string domain;
            string usagetype;
            string asn;
            string @as;
            string lastseen;
            string threat;
            string provider;

            string ip = "221.121.146.0";

            if (proxy.Open("C:\\data\\IP2PROXY-IP-PROXYTYPE-COUNTRY-REGION-CITY-ISP-DOMAIN-USAGETYPE-ASN-LASTSEEN-THREAT-RESIDENTIAL-PROVIDER.BIN", IP2Proxy.Component.IOModes.IP2PROXY_MEMORY_MAPPED) == 0)
            {
                Console.WriteLine("GetModuleVersion: " + proxy.GetModuleVersion());
                Console.WriteLine("GetPackageVersion: " + proxy.GetPackageVersion());
                Console.WriteLine("GetDatabaseVersion: " + proxy.GetDatabaseVersion());

                // reading all available fields
                all = proxy.GetAll(ip);
                Console.WriteLine("Is_Proxy: " + all.Is_Proxy.ToString());
                Console.WriteLine("Proxy_Type: " + all.Proxy_Type);
                Console.WriteLine("Country_Short: " + all.Country_Short);
                Console.WriteLine("Country_Long: " + all.Country_Long);
                Console.WriteLine("Region: " + all.Region);
                Console.WriteLine("City: " + all.City);
                Console.WriteLine("ISP: " + all.ISP);
                Console.WriteLine("Domain: " + all.Domain);
                Console.WriteLine("Usage_Type: " + all.Usage_Type);
                Console.WriteLine("ASN: " + all.ASN);
                Console.WriteLine("AS: " + all.AS);
                Console.WriteLine("Last_Seen: " + all.Last_Seen);
                Console.WriteLine("Threat: " + all.Threat);
                Console.WriteLine("Provider: " + all.Provider);

                // reading individual fields
                isproxy = proxy.IsProxy(ip);
                Console.WriteLine("Is_Proxy: " + isproxy.ToString());

                proxytype = proxy.GetProxyType(ip);
                Console.WriteLine("Proxy_Type: " + proxytype);

                countryshort = proxy.GetCountryShort(ip);
                Console.WriteLine("Country_Short: " + countryshort);

                countrylong = proxy.GetCountryLong(ip);
                Console.WriteLine("Country_Long: " + countrylong);

                region = proxy.GetRegion(ip);
                Console.WriteLine("Region: " + region);

                city = proxy.GetCity(ip);
                Console.WriteLine("City: " + city);

                isp = proxy.GetISP(ip);
                Console.WriteLine("ISP: " + isp);

                domain = proxy.GetDomain(ip);
                Console.WriteLine("Domain: " + domain);

                usagetype= proxy.GetUsageType(ip);
                Console.WriteLine("Usage_Type: " + usagetype);

                asn = proxy.GetASN(ip);
                Console.WriteLine("ASN: " + asn);

                @as = proxy.GetAS(ip);
                Console.WriteLine("AS: " + @as);

                lastseen = proxy.GetLastSeen(ip);
                Console.WriteLine("Last_Seen: " + lastseen);

                threat = proxy.GetThreat(ip);
                Console.WriteLine("Threat: " + threat);

                provider = proxy.GetProvider(ip);
                Console.WriteLine("Provider: " + provider);
            }
            else
            {
                Console.WriteLine("Error reading BIN file.");
            }
            proxy.Close();
            
            // Query using web service
            IP2Proxy.ComponentWebService proxyws = new IP2Proxy.ComponentWebService();
            string apikey = "YOUR_API_KEY";
            string package = "PX11";
            bool ssl = true;

            proxyws.Open(apikey, package, ssl);
            JObject myresult = proxyws.IPQuery(ip);

            if (myresult["response"] != null)
            {
                if (myresult["response"].ToString() != "OK")
                {
                    Console.WriteLine("Error: " + myresult["response"]);
                }
                else
                {
                    Console.WriteLine("countryCode: " + (myresult["countryCode"] ?? ""));
                    Console.WriteLine("countryName: " + (myresult["countryName"] ?? ""));
                    Console.WriteLine("regionName: " + (myresult["regionName"] ?? ""));
                    Console.WriteLine("cityName: " + (myresult["cityName"] ?? ""));
                    Console.WriteLine("isp: " + (myresult["isp"] ?? ""));
                    Console.WriteLine("domain: " + (myresult["domain"] ?? ""));
                    Console.WriteLine("usageType: " + (myresult["usageType"] ?? ""));
                    Console.WriteLine("asn: " + (myresult["asn"] ?? ""));
                    Console.WriteLine("as: " + (myresult["as"] ?? ""));
                    Console.WriteLine("lastSeen: " + (myresult["lastSeen"] ?? ""));
                    Console.WriteLine("proxyType: " + (myresult["proxyType"] ?? ""));
                    Console.WriteLine("threat: " + (myresult["threat"] ?? ""));
                    Console.WriteLine("isProxy: " + (myresult["isProxy"] ?? ""));
                    Console.WriteLine("provider: " + (myresult["provider"] ?? ""));
                }
            }

            myresult = proxyws.GetCredit();
            if (myresult["response"] != null)
            {
                Console.WriteLine("Credit balance: " + (myresult["response"] ?? ""));
            }

        }
    }
}
