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
    public partial class Ipv4Address : Ipv4AddressPost
    {
        [JsonProperty("_ref")]
        public string Reference { get; set; }

        [JsonProperty("configure_for_dhcp")]
        public bool ConfigureForDhcp { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }
    }
}