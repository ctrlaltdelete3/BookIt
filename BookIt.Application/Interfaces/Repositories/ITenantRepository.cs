using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant?> GetByIdAsync(int id);
        Task<Tenant?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug);
        Task UpdateAsync(Tenant tenant);
        Task<Tenant?> GetMyTenantAsync(int ownerUserId);
    }
}