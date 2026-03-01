using Backend.DTOs.Auth;

namespace Backend.Interfaces
{
    public interface IAuthService
    {
        Task<(GoogleLoginResponseDTO dto, string refreshToken)> LoginWithGoogleAsync(GoogleLoginRequestDTO request);
        Task LogoutAsync();
        Task<(RefreshTokenResponseDTO dto, string refreshToken)> RefreshTokenAsync(string refreshToken);
    }
}
