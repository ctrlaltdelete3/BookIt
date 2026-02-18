using BookIt.Application.DTOs.User;

namespace BookIt.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<UserResponseDto?> GetUserAsync(string? userId);
    }
}
