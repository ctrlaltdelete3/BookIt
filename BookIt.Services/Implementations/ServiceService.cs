using BookIt.Application.DTOs.Service;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;

namespace BookIt.Services.Implementations
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ITenantRepository _tenantRepository;
        public ServiceService(IServiceRepository serviceRepository, ITenantRepository tenantRepository)
        {
            _serviceRepository = serviceRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResponseDto> CreateServiceAsync(CreateServiceDto createServiceDto, int userId)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var service = new Service
            {
                Name = createServiceDto.Name,
                Description = createServiceDto.Description,
                Price = createServiceDto.Price.Value,
                DurationMinutes = createServiceDto.DurationMinutes,
                BreakMinutesAfterService = createServiceDto.BreakMinutesAfterService.Value,
                IsActive = true,
                TenantId = tenant.Id,
                Tenant = tenant
            };

            await _serviceRepository.CreateAsync(service);
            return await GenerateServiceResponseAsync(service);
        }

        public async Task<ServiceResponseDto> GetByIdAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found.");
            }

            return await GenerateServiceResponseAsync(service);
        }

        public async Task<List<ServiceResponseDto>> GetAllServicesByTenant(string slug)
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var services = await _serviceRepository.GetByTenantIdAsync(tenant.Id);

            var responses = new List<ServiceResponseDto>();

            foreach (var service in services)
            {
                var serviceResponse = await GenerateServiceResponseAsync(service);
                responses.Add(serviceResponse);
            }

            return responses;
        }

        public async Task<ServiceResponseDto> UpdateServiceAsync(int serviceId, UpdateServiceDto updateServiceDto, int userId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new KeyNotFoundException("Requested service not found or it may be deleted.");
            }

            await CheckAuthorizationBeforeChangesAsync(userId, service.TenantId);

            service.Name = updateServiceDto.Name;
            service.Description = updateServiceDto.Description;
            service.DurationMinutes = updateServiceDto.DurationMinutes;
            service.BreakMinutesAfterService = updateServiceDto.BreakMinutesAfterService.Value;
            service.Price = updateServiceDto.Price.Value;

            await _serviceRepository.UpdateAsync();
            return await GenerateServiceResponseAsync(service);
        }

        public async Task DeleteServiceAsync(int serviceId, int userId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new KeyNotFoundException("Requested service not found or it may be deleted.");
            }

            await CheckAuthorizationBeforeChangesAsync(userId, service.TenantId);

            //soft delete
            service.IsActive = false;
            await _serviceRepository.UpdateAsync();
        }

        public async Task CreateServiceTimeSlotsAsync(int serviceId, int userId, List<CreateServiceTimeSlotDto> timeSlots)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var service = await GetServiceIfAuthorizedAsync(tenant.Id, serviceId);

            foreach (var timeSlotDto in timeSlots)
            {
                var timeSlot = new ServiceTimeSlot
                {
                    DayOfWeek = timeSlotDto.DayOfWeek.Value,
                    StartTime = timeSlotDto.StartTime,
                    IsActive = true,
                    ServiceId = serviceId
                };
                service.TimeSlots.Add(timeSlot);
            }
            await _serviceRepository.UpdateAsync();
            //TODO: maybe add serivceTimeSlots to ServiceResponseDTO and return it here?
        }

        public async Task DeleteServiceTimeSlotAsync(int serviceId, int userId, int serviceTimeSlotId)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var service = await GetServiceIfAuthorizedAsync(tenant.Id, serviceId);
            var timeSlot = service.TimeSlots.FirstOrDefault(t => t.Id == serviceTimeSlotId);

            if (timeSlot == null)
            {
                throw new KeyNotFoundException("Requested time slot not found.");
            }

            timeSlot.IsActive = false;
            await _serviceRepository.UpdateAsync();
        }

        #region HelperMethods

        private async Task CheckAuthorizationBeforeChangesAsync(int userId, int serviceTenantId)
        {
            var tenant = await _tenantRepository.GetMyTenantAsync(userId);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            if (serviceTenantId != tenant.Id)
            {
                throw new UnauthorizedAccessException("You are not authorized to make changes.");
            }
        }

        private async Task<Service> GetServiceIfAuthorizedAsync(int tenantId, int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);

            if (service == null)
            {
                throw new KeyNotFoundException("Requested service not found.");
            }

            if (service.TenantId != tenantId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make changes.");
            }

            return service;
        }

        private async Task<ServiceResponseDto> GenerateServiceResponseAsync(Service service)
        {
            var serviceResponseDto = new ServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                BreakMinutesAfterService = service.BreakMinutesAfterService,
                DurationMinutes = service.DurationMinutes,
                IsActive = service.IsActive,
                TenantId = service.TenantId,
                TenantName = service.Tenant.Name
            };

            return serviceResponseDto;
        }
        #endregion

    }
}
