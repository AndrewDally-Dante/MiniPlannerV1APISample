namespace DanteAPI.Entities.References
{
    public class DealCourseItemDate
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
