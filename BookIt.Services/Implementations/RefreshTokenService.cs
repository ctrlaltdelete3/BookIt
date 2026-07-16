using BookIt.Application.DTOs.RefreshToken;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;

namespace BookIt.Services.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
        }

        public async Task<NewAccessTokenDto?> GenerateNewAccessTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetAsync(token);
            if (refreshToken == null)
            {
                throw new KeyNotFoundException("Refresh token not found.");
            }

            if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has been revoked.");
            }

            var accessToken = _jwtService.GenerateToken(refreshToken.User);


            var refreshTokenDto = new NewAccessTokenDto
            {
                AccessToken = accessToken
            };
            return refreshTokenDto;
        }

        public async Task<string> CreateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(30), 
                IsRevoked = false,
                UserId = userId
            };
            await _refreshTokenRepository.CreateAsync(refreshToken);
            return refreshToken.Token;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetAsync(token);
            if (refreshToken == null)
            {
                throw new KeyNotFoundException("Refresh token not found.");
            }
            refreshToken.IsRevoked = true;
            await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
        }
    }
}
