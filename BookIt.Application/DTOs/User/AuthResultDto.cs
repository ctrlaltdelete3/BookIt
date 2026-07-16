namespace BookIt.Application.DTOs.User
{
    public class AuthResultDto
    {
        public AuthResponseDto AuthResponseDto { get; set; }
        public string RefreshToken { get; set; }
    }
}
