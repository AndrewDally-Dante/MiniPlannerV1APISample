using System;
namespace DanteAPI.Entities {
    public class Cart {
        public int ID { get; set; }
        public string ImportID { get; set; }
        public int? BookingID { get; set; }
        public Booking Booking { get; set; }
        public int? DelegateID { get; set; }
        public Delegate Delegate { get; set; }
        public DateTime? EnteredDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CustomField { get; set; }
    }
}