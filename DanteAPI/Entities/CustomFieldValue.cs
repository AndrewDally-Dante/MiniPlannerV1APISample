namespace DanteAPI.Entities
{
    public class CustomFieldValue
    {
        public int ID { get; set; }
        public int CustomFieldID { get; set; }
        public CustomField CustomField { get; set; }
        public string Value { get; set; }
    }
}
