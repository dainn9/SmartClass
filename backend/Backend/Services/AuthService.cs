using Backend.Common;
using Backend.Configurations;
using Backend.DTOs.Auth;
using Backend.DTOs.User;
using Backend.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly GoogleConfig _googleConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRefreshTokenService _refreshTokenService;


        public AuthService(IUserService userService, IJwtService jwtService, IOptions<GoogleConfig> googleOptions, IHttpContextAccessor httpContextAccessor, IRefreshTokenService refreshTokenService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _googleConfig = googleOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<(GoogleLoginResponseDTO dto, string refreshToken)> LoginWithGoogleAsync(GoogleLoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                throw new InvalidGoogleTokenException("ID token is missing.");

            GoogleJsonWebSignature.Payload payload;

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleConfig.ClientId }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            }
            catch (Exception ex)
            {
                throw new InvalidGoogleTokenException("Invalid Google ID token.", ex);
            }

            if (!payload.EmailVerified)
                throw new InvalidGoogleTokenException("Email not verified.");

            var user = await _userService.GetByGoogleIdAsync(payload.Subject);

            if (user == null)
                user = await _userService.CreateFromGoogleAsync(payload);
            else
                await _userService.UpdateGoogleInfoAsync(user, payload);


            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            };

            return (new GoogleLoginResponseDTO { AccessToken = accessToken, User = userDTO }, refreshToken);
        }

        public async Task LogoutAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            await _refreshTokenService.ValidateAndRevokeAsync(refreshToken);
        }

        public async Task<(RefreshTokenResponseDTO dto, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var (userId, newRefreshToken) = await _refreshTokenService.RotateRefreshTokenAsync(refreshToken);

            var user = await _userService.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("Invalid refresh token.");

            var newAccessToken = _jwtService.GenerateToken(user);

            return (new RefreshTokenResponseDTO
            {
                AccessToken = newAccessToken,
            }, newRefreshToken);
        }
    }
}