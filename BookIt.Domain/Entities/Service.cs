namespace BookIt.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int DurationMinutes { get; set; }
        //if tenant desides to have a break after some service
        public int BreakMinutesAfterService { get; set; } = 0;

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ServiceAvailability> Availabilities { get; set; } = new List<ServiceAvailability>();
        public ICollection<ServiceTimeSlot> TimeSlots { get; set; } = new List<ServiceTimeSlot>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
