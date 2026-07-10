using BookIt.Application.DTOs.WorkingHours;
using Xunit;

namespace BookIt.Tests.Services
{
    public class WorkingHourServiceTests : BookItBaseUnitTest
    {
        #region GetAll

        [Fact]
        public async Task GetAll_ValidSlug_ReturnsWorkingHourList()
        {
            var result = await WorkingHourService.GetAllAsync(OwnerTenant.Slug);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.All(result, wh => Assert.Equal(OwnerTenant.Id, wh.TenantId));
        }

        [Fact]
        public async Task GetAll_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => WorkingHourService.GetAllAsync("nonexistent-slug"));
        }

        #endregion

        #region SetWorkingHours

        [Fact]
        public async Task SetWorkingHours_ValidOwner_ReturnsWorkingHourList()
        {
            var dto = new List<WorkingHourDto>
            {
                new WorkingHourDto { DayOfWeek = 1, IsWorkingDay = true, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) },
                new WorkingHourDto { DayOfWeek = 2, IsWorkingDay = false }
            };

            var result = await WorkingHourService.SetWorkingHoursAsync(OwnerUser.Id, OwnerTenant.Id, dto);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SetWorkingHours_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                WorkingHourService.SetWorkingHoursAsync(OwnerUser.Id, 9999, new List<WorkingHourDto>()));
        }

        [Fact]
        public async Task SetWorkingHours_NotOwner_ThrowsUnauthorizedAccessException()
        {
            // ClientUser is not the owner of OwnerTenant
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                WorkingHourService.SetWorkingHoursAsync(ClientUser.Id, OwnerTenant.Id, new List<WorkingHourDto>()));
        }

        #endregion
    }
}
