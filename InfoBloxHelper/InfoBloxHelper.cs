#region library references

using System;
using System.IO;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Genvio.Utility;
using InfoBlox.Automation.Model;

#endregion

namespace InfoBlox.Automation
{
    public sealed partial class Helper
    {

        //InfoBlox specific options.
        public async Task<List<InfobloxNetwork>> GetNetworkListsAsync()
        {

            // https://10.10.10.10/wapi/v2.9/network

            string apifunction = "network";

            // UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Port = port;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";

            //  NewMethod(_acceptInvalidSSL);

            HttpRequestMessage _reqMessage = new HttpRequestMessage();

            _reqMessage.Headers.Authorization = httpclientAuthHeaderValue;

            _reqMessage.RequestUri = uriBuilder.Uri;
            _reqMessage.Method = HttpMethod.Get;

            HttpResponseMessage _httpResponse = await httpClient.SendAsync(_reqMessage);

            //Get the response back
            string content = await _httpResponse.Content.ReadAsStringAsync();
            _httpResponse.EnsureSuccessStatusCode();
            return InfobloxNetwork.FromJson(content);
        }

        public async Task<IpResult> GetIPAsync(int totalIPRequested = 1)
        {
            return await GetIPAsync(totalIPRequested, String.Empty);
        }

        public async Task<IpResult> GetIPAsync(int totalIPRequested = 1, string subnetIp = "")
        {
            // https://10.10.10.10/wapi/v2.9/network/ZG5zLm5ldHdvcmskMTAuMTI4LjAuMC8yNC8w?_function=next_available_ip

            if (totalIPRequested <= 0)
            {
                return null;
            }


            string apifunction = "network";
            string refnetwork = "ZG5zLm5ldHdvcmskMTAuMTI4LjAuMC8yNC8w";
            string apicommand = "_function=next_available_ip";

            // UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Port = port;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}/{refnetwork}";
            uriBuilder.Query = apicommand;

            string iprequested = new IpRequest(totalIPRequested).ToJson();

            HttpRequestMessage _reqMessage = new HttpRequestMessage();

            _reqMessage.Headers.Authorization = httpclientAuthHeaderValue;

            _reqMessage.RequestUri = uriBuilder.Uri;
            _reqMessage.Method = HttpMethod.Post;
            _reqMessage.Content = new StringContent(iprequested, Encoding.UTF8, "application/json");


            HttpResponseMessage _httpResponse = await httpClient.SendAsync(_reqMessage);

            //Get the response back
            string content = await _httpResponse.Content.ReadAsStringAsync();
            _httpResponse.EnsureSuccessStatusCode();

            return IpResult.FromJson(content);
        }
        public async Task<HostRecord> GetHostRecordAsync(string HostName)
        {
            //https://10.10.10.10/wapi/v2.9/record:host?name~=host.url.path

            if (String.IsNullOrEmpty(HostName))
            {
                return null;
            }

            string apifunction = "record:host";
            string apicommand = $"name~={HostName}";

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";
            uriBuilder.Query = apicommand;

            HttpRequestMessage _reqMessage = new HttpRequestMessage();

            _reqMessage.Headers.Authorization = httpclientAuthHeaderValue;

            _reqMessage.RequestUri = uriBuilder.Uri;
            _reqMessage.Method = HttpMethod.Get;

            HttpResponseMessage _httpResponse = await httpClient.SendAsync(_reqMessage);

            //Get the response back
            string content = _httpResponse.Content.ReadAsStringAsync().Result;

            _httpResponse.EnsureSuccessStatusCode();

            return (HostRecord.FromJson(content));
        }

        //Function to call when the library will automatically fetch the next IP Address and pass the value of the hostname
        public async Task<string> CreateHostRecordAsync(string HostName, string HostMac = null)
        {
            if (String.IsNullOrEmpty(HostName))
            {
                return null;
            }

            IpResult nextIP = this.GetIPAsync(1).Result;

            return this.CreateHostRecordAsync(HostName, nextIP.IPAddresses[0], null).ToString();

        }
        public async Task<string> CreateHostRecordAsync(string HostName, string Ipv4Address, string HostMac = null)
        {
            // https://10.10.10.10/wapi/v2.9/record:host

            if (String.IsNullOrEmpty(HostName) || string.IsNullOrEmpty(Ipv4Address))
            {
                return null;
            }

            string apifunction = "record:host";

            //Single line model construct. Pick your poison by uncommenting the chosen method. ;-)
            //HostRecord newHost = new HostRecord() { Name = HostName, Ipv4Addresses = new Ipv4Address[] { new Ipv4Address() { Value = Ipv4Address } } };

            //Multi-line model construct. Pick your poison by uncommenting the chosen method. ;-)
            HostRecord newHost = new HostRecord();
            newHost.Name = HostName;
            newHost.Ipv4Addresses = new Ipv4Address[] { new Ipv4Address() { Value = Ipv4Address } };

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";

            HttpRequestMessage _reqMessage = new HttpRequestMessage();

            _reqMessage.Headers.Authorization = httpclientAuthHeaderValue;

            _reqMessage.RequestUri = uriBuilder.Uri;
            _reqMessage.Method = HttpMethod.Post;
            //Payload of Record goes here.
            _reqMessage.Content = new StringContent(newHost.ToJson(), Encoding.UTF8, "application/json");


            HttpResponseMessage _httpResponse = await httpClient.SendAsync(_reqMessage);

            //Get the response back
            string content = _httpResponse.Content.ReadAsStringAsync().Result;


            _httpResponse.EnsureSuccessStatusCode();


            // return IpResult.FromJson(content);
            return (content);
        }
        public async Task UpdateHostRecordAsync()
        { }
        public async Task DeleteHostRecordAsync()
        { }


    }

