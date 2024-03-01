using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CloudBeat.Kit.Common.Json
{
    public static class JsonSerializerSettingsHelper
	{
        public static JsonSerializerSettings GetSettings(
            bool processDictionaryKeys = false,
            List<JsonConverter> additionalConverters = null)
        {
            var converters = new List<JsonConverter> {
                new EpochDateTimeConverter(),
                new StringEnumConverter()
            };
            if (additionalConverters != null)
                converters.AddRange(additionalConverters);
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = processDictionaryKeys
                    }
                },
                Formatting = Formatting.None,
                Converters = converters
            };
        }
    }
}

