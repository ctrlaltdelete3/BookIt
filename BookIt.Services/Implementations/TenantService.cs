using BookIt.Application.DTOs.Tenant;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;
using BookIt.Domain.Enums;

namespace BookIt.Services.Implementations
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;

        public TenantService(ITenantRepository tenantRepository, IUserRepository userRepository)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        public async Task<TenantResponseDto> CreateTenantAsync(CreateTenantDto tenantDto, int ownerUserId)
        {
            var myTenant = await _tenantRepository.GetMyTenantAsync(ownerUserId);

            if (myTenant != null)
            {
                //TODO: this will be changed later on, one userOwner will be able to have multiple tenants
                throw new InvalidOperationException("User already owns a tenant.");
            }

            var user = await _userRepository.GetByIdAsync(ownerUserId);

            if (user == null)
            {
                throw new KeyNotFoundException("User is not found.");
            }
            var tenant = new Tenant
            {
                Name = tenantDto.Name,
                ActivityType = tenantDto.ActivityType,
                Description = tenantDto.Description,
                Address = tenantDto.Address,
                City = tenantDto.City,
                ContactEmail = tenantDto.ContactEmail,
                ContactPhone = tenantDto.ContactPhone,
                //TODO: implement this later
                //Slug = GenerateSlug(tenantDto.Name),
                Slug = tenantDto.Name.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                OwnerUserId = ownerUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Configuration = CreateGenericTenantConfiguration(),
                Services = new List<Service>(),
            };

            await _tenantRepository.CreateAsync(tenant);

            //update user for tenant info: 
            user.IsTenantOwner = true;
            user.OwnedTenantId = tenant.Id;
            await _userRepository.UpdateAsync(user);

            return GenerateTenantResponse(tenant);
        }

        public async Task<TenantResponseDto?> GetTenantByIdAsync(int id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Missing tenant.");
            }
            return GenerateTenantResponse(tenant);
        }

        public async Task<TenantResponseDto> GetTenantBySlugAsync(string slug)
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Missing tenant.");
            }
            return GenerateTenantResponse(tenant);
        }

        public async Task<TenantResponseDto> UpdateTenantAsync(int id, UpdateTenantDto updateTenantDto, int userId)
        {
            var existingTenant = await _tenantRepository.GetByIdAsync(id);
            if (existingTenant == null)
            {
                throw new KeyNotFoundException("Missing tenant.");
            }
            if (existingTenant.OwnerUserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this tenant.");
            }

            //TODO: use AutoMapper then remove this
            //manual mapping
            existingTenant.Name = updateTenantDto.Name;
            existingTenant.ActivityType = updateTenantDto.ActivityType;
            existingTenant.Description = updateTenantDto.Description;
            existingTenant.Address = updateTenantDto.Address;
            existingTenant.City = updateTenantDto.City;
            existingTenant.ContactEmail = updateTenantDto.ContactEmail;
            existingTenant.ContactPhone = updateTenantDto.ContactPhone;

            await _tenantRepository.UpdateAsync();
            return GenerateTenantResponse(existingTenant);

        }

        public async Task<TenantResponseDto> GetMyTenantAsync(int ownerUserId)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(ownerUserId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }
            return GenerateTenantResponse(tenant);
        }

        #region HelperMethods
        private TenantResponseDto GenerateTenantResponse(Tenant tenant)
        {
            var ownerName = tenant.OwnerUser != null ? tenant.OwnerUser.FirstName + " " + tenant.OwnerUser.LastName : "Unknown";

            return new TenantResponseDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ActivityType = tenant.ActivityType,
                Description = tenant.Description,
                Address = tenant.Address,
                City = tenant.City,
                ContactEmail = tenant.ContactEmail,
                ContactPhone = tenant.ContactPhone,
                Slug = tenant.Slug,
                OwnerName = ownerName,
                IsActive = tenant.IsActive,
                CreatedAt = tenant.CreatedAt
            };
        }

        //TODO: this is only temporary, there will be a separate flow for tenant configuration later on, and this will be removed
        private TenantConfiguration CreateGenericTenantConfiguration()
        {
            var configuration = new TenantConfiguration
            {
                BookingWeeksAhead = 4,
                AutoRejectHours = 12,
                AllowCancellation = true,
                CancellationHoursBefore = 24,
                NotificationType = NotificationType.Email
            };
            return configuration;
        }

        #endregion
    }
}
