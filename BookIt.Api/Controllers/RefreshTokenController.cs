using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;
        public RefreshTokenController(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> GenerateNewAccessToken()
        {
            var refreshToken = GetRefreshTokenCookie();
            var result = await _refreshTokenService.GenerateNewAccessTokenAsync(refreshToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = GetRefreshTokenCookie();
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
            return Ok();
        }

        private string GetRefreshTokenCookie()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token is missing.");
            }
            return refreshToken;
        }
    }
}
