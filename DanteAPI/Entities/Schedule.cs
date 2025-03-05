namespace DanteAPI.Entities
{
    public class Schedule
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int? MasterScheduleID { get; set; }
        public string Reference { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short DayCount { get; set; }
        public short DayNumber { get; set; }
        public int? StatusID { get; set; }

        public int CountDelegates { get; set; }
        public short MinDelegates { get; set; }
        public short MaxDelegates { get; set; }
        public int PlacesLeft { get; set; }
        public TimeSpan? ArriveTime { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? LunchTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? BookingID { get; set; }
        public decimal DelegatePrice { get; set; }
        public decimal CoursePrice { get; set; }
        public decimal DelegateFees { get; set; }
        public int? TaxCodeID { get; set; }
        //public Misc.TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }
        public DateTime? ExpiryDate { get; set; }

        #region Venue Address
        public string CourseLocation { get; set; }

        public string VenueAddressLine1 { get; set; }

        public string VenueAddressLine2 { get; set; }

        public string VenueAddressLine3 { get; set; }

        public string VenueAddressLine4 { get; set; }

        public string VenueAddressLine5 { get; set; }

        public string VenuePostcode { get; set; }

        public string VenueEmail { get; set; }

        public string VenuePhoneNumber { get; set; }

        public string VenueNotes { get; set; }

        public int? VenueResourceID { get; set; }

        #endregion


        public int? TutorResourceID { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string Notes { get; set; }

        public bool ShowOnline { get; set; }

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
