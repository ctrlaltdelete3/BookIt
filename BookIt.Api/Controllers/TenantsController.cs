using BookIt.Application.DTOs.Tenant;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : BaseController
    {
        private readonly ITenantService _tenantService;
        private readonly IServiceService _service;
        private readonly IWorkingHourService _workingHourService;

        public TenantsController(ITenantService tenantService, IServiceService service, IWorkingHourService workingHourService)
        {
            _tenantService = tenantService;
            _service = service;
            _workingHourService = workingHourService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantDto createTenantDto)
        {
            var userId = GetUserId();
            var tenantResponse = await _tenantService.CreateTenantAsync(createTenantDto, userId);
            return Ok(tenantResponse);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTenant()
        {
            var userId = GetUserId();
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
            var userId = GetUserId();
            var response = await _tenantService.UpdateTenantAsync(id, updateTenantDto, userId);
            return Ok(response);
        }
    }
}
