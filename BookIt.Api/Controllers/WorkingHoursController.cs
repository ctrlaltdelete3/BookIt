using BookIt.Application.DTOs.WorkingHours;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkingHoursController : ControllerBase
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

        #region HelperMethods
        private int GetUserIdHelper()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)
                || int.TryParse(userIdString, out int id) == false)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }
            return id;
        }
        #endregion
    }
}
