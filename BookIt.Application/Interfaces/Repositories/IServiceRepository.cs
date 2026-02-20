using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Repositories
{
    public interface IServiceRepository
    {
        Task CreateAsync(Service service);
        Task<Service?> GetByIdAsync(int id);
        Task<List<Service>> GetByTenantIdAsync(int tenantId);
        Task UpdateAsync(Service service);
    }
}