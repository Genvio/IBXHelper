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
        private string refIpv4Address;

        [JsonProperty("_ref")]
        public string Reference
        {
            get
            {
                return refIpv4Address;
            }
            set
            {
                refIpv4Address = value;
                ExtractBaseRef();
            }
        }

        [JsonProperty("configure_for_dhcp")]
        public bool ConfigureForDhcp { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }
    }

    public partial class Ipv4Address : Ipv4AddressPost
    {

        private string baseRef;
        public string BaseRef
        {
            get
            {
                return baseRef;
            }
        }
        private void ExtractBaseRef()
        {
            int _startExtract = refIpv4Address.IndexOf("/", 0);

            int _endExtract = refIpv4Address.IndexOf(":", _startExtract);

            if (refIpv4Address.Length > 0 && _startExtract >= 1 && _endExtract >= 0)
            {
                baseRef = refIpv4Address.Substring(_startExtract + 1, ((_endExtract - _startExtract) - 1));
            }
        }
    }

}