using System;

namespace DanteAPI.Entities.References
{
    public class TaxCode
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public decimal Rate { get; set; }
        public string Description { get; set; }
    }
}