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
    public sealed partial class Helper
    {
        //InfoBlox specific options.
        public async Task<InfobloxNetwork> GetNetworkAsync(string cidrNetwork)
        {

            // https://10.10.10.10/wapi/v2.9/network?network=10.10.10.10/8

            string apifunction = "network";
            string apicommand = $"network={cidrNetwork}";
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";
            string content = await IBXCallApi(HttpMethod.Get, apifunction, apipath, apicommand);

            InfobloxNetwork _subnet = (from subnet in (InfobloxNetworks.FromJson(content))
                                       select subnet).FirstOrDefault();

            return _subnet;
        }
        public async Task<InfobloxNetworks> GetNetworkListsAsync()
        {

            // https://10.10.10.10/wapi/v2.9/network

            string apifunction = "network";
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";

            string content = await IBXCallApi(HttpMethod.Get, apifunction, apipath);
            return InfobloxNetworks.FromJson(content);
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
                return default(IpResult);
            }

            string apifunction = "network";
            string refnetwork = FindSubnetBaseRef(subnetIp);
            string apicommand = "_function=next_available_ip";
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}/{refnetwork}";

            string iprequested = new IpRequest(totalIPRequested).ToJson();

            string content = await IBXCallApi(HttpMethod: HttpMethod.Post, ApiFunction: apifunction, ApiPath: apipath, ApiCommand: apicommand, RequestContent: iprequested);

            return IpResult.FromJson(content);
        }
        public async Task<HostRecord> GetHostRecordAsync(string HostName)
        {
            //https://10.10.10.10/wapi/v2.9/record:host?name~=host.url.path

            if (String.IsNullOrEmpty(HostName))
            {
                return default(HostRecord);
            }

            string apifunction = "record:host";
            string apicommand = $"name~={HostName}";
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";

            string content = await IBXCallApi(HttpMethod: HttpMethod.Get, ApiFunction: apifunction, ApiPath: apipath, ApiCommand: apicommand);

            var returnedHost = HostRecords.FromJson(content);

            HostRecord host = (from record in returnedHost
                               select record).FirstOrDefault();
            return (host);

        }
        public async Task<HostRecord> GetHostRecordByIPAddressAsync(string Ipv4Address)
        {
            //https://10.10.10.10/wapi/v2.9/ipv4address?status=USED&ip_address=10.10.10.10

            HostRecord createdHostRecord = new HostRecord();

            if (!String.IsNullOrEmpty(Ipv4Address))
            {
                // Check#2 - Validate the ipV4 address sent is a valid IP address
                if ((NetworkUtilities.ParseSingleIPv4Address(Ipv4Address).Value.ToString()) != Ipv4Address)
                {
                    throw new ArgumentException($"The value of {Ipv4Address} is invalid. Check your values and try again.");
                }
                // Check#3 - Validate the ipV4 address is in one of the managed Ip Subnets in the InfoBlox API
                if (!IsIpv4AddressInSubnetsRange(Ipv4Address))
                {
                    throw new ArgumentException($"The value of {Ipv4Address} is not within the range of the subnets managed by the InfoBlox Grid. Check your values and try again.");
                }

                string apifunction = "ipv4address";
                string apicommand = $"status=USED&ip_address={Ipv4Address}";
                string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";

                string content = await IBXCallApi(HttpMethod: HttpMethod.Get, ApiFunction: apifunction, ApiPath: apipath, ApiCommand: apicommand);

                var returnedSearchResults = IPSearchResults.FromJson(content);

                if (returnedSearchResults.Count > 0)
                {
                    string hostname = (from item in returnedSearchResults
                                       select item.Names[0]).FirstOrDefault();

                    createdHostRecord = await GetHostRecordAsync(hostname);
                }
            }

            return (createdHostRecord);
        }

        //Function to call when the library will automatically fetch the next IP Address and pass the value of the hostname
        public async Task<HostRecord> CreateHostRecordAsync(string HostName, string HostMac = null)
        {
            //Validations(2)
            // Check#1 - Validate input to ensure that a hostname was supplied
            if (String.IsNullOrEmpty(HostName))
            {
                return default(HostRecord);
            }

            IpResult nextIP = GetIPAsync(1).Result;

            return (await CreateHostRecordAsync(HostName, nextIP.IPAddresses[0], null));

        }
        public async Task<HostRecord> CreateHostRecordAsync(string HostName, string Ipv4Address, string HostMac = null)
        {
            // https://10.10.10.10/wapi/v2.9/record:host

            //Validations (4)
            // This area perform validations to the information provided to the function.
            // Check#1 - Validate input to ensure that thereis a hostname and an Ipv4Address
            if (String.IsNullOrEmpty(HostName) || string.IsNullOrEmpty(Ipv4Address))
            {
                return default(HostRecord);
            }
            // Check#2 - Ensure that the host is not already in the DNS registry

            var hostAlreadyExists = await GetHostRecordAsync(HostName);

            if (hostAlreadyExists != null)
            {
                return hostAlreadyExists; //record for that hostname already exists, return the existing record
            }
            // Check#3 - Validate the ipV4 address sent is a valid IP address
            if ((NetworkUtilities.ParseSingleIPv4Address(Ipv4Address).Value.ToString()) != Ipv4Address)
            {
                throw new ArgumentException($"The value of {Ipv4Address} is invalid. Check your values and try again.");
            }
            // Check#4 - Validate the ipV4 address is in one of the managed Ip Subnets in the InfoBlox API
            if (!IsIpv4AddressInSubnetsRange(Ipv4Address))
            {
                throw new ArgumentException($"The value of {Ipv4Address} is not within the range of the subnets managed by the InfoBlox Grid. Check your values and try again.");
            }

            //If everything is good so far... let's go ahead and make us a HostRecord!

            //Add spices to the recipe

            //Single line model construct. Pick your poison by uncommenting the chosen method. ;-)
            //HostRecord newHost = new HostRecord() { Name = HostName, Ipv4Addresses = new Ipv4Address[] { new Ipv4Address() { Value = Ipv4Address } } };

            //Multi-line model construct. Pick your poison by uncommenting the chosen method. ;-)
            HostRecordPost newHost = new HostRecordPost();
            newHost.Name = HostName;
            newHost.Ipv4Addresses = new Ipv4AddressPost[] { new Ipv4AddressPost() { Value = Ipv4Address } };

            //return newHost.ToJson();
            string apifunction = "record:host";
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}";
            string requestcontent = newHost.ToJson();

            string content = await IBXCallApi(HttpMethod: HttpMethod.Post, ApiFunction: apifunction, ApiPath: apipath, RequestContent: requestcontent);

            HostRecord createdHostRecord = await GetHostRecordAsync(HostName);

            return (createdHostRecord);
        }
        public async Task<HostRecord> UpdateHostRecordAsync(string HostName, HostRecordPost changedRecord)
        {
            // https://10.10.10.10/wapi/v2.9/record:host

            //Validations (4)
            // This area perform validations to the information provided to the function.
            // Check#1 - Validate input to ensure that thereis a hostname and Host object to update
            if (String.IsNullOrEmpty(HostName) || (changedRecord is null))
            {
                return default(HostRecord);
            }
            // Check#2 - Ensure that the host is already in the DNS registry
            var hostAlreadyExists = await GetHostRecordAsync(HostName);

            if (hostAlreadyExists is null)
            {
                throw new ArgumentException($"The hostname  {HostName} is does not exist in the server or is invalid. Check your values and try again.");
            }
            // Check#3 - Validate the ipV4 address sent is a valid IP address
            string ipv4AddressToValidate = changedRecord.Ipv4Addresses[0].Value;

            if ((NetworkUtilities.ParseSingleIPv4Address(ipv4AddressToValidate).Value.ToString()) != ipv4AddressToValidate)
            {
                throw new ArgumentException($"The value of {ipv4AddressToValidate} is invalid. Check your values and try again.");
            }
            // Check#4 - Validate the ipV4 address is in one of the managed Ip Subnets in the InfoBlox API
            if (!IsIpv4AddressInSubnetsRange(ipv4AddressToValidate))
            {
                throw new ArgumentException($"The value of {ipv4AddressToValidate} is not within the range of the subnets managed by the InfoBlox Grid. Check your values and try again.");
            }

            //If everything is good so far... let's go ahead and prepare the info for the HostRecord!


            string apifunction = "record:host";
            string refhost = hostAlreadyExists.BaseRef;
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}/{refhost}";
            string requestcontent = changedRecord.ToJson();

            string content = await IBXCallApi(HttpMethod: HttpMethod.Put, ApiFunction: apifunction, ApiPath: apipath, RequestContent: requestcontent);

            HostRecord createdHostRecord = await GetHostRecordAsync(changedRecord.Name);

            return (createdHostRecord);
        }
        public async Task<bool> DeleteHostRecordAsync(HostRecordPost hostToDelete)
        {
            //Validations (3)
            // This area perform validations to the information provided to the function.
            // Check#1 - Validate input to ensure that there is a host object to delete
            if (hostToDelete is null)
            {
                return false;
            }
            // Check#2 - Ensure that the host is already in the DNS registry
            var existingHost = await GetHostRecordAsync(hostToDelete.Name);

            if (existingHost is null)
            {
                throw new ArgumentException($"The host {hostToDelete.Name} is does not exist in the server or is invalid. Check your values and try again.");
            }
            // Check#3 - Validate the existing host has a valid Base Reference
            string baseReferenceToValidate = existingHost.BaseRef;

            if (String.IsNullOrEmpty(baseReferenceToValidate))
            {
                throw new ArgumentOutOfRangeException($"The record doesn't have a valid Base Reference or doesn't exists. Check your values and try again.");
            }

            //If everything is good so far... let's go ahead and prepare the info for the HostRecord!

            string apifunction = "record:host";
            string refhost = existingHost.BaseRef;
            string apipath = $"{helperConfig.ApiRoute}/{helperConfig.ApiVersion}/{apifunction}/{refhost}";

            string content = await IBXCallApi(HttpMethod: HttpMethod.Delete, ApiFunction: apifunction, ApiPath: apipath);

            return (!String.IsNullOrEmpty(content));
        }
        public async Task<bool> DeleteHostRecordAsync(string hostName)
        {
            return (await DeleteHostRecordAsync(new HostRecordPost() { Name = hostName })); ;
        }
    }
}