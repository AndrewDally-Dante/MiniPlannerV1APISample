namespace DanteAPI.Entities
{
    public class Booking
    {
        public int ID { get; set; }

        public int TenantID { get; set; }

        public string ImportID { get; set; }

        public int Reference { get; set; }
        public bool CreditNote { get; set; }
        public int? ParentBookingID { get; set; }
        public System.DateTime EnteredDate { get; set; }
        public System.DateTime? ModifiedDate { get; set; }

        public int? CompanyID { get; set; }
        public Company Company { get; set; }

        public int? CoordinatorDelegateID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string AddressCountryID { get; set; }
        public string Postcode { get; set; }

        public int? StatusID { get; set; }

        public System.DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public System.DateTime? PaymentDueDate { get; set; }
        public System.DateTime? PaymentDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public int? PaymentStatusID { get; set; }
        public string NotesInternal { get; set; }
        public string NotesExternal { get; set; }
        public string PurchaseOrder { get; set; }

        public Decimal? Total { get; private set; }

        public Decimal? TotalTax { get; private set; }
        public Decimal? TotalWithTax { get; private set; }

        //public ICollection<Misc.BookingItem> Items { get; set; }


    }
}
