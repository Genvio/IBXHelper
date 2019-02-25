/*====================================================================================================
 *
 * Copyright (c) 2019 Genvio Inc - All Rights Reserved
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 * Written by Antonio Luevano <antonio@genvio.net>, January 2019
 *
 *=====================================================================================================*/

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
}