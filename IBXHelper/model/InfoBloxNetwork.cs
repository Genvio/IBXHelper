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
    #region  InfoBlox Models

    public partial class InfobloxNetwork
    {
        private string refNetwork;

        [JsonProperty("_ref")]
        public string Ref
        {
            get
            {
                return refNetwork;
            }
            set
            {
                refNetwork = value;
                ExtractBaseRef();
            }
        }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("network_view")]
        public string NetworkView { get; set; }
    }
    public partial class InfobloxNetwork
    {
        private string baseRef;
        public static InfobloxNetwork FromJson(string json) => JsonConvert.DeserializeObject<InfobloxNetwork>(json, Converter.Settings); //TODO - change here
        public string BaseRef
        {
            get
            {
                return baseRef;
            }
        }

        // Extracts the unique reference id for the InfoBlox network object
        private void ExtractBaseRef()
        {
            int _startExtract = refNetwork.IndexOf("/", 0);

            int _endExtract = refNetwork.IndexOf(":", _startExtract);

            if (refNetwork.Length > 0 && _startExtract >= 1 && _endExtract >= 0)
            {
                baseRef = refNetwork.Substring(_startExtract + 1, ((_endExtract - _startExtract) - 1));
            }
        }
    }
    public partial class InfobloxNetworks : List<InfobloxNetwork>
    {
        public static InfobloxNetworks FromJson(string json) => JsonConvert.DeserializeObject<InfobloxNetworks>(json, Converter.Settings); //TODO - change here
    }

    #endregion

}