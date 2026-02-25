namespace BookIt.Domain.Entities
{
    public class WorkingHour
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        // currently: 0 = sunday, 1 = monday, ....,  6 = saturday
        public int DayOfWeek { get; set; }

        public bool IsWorkingDay { get; set; } = true;

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        //TODO: think how to implement validation so pause and service don't overlap
        public TimeOnly? PauseStart { get; set; }
        public TimeOnly? PauseEnd { get; set; }
    }
}
