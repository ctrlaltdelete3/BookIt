using BookIt.Application.DTOs.Appointment;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentByIdAsync(int id)
        {
            var userId = GetUserIdHelper();
            var result = await _appointmentService.GetAppointmentByIdAsync(id, userId);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointmentsAsync()
        {
            var userId = GetUserIdHelper();
            var result = await _appointmentService.GetMyAppointmentsAsync(userId);
            return Ok(result);
        }

        [HttpGet("tenant")]
        public async Task<IActionResult> GetTenantAppointmentsAsync()
        {
            var userId = GetUserIdHelper();
            var result = await _appointmentService.GetTenantAppointmentsAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] CreateAppointmentDto createAppointmentDto)
        {
            var userId = GetUserIdHelper();
            var result = await _appointmentService.CreateAppointmentAsync(createAppointmentDto, userId);
            return Created($"/api/appointments/{result.Id}", result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentAsync(int id, [FromBody] AppointmentStatus status)
        {
            var userId = GetUserIdHelper();
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, userId, status);
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointmentAsync(int id, [FromBody] string? cancelationMessage)
        {
            var userId = GetUserIdHelper();
            var result = await _appointmentService.CancelAppointmentAsync(id, userId, cancelationMessage);
            return Ok(result);
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
