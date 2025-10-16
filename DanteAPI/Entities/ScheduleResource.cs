using System;
namespace DanteAPI.Entities {
    public class ScheduleResource {
        public int ID { get; set; }
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
        public short ResourceTypeID { get; set; }
        public string ResourceType { get; set; }
        public DateTime? EnteredDate { get; set; }
        public DateTime? BookedDate { get; set; }
        public string ImportID { get; set; }
        public decimal? Cost { get; set; }
        public byte CostPer { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomField { get; set; }
    }
}