using BookIt.Application.DTOs.Appointment;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Enums;

namespace BookIt.Services.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IServiceRepository _serviceRepository;
        public AvailabilityService(IAppointmentRepository appointmentRepository, ITenantRepository tenantRepository, IServiceRepository serviceRepository)
        {
            _appointmentRepository = appointmentRepository;
            _tenantRepository = tenantRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int tenantId, int serviceId, DateOnly date)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            var service = await _serviceRepository.GetByIdAsync(serviceId);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            if (service == null)
            {
                throw new KeyNotFoundException("Service not found.");
            }

            var appointmentDayOfWeek = (int)date.DayOfWeek;

            var workingDayForTenant = tenant.WorkingHours
                                    .Any(h => h.DayOfWeek == appointmentDayOfWeek);

            if (workingDayForTenant == false)
            {
                //TODO: maybe better exception message? 
                throw new InvalidOperationException("You can't book an appointment on non working day.");
            }

            var timeSlotsAvailableOnCurrentDate = service.TimeSlots
                                                .Where(t => t.DayOfWeek == appointmentDayOfWeek && t.IsActive)
                                                .Select(t => t.StartTime);

            if (timeSlotsAvailableOnCurrentDate.Any() == false)
            {
                throw new InvalidOperationException("This service is not available on current day of the week.");
            }

            var appointmentsOfTheDay = await _appointmentRepository.GetAppointmentsByTenantAndDateAsync(tenantId, date);

            var startTimeOfBookedAppointments = appointmentsOfTheDay
                                         .Where(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed) //appointments that are confirmed or waiting for confirmation
                                         .Select(a => a.StartTime);

            var startTimeOfAvailableTimeSlots = timeSlotsAvailableOnCurrentDate.Except(startTimeOfBookedAppointments).ToList();

            return GenerateAvailableSlotsDto(startTimeOfAvailableTimeSlots, date, service.DurationMinutes);

        }

        #region HelperMethods
        private List<AvailableSlotDto> GenerateAvailableSlotsDto(List<TimeOnly> startTimes, DateOnly date, int serviceDurationInMinutes)
        {
            var list = new List<AvailableSlotDto>();

            foreach (var item in startTimes)
            {
                var availableSlotDto = new AvailableSlotDto
                {
                    Date = date,
                    StartTime = item,
                    EndTime = item.AddMinutes(serviceDurationInMinutes)
                };
                list.Add(availableSlotDto);
            }

            return list;
        }
        #endregion

    }
}
