using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Repositories
{
    public interface IWorkingHourRepository
    {
        Task<List<WorkingHour>> GetAllAsync(int tenantId);
        Task SetAllAsync(List<WorkingHour> workingHours, int tenantId);
    }
}