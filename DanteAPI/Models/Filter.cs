namespace DanteAPI
{
    public class Filter
    {
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public int? DecryptValue { get; set; }
    }
}
