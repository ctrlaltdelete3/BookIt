using BookIt.Application.DTOs.Tenant;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IServiceService _service;
        private readonly IWorkingHourService _workingHourService;

        public TenantController(ITenantService tenantService, IServiceService service, IWorkingHourService workingHourService)
        {
            _tenantService = tenantService;
            _service = service;
            _workingHourService = workingHourService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantDto createTenantDto)
        {
            var userId = GetUserIdHelper();
            var tenantResponse = await _tenantService.CreateTenantAsync(createTenantDto, userId);
            return Ok(tenantResponse);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTenant()
        {
            var userId = GetUserIdHelper();
            var response = await _tenantService.GetMyTenantAsync(userId);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetTenantBySlugAsync(string slug)
        {
            var response = await _tenantService.GetTenantBySlugAsync(slug);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{slug}/services")]
        public async Task<IActionResult> GetAllServicesByTenantAsync(string slug)
        {
            var result = await _service.GetAllServicesByTenant(slug);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{slug}/working-hours")]
        public async Task<IActionResult> GetWorkingHoursByTenantAsync(string slug)
        {
            var result = await _workingHourService.GetAllAsync(slug);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantDto updateTenantDto, int id)
        {
            var userId = GetUserIdHelper();
            var response = await _tenantService.UpdateTenantAsync(id, updateTenantDto, userId);
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
