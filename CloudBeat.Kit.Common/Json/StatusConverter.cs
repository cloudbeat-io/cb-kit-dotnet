using CloudBeat.Kit.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace CloudBeat.Kit.Common.Json
{
    public class StatusConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch 
            {
                return TestStatusEnum.Skipped;
            }
        }
    }
}
