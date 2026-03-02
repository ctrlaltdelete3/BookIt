using BookIt.Application.Interfaces.Repositories;
using BookIt.DAL.Context;
using BookIt.Domain.Entities;
using BookIt.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookIt.DAL.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {

        private readonly BookItDbContext _context;
        public AppointmentRepository(BookItDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Tenant)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync()
        {
            await _context.SaveChangesAsync();
        }
        //TODO: Add summary to all methods, before it becomes more complicated 

        /// <summary>
        /// All appointments for one tenant - get by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<Appointment>> GetAppointmentsByTenantIdAsync(int tenantId)
        {
            return await _context.Appointments
                    .Include(a => a.Tenant)
                    .Include(a => a.User)
                    .Include(a => a.Service)
                    .Where(a => a.TenantId == tenantId && a.Status != AppointmentStatus.Canceled)
                    .ToListAsync();
        }

        /// <summary>
        /// All appointments for one user - get by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId)
        {
            return await _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Tenant)
                    .Include(a => a.Service)
                    .Where(a => a.UserId == userId && a.Status != AppointmentStatus.Canceled)
                    .ToListAsync();
        }

        /// <summary>
        /// Get list of all appointments for tenant for one date - by tenant ID and Date info
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<Appointment>> GetAppointmentsByTenantAndDateAsync(int tenantId, DateOnly date)
        {
            return await _context.Appointments
                    .Include(a => a.Tenant)
                    .Where(a => a.TenantId == tenantId && a.Date == date)
                    .ToListAsync();
        }
    }
}
