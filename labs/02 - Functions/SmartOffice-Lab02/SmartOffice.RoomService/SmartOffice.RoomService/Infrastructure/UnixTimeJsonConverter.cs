using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Neudesic.SmartOffice.RoomService.Infrastructure {

    internal class UnixTimeJsonConverter: IsoDateTimeConverter {
        private static readonly DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value is DateTime) {
                long totalSeconds = (long)((DateTime)value - UnixStartTime).TotalSeconds;
                writer.WriteValue(totalSeconds);
            } else {
                base.WriteJson(writer, value, serializer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType != Newtonsoft.Json.JsonToken.Integer) {                
                return base.ReadJson(reader, objectType, existingValue, serializer);                
            }

            double totalSeconds = 0;

            try {
                totalSeconds = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
            }
            catch {
                throw new Exception("Invalid double value.");
            }

            return UnixStartTime.AddSeconds(totalSeconds);
        }
    }
}
