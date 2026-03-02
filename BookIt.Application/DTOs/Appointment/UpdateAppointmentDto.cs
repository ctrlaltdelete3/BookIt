using BookIt.Domain.Enums;

namespace BookIt.Application.DTOs.Appointment
{
    public class UpdateAppointmentDto
    {
        public AppointmentStatus Status { get; set; }

        //for update-reject --> RejectionReason property
        //for update-confirm --> OwnerNote property
        public string? Note { get; set; }
    }
}
