using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Regular.Models;

namespace Regular.Converters
{
    public class RegexRuleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(RegexRule);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return jObject.ToObject<RegexRule>(serializer);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
