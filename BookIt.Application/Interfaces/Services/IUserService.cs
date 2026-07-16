using BookIt.Application.DTOs.User;

namespace BookIt.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<AuthResultDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<AuthResultDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<UserResponseDto> GetUserAsync(int userId);
    }
}
