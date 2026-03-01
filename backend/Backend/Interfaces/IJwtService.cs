using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token);

    }
}
