using BookIt.Domain.Enums;

namespace BookIt.Domain.Entities
{
    public class TenantConfiguration
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public int BookingWeeksAhead { get; set; } = 3;

        public int? AutoRejectHours { get; set; }

        public bool AllowCancellation { get; set; } = true;

        public int CancellationHoursBefore { get; set; } = 24;

        public NotificationType NotificationType { get; set; } = NotificationType.Email;
    }

}
