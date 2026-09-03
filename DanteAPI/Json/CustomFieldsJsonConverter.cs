using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DanteAPI.Entities;

namespace DanteAPI.Json
{
    /// <summary>
    /// Maps top-level JSON properties named Custom1, Custom2, … into <see cref="EntityWithCustomFields.CustomFields"/>.
    /// </summary>
    public sealed class CustomFieldsJsonConverterFactory : JsonConverterFactory
    {
        private readonly Type? _excludeType;

        public CustomFieldsJsonConverterFactory()
            : this(null)
        {
        }

        internal CustomFieldsJsonConverterFactory(Type? excludeType)
        {
            _excludeType = excludeType;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (_excludeType != null && typeToConvert == _excludeType)
                return false;

            return typeof(EntityWithCustomFields).IsAssignableFrom(typeToConvert)
                && typeToConvert.IsClass
                && !typeToConvert.IsAbstract;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type converterType = typeof(CustomFieldsJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }

    internal sealed class CustomFieldsJsonConverter<T> : JsonConverter<T> where T : EntityWithCustomFields
    {
        private static readonly Regex CustomFieldNameRegex = new(
            @"^Custom(\d+)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Null)
                return null;

            Dictionary<int, string> customFields = ExtractCustomFields(root);

            JsonSerializerOptions innerOptions = CreateOptionsExcluding(options, typeof(T));
            T? entity = JsonSerializer.Deserialize<T>(root.GetRawText(), innerOptions);

            if (entity != null)
                entity.CustomFields = customFields;

            return entity;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializerOptions innerOptions = CreateOptionsExcluding(options, typeof(T));
            JsonSerializer.Serialize(writer, value, innerOptions);
        }

        private static Dictionary<int, string> ExtractCustomFields(JsonElement root)
        {
            var customFields = new Dictionary<int, string>();

            if (root.ValueKind != JsonValueKind.Object)
                return customFields;

            foreach (JsonProperty property in root.EnumerateObject())
            {
                Match match = CustomFieldNameRegex.Match(property.Name);
                if (!match.Success)
                    continue;

                int number = int.Parse(match.Groups[1].Value);
                customFields[number] = GetStringValue(property.Value);
            }

            return customFields;
        }

        private static string GetStringValue(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.Null => null!,
                JsonValueKind.String => value.GetString()!,
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => value.ToString()
            };
        }

        private static JsonSerializerOptions CreateOptionsExcluding(JsonSerializerOptions options, Type excludeType)
        {
            var innerOptions = new JsonSerializerOptions(options);
            innerOptions.Converters.Clear();

            foreach (JsonConverter converter in options.Converters)
            {
                if (converter is CustomFieldsJsonConverterFactory)
                    innerOptions.Converters.Add(new CustomFieldsJsonConverterFactory(excludeType));
                else
                    innerOptions.Converters.Add(converter);
            }

            return innerOptions;
        }
    }
}
