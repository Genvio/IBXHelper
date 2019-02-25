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
    public partial class IPSeachResult
    {
        [JsonProperty("_ref")]
        public string Ref { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("mac_address")]
        public string MacAddress { get; set; }

        [JsonProperty("names")]
        public string[] Names { get; set; }

        [JsonProperty("objects")]
        public string[] Objects { get; set; }

    }

    public partial class IPSearchResults : List<IPSeachResult>
    {
        public static IPSearchResults FromJson(string json) => JsonConvert.DeserializeObject<IPSearchResults>(json, Converter.Settings);
    }

}
