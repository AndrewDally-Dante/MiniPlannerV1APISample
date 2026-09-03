using System.Text.Json;

namespace DanteAPI.Json
{
    internal static class JsonDefaults
    {
        public static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new CustomFieldsJsonConverterFactory());
            return options;
        }
    }
}
