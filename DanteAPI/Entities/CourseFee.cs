using System;
namespace DanteAPI.Entities {
    public class CourseFee {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public int? TaxCodeID { get; set; }
        public References.TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }
        public References.GenericItem FeeType { get; set; }
        public int FeeTypeID { get; set; }
    }
}