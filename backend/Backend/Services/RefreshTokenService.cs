using Backend.Common;
using Backend.Configurations;
using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly SmartClassDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly JwtConfig _jwtConfig;

        public RefreshTokenService(SmartClassDbContext context, IJwtService jwtService, IOptions<JwtConfig> options)
        {
            _context = context;
            _jwtService = jwtService;
            _jwtConfig = options.Value;
        }

        public async Task<string> CreateRefreshTokenAsync(Guid userId)
        {
            var refreshToken = _jwtService.GenerateRefreshToken();
            var hashedRefreshToken = _jwtService.HashToken(refreshToken);

            var refreshTokenEnity = new RefreshToken
            {
                Token = hashedRefreshToken,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationDays),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEnity);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<(Guid userId, string newRefreshToken)> RotateRefreshTokenAsync(string refreshToken)
        {
            var userId = await ValidateAndRevokeAsync(refreshToken);

            var newRefreshToken = await CreateRefreshTokenAsync(userId);

            return (userId, newRefreshToken);
        }

        public async Task<Guid> ValidateAndRevokeAsync(string refreshToken)
        {
            var hashedToken = _jwtService.HashToken(refreshToken);

            var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rf => rf.Token == hashedToken);

            if (storedRefreshToken == null
                 || storedRefreshToken.IsRevoked
                 || storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Invalid refresh token.");
            }

            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnauthorizedException("Refresh token already used.");
            }

            return storedRefreshToken.UserId;
        }
    }
}
