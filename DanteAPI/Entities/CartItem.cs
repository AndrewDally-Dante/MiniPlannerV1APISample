using System;
namespace DanteAPI.Entities {
    public class CartItem {
        public int ID { get; set; }
        public int CartID { get; set; }
        public Cart Cart { get; set; }
        public int? CourseID { get; set; }
        public Course Course { get; set; }
        public int? ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
        public int? DelegateID { get; set; }
        public Delegate Delegate { get; set; }
        public decimal? Price { get; set; }
        public string CustomField { get; set; }
    }
}