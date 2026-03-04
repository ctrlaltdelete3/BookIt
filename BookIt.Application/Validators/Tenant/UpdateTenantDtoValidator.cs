using BookIt.Application.DTOs.Tenant;
using FluentValidation;

namespace BookIt.Application.Validators.Tenant
{
    public class UpdateTenantDtoValidator : AbstractValidator<UpdateTenantDto>
    {
        public UpdateTenantDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tenant name is required.")
                .MinimumLength(1).WithMessage("Tenant name must be at least one character.")
                .MaximumLength(50).WithMessage("Tenant name cannot exceed length of 50 characters.");

            RuleFor(x => x.ActivityType)
                .NotEmpty().WithMessage("Activity type is required.")
                .MinimumLength(5).WithMessage("Activity type must be at least five character.")
                .MaximumLength(100).WithMessage("Activity type cannot exceed length of 100 characters.");

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
