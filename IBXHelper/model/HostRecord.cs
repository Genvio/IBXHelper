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
        private string refHost;

        [JsonProperty("ipv4addrs")]
        new public Ipv4Address[] Ipv4Addresses { get; set; }

        [JsonProperty("_ref")]
        public string Reference
        {
            get
            {
                return refHost;
            }
            set
            {
                refHost = value;
                ExtractBaseRef();
            }
        }

        [JsonProperty("view")]
        public string View { get; set; }
    }
    public partial class HostRecord
    {
        new public static HostRecord FromJson(string json) => JsonConvert.DeserializeObject<HostRecord>(json, Converter.Settings);
    }

    public partial class HostRecord
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
            int _startExtract = refHost.IndexOf("/", 0);

            int _endExtract = refHost.IndexOf(":", _startExtract);

            if (refHost.Length > 0 && _startExtract >= 1 && _endExtract >= 0)
            {
                baseRef = refHost.Substring(_startExtract + 1, ((_endExtract - _startExtract) - 1));
            }
        }
    }

    public partial class HostRecords : List<HostRecord>
    {
        public static HostRecords FromJson(string json) => JsonConvert.DeserializeObject<HostRecords>(json, Converter.Settings);
    }

}