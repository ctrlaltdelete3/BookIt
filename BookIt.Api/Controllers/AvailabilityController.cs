using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookIt.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : BaseController
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlotsAsync([FromQuery] int tenantId, [FromQuery] int serviceId, [FromQuery] DateOnly date)
        {
            var result = await _availabilityService.GetAvailableSlotsAsync(tenantId, serviceId, date);
            return Ok(result);
        }
    }
}
