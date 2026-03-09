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
        private readonly ITenantRepository _tenantRepository;
        private readonly IAvailabilityService _availabilityService;

        public AppointmentService(IAppointmentRepository appointmentRepository, ITenantRepository tenantRepository, IAvailabilityService availabilityService)
        {
            _appointmentRepository = appointmentRepository;
            _tenantRepository = tenantRepository;
            _availabilityService = availabilityService;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int userId)
        {
            //check if requested date is still available for this service

            var listOfAvailableTimeSlots = await _availabilityService.GetAvailableSlotsAsync(appointmentDto.TenantId, appointmentDto.ServiceId, appointmentDto.Date);
            var appointmentTimeSlot = listOfAvailableTimeSlots.FirstOrDefault(t => t.StartTime == appointmentDto.StartTime);
            if (appointmentTimeSlot == null)
            {
                throw new InvalidOperationException("Requested appointment date and/or time is not available.");
            }

            var serviceDurationTime = appointmentTimeSlot.EndTime - appointmentTimeSlot.StartTime;

            var appointment = new Appointment
            {
                Date = appointmentDto.Date,
                Note = appointmentDto.Note,
                StartTime = appointmentDto.StartTime,
                EndTime = appointmentDto.StartTime.Add(serviceDurationTime),
                TenantId = appointmentDto.TenantId,
                ServiceId = appointmentDto.ServiceId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = AppointmentStatus.Pending,
            };

            await _appointmentRepository.CreateAsync(appointment);

            var appointmentDbo = await _appointmentRepository.GetByIdAsync(appointment.Id);

            //in case there was some issue with creating this appointment:
            if (appointmentDbo == null)
            {
                throw new InvalidOperationException("There was an issue with creating the appointment. Please try again.");
            }

            return GenerateResponse(appointmentDbo);
        }

        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(int appointmentId, int userId)
        {
            var appointment = await GetAppointmentIfAuthorizedAsync(appointmentId, userId, compareBoth: true);
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

        public async Task<List<AppointmentResponseDto>> GetTenantAppointmentsAsync(int userId)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var appointments = await _appointmentRepository.GetAppointmentsByTenantIdAsync(tenant.Id);

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
        public async Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(int appointmentId, int userId, UpdateAppointmentDto updateAppointmentDto)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);

            if (tenant == null)
            {
                //TODO: check - this should not happen? Something prior to this in flow will fail if tenant is null?
                throw new KeyNotFoundException("Tenant not found.");
            }

            var appointment = await GetAppointmentIfAuthorizedAsync(appointmentId, tenant.Id, isTenant: true);

            appointment.Status = updateAppointmentDto.Status;

            if (appointment.Status == AppointmentStatus.Confirmed)
            {
                appointment.ConfirmedAt = DateTime.UtcNow;
                appointment.TenantNote = updateAppointmentDto.Note;
            }
            else if (appointment.Status == AppointmentStatus.Rejected)
            {
                appointment.RejectedAt = DateTime.UtcNow;
                appointment.RejectionReason = updateAppointmentDto.Note;
            }

            await _appointmentRepository.UpdateAsync();

            return GenerateResponse(appointment);
        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(int appointmentId, int userId, CancelAppointmentDto cancelAppointmentDto)
        {
            bool isTenant = false;
            int idToCompare = userId;

            var tenant = await _tenantRepository.GetMyTenantAsync(userId);

            if (tenant != null)
            {
                isTenant = true;
                idToCompare = tenant.Id;
            }
            //TODO IVANA: moram prvo provjeriti je li user koji se ulogirao i zeli cancel napraviti - tenant
            //ako jeste onda to moram proslijediti ovdje + pogledati ima li ova metoda dobru logiku.
            //nakon toga, ako je tenant null bio, onda ga ispod moram dohvatiti zbog ovog canclation without fee
            var appointment = await GetAppointmentIfAuthorizedAsync(appointmentId, idToCompare, isTenant: isTenant);

            //get tenant if by this point we know that user requesting cancelation is not tenant
            if (tenant == null)
            {
                tenant = await _tenantRepository.GetByIdAsync(appointment.TenantId);
            }

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            //check if user has rights to cancel appointment without paying the fee
            var cancelationWithoutPayingFee = CancelWithoutPayingFee(appointment.Date, appointment.StartTime, tenant.Configuration?.CancellationHoursBefore);

            if (cancelationWithoutPayingFee == false)
            {
                //TODO: implement logic for service payment because cancelation is late.
                appointment.TenantNote = "Late cancellation.";
            }

            appointment.Status = AppointmentStatus.Canceled;
            appointment.CanceledAt = DateTime.UtcNow;
            appointment.CancellationReason = cancelAppointmentDto.CancelationMessage;

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
                RejectionReason = appointment.RejectionReason,
                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Note = appointment.Note,
                TenantNote = appointment.TenantNote,
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

        private async Task<Appointment> GetAppointmentIfAuthorizedAsync(int appointmentId, int idToCompare, bool isTenant = false, bool compareBoth = false)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            if (compareBoth)
            {
                if (idToCompare != appointment.UserId
                    && idToCompare != appointment.Tenant.OwnerUserId)
                {
                    throw new UnauthorizedAccessException("You are not authorized.");
                }
            }
            else if (isTenant)
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
