using BookIt.Application.DTOs.WorkingHours;
using FluentValidation;

namespace BookIt.Application.Validators.WorkingHours
{
    public class WorkingHourDtoValidator : AbstractValidator<WorkingHourDto>
    {
        public WorkingHourDtoValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .InclusiveBetween(0, 6).WithMessage("Day of week information is required. It must be between 0 (sunday) and 6 (saturday).");
        }
    }
}
