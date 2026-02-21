using BookIt.Application.Interfaces.Repositories;
using BookIt.DAL.Context;
using BookIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookIt.DAL.Repositories
{
    public class WorkingHourRepository : IWorkingHourRepository
    {
        private readonly BookItDbContext _context;
        public WorkingHourRepository(BookItDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkingHour>> GetAll(int tenantId)
        {
            return await _context.WorkingHours.Where(h => h.TenantId == tenantId).ToListAsync();
        }

        public async Task SetAll(List<WorkingHour> workingHours, int tenantId)
        {
            //remove old working hours before setting up new ones
            DeleteAllForTenant(tenantId);
            foreach (var workingHour in workingHours)
            {
                _context.WorkingHours.Add(workingHour);
            }

            await _context.SaveChangesAsync();
        }

        private void DeleteAllForTenant(int tenantId)
        {
            var workingHoursToRemove = _context.WorkingHours.Where(h => h.TenantId == tenantId);
            _context.RemoveRange(workingHoursToRemove);
        }
    }
}
