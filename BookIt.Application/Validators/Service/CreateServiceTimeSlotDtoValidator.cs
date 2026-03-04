using BookIt.Application.DTOs.Service;
using FluentValidation;

namespace BookIt.Application.Validators.Service
{
    public class CreateServiceTimeSlotDtoValidator : AbstractValidator<CreateServiceTimeSlotDto>
    {
        public CreateServiceTimeSlotDtoValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .InclusiveBetween(0,6).WithMessage("Day of week information is required. It must be between 0 (sunday) and 6 (saturday).");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Service start time information is required.");
        }
    }
}
