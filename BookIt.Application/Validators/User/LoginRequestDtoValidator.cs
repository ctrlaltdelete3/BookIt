using BookIt.Application.DTOs.User;
using FluentValidation;

namespace BookIt.Application.Validators.User
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // login only checks that a password was sent - complexity rules belong to RegisterRequestDtoValidator
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
