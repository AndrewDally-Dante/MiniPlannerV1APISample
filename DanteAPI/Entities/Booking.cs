namespace DanteAPI.Entities
{
    public class Booking
    {
        public int ID { get; set; }
        public string ImportID { get; set; }
        public int Reference { get; set; }
        public bool CreditNote { get; set; }
        public int? ParentBookingID { get; set; }
        public DateTime EnteredDate { get; set; }
        public int? EnteredByUserID { get; set; }
        public References.User EnteredByUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CompanyID { get; set; }
        public Company Company { get; set; }
        public int? CoordinatorDelegateID { get; set; }
        public Delegate CoordinatorDelegate { get; set; }
        public string CompanyReference { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string AddressCountryID { get; set; }
        public References.Country AddressCountry { get; set; }
        public string Postcode { get; set; }
        public int? StatusID { get; set; }
        public References.Status Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public int? PaymentStatusID { get; set; }
        public References.Status PaymentStatus { get; set; }
        public string NotesInternal { get; set; }
        public string NotesExternal { get; set; }
        public string PurchaseOrder { get; set; }
        public decimal? Total { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? TotalWithTax { get; set; }
        public ICollection<References.BookingItem> Items { get; set; }
        public string CustomField { get; set; }
        public string AddressCountryLookup { get; set; }
        public string StatusLookup { get; set; }
        public string PaymentStatusLookup { get; set; }
    }
}
