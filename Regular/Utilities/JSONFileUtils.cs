using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Regular.Converters;
using Regular.Models;

namespace Regular.Utilities
{
    public static class JSONFileUtils
    {
        public static void WriteTextToJSON(List<RegexRule> regexRules, string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer
            {
                Converters = { new RegexRuleConverter() },
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonSerializer.Serialize(jsonWriter, regexRules);
            }
        }

        public static string ReadJSONFromFile(string filePath)
        {
            using (StreamReader streamReader = File.OpenText(filePath))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
