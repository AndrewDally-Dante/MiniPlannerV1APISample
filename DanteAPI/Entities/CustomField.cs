namespace DanteAPI.Entities
{
    public class CustomField
    {
        public int ID { get; set; }
        public int ContextTableID { get; set; }
        public byte Number { get; set; }
        public string Label { get; set; }
        public byte Type { get; set; }
        public string DefaultValue { get; set; }
        public string GroupName { get; set; }
        public short? Priority { get; set; }
    }
}
