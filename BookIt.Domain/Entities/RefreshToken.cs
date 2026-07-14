namespace BookIt.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Token { get; set; } = null!;
        public bool IsRevoked { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
