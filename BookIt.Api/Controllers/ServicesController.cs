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
        public ServicesController(IServiceService service)
        {
            _service = service;
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
            var userId = GetUserId();
            var result = await _service.UpdateServiceAsync(id, updateServiceDto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceAsync(int id)
        {
            var userId = GetUserId();
            await _service.DeleteServiceAsync(id, userId);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceAsync([FromBody] CreateServiceDto createServiceDto)
        {
            var userId = GetUserId();
            var result = await _service.CreateServiceAsync(createServiceDto, userId);
            return Ok(result);
        }

        [HttpPost("{id}/timeslots")]
        public async Task<IActionResult> CreateTimeSlotsAsync(int id, [FromBody] List<CreateServiceTimeSlotDto> timeSlots)
        {
            var userId = GetUserId();
            await _service.CreateServiceTimeSlotsAsync(id, userId, timeSlots);
            return Created();
        }

        [HttpDelete("{id}/timeslots/{slotId}")]
        public async Task<IActionResult> DeleteTimeSlotAsync(int id, int slotId)
        {
            var userId = GetUserId();
            await _service.DeleteServiceTimeSlotAsync(id, userId, slotId);
            return NoContent();
        }
    }
}
