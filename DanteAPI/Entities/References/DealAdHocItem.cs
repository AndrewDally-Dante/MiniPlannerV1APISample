namespace DanteAPI.Entities.References
{
    public class DealAdHocItem
    {
        public int ID { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int? TaxCodeID { get; set; }
        public string NominalCode { get; set; }
        public int? ProductID { get; set; }
        public decimal Total { get; set; }
        public TaxCode TaxCode { get; set; }
        public DealProductRef Product { get; set; }
    }
}
