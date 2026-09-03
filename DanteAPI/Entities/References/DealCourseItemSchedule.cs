namespace DanteAPI.Entities.References
{
    public class DealCourseItemSchedule
    {
        public int ID { get; set; }
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
    }
}
