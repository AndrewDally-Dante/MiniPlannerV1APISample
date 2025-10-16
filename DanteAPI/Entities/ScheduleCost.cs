using System;
namespace DanteAPI.Entities {
    public class ScheduleCost {
        public int ID { get; set; }
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
        public References.GenericItem CostType { get; set; }
        public int? CostTypeID { get; set; }
        public decimal Cost { get; set; }
        public byte CostPer { get; set; }
        public decimal TotalCost { get; set; }
    }
}