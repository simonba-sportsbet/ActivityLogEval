using System;
using System.Text.Json;

namespace ActivityLogEval.Client
{
    public static class JsonExt
    {
        public static string ToJson<T>(this T data)
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(data, options);
        }
    }
}
