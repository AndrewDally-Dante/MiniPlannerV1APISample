namespace DanteAPI.Entities
{
    public class Delegate
    {
        public int ID { get; set; }
        public string ImportID { get; set; }
        public int Reference { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Initial { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberExtension { get; set; }
        public string MobileNumber { get; set; }
        public System.DateTime? DateOfBirth { get; set; }

        public int? GenderID { get; set; }
        public int? EthnicityID { get; set; }

        public string NationalInsuranceNumber { get; set; }
        public string PayrollNumber { get; set; }
        public string DietaryRequirements { get; set; }
        public References.GenericItem Gender { get; set; }
        public References.GenericItem Ethnicity { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? LeaveDate { get; set; }
        public int? CompanyID { get; set; }
        public Company Company { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }

        public string AddressCountryID { get; set; }

        public string Postcode { get; set; }
        public System.DateTime? EnteredDate { get; set; }

        public System.DateTime? ModifiedDate { get; set; }

        public string ImageURL { get; set; }
        public References.File ImageFile { get; set; }
        public bool Coordinator { get; set; }
        public string CustomField { get; set; }
        public string GenderLookup { get; set; }
        public string EthnicityLookup { get; set; }
        public string AddressCountryLookup { get; set; }
        public string CompanyNameLookup { get; set; }
        public string CompanyReferenceLookup { get; set; }
        public string CompanyImportIDLookup { get; set; }

    }
}
