using BookIt.Application.Interfaces.Repositories;
using BookIt.DAL.Context;
using BookIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookIt.DAL.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly BookItDbContext _context;

        public ServiceRepository(BookItDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            var service = await _context.Services
                .Include(s => s.Tenant)
                .Include(s=>s.TimeSlots)
                .FirstOrDefaultAsync(s => s.Id == id);
            return service;
        }

        public async Task<List<Service>> GetByTenantIdAsync(int tenantId)
        {
            var service = await _context.Services
                .Include(s => s.Tenant)
                .Where(s => s.IsActive)
                .Where(s => s.TenantId == tenantId).ToListAsync();
            return service;
        }

        public async Task UpdateAsync(Service service)
        {
            await _context.SaveChangesAsync();
        }
    }
}
