#region library references

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace InfoBlox.Automation.Model
{
    #region  InfoBlox Models

    public partial class InfobloxNetwork
    {
        private string refNetwork;

        [JsonProperty("_ref")]
        public string Ref
        {
            get
            {
                return refNetwork;
            }
            set
            {
                refNetwork = value;
                ExtractBaseRef();
            }
        }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("network_view")]
        public string NetworkView { get; set; }
    }
    public partial class InfobloxNetwork
    {
        private string baseRef;
        public static InfobloxNetwork FromJson(string json) => JsonConvert.DeserializeObject<InfobloxNetwork>(json, Converter.Settings); //TODO - change here
        public string BaseRef
        {
            get
            {
                return baseRef;
            }
        }

        // Extracts the unique reference id for the InfoBlox network object
        private void ExtractBaseRef()
        {
            int _startExtract = refNetwork.IndexOf("/", 0);

            int _endExtract = refNetwork.IndexOf(":", _startExtract);

            if (refNetwork.Length > 0 && _startExtract >= 1 && _endExtract >= 0)
            {
                baseRef = refNetwork.Substring(_startExtract + 1, ((_endExtract - _startExtract) - 1));
            }
        }
    }
    public partial class InfobloxNetworks : List<InfobloxNetwork>
    {
        public static InfobloxNetworks FromJson(string json) => JsonConvert.DeserializeObject<InfobloxNetworks>(json, Converter.Settings); //TODO - change here
    }

    public partial class IpRequest
    {
        private int number;

        public IpRequest(int requestnumber)
        {
            number = requestnumber;
        }

        [JsonProperty("num")]
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }
    }
    public partial class IpRequest
    {
        public static IpRequest FromJson(string json) => JsonConvert.DeserializeObject<IpRequest>(json, Converter.Settings);
    }
    public partial class IpResult
    {
        [JsonProperty("ips")]
        //public string[] Ips { get; set; }
        public List<string> IPAddresses { get; set; }
    }
    public partial class IpResult
    {
        public static IpResult FromJson(string json) => JsonConvert.DeserializeObject<IpResult>(json, Converter.Settings);
    }
    public partial class HostRecordPost
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ipv4addrs")]
        public Ipv4AddressPost[] Ipv4Addresses { get; set; }
    }
    public partial class HostRecordPost
    {
        public static HostRecord FromJson(string json) => JsonConvert.DeserializeObject<HostRecord>(json, Converter.Settings);
    }
    public partial class HostRecord : HostRecordPost
    {
        [JsonProperty("ipv4addrs")]
        new public Ipv4Address[] Ipv4Addresses { get; set; }

        [JsonProperty("_ref")]
        public string Reference { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }
    }
    public partial class HostRecord
    {
        new public static HostRecord FromJson(string json) => JsonConvert.DeserializeObject<HostRecord>(json, Converter.Settings); //TODO: Fix the Deserialization of IP addresses.
    }
    public partial class Ipv4AddressPost
    {

        [JsonProperty("ipv4addr")]
        public string Value { get; set; }
    }
    public partial class Ipv4Address : Ipv4AddressPost
    {
        [JsonProperty("_ref")]
        public string Reference { get; set; }

        [JsonProperty("configure_for_dhcp")]
        public bool ConfigureForDhcp { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }
    }

    #endregion

    #region Configuration Models
    public partial class Configuration
    {
        [JsonProperty("InfoBloxHelper")]
        public HelperConfiguration InfoBloxHelper { get; set; }
    }

    public partial class Configuration
    {
        public static Configuration FromJson(string json) => JsonConvert.DeserializeObject<Configuration>(json, Converter.Settings);
    }

    public partial class HelperConfiguration
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("credential")]
        public string Credential { get; set; }

        [JsonProperty("serveruri")]
        public string ServerUri { get; set; }

        [JsonProperty("apiroute")]
        public string ApiRoute { get; set; }

        [JsonProperty("apiversion")]
        public string ApiVersion { get; set; }

        [JsonProperty("networkpath")]
        public string NetworkPath { get; set; }

        [JsonProperty("acceptanySSL")]
        public bool AcceptAnySsl { get; set; }

        [JsonProperty("defaultnetworkcidr")]
        public string DefaultNetworkCIDR { get; set; }

    }

    #endregion

    #region JsonUtilities

    //Serialization and Deserialization components.
    public static class Serialize
    {
        public static string ToJson(this InfobloxNetworks self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this InfobloxNetwork self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this IpResult self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this IpRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this HostRecord self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this HostRecordPost self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this Configuration self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    #endregion
}