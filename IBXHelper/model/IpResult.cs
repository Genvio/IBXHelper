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
}