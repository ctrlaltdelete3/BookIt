namespace BookIt.Application.DTOs.WorkingHours
{
    public class WorkingHourResponseDto
    {
        public int Id { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsWorkingDay { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? PauseStart { get; set; }
        public TimeOnly? PauseEnd { get; set; }
        public int TenantId { get; set; }
    }
}
