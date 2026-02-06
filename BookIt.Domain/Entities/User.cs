namespace BookIt.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }

        //TODO: in some time add Role property, so there could be more than clinet/tenant(owner)testira
        //public string Role { get; set; } = "Client";

        public string PasswordHash { get; set; } = string.Empty;

        public List<FavoriteTenant> FavoriteTenants { get; set; } = new();

        public int? OwnedTenantId { get; set; }
        public bool IsTenantOwner { get; set; }

        public DateTime CreatedAt { get; set; }

        // navigacija
        public Tenant? Tenant { get; set; }
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
