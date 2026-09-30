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

            When(x => x.IsWorkingDay, () =>
           {
               RuleFor(x => x.StartTime)
                   .NotNull().WithMessage(x => $"Day {x.DayOfWeek}: start time is required since it's a working day.");
               RuleFor(x => x.EndTime)
                   .NotNull().WithMessage(x => $"Day {x.DayOfWeek}: end time is required since it's a working day.")
                   .GreaterThan(x => x.StartTime).WithMessage(x => $"Day {x.DayOfWeek}: end time must be greater than start time.");
           });

            When(x => x.IsWorkingDay == false, () =>
            {
                RuleFor(x => x.StartTime)
                    .Null().WithMessage(x => $"Day {x.DayOfWeek}: start time must be null since it's not a working day.");
                RuleFor(x => x.EndTime)
                    .Null().WithMessage(x => $"Day {x.DayOfWeek}: end time must be null since it's not a working day.");
                RuleFor(x => x.PauseStart)
                    .Null().WithMessage(x => $"Day {x.DayOfWeek}: pause start time must be null since it's not a working day.");
                RuleFor(x => x.PauseEnd)
                    .Null().WithMessage(x => $"Day {x.DayOfWeek}: pause end time must be null since it's not a working day.");
            });


            RuleFor(x => x.PauseStart)
                .NotNull().When(x => x.PauseEnd != null).WithMessage(x => $"Day {x.DayOfWeek}: pause start time is required when pause end time is specified.")
                .GreaterThan(x => x.StartTime).WithMessage(x => $"Day {x.DayOfWeek}: pause start time must be greater than start time.");

            RuleFor(x => x.PauseEnd)
                .NotNull().When(x => x.PauseStart != null).WithMessage(x => $"Day {x.DayOfWeek}: pause end time is required when pause start time is specified.")
                .GreaterThan(x => x.PauseStart).WithMessage(x => $"Day {x.DayOfWeek}: pause end time must be greater than pause start time.")
                .LessThan(x => x.EndTime).WithMessage(x => $"Day {x.DayOfWeek}: pause end time must be less than end time.");
        }

    }
}
