using Newtonsoft.Json;
using Regular.Models;

namespace Regular.Utilities
{
    public static class SerializationUtils
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static string SerializeRegexRule(RegexRule regexRule)
        {
            string updaterIdString = regexRule.RegularUpdater.GetUpdaterId().GetGUID().ToString();
            return JsonConvert.SerializeObject(regexRule, JsonSerializerSettings);
        }
        
        public static T DeepCopyObject<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source, JsonSerializerSettings);
            return JsonConvert.DeserializeObject<T>(serialized, JsonSerializerSettings);
        }
        
        public static RegexRule DeserializeRegexRule(string serializedRegexRule)
        {
            RegexRule deserializedRegexRule = JsonConvert.DeserializeObject<RegexRule>(serializedRegexRule, JsonSerializerSettings);
            string updaterIdString = deserializedRegexRule.RegularUpdater.GetUpdaterId().GetGUID().ToString();
            return deserializedRegexRule;
        }
    }
}
