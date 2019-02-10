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
using KPMG.KTech.Automation.InfoBlox.Model;

#endregion

namespace KPMG.KTech.Automation.InfoBlox
{
    public sealed partial class Helper
    {

        //InfoBlox specific options.
        public async Task<List<InfobloxNetwork>> RetrieveNetworkLists()
        {

            // https://10.10.10.10/wapi/v2.9/network

            string apifunction = "network";
            bool _acceptInvalidSSL = true;

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
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

        public async Task<IpResult> RetrieveIP(int totalIPRequested = 1)
        {
            // https://10.10.10.10/wapi/v2.9/network/ZG5zLm5ldHdvcmskMTAuMTI4LjAuMC8yNC8w?_function=next_available_ip

            if (totalIPRequested <= 0)
            {
                return null;
            }


            string apifunction = "network";
            string refnetwork = "ZG5zLm5ldHdvcmskMTAuMTI4LjAuMC8yNC8w";
            string apicommand = "_function=next_available_ip";

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}/{refnetwork}";
            uriBuilder.Query = apicommand;

            string iprequested = new IpRequest(totalIPRequested).ToJson();

            //            NewMethod(_acceptInvalidSSL);

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

        public async Task<string> CreateHostRecord(string HostName, string HostMac = null)
        {

            // https://10.10.10.10/wapi/v2.9/record:host

            string apifunction = "record:host";

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";

            HttpRequestMessage _reqMessage = new HttpRequestMessage();

            _reqMessage.Headers.Authorization = httpclientAuthHeaderValue;

            _reqMessage.RequestUri = uriBuilder.Uri;
            _reqMessage.Method = HttpMethod.Post;
            //   _reqMessage.Content = new StringContent(iprequested, Encoding.UTF8, "application/json");


            HttpResponseMessage _httpResponse = await httpClient.SendAsync(_reqMessage);

            //Get the response back
            string content = _httpResponse.Content.ReadAsStringAsync().Result;


            _httpResponse.EnsureSuccessStatusCode();


            // return IpResult.FromJson(content);
            return (null);
        }
        public async Task UpdateHostRecord()
        { }
        public async Task DeleteHostRecord()
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

        const string scheme = "https";
        private string username;
        private string password;
        private string credentials;
        private string apiPath;
        private string apiversion;
        private string apifunction;
        private bool acceptInvalidSSL;

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
            RetrieveConfiguration();

        }

        private void NewMethod(bool _acceptInvalidSSL) //TODO: Config and SSL bypass
        {
            //The request will not validate the certificate from the server (testing/poc) -- DO NOT DEPLOY in production with the flag equals to true
            if (_acceptInvalidSSL)
            {
                //Accept all server certificate (Y/n)
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return _acceptInvalidSSL; };

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
                if (!isClientInitialized)
                {
                    httpClient = new HttpClient();
                    isClientInitialized = true;
                }
            }
        }


        private void RetrieveConfiguration()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true);

                var config = builder.Build();

                helperConfig = config.GetSection("InfoBloxHelper").Get<HelperConfiguration>();

                //Create the credentials for the helper once so that they are used across the helper functions.
                if (!String.IsNullOrEmpty(helperConfig.Credential))
                {
                    httpclientAuthHeaderValue = new AuthenticationHeaderValue("Basic", helperConfig.Credential);
                }
                else if (String.IsNullOrEmpty(helperConfig.Credential) && !String.IsNullOrEmpty(helperConfig.Username) && !String.IsNullOrEmpty(helperConfig.Password))
                {
                    helperConfig.Credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{helperConfig.Username}:{helperConfig.Password}"));
                    httpclientAuthHeaderValue = new AuthenticationHeaderValue("Basic", helperConfig.Credential);
                }
                else if (String.IsNullOrEmpty(helperConfig.Username) || String.IsNullOrEmpty(helperConfig.Password))
                {
                    throw new System.Exception();
                }
            }
            catch (System.Exception)
            {

                throw;
            }

        }

    }
}



