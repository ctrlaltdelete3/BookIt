namespace BookIt.Application.DTOs.Service
{
    public class UpdateServiceDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        //nullable needed for fulentValidator --> to catch if not provided, otherwise it'll be set to 0 and validation will pass
        public int? BreakMinutesAfterService { get; set; }
        public decimal? Price { get; set; }
    }
}
