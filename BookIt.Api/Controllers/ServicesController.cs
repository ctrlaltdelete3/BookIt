using BookIt.Application.DTOs.Service;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : BaseController
    {
        private readonly IServiceService _service;
        private readonly ITenantService _tenantService;
        public ServicesController(IServiceService service, ITenantService tenantService)
        {
            _service = service;
            _tenantService = tenantService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceByIdAsync(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceAsync(int id, [FromBody] UpdateServiceDto updateServiceDto)
        {
            var userId = GetUserIdHelper();
            var result = await _service.UpdateServiceAsync(id, updateServiceDto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceAsync(int id)
        {
            var userId = GetUserIdHelper();
            await _service.DeleteServiceAsync(id, userId);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceAsync([FromBody] CreateServiceDto createServiceDto)
        {
            var userId = GetUserIdHelper();
            var tenant = await _tenantService.GetMyTenantAsync(userId);
            var result = await _service.CreateServiceAsync(createServiceDto, tenant.Id);
            return Ok(result);
        }

        [HttpPost("{id}/timeslots")]
        public async Task<IActionResult> CreateTimeSlotsAsync(int id, [FromBody] List<CreateServiceTimeSlotDto> timeSlots)
        {
            var userId = GetUserIdHelper();
            var tenant = await _tenantService.GetMyTenantAsync(userId);
            await _service.CreateServiceTimeSlotsAsync(id, tenant.Id, timeSlots);
            return Created();
        }

        [HttpDelete("{id}/timeslots/{slotId}")]
        public async Task<IActionResult> DeleteTimeSlotAsync(int id, int slotId)
        {
            var userId = GetUserIdHelper();
            var tenant = await _tenantService.GetMyTenantAsync(userId);
            await _service.DeleteServiceTimeSlotAsync(id, tenant.Id, slotId);
            return NoContent();
        }
    }
}
