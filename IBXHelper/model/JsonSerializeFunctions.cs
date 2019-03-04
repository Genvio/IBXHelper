/*====================================================================================================
 *
 * Copyright (c) 2019 Genvio Inc - All Rights Reserved
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 * Written by Antonio Luevano <antonio@genvio.net>, January 2019
 *
 *=====================================================================================================*/

#region library references

using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace InfoBlox.Automation.Model
{
    #region JsonUtilities

    //Serialization and Deserialization components.
    public static class Serialize
    {
        public static string ToJson(this IPSearchResults self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this InfobloxNetworks self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this InfobloxNetwork self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this IpResult self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this IpRequest self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this HostRecord self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this HostRecordPost self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
        public static string ToJson(this Configuration self) => JsonConvert.SerializeObject(self, Formatting.None, Converter.Settings);
    }
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    #endregion
}