    //All non-InfoBlox members go here (configuration, httpClient, Singleton)
    public sealed partial class Helper
    {
        #region HttpClient instance variables
        private static HttpClient httpClient;
        private static HttpClientHandler handler = new HttpClientHandler();

        private static AuthenticationHeaderValue httpclientAuthHeaderValue;
        private static bool isClientInitialized = false;
        private static UriBuilder uriBuilder = new UriBuilder();

        //Configuration settings for the Helper
        private HelperConfiguration helperConfig;

        const string scheme = "https"; //TLS protocol.
        const int port = 443; // TLS or SSL default port. Adjust as needed.
        private static bool acceptAnySsl = false;

        private static List<InfobloxNetwork> infoBloxSubnets;

        #endregion

        #region Singleton member variables

        //Singleton Implementation of the Helper
        private static Helper instance = null;

        //Thread Safety object to ensure thread safety.
        private static readonly object padlock = new object();

        #endregion

        //Instance Reference
        public static Helper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new Helper();
                        }
                    }
                }
                return instance;
            }
        }

        Helper()
        {
            RetrieveConfigurationAsync().Wait();
            SslBypassCheckAsync().Wait();
            RefreshSubnetsAsync().Wait();
        }

        private async Task RefreshSubnetsAsync()
        {
            try
            {
                infoBloxSubnets = await this.GetNetworkListsAsync();
                //SelectDefaultSubnetAsync(infoBloxSubnets);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private static async Task SslBypassCheckAsync()
        {
            //The request will not validate the certificate from the server (testing/poc) -- DO NOT DEPLOY in production with the flag equals to true
            if (acceptAnySsl)
            {
                //Accept all server certificate (Y/n)
                await Task.Run(() =>
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return acceptAnySsl; };
                });

                //The HttpClient is pooled to increase performance and scalability of connections.
                if (!isClientInitialized)
                {
                    httpClient = new HttpClient(handler, false);
                    isClientInitialized = true;
                }
            }
            else
            // This path is taken if the application is in Production vs. Test/Dev
            {

                //The HttpClient is pooled to increase performance and scalability of connections.
                await Task.Run(() =>
                {
                    if (!isClientInitialized)
                    {
                        httpClient = new HttpClient();
                        isClientInitialized = true;
                    }
                });
            }
        }

        private async Task RetrieveConfigurationAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var builder = new ConfigurationBuilder().SetBasePath(System.Environment.CurrentDirectory).AddJsonFile("appsettings.json", false, true);

                    var config = builder.Build();

                    helperConfig = config.GetSection("InfoBloxHelper").Get<HelperConfiguration>();

                    CreateAutorizationContextAsync().Wait();

                    acceptAnySsl = helperConfig.AcceptAnySsl;
                });
            }
            catch (System.Exception)
            {

                throw;
            }

        }


        private async Task CreateAutorizationContextAsync()
        {

            await Task.Run(() =>
            {
                //Create the credentials for the helper once so that they are used across the helper functions.
                if (!String.IsNullOrEmpty(helperConfig.Credential)) // We have a credential already in place, we use that instead of the user/pass
                {
                    httpclientAuthHeaderValue = new AuthenticationHeaderValue("Basic", helperConfig.Credential);
                    return;
                }
                else if (String.IsNullOrEmpty(helperConfig.Credential) && !String.IsNullOrEmpty(helperConfig.Username) && !String.IsNullOrEmpty(helperConfig.Password)) //we don't have the credentials but we have user/pass, we generate the credentials.
                {
                    helperConfig.Credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{helperConfig.Username}:{helperConfig.Password}"));
                    httpclientAuthHeaderValue = new AuthenticationHeaderValue("Basic", helperConfig.Credential);
                    return;
                }
                else if (String.IsNullOrEmpty(helperConfig.Username) || String.IsNullOrEmpty(helperConfig.Password) && String.IsNullOrEmpty(helperConfig.Credential))
                {
                    throw new System.Exception();
                }
            });
        }
    }
}



