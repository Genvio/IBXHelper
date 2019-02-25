/*====================================================================================================
 *
 * Copyright (c) 2019 Genvio Inc - All Rights Reserved
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 * Written by Antonio Luevano <antonio@genvio.net>, January 2019
 *
 *=====================================================================================================*/

#region library references

using Newtonsoft.Json;

#endregion

namespace InfoBlox.Automation.Model
{

    public partial class IpRequest
    {
        private int number;

        public IpRequest(int requestnumber)
        {
            number = requestnumber;
        }

        [JsonProperty("num")]
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }
    }
    public partial class IpRequest
    {
        public static IpRequest FromJson(string json) => JsonConvert.DeserializeObject<IpRequest>(json, Converter.Settings);
    }

}