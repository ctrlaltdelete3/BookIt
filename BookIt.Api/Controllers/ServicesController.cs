using BookIt.Application.DTOs.Service;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
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

        //TODO: move somewhere? same method used in TenantController
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
