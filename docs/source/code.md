# IP2Proxy .NET API

## Component Class
```{py:function} Open(DatabasePath, IOMode)
Load the IP2Proxy BIN database for lookup.

:param String DatabasePath: (Required) The file path links to IP2Proxy BIN databases.
:param Enum IOMode: (Optional) Specify which mode to use when loading the BIN database. Available values are IOModes.IP2PROXY_FILE_IO and IOModes.IP2PROXY_MEMORY_MAPPED. Default is IOModes.IP2PROXY_FILE_IO.
```

```{py:function} Open(DBStream)
Initialize component with a stream that contains the BIN database then preload BIN file.

:param Stream DBStream: (Required) A stream that contains the BIN database.
```

```{py:function} Close()
Close and clean up the file pointer.
```

```{py:function} GetPackageVersion()
Return the database's type, 1 to 12 respectively for PX1 to PX12. Please visit https://www.ip2location.com/databases/ip2proxy for details.

:return: Returns the package version.
:rtype: String
```

```{py:function} GetModuleVersion()
Return the version of module.

:return: Returns the module version.
:rtype: String
```

```{py:function} GetDatabaseVersion()
Return the database's compilation date as a string of the form 'YYYY-MM-DD'.

:return: Returns the database version.
:rtype: String
```

```{py:function} GetAll(IP)
Retrieve geolocation information for an IP address.

:param String IP: (Required) The IP address (IPv4 or IPv6).
:return: Returns the geolocation information in array. Refer below table for the fields avaliable in the array
:rtype: array

**RETURN FIELDS**

| Field Name       | Description                                                  |
| ---------------- | ------------------------------------------------------------ |
| Is_Proxy    |     Determine whether if an IP address was a proxy or not. Returns 0 is not proxy, 1 if proxy, and 2 if it's data center IP |
| Country_Short    |     Two-character country code based on ISO 3166. |
| Country_Long    |     Country name based on ISO 3166. |
| Region     |     Region or state name. |
| City       |     City name. |
| Isp            |     Internet Service Provider or company\'s name. |
| Domain         |     Internet domain name associated with IP address range. |
| Usage_Type      |     Usage type classification of ISP or company. |
| Asn            |     Autonomous system number (ASN). |
| As             |     Autonomous system (AS) name. |
| Last_Seen       |     Proxy last seen in days. |
| Threat         |     Security threat reported. |
| Proxy_Type      |     Type of proxy. |
| Provider       |     Name of VPN provider if available. |
| Fraud_Score       |     Potential risk score (0 - 99) associated with IP address. |
```

```{py:function} GetAllAsync(IP)
Retrieve geolocation information for an IP address.

:param String IP: (Required) The IP address (IPv4 or IPv6).
:return: Returns the geolocation information in array. Refer below table for the fields avaliable in the array
:rtype: array

**RETURN FIELDS**

| Field Name       | Description                                                  |
| ---------------- | ------------------------------------------------------------ |
| Is_Proxy    |     Determine whether if an IP address was a proxy or not. Returns 0 is not proxy, 1 if proxy, and 2 if it's data center IP |
| Country_Short    |     Two-character country code based on ISO 3166. |
| Country_Long    |     Country name based on ISO 3166. |
| Region     |     Region or state name. |
| City       |     City name. |
| Isp            |     Internet Service Provider or company\'s name. |
| Domain         |     Internet domain name associated with IP address range. |
| Usage_Type      |     Usage type classification of ISP or company. |
| Asn            |     Autonomous system number (ASN). |
| As             |     Autonomous system (AS) name. |
| Last_Seen       |     Proxy last seen in days. |
| Threat         |     Security threat reported. |
| Proxy_Type      |     Type of proxy. |
| Provider       |     Name of VPN provider if available. |
| Fraud_Score       |     Potential risk score (0 - 99) associated with IP address. |
```