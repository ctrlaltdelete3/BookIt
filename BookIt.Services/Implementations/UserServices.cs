using BookIt.Application.DTOs;
using BookIt.Application.Interfaces.Repositories;
using BookIt.Application.Interfaces.Services;
using BookIt.Domain.Entities;

namespace BookIt.Services.Implementations
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        public UserServices(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            if (await _userRepository.EmailExistsAsync(registerRequestDto.Email))
            {
                //TODO: fix later - message and details in error response are the same, check it out
                throw new InvalidOperationException("Email already exists.");
            }

            var passwordHashed = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password);

            //TODO: use mapper for this!
            var user = new User
            {
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                Email = registerRequestDto.Email,
                Phone = registerRequestDto.Phone,
                PasswordHash = passwordHashed,
                IsTenantOwner = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            var response = GenerateResponse(user);

            return response;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash);

            if (isPasswordValid == false)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            var response = GenerateResponse(user);

            return response;
        }

        public async Task<UserResponseDto?> GetUserAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId) == false
                && int.TryParse(userId, out int id))
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    //TODO: handle this case here!
                }
                var userResponseDto = new UserResponseDto
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsTenantOwner = user.IsTenantOwner,
                    OwnedTenantId = user.OwnedTenantId
                };
                return userResponseDto;
            }

            return null;
        }

        public AuthResponseDto GenerateResponse(User user)
        {
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                IsTenantOwner = user.IsTenantOwner
            };
        }
    }
}
