namespace BookIt.Application.DTOs.Appointment
{
    public class AvailableSlotDto
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
