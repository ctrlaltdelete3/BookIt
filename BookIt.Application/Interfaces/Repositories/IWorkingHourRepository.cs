using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Repositories
{
    public interface IWorkingHourRepository
    {
        Task<List<WorkingHour>> GetAll(int tenantId);
        Task SetAll(List<WorkingHour> workingHours, int tenantId);
    }
}