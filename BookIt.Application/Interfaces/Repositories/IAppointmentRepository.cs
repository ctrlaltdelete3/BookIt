using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task CreateAsync(Appointment appointment);
        Task<Appointment?> GetByIdAsync(int id);
        Task UpdateAsync();
        //TODO: Add summary to all methods, before it becomes more complicated 

        /// <summary>
        /// All appointments for one tenant - get by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<List<Appointment>> GetAppointmentsByTenantIdAsync(int tenantId);

        /// <summary>
        /// All appointments for one user - get by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId);

        /// <summary>
        /// Get list of all appointments for tenant for one date - by tenant ID and Date info
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<List<Appointment>> GetAppointmentsByTenantAndDateAsync(int tenantId, DateOnly date);

        /// <summary>
        /// Check if requested date and time for an appointment are available
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Task<bool> IsTimeSlotTaken(int tenantId, DateOnly date, TimeOnly time);
    }
}