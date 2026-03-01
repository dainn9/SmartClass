namespace Backend.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(Guid userId);
        Task<Guid> ValidateAndRevokeAsync(string refreshToken);
        Task<(Guid userId, string newRefreshToken)> RotateRefreshTokenAsync(string refreshToken);
    }
}
