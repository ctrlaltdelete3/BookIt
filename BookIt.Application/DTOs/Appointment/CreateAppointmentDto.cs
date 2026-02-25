namespace BookIt.Application.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        public int TenantId { get; set; }
        public int ServiceId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public string? Note { get; set; }
    }
}
