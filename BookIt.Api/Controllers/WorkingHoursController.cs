using BookIt.Application.DTOs.WorkingHours;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkingHoursController : BaseController
    {
        private readonly IWorkingHourService _workingHourService;
        private readonly ITenantService _tenantService;

        public WorkingHoursController(IWorkingHourService workingHourService, ITenantService tenantService)
        {
            _workingHourService = workingHourService;
            _tenantService = tenantService;
        }

        [HttpPut]
        public async Task<IActionResult> SetWorkingHours([FromBody] List<WorkingHourDto> workingHours)
        {
            var userId = GetUserIdHelper();
            var tenant = await _tenantService.GetMyTenantAsync(userId);
            var response = await _workingHourService.SetWorkingHoursAsync(userId, tenant.Id, workingHours);
            return Ok(response);
        }
    }
}
