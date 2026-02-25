namespace BookIt.Domain.Entities
{
    //TODO: I think I wont use this, for now at least. everything wil be defined in ServiceTimeSlots
    public class ServiceAvailability
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        // null = svi dani, 1-7 = konkretan dan u tjednu
        public int? DayOfWeek { get; set; }

        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }

        // max termina po danu za tu uslugu (null = nema limita)
        public int? MaxPerDay { get; set; }
    }

}
