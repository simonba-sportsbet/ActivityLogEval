using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Serialize(data, options);
        }
    }
}
