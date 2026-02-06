using System.ComponentModel.DataAnnotations;

namespace BookIt.Application.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        public string? Phone { get; set; }
    }
}
