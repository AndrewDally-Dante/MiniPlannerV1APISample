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
        public string SpecialRequirements { get; set; }
        public string Notes { get; set; }
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

        public bool Coordinator { get; set; }

        #region Custom Fields
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
