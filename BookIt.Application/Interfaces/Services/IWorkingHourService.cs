using BookIt.Application.DTOs.WorkingHours;

namespace BookIt.Application.Interfaces.Services
{
    public interface IWorkingHourService
    {
        Task<List<WorkingHourResponseDto>> GetAllAsync(string slug);
        Task<List<WorkingHourResponseDto>> SetWorkingHoursAsync(int userId, int tenantId, List<WorkingHourDto> workingHoursDto);
    }
}