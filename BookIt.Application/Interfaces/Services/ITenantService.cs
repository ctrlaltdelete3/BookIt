using BookIt.Application.DTOs.Tenant;

namespace BookIt.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Task<TenantResponseDto> CreateTenantAsync(CreateTenantDto tenantDto, int ownerUserId);
        Task<TenantResponseDto> GetMyTenantAsync(int ownerUserId);
        Task<TenantResponseDto?> GetTenantByIdAsync(int id);
        Task<TenantResponseDto> GetTenantBySlugAsync(string slug);
        Task<TenantResponseDto> UpdateTenantAsync(int id, UpdateTenantDto updateTenantDto, int userId);
    }
}