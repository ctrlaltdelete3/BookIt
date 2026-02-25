using BookIt.Application.DTOs.Appointment;

namespace BookIt.Application.Interfaces.Services
{
    public interface IAvailabilityService
    {
        Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int tenantId, int serviceId, DateOnly date);
    }
}