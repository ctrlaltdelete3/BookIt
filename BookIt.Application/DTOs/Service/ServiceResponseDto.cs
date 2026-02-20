namespace BookIt.Application.DTOs.Service
{
    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int BreakMinutesAfterService { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
    }
}
