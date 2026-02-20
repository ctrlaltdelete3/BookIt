using BookIt.Application.Interfaces.Repositories;
using BookIt.DAL.Context;
using BookIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookIt.DAL.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly BookItDbContext _context;

        public TenantRepository(BookItDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task<Tenant?> GetByIdAsync(int id)
        {
            return await _context.Tenants
                .Include(t => t.OwnerUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant?> GetBySlugAsync(string slug)
        {
            return await _context.Tenants
                .Include(t => t.OwnerUser)
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Tenants.AnyAsync(t => t.Slug == slug);
        }

        public async Task<Tenant?> GetMyTenantAsync(int ownerUserId)
        {
            return await _context.Tenants
                .Include(t => t.OwnerUser)
                .FirstOrDefaultAsync(t => t.OwnerUserId == ownerUserId);
        }
    }
}
