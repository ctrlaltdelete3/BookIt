using BookIt.Domain.Enums;

namespace BookIt.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        // client that booked this appointment
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancellationReason { get; set; }
    }
}
