using BookIt.Application.DTOs.Appointment;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;
using BookIt.Domain.Enums;

namespace BookIt.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ITenantRepository _tenantRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IServiceRepository serviceRepository, ITenantRepository tenantRepository)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int userId)
        {
            var service = await _serviceRepository.GetByIdAsync(appointmentDto.ServiceId);
            if (service == null)
            {
                throw new KeyNotFoundException("Requested service not found.");
            }
            var tenant = await _tenantRepository.GetByIdAsync(appointmentDto.TenantId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            //check if service can still be booked 
            var timeSlotTaken = await _appointmentRepository.IsTimeSlotTaken(tenant.Id, appointmentDto.Date, appointmentDto.StartTime);

            if (timeSlotTaken)
            {
                throw new InvalidOperationException("Requested appointment date and/or time is no longer available.");
            }

            var appointment = new Appointment
            {
                Date = appointmentDto.Date,
                Note = appointmentDto.Note,
                StartTime = appointmentDto.StartTime,
                EndTime = appointmentDto.StartTime.AddMinutes(service.DurationMinutes),
                TenantId = appointmentDto.TenantId,
                ServiceId = appointmentDto.ServiceId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = AppointmentStatus.Pending,
            };

            await _appointmentRepository.CreateAsync(appointment);

            var appointmentDbo = await _appointmentRepository.GetByIdAsync(appointment.Id);

            //in case there was some issue with creating this appointment:
            if(appointmentDbo == null)
            {
                throw new InvalidOperationException("There was an issue with creating the appointment. Please try again.");
            }

            return GenerateResponse(appointmentDbo);
        }

        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }
            return GenerateResponse(appointment);
        }

        public async Task<List<AppointmentResponseDto>> GetMyAppointmentsAsync(int userId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByUserIdAsync(userId);
            var listOfAppointments = new List<AppointmentResponseDto>();
            if (appointments.Any() == false)
            {
                return listOfAppointments;
            }
            foreach (var appointment in appointments)
            {
                var response = GenerateResponse(appointment);
                listOfAppointments.Add(response);
            }
            return listOfAppointments;
        }

        //TODO: this should be only available to owner, check after controller is created
        public async Task<List<AppointmentResponseDto>> GetTenantAppointmentsAsync(int tenantId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByTenantIdAsync(tenantId);
            var listOfAppointments = new List<AppointmentResponseDto>();
            if (appointments.Any() == false)
            {
                return listOfAppointments;
            }
            foreach (var appointment in appointments)
            {
                var response = GenerateResponse(appointment);
                listOfAppointments.Add(response);
            }
            return listOfAppointments;
        }

        //only owner can do this - used to confirm or reject appointment
        public async Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(int appointmentId, int userId, AppointmentStatus status)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);

            if (tenant == null)
            {
                //TODO: check - this should not happen? Something prior to this in flow will fail if tenant is null?
                throw new KeyNotFoundException("Tenant not found.");
            }

            var appointment = await GetAppointmentIfAuthorizedAsync(appointmentId, tenant.Id, true);

            appointment.Status = status;

            if (status == AppointmentStatus.Confirmed)
            {
                appointment.ConfirmedAt = DateTime.UtcNow;
            }
            else if (status == AppointmentStatus.Rejected)
            {
                appointment.RejectedAt = DateTime.UtcNow;
            }

            await _appointmentRepository.UpdateAsync();

            return GenerateResponse(appointment);
        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(int appointmentId, int userId, string? cancelationMessage)
        {
            var appointment = await GetAppointmentIfAuthorizedAsync(appointmentId, userId);
            
            //check if user has rights to cancel appointment without paying the fee
            var tenant = await _tenantRepository.GetByIdAsync(appointment.TenantId);

            if (tenant == null)
            {
                //TODO: check - this should not happen? Something prior to this in flow will fail if tenant is null?
                throw new KeyNotFoundException("Tenant not found.");
            }
            var cancelationWithoutPayingFee = CancelWithoutPayingFee(appointment.Date, appointment.StartTime, tenant.Configuration?.CancellationHoursBefore);

            if (cancelationWithoutPayingFee == false)
            {
                //TODO: implement logic for service payment because cancelation is late.
                appointment.Note = "Late cancellation.";
            }

            appointment.Status = AppointmentStatus.Canceled;
            appointment.CanceledAt = DateTime.UtcNow;
            appointment.CancellationReason = cancelationMessage;

            await _appointmentRepository.UpdateAsync();
            return GenerateResponse(appointment);
        }

        #region HelperMethods
        private AppointmentResponseDto GenerateResponse(Appointment appointment)
        {
            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                ServiceId = appointment.ServiceId,
                ServiceName = appointment.Service.Name,
                UserId = appointment.UserId,
                UserName = appointment.User.FirstName + " " + appointment.User.LastName,
                TenantId = appointment.TenantId,
                TenantName = appointment.Tenant.Name,
                CreatedAt = appointment.CreatedAt,
                ConfirmedAt = appointment.ConfirmedAt,
                RejectedAt = appointment.RejectedAt,
                CanceledAt = appointment.CanceledAt,
                CancellationReason = appointment.CancellationReason,
                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Note = appointment.Note,
                Status = appointment.Status
            };
        }

        private bool CancelWithoutPayingFee(DateOnly appointmentDate, TimeOnly appointmentStartTime, int? cancellationHoursBefore)
        {
            if (cancellationHoursBefore == null)
            {
                return true;
            }
            var dateOfAppointment = new DateTime(appointmentDate, appointmentStartTime);
            var timeDifference = dateOfAppointment - DateTime.UtcNow;
            if (timeDifference.TotalHours >= cancellationHoursBefore)
            {
                return true;
            }
            return false;
        }

        private async Task<Appointment> GetAppointmentIfAuthorizedAsync(int appointmentId, int idToCompare, bool isTenant=false)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            if (isTenant)
            {
                CheckAuthorization(idToCompare, appointment.TenantId);
            }
            else 
            {
                CheckAuthorization(idToCompare, appointment.UserId);
            }

            return appointment;
        }

        private void CheckAuthorization(int userId, int appointmentUserId)
        {
            if (userId != appointmentUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make changes.");
            }
        }
        #endregion
    }
}
