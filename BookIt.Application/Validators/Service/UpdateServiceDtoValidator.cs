using BookIt.Application.DTOs.Service;
using FluentValidation;

namespace BookIt.Application.Validators.Service
{
    public class UpdateServiceDtoValidator : AbstractValidator<UpdateServiceDto>
    {
        public UpdateServiceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MinimumLength(1).WithMessage("Service name must be at least one character.")
                .MaximumLength(50).WithMessage("Service name cannot exceed length of 50 characters.");

            RuleFor(x => x.DurationMinutes)
                .NotEmpty().WithMessage("Service duration is required.")
                .GreaterThan(0).WithMessage("Service duration must be greater than zero.");

            RuleFor(x => x.BreakMinutesAfterService)
                .NotNull().WithMessage("Break after service value is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Break after service value must be greater than or equal to zero.");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("Service price is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Service price must be greater than or equal to zero.");
        }
    }
}
