/*====================================================================================================
 *
 * Copyright (c) 2019 Genvio Inc - All Rights Reserved
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 * Written by Antonio Luevano <antonio@genvio.net>, January 2019
 *
 *=====================================================================================================*/

#region library references

using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Linq;
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
using InfoBlox.Automation.Model;
using Genvio.Utility.Network;

#endregion

namespace InfoBlox.Automation
{
    //All non-InfoBlox members go here (configuration, httpClient, Singleton)
    public sealed partial class Helper
    {
        #region HttpClient instance variables
        private static HttpClient httpClient;
        private static HttpClientHandler handler = new HttpClientHandler();
        private static AuthenticationHeaderValue httpclientAuthHeaderValue;
        private static bool isClientInitialized = false;
        private static List<InfobloxNetwork> infoBloxSubnets = new List<InfobloxNetwork>();
        private static InfobloxNetwork defaultSubnet = new InfobloxNetwork();

        //Configuration settings for the Helper
        private HelperConfiguration helperConfig;
        const string scheme = "https"; //TLS protocol.
        const int port = 443; // TLS or SSL default port. Adjust as needed.
        private static bool acceptAnySsl = false;

        #endregion

        #region Singleton member variables

        //Singleton Implementation of the Helper
        private static Helper instance = null;
        private static Assembly assembly;

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
        public static string GetVersion()
        {
            //Reference the local assembly
            assembly = Assembly.GetEntryAssembly();

            StringBuilder versionBuilder = new StringBuilder();

            versionBuilder.AppendFormat($"IBXHelper Assembly Version: {assembly.GetName().Version.ToString()}\n");
            versionBuilder.AppendFormat($"IBXHelper File Version: {assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version}\n");
            versionBuilder.AppendFormat($"IBXHelper Assembly Informational Version: {assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}\n");

            return versionBuilder.ToString();
        }

        Helper()
        {
            RetrieveConfigurationAsync().Wait();
        }
        private async Task<string> IBXCallApi(HttpMethod HttpMethod, string ApiFunction, string ApiPath, string ApiCommand = "", string RequestContent = "")
        {


            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Port = port;
            uriBuilder.Host = helperConfig.ServerUri;
            uriBuilder.Path = ApiPath;
            uriBuilder.Query = ApiCommand;

            HttpRequestMessage _reqMessage = new HttpRequestMessage();

            _reqMessage.Headers.Authorization = httpclientAuthHeaderValue;

            _reqMessage.RequestUri = uriBuilder.Uri;
            _reqMessage.Method = HttpMethod;

            //If there is a request payload to be sent (post) let's add it to the message content.
            _reqMessage.Content = (!String.IsNullOrEmpty(RequestContent)) ? new StringContent(RequestContent, Encoding.UTF8, "application/json") : default(StringContent);

            HttpResponseMessage _httpResponse = await httpClient.SendAsync(_reqMessage);

            //Get the response back
            string content = await _httpResponse.Content.ReadAsStringAsync();
            _httpResponse.EnsureSuccessStatusCode();
            return content;
        }
        private async Task RefreshSubnetsAsync()
        {
            try
            {
                infoBloxSubnets = await GetNetworkListsAsync();
                if (infoBloxSubnets.Count >= 2)
                {
                    SelectDefaultSubnetAsync(infoBloxSubnets);
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        private void SelectDefaultSubnetAsync(List<InfobloxNetwork> infoBloxSubnets)
        {
            throw new NotImplementedException();
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
            //@{ var dataFileName = Environment.GetEnvironmentVariable("HOME").ToString() + "\\site\\wwwroot\\data.txt"; } Option 1
            //Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "file_at_root.txt");


            try
            {
                await Task.Run(() =>
                {
                    string configPath = "";
                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME")))
                        configPath = Environment.GetEnvironmentVariable("HOME") + "\\site\\wwwroot\\";
                    else
                        configPath = System.Environment.CurrentDirectory;

                    var builder = new ConfigurationBuilder().SetBasePath(configPath).AddJsonFile("appsettings.json", false, true);

                    var config = builder.Build();

                    helperConfig = config.GetSection("InfoBloxHelper").Get<HelperConfiguration>();

                    CreateAutorizationContextAsync().Wait();

                    acceptAnySsl = helperConfig.AcceptAnySsl;

                    SslBypassCheckAsync().Wait();

                    infoBloxSubnets = GetNetworkListsAsync().Result;

                    if (!String.IsNullOrEmpty(helperConfig.DefaultNetworkCIDR))
                    {
                        if (NetworkUtilities.ValidateCidrIp(helperConfig.DefaultNetworkCIDR).Value)
                        {

                            //defaultSubnet =  instance.GetNetworkAsync(helperConfig.DefaultNetworkCIDR).Result;
                            var _subnetResult = from subnet in infoBloxSubnets
                                                where (subnet.Network == helperConfig.DefaultNetworkCIDR)
                                                select subnet;

                            defaultSubnet = _subnetResult.FirstOrDefault();
                        }
                    }
                    else
                    {
                        //defaultSubnet =  instance.GetNetworkAsync(helperConfig.DefaultNetworkCIDR).Result;
                        var _subnetResult = (from subnet in infoBloxSubnets
                                             select subnet).Take(1);

                        defaultSubnet = _subnetResult.FirstOrDefault();
                    }

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
        private string FindSubnetBaseRef(string subnetIp)
        {
            if (String.IsNullOrEmpty(subnetIp) && String.IsNullOrEmpty(defaultSubnet.Network))
            {

                throw new Exception("No default subnet in the InfoBlox server. Cannot continue. ");
            }
            else
            {
                if (String.IsNullOrEmpty(subnetIp))
                {
                    return defaultSubnet.BaseRef;
                }
                else
                {
                    var searchBaseRef = (from subnet in infoBloxSubnets
                                         where (subnet.Network == subnetIp)
                                         select subnet.BaseRef).FirstOrDefault();
                    return searchBaseRef;
                }
            }
        }
        private bool IsIpv4AddressInSubnetsRange(string Ipv4Address)
        {
            foreach (InfobloxNetwork item in infoBloxSubnets)
            {
                if (NetworkUtilities.IpAddressIsInRange(Ipv4Address, item.Network).Value)
                {
                    //We found the subnet range that the ip address belongs. Let's pass this and return.
                    return true;
                }
            }

            //if we get to this line, the ip address is not in the range of any subnets managed. Nice Try! Play again. Game Over.
            return false;
        }

    }
}


//
// Summary:
//     Creates a cancelable continuation that executes asynchronously when the target
//     System.Threading.Tasks.Task`1 completes.
//
// Parameters:
//   continuationAction:
//     An action to run when the System.Threading.Tasks.Task`1 completes. When run,
//     the delegate is passed the completed task as an argument.
//
//   cancellationToken:
//     The cancellation token that is passed to the new continuation task.
//
// Returns:
//     A new continuation task.
//
// Exceptions:
//   T:System.ObjectDisposedException:
//     The System.Threading.Tasks.Task`1 has been disposed. -or- The System.Threading.CancellationTokenSource
//     that created cancellationToken has been disposed.
//
//   T:System.ArgumentNullException:
//     The continuationAction argument is null.