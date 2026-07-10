using BookIt.Application.DTOs.Tenant;
using Xunit;

namespace BookIt.Tests.Services
{
    public class TenantServiceTests : BookItBaseUnitTest
    {
        #region CreateTenant

        [Fact]
        public async Task CreateTenant_ValidData_ReturnsTenantResponseDto()
        {
            var dto = new CreateTenantDto
            {
                Name = "New Salon",
                ActivityType = "Massage Studio",
                ContactEmail = "newsalon@test.com"
            };

            var result = await TenantService.CreateTenantAsync(dto, ClientUser.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.ActivityType, result.ActivityType);
        }

        [Fact]
        public async Task CreateTenant_UserAlreadyOwnsTenant_ThrowsInvalidOperationException()
        {
            var dto = new CreateTenantDto { Name = "Second Salon", ActivityType = "Frizerski salon" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => TenantService.CreateTenantAsync(dto, OwnerUser.Id));
        }

        [Fact]
        public async Task CreateTenant_UserNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => TenantService.CreateTenantAsync(new CreateTenantDto(), 9999));
        }

        #endregion

        #region GetTenantById

        [Fact]
        public async Task GetTenantById_ValidId_ReturnsTenantResponseDto()
        {
            var result = await TenantService.GetTenantByIdAsync(OwnerTenant.Id);

            Assert.NotNull(result);
            Assert.Equal(OwnerTenant.Name, result.Name);
        }

        [Fact]
        public async Task GetTenantById_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => TenantService.GetTenantByIdAsync(9999));
        }

        #endregion

        #region GetTenantBySlug

        [Fact]
        public async Task GetTenantBySlug_ValidSlug_ReturnsTenantResponseDto()
        {
            var result = await TenantService.GetTenantBySlugAsync(OwnerTenant.Slug);

            Assert.NotNull(result);
            Assert.Equal(OwnerTenant.Slug, result.Slug);
        }

        [Fact]
        public async Task GetTenantBySlug_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => TenantService.GetTenantBySlugAsync("nonexistent-slug"));
        }

        #endregion

        #region UpdateTenant

        [Fact]
        public async Task UpdateTenant_ValidOwner_ReturnsTenantResponseDto()
        {
            var dto = new UpdateTenantDto { Name = "Updated Salon", ActivityType = "Novi tip" };

            var result = await TenantService.UpdateTenantAsync(OwnerTenant.Id, dto, OwnerUser.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.ActivityType, result.ActivityType);
        }

        [Fact]
        public async Task UpdateTenant_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                TenantService.UpdateTenantAsync(9999, new UpdateTenantDto(), OwnerUser.Id));
        }

        [Fact]
        public async Task UpdateTenant_NotOwner_ThrowsUnauthorizedAccessException()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                TenantService.UpdateTenantAsync(OwnerTenant.Id, new UpdateTenantDto(), ClientUser.Id));
        }

        #endregion

        #region GetMyTenant

        [Fact]
        public async Task GetMyTenant_ValidOwner_ReturnsTenantResponseDto()
        {
            var result = await TenantService.GetMyTenantAsync(OwnerUser.Id);

            Assert.NotNull(result);
            Assert.Equal(OwnerTenant.Name, result.Name);
        }

        [Fact]
        public async Task GetMyTenant_TenantNotFound_ThrowsKeyNotFoundException()
        {
            // ClientUser has no tenant
            await Assert.ThrowsAsync<KeyNotFoundException>(() => TenantService.GetMyTenantAsync(ClientUser.Id));
        }

        #endregion
    }
}
