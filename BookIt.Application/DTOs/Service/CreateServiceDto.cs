namespace BookIt.Application.DTOs.Service
{
    public class CreateServiceDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        //needed to set BreakMinutesAfterService and Price as nullable, so that validation can catch if they are not provided
        public int DurationMinutes { get; set; }
        public int? BreakMinutesAfterService { get; set; }
        //TODO - check if this is nullable in ServiceResponseDTO and elsewhere? 
        public decimal? Price { get; set; }
    }
}
