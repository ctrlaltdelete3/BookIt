namespace BookIt.Application.DTOs.Service
{
    public class UpdateServiceDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int BreakMinutesAfterService { get; set; }
        public decimal Price { get; set; }
    }
}
