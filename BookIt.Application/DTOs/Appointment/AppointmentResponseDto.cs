using BookIt.Domain.Enums;

namespace BookIt.Application.DTOs.Appointment
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public AppointmentStatus Status { get; set; } 
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancellationReason { get; set; }
    }
}
