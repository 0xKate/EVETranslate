using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EVETranslate.Services
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Object → JSON string
        public static string ToJson<T>(T obj, bool indented = false)
        {
            if (obj == null)
                return string.Empty;

            var options = indented
                ? new JsonSerializerOptions(DefaultOptions) { WriteIndented = true }
                : DefaultOptions;

            return JsonSerializer.Serialize(obj, options);
        }

        // JSON string → Object
        public static T? FromJson<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
    }
}