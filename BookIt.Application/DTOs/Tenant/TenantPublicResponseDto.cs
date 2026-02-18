namespace BookIt.Application.DTOs.Tenant
{
    public class TenantPublicResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}
