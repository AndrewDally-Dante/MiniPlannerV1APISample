using System.Collections.Generic;

namespace DanteAPI.Entities.References
{
    public class DealCourseItem
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public short Quantity { get; set; }
        public bool SellWholeCourse { get; set; }
        public decimal Price { get; set; }
        public int? TaxCodeID { get; set; }
        public string Notes { get; set; }
        public Course Course { get; set; }
        public TaxCode TaxCode { get; set; }
        public ICollection<DealCourseItemFee> Fees { get; set; }
        public ICollection<DealCourseItemDate> Dates { get; set; }
        public ICollection<DealCourseItemSchedule> Schedules { get; set; }
        public decimal Total { get; set; }
    }
}
