using BookIt.Application.DTOs.Appointment;
using FluentValidation;

namespace BookIt.Application.Validators.Appointment
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty().WithMessage("Tenant id required.")
                .GreaterThan(0).WithMessage("Tenant id must be greater than zero.");

            RuleFor(x => x.ServiceId)
                           .NotEmpty().WithMessage("Service id required.")
                           .GreaterThan(0).WithMessage("Service id must be greater than zero.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date information is required.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Date must be today or in the future.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time information is required.");
        }
    }
}
