namespace BookIt.Application.DTOs.Tenant
{
    public class CreateTenantDto
    {
        public string Name { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; // npr. "Fizioterapija", "Frizer"
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }

        // TODO: give user option to choose slug or generate it from name, but validate uniqueness in any case
        //public string Slug { get; set; } = string.Empty;
    }
}
