namespace BookIt.Domain.Entities
{
    public class Tenant
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; // npr. "Fizioterapija", "Frizer"

        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        // javni identifikator u URL-u: /marko-fizio
        public string Slug { get; set; } = string.Empty;

        // vlasnik
        public int OwnerUserId { get; set; }
        public User? OwnerUser { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // dani kada ne radi (praznici, godisnji, specifični slobodni dani)
        public List<DateTime> OffDays { get; set; } = new List<DateTime>();

        //but i think this should always be configured at the same time as tenant registration!
        public TenantConfiguration? Configuration { get; set; }
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
