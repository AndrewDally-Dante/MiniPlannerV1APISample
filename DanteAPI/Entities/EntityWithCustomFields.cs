using System.Text.Json.Serialization;

namespace DanteAPI.Entities
{
    public abstract class EntityWithCustomFields
    {
        [JsonIgnore]
        public Dictionary<int, string> CustomFields { get; set; } = new();
    }
}
