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
}