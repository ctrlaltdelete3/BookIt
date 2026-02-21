using BookIt.Application.DTOs.WorkingHours;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;

namespace BookIt.Services.Implementations
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly IWorkingHourRepository _workingHourRepository;
        private readonly ITenantRepository _tenantRepository;

        public WorkingHourService(IWorkingHourRepository workingHourRepository, ITenantRepository tenantRepository)
        {
            _workingHourRepository = workingHourRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<List<WorkingHourResponseDto>> GetAllAsync(string slug)
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found");
            }
            var result = await _workingHourRepository.GetAll(tenant.Id);
            return GenerateResponse(result);
        }

        public async Task<List<WorkingHourResponseDto>> SetWorkingHoursAsync(int userId, int tenantId, List<WorkingHourDto> workingHoursDto)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found");
            }
            if (tenant.OwnerUserId != userId)
            {
                throw new UnauthorizedAccessException("You have no authorization to change working hours.");
            }
            var workingHours = new List<WorkingHour>();
            foreach (var item in workingHoursDto)
            {
                var workingHour = new WorkingHour
                {
                    DayOfWeek = item.DayOfWeek,
                    IsWorkingDay = item.IsWorkingDay,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    PauseStart = item.PauseStart,
                    PauseEnd = item.PauseEnd,
                    TenantId = tenantId
                };
                workingHours.Add(workingHour);
            }
            await _workingHourRepository.SetAll(workingHours, tenantId);
            return GenerateResponse(workingHours);
        }

        #region HelperMethods

        private List<WorkingHourResponseDto> GenerateResponse(List<WorkingHour> workingHours)
        {
            var responses = new List<WorkingHourResponseDto>();

            if (workingHours.Count < 1)
            {
                return responses;
            }

            foreach (var item in workingHours)
            {
                var response = new WorkingHourResponseDto
                {
                    Id = item.Id,
                    TenantId = item.TenantId,
                    DayOfWeek = item.DayOfWeek,
                    IsWorkingDay = item.IsWorkingDay,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    PauseStart = item.PauseStart,
                    PauseEnd = item.PauseEnd
                };

                responses.Add(response);
            }

            return responses;
        }
        #endregion
    }
}
