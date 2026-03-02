using BookIt.Application.DTOs.Appointment;
using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Services
{
    public interface IAvailabilityService
    {
        Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int tenantId, int serviceId, DateOnly date);
        Task<List<TimeOnly>> GetAvailableTimeSlotsAsync(Tenant tenant, Service service, DateOnly date);
    }
}