/*====================================================================================================
 *
 * Copyright (c) 2019 Genvio Inc - All Rights Reserved
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 * Written by Antonio Luevano <antonio@genvio.net>, January 2019
 *
 *=====================================================================================================*/

#region library references

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion


namespace InfoBlox.Automation.Model
{

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

    public partial class HostRecords : List<HostRecord>
    {
        public static HostRecords FromJson(string json) => JsonConvert.DeserializeObject<HostRecords>(json, Converter.Settings);
    }

}