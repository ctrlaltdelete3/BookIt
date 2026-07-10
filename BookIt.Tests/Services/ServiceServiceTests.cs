using BookIt.Application.DTOs.Service;
using BookIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookIt.Tests.Services
{
    public class ServiceServiceTests : BookItBaseUnitTest
    {
        #region CreateService

        [Fact]
        public async Task CreateService_ValidData_ReturnsServiceResponseDto()
        {
            var dto = new CreateServiceDto
            {
                Name = "New Service",
                DurationMinutes = 45,
                Price = 15m,
                BreakMinutesAfterService = 0
            };

            var result = await ServiceService.CreateServiceAsync(dto, OwnerUser.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(OwnerTenant.Id, result.TenantId);
        }

        [Fact]
        public async Task CreateService_TenantNotFound_ThrowsKeyNotFoundException()
        {
            var dto = new CreateServiceDto { Price = 10m, BreakMinutesAfterService = 0 };

            // ClientUser has no tenant
            await Assert.ThrowsAsync<KeyNotFoundException>(() => ServiceService.CreateServiceAsync(dto, ClientUser.Id));
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_ValidId_ReturnsServiceResponseDto()
        {
            var result = await ServiceService.GetByIdAsync(ActiveService.Id);

            Assert.NotNull(result);
            Assert.Equal(ActiveService.Name, result.Name);
        }

        [Fact]
        public async Task GetById_ServiceNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => ServiceService.GetByIdAsync(9999));
        }

        #endregion

        #region GetAllServicesByTenant

        [Fact]
        public async Task GetAllServicesByTenant_ValidSlug_ReturnsServiceList()
        {
            var result = await ServiceService.GetAllServicesByTenant(OwnerTenant.Slug);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.All(result, s => Assert.True(s.IsActive));
        }

        [Fact]
        public async Task GetAllServicesByTenant_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => ServiceService.GetAllServicesByTenant("nonexistent-slug"));
        }

        #endregion

        #region UpdateService

        [Fact]
        public async Task UpdateService_ValidOwner_ReturnsUpdatedServiceResponseDto()
        {
            var dto = new UpdateServiceDto
            {
                Name = "Premium Haircut",
                DurationMinutes = 90,
                Price = 35m,
                BreakMinutesAfterService = 10
            };

            var result = await ServiceService.UpdateServiceAsync(ActiveService.Id, dto, OwnerUser.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Price, result.Price);
        }

        [Fact]
        public async Task UpdateService_ServiceNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                ServiceService.UpdateServiceAsync(9999, new UpdateServiceDto { Price = 10m, BreakMinutesAfterService = 0 }, OwnerUser.Id));
        }

        [Fact]
        public async Task UpdateService_NotOwner_ThrowsUnauthorizedAccessException()
        {
            // OtherOwnerUser owns a different tenant
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                ServiceService.UpdateServiceAsync(ActiveService.Id, new UpdateServiceDto { Price = 10m, BreakMinutesAfterService = 0 }, OwnerUser2.Id));
        }

        #endregion

        #region DeleteService

        [Fact]
        public async Task DeleteService_ValidOwner_SoftDeletesService()
        {
            await ServiceService.DeleteServiceAsync(ActiveService.Id, OwnerUser.Id);

            // ServiceRepository.GetByIdAsync filters IsActive - so it returns null after soft delete
            await Assert.ThrowsAsync<KeyNotFoundException>(() => ServiceService.GetByIdAsync(ActiveService.Id));
        }

        [Fact]
        public async Task DeleteService_ServiceNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => ServiceService.DeleteServiceAsync(9999, OwnerUser.Id));
        }

        [Fact]
        public async Task DeleteService_NotOwner_ThrowsUnauthorizedAccessException()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                ServiceService.DeleteServiceAsync(ActiveService.Id, OwnerUser2.Id));
        }

        #endregion

        #region CreateServiceTimeSlots

        [Fact]
        public async Task CreateTimeSlots_ValidOwner_AddsTimeSlotsToService()
        {
            var timeSlots = new List<CreateServiceTimeSlotDto>
            {
                new CreateServiceTimeSlotDto { DayOfWeek = 2, StartTime = new TimeOnly(9, 0) },
                new CreateServiceTimeSlotDto { DayOfWeek = 2, StartTime = new TimeOnly(10, 0) }
            };

            await ServiceService.CreateServiceTimeSlotsAsync(ActiveService.Id, OwnerUser.Id, timeSlots);

            var tuesdaySlots = await Context.ServiceTimeSlots
                .Where(s => s.ServiceId == ActiveService.Id && s.DayOfWeek == 2 && s.IsActive)
                .ToListAsync();

            Assert.Equal(2, tuesdaySlots.Count);
        }

        [Fact]
        public async Task CreateTimeSlots_TenantNotFound_ThrowsKeyNotFoundException()
        {
            // ClientUser has no tenant
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                ServiceService.CreateServiceTimeSlotsAsync(ActiveService.Id, ClientUser.Id, new List<CreateServiceTimeSlotDto>()));
        }

        [Fact]
        public async Task CreateTimeSlots_ServiceBelongsToOtherTenant_ThrowsUnauthorizedAccessException()
        {
            // OtherOwnerUser's tenant doesn't own ActiveService
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                ServiceService.CreateServiceTimeSlotsAsync(ActiveService.Id, OwnerUser2.Id, new List<CreateServiceTimeSlotDto>()));
        }

        #endregion

        #region DeleteServiceTimeSlot

        [Fact]
        public async Task DeleteTimeSlot_ValidOwner_SoftDeletesTimeSlot()
        {
            var timeSlot = await Context.ServiceTimeSlots
                .FirstAsync(s => s.ServiceId == ActiveService.Id && s.IsActive);

            await ServiceService.DeleteServiceTimeSlotAsync(ActiveService.Id, OwnerUser.Id, timeSlot.Id);

            var updated = await Context.ServiceTimeSlots.FindAsync(timeSlot.Id);
            Assert.False(updated!.IsActive);
        }

        [Fact]
        public async Task DeleteTimeSlot_TenantNotFound_ThrowsKeyNotFoundException()
        {
            // ClientUser has no tenant
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                ServiceService.DeleteServiceTimeSlotAsync(ActiveService.Id, ClientUser.Id, 1));
        }

        [Fact]
        public async Task DeleteTimeSlot_TimeSlotNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                ServiceService.DeleteServiceTimeSlotAsync(ActiveService.Id, OwnerUser.Id, 9999));
        }

        #endregion
    }
}
