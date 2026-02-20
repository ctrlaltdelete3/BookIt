using BookIt.Application.DTOs.Service;

namespace BookIt.Application.Interfaces.Services
{
    public interface IServiceService
    {
        Task<ServiceResponseDto> CreateServiceAsync(CreateServiceDto createServiceDto, int tenantId);
        Task DeleteServiceAsync(int serviceId, int userId);
        Task<List<ServiceResponseDto>> GetAllServicesByTenant(string slug);
        Task<ServiceResponseDto> GetByIdAsync(int serviceId);
        Task<ServiceResponseDto> UpdateServiceAsync(int serviceId, UpdateServiceDto updateServiceDto, int userId);
    }
}