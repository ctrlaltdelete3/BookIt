namespace BookIt.Domain.Entities
{
    public class ServiceTimeSlot
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        // 1 = monday, 7 = sunday (recurring pattern)
        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        // tenant može deaktivirati slot bez brisanja
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
