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
    public partial class Ipv4AddressPost
    {

        [JsonProperty("ipv4addr")]
        public string Value { get; set; }
    }
}