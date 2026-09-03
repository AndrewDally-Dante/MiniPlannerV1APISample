namespace DanteAPI.Entities
{
    public class Deal
    {
        public int ID { get; set; }
        public int Reference { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; }
        public string Name { get; set; }
        public DateTime EnteredDate { get; set; }
        public int EnteredByUserID { get; set; }
        public References.User EnteredByUser { get; set; }
        public DateTime Date { get; set; }
        public int DealStageID { get; set; }
        public References.DealStage DealStage { get; set; }
        public int OwnerUserID { get; set; }
        public References.User OwnerUser { get; set; }
        public int? CoordinatorDelegateID { get; set; }
        public Delegate CoordinatorDelegate { get; set; }
        public int? EnquirySourceGIID { get; set; }
        public References.GenericItem EnquirySource { get; set; }
        public string NotesInternal { get; set; }
        public string NotesExternal { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? ClosedLostReasonGIID { get; set; }
        public References.GenericItem ClosedLostReason { get; set; }
        public int? SiteGIID { get; set; }
        public References.GenericItem Site { get; set; }
        public bool Open { get; set; }
        public decimal? Total { get; set; }
        public ICollection<References.DealCourseItem> CourseItems { get; set; }
        public ICollection<References.DealAdHocItem> AdHocItems { get; set; }
        public string CustomField { get; set; }
    }
}
