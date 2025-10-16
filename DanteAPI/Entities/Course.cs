namespace DanteAPI.Entities
{
    public class Course
    {
        public int ID { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public decimal Days { get; set; }
        public string Notes { get; set; }
        public short MinDelegates { get; set; }
        public short MaxDelegates { get; set; }
        public int? OwnerUserID { get; set; }
        public byte? ExpiryMonths { get; set; }
        public decimal DelegatePrice { get; set; }
        public decimal CoursePrice { get; set; }
        public string Prerequisites { get; set; }
        public int? TaxCodeID { get; set; }
        public References.TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }
        public TimeSpan? ArriveTime { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? LunchTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? EnteredDate { get; set; }
        public int? BackgroundColour { get; set; }
        public int? ForegroundColour { get; set; }
        public string Duration { get; set; }
        public string ShortReference { get; set; }
        public string ShortDescription { get; set; }
        public bool ShowOnline { get; set; }
        public string FriendlyURL { get; set; }
        public string OnlineLink { get; set; }
        public int? OnlineBookingClosesHours { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public References.CourseType CourseType { get; set; }
        public int? CourseTypeID { get; set; }
        public string CustomField { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }
        public string Content4 { get; set; }
        public string Content5 { get; set; }
        public string Content6 { get; set; }
        public string Content7 { get; set; }
        public string Content8 { get; set; }
        public string Content9 { get; set; }
        public string Content10 { get; set; }
        public ICollection<References.CourseCategoryCourseLink> Categories { get; set; }
        public ICollection<CourseFee> Fees { get; set; }
        public References.File ImageFile { get; set; }
        public References.File ImageBannerFile { get; set; }

        #region Image Fields
        public string ImageURL { get; set; }

        public string ImageBannerURL { get; set; }
        #endregion


    }
}
