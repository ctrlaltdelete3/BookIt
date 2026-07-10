using BookIt.Application.DTOs.User;
using Xunit;

namespace BookIt.Tests.Services
{
    public class UserServiceTests : BookItBaseUnitTest
    {
        #region Register

        [Fact]
        public async Task Register_ValidData_ReturnsAuthResponse()
        {
            var dto = new RegisterRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "newuser@test.com",
                Password = ValidPassword,
                Phone = "+385912345678"
            };

            var result = await UserService.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("fake-token", result.Token);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task Register_EmailAlreadyExists_ThrowsInvalidOperationException()
        {
            var dto = new RegisterRequestDto
            {
                Email = OwnerUser.Email,
                Password = ValidPassword
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => UserService.RegisterAsync(dto));
        }

        #endregion

        #region Login

        [Fact]
        public async Task Login_ValidCredentials_ReturnsAuthResponse()
        {
            var dto = new LoginRequestDto { Email = ClientUser.Email, Password = ValidPassword };

            var result = await UserService.LoginAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("fake-token", result.Token);
            Assert.Equal(ClientUser.Email, result.Email);
        }

        [Fact]
        public async Task Login_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            var dto = new LoginRequestDto { Email = "nonexistent@test.com", Password = ValidPassword };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => UserService.LoginAsync(dto));
        }

        [Fact]
        public async Task Login_WrongPassword_ThrowsUnauthorizedAccessException()
        {
            var dto = new LoginRequestDto { Email = ClientUser.Email, Password = "WrongPassword!" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => UserService.LoginAsync(dto));
        }

        #endregion

        #region GetUser

        [Fact]
        public async Task GetUser_ValidId_ReturnsUserResponseDto()
        {
            var result = await UserService.GetUserAsync(ClientUser.Id);

            Assert.NotNull(result);
            Assert.Equal(ClientUser.Id, result.UserId);
            Assert.Equal(ClientUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUser_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => UserService.GetUserAsync(9999));
        }

        #endregion
    }
}
