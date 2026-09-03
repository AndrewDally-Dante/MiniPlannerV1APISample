namespace DanteAPI.Entities.References
{
    public class DealCourseItemFee
    {
        public int ID { get; set; }
        public int FeeTypeGIID { get; set; }
        public decimal Amount { get; set; }
        public int? TaxCodeID { get; set; }
        public TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }
        public GenericItem FeeType { get; set; }
    }
}
