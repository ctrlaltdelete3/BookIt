using BookIt.Application.DTOs.Appointment;
using BookIt.Domain.Enums;
using FluentValidation;

namespace BookIt.Application.Validators.Appointment
{
    public class UpdateAppointmentDtoValidator : AbstractValidator<UpdateAppointmentDto>
    {
        public UpdateAppointmentDtoValidator()
        {
            RuleFor(x => x.Status)
                .Must(s => s == AppointmentStatus.Confirmed || s == AppointmentStatus.Rejected).WithMessage("Status can only be set to 'Confirmed' or 'Rejected'."); ;
        }
    }
}
