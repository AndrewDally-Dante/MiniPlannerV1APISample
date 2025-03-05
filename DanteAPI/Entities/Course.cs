namespace DanteAPI.Entities
{
    public class Course
    {
        public int ID { get; set; }
        public int TenantID { get; set; }

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
        public int? TaxCodeID { get; set; }
        //public Misc.TaxCode TaxCode { get; set; }
        public string NominalCode { get; set; }
        public System.TimeSpan? ArriveTime { get; set; }
        public System.TimeSpan? StartTime { get; set; }
        public System.TimeSpan? LunchTime { get; set; }
        public System.TimeSpan? EndTime { get; set; }
        public System.DateTime? EnteredDate { get; set; }
        public int? BackgroundColour { get; set; }
        public int? ForegroundColour { get; set; }
        public string Duration { get; set; }

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

        public string ShortReference { get; set; }
        public string ShortDescription { get; set; }
        public bool ShowOnline { get; set; }
        public string FriendlyURL { get; set; }


        #region Content Fields
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
        #endregion

        #region Image Fields
        public string ImageURL { get; set; }

        public string ImageBannerURL { get; set; }
        #endregion


    }
}
