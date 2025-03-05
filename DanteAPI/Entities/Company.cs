namespace DanteAPI.Entities
{
    public  class Company
    {
        public int ID { get; set; }
        public string ImportID { get; set; }
        public short CompanyType { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string AddressCountryID { get; set; }
        public string Postcode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Notes { get; set; }
        public string InvoiceAddressLine1 { get; set; }
        public string InvoiceAddressLine2 { get; set; }
        public string InvoiceAddressLine3 { get; set; }
        public string InvoiceAddressLine4 { get; set; }
        public string InvoiceAddressLine5 { get; set; }

        public string InvoiceAddressCountryID { get; set; }

        public string InvoicePostcode { get; set; }
        public string InvoiceEmail { get; set; }
        public string InvoiceName { get; set; }
        public string InvoicePhoneNumber { get; set; }
        public string VATNumber { get; set; }

        public System.DateTime? EnteredDate { get; set; }

        public System.DateTime? ModifiedDate { get; set; }

        #region Custom Fields
        //Custom fields are dynamic, so the below can be expanded to include additional fields
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public string Custom6 { get; set; }
        public string Custom7 { get; set; }
        public string Custom8 { get; set; }
        public string Custom9 { get; set; }
        public string Custom10 { get; set; }

        #endregion


    }
}
