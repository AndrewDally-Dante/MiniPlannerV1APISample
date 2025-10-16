using DanteAPI.Entities.References;

namespace DanteAPI.Entities
{
    public class Schedule
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public string ImportID { get; set; }
        public int? MasterScheduleID { get; set; }
        public string Reference { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<ScheduleDate> Dates { get; set; }
        public short DayCount { get; set; }
        public short DayNumber { get; set; }
        public string Duration { get; set; }
        public int? StatusID { get; set; }
        public References.Status Status { get; set; }
        public int PlacesLeft { get; set; }
        public int CountDelegates { get; set; }
        public short MinDelegates { get; set; }
        public short MaxDelegates { get; set; }
        public TimeSpan? ArriveTime { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? LunchTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? BookingID { get; set; }
        public decimal DelegatePrice { get; set; }
        public decimal CoursePrice { get; set; }
        public int? TaxCodeID { get; set; }
        public References.TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CourseLocation { get; set; }
        public string VenueAddressLine1 { get; set; }
        public string VenueAddressLine2 { get; set; }
        public string VenueAddressLine3 { get; set; }
        public string VenueAddressLine4 { get; set; }
        public string VenueAddressLine5 { get; set; }
        public string VenueAddressCountryID { get; set; }
        public string VenuePostcode { get; set; }
        public string VenueEmail { get; set; }
        public string VenuePhoneNumber { get; set; }
        public string VenueNotes { get; set; }
        public int? VenueResourceID { get; set; }
        public References.GenericItem VenueSite { get; set; }
        public Resource VenueResource { get; set; }
        public References.GenericItem Site { get; set; }
        public Resource TutorResource { get; set; }
        public int? TutorResourceID { get; set; }
        public Resource EquipmentResource { get; set; }
        public int? EquipmentResourceID { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string Notes { get; set; }
        public bool ShowOnline { get; set; }
        public DateTime? OnlineBookingCloses { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public References.CourseType CourseType { get; set; }
        public int? CourseTypeID { get; set; }
        public string CustomField { get; set; }
        public string StatusLookup { get; set; }
        public string VenueAddressCountryLookup { get; set; }
    }
}
