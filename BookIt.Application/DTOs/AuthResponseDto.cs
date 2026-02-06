namespace BookIt.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } //Jwt token
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool IsTenantOwner { get; set; }
    }
}
