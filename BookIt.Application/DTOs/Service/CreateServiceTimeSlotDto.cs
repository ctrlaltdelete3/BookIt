namespace BookIt.Application.DTOs.Service
{
    public class CreateServiceTimeSlotDto
    {
        //nullable needed for fluentValidator --> to catch if not provided, otherwise it'll be set to 0 and validation will pass
        public int? DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
