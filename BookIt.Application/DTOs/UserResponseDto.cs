namespace BookIt.Application.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public bool IsTenantOwner { get; set; }
        public int? OwnedTenantId { get; set; }
    }
}
