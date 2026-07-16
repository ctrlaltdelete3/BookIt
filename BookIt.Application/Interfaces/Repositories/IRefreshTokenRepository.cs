using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
    }
}