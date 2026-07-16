using BookIt.Application.DTOs.RefreshToken;

namespace BookIt.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(int userId);
        Task<RefreshTokenDto?> GenerateNewAccessTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
    }
}