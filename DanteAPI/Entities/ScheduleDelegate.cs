namespace DanteAPI.Entities
{
    public class ScheduleDelegate : EntityWithCustomFields
    {
        public int ID { get; set; }
        public int ScheduleID { get; set; }
        public int Reference { get; set; }
        public string ImportID { get; set; }
        public DateTime EnteredDate { get; set; }
        public Schedule Schedule { get; set; }
        public int BookingID { get; set; }
        public Booking Booking { get; set; }

        public int DelegateID { get; set; }
        public Delegate Delegate { get; set; }
        public short Quantity { get; set; }
        public bool GroupBooking { get; set; }

        public int? StatusID { get; set; }
        public References.Status Status { get; set; }
        public decimal Price { get; set; }
        public int? TaxCodeID { get; set; }
        public References.TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }

        public References.Result Result { get; set; }
        public int? ResultID { get; set; }

        public decimal? ResultMark { get; set; }
        public System.DateTime? CertificateRequested { get; set; }
        public System.DateTime? CertificateReceived { get; set; }
        public System.DateTime? CertificateSent { get; set; }
        public string Notes { get; set; }
        public System.DateTime? ModifiedDate { get; set; }

        public string StatusLookup { get; set; }
        public string ResultLookup { get; set; }
    }
}
