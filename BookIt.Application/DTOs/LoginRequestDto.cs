using System.ComponentModel.DataAnnotations;

namespace BookIt.Application.DTOs
{
    public class LoginRequestDto
    {
        //TODO: check should i use FluidValidation!?!?!
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
