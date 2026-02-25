namespace BookIt.Domain.Entities
{
    public class ServiceTimeSlot
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        // currently: 0 = sunday, 1 = monday, ....,  6 = saturday
        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        // tenant može deaktivirati slot bez brisanja
        public bool IsActive { get; set; } = true;

        //TODO: i think i won't be needing this (remove property later if that's correct)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
