namespace BookIt.Application.DTOs.WorkingHours
{
    public class WorkingHourDto
    {
        public int DayOfWeek { get; set; }
        public bool IsWorkingDay { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? PauseStart { get; set; }
        public TimeOnly? PauseEnd { get; set; }
    }
}
