namespace BookIt.Application.DTOs.Service
{
    public class CreateServiceTimeSlotDto
    {
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
