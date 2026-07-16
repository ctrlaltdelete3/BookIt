using BookIt.Application.DTOs.User;
using BookIt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookIt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerRequestDto)
        {
            var response = await _userService.RegisterAsync(registerRequestDto);

            SetRefreshTokenCookie(response.RefreshToken);

            return Ok(response.AuthResponseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginRequestDto)
        {
            var response = await _userService.LoginAsync(loginRequestDto);

            SetRefreshTokenCookie(response.RefreshToken);

            return Ok(response.AuthResponseDto);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserAsync()
        {
            var userId = GetUserId();
            var user = await _userService.GetUserAsync(userId);
            return Ok(user);
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //currently set to false since I'm not using https, but should be changed !!!
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(30)
            });
        }
    }
}
