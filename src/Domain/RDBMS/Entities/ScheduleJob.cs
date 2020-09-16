namespace Domain.RDBMS.Entities
{
    public class ScheduleJob : IEntityBase
    {
        public int Id { get; set; }
        public string ScheduleId { get; set; }
        public int RequestId { get; set; }
    }
}
