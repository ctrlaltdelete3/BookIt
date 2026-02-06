namespace BookIt.Domain.Entities
{
    public class WorkingHour
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        // 1 = monday, 7 = sunday
        public int DayOfWeek { get; set; }

        public bool IsWorkingDay { get; set; } = true;

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public TimeSpan? PauseStart { get; set; }
        public TimeSpan? PauseEnd { get; set; }
    }
}
