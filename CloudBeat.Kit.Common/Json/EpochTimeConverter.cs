using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CloudBeat.Kit.Common.Json
{
    public class EpochDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            long msValue = (long)Math.Ceiling(((DateTime)value - _epoch).TotalMilliseconds);

            //writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "000");
            writer.WriteRawValue(msValue.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            { 
                return null; 
            }

            if (reader.Value is DateTime)
            {
                return (DateTime)reader.Value;
            }

            // determine whether timestamp is in milliseconds or microseconds (digits count is >= 16)
            if (reader.Value.ToString().Length >= 16)
            {
                return _epoch.AddMilliseconds((long)reader.Value / 1000);
            }
            else
            {
                long msValue;
                if (reader.Value is double)
                    msValue = Convert.ToInt64((double)reader.Value);
                else
                    msValue = (long)reader.Value;

				return _epoch.AddMilliseconds(msValue);
            }
        }
    }
}
