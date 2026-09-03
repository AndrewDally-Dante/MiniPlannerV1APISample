namespace DanteAPI.Entities
{
    public class OtherTraining : EntityWithCustomFields
    {
        public int ID { get; set; }
        public string ImportID { get; set; }
        public int DelegateID { get; set; }
        public Delegate Delegate { get; set; }
        public int? CoordinatorDelegateID { get; set; }
        public string CourseName { get; set; }
        public string CourseLocation { get; set; }
        public decimal Price { get; set; }
        public decimal? Days { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? StatusID { get; set; }
        public References.Status Status { get; set; }
        public References.Result Result { get; set; }
        public int? ResultID { get; set; }
        public decimal ResultMark { get; set; }
        public string Notes { get; set; }
        public string StatusLookup { get; set; }
        public string ResultLookup { get; set; }
    }
}
