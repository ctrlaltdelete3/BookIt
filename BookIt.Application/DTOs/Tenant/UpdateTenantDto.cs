namespace BookIt.Application.DTOs.Tenant
{
    public class UpdateTenantDto
    {
        public string Name { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }

        // TODO: give user option to update slug (after enabling it in CreateTenantDto), but validate uniqueness in any case
        //public string Slug { get; set; } = string.Empty;
    }
}
