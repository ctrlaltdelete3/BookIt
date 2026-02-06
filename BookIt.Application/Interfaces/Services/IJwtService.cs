using BookIt.Domain.Entities;

namespace BookIt.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
