using CloudBeat.Kit.Common.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBeat.Kit.Common.Json
{
	public static class CbJsonConvert
	{
		private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy
				{
					ProcessDictionaryKeys = false
				}
			},
			Formatting = Formatting.None,
			Converters = new List<JsonConverter> { new EpochDateTimeConverter(), new StringEnumConverter(), new StatusConverter() }
		};
		public static string SerializeObject(object obj)
		{
			return JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
		}
	}
}
