using BookIt.Application.DTOs.Appointment;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;
using BookIt.Services.Models;

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
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found.");
            }

            var availableTimeSlotsForDate = await GetAvailableTimeSlotsAsync(tenantId, service, date);

            return GenerateAvailableSlotsDto(availableTimeSlotsForDate, date, service.DurationMinutes);

        }

        #region HelperMethods

        private async Task<List<TimeOnly>> GetAvailableTimeSlotsAsync(int tenantId, Service service, DateOnly date)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var appointmentDayOfWeek = (int)date.DayOfWeek;
            //working hours of selected day of the week, for this tenant.
            var workingHours = tenant.WorkingHours
                                            .FirstOrDefault(h => h.DayOfWeek == appointmentDayOfWeek && h.IsWorkingDay);


            if (workingHours == null)
            {
                //TODO: maybe better exception message? 
                throw new InvalidOperationException("You can't book an appointment on non working day.");
            }

            //TODO: i think i need more basic model for time slots.
            //time slots that are available for this service on selected day. 
            //if there is a break, then time slots will be split into two parts, before and after the break.
            var timeSlotsOfTheDay = new List<TimeSlot>();
            if (workingHours.PauseStart != null)
            {
                timeSlotsOfTheDay.Add(new TimeSlot
                {
                    StartTime = workingHours.StartTime.Value,
                    EndTime = workingHours.PauseStart.Value,
                });

                timeSlotsOfTheDay.Add(new TimeSlot
                {
                    StartTime = workingHours.PauseEnd.Value,
                    EndTime = workingHours.EndTime.Value

                });
            }
            else
            {
                timeSlotsOfTheDay.Add(new TimeSlot
                {
                    StartTime = workingHours.StartTime.Value,
                    EndTime = workingHours.EndTime.Value,
                });
            }

            //we need to check if there are any other appointments booked for this date for this tenant.
            var appointmentsOfTheDay = await _appointmentRepository.GetFilteredAppointmentsByTenantAndDateAsync(tenant.Id, date);

            //now we need to "update" timeSlotsOfTheDay (by removing booked time slots),
            //so we can return only available time slots.
            if (appointmentsOfTheDay.Any())
            {
                for (int i = 0; i < appointmentsOfTheDay.Count; i++)
                {
                    for (int j = 0; j < timeSlotsOfTheDay.Count; j++)
                    {
                        //here we check if the booked appointment is within this time slot[j], and if it is,
                        //we need to split the time slot into two parts (before and after the booked appointment)
                        if (appointmentsOfTheDay[i].StartTime >= timeSlotsOfTheDay[j].StartTime
                            && appointmentsOfTheDay[i].EndTime <= timeSlotsOfTheDay[j].EndTime)
                        {
                            //we need to store the old end time of the time slot,
                            //because we will use it for the new time slot that we will create after the booked appointment
                            var oldTimeSlotOfTheDayEndTime = timeSlotsOfTheDay[j].EndTime;

                            timeSlotsOfTheDay[j].EndTime = appointmentsOfTheDay[i].StartTime;
                            timeSlotsOfTheDay.Add(new TimeSlot
                            {
                                StartTime = appointmentsOfTheDay[i].EndTime,
                                EndTime = oldTimeSlotOfTheDayEndTime,
                            });

                            //break inner loop, because we added new time slot, we need to start over.
                            j = timeSlotsOfTheDay.Count;
                        }
                    }
                }
            }

            //now we need to return only available time slots:

            //we need to use service duration AND break time after service,
            //because we need to make sure that the next appointment can be booked only after the break time
            //TODO: i want to make changes in future, to check if EndTime of time slot we are looking at is before lunch break or end of working day, and if so, then we will not add break time after service in that case
            var serviceDuration = TimeSpan.FromMinutes(service.DurationMinutes + service.BreakMinutesAfterService); 

            var availableTimeSlots = new List<TimeOnly>();

            //helper variable for storing current StartTime we are checking for availability
            var helperTimeOnlyVariable = new TimeOnly();

            for (int i = 0; i < timeSlotsOfTheDay.Count; i++)
            {
                //using this helper variable for calculating available time slots; 
                //reseting value to the startTime of current available time slot we are iterating through
                helperTimeOnlyVariable = timeSlotsOfTheDay[i].StartTime;

                while ((timeSlotsOfTheDay[i].EndTime - helperTimeOnlyVariable) >= serviceDuration)
                {
                    availableTimeSlots.Add(helperTimeOnlyVariable);
                    helperTimeOnlyVariable = helperTimeOnlyVariable.AddMinutes(serviceDuration.TotalMinutes);
                }
            }

            return availableTimeSlots;
        }

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
