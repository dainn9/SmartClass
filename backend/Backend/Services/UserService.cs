using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly SmartClassDbContext _context;

        public UserService(SmartClassDbContext context) => _context = context;

        public async Task<User> CreateFromGoogleAsync(GoogleJsonWebSignature.Payload payload)
        {
            var user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name,
                AvatarUrl = payload.Picture
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> FindByIdAsync(Guid id)
            => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetByGoogleIdAsync(string googleId)
            => await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

        public async Task UpdateGoogleInfoAsync(User user, GoogleJsonWebSignature.Payload payload)
        {
            user.Name = payload.Name;
            user.AvatarUrl = payload.Picture;
            await _context.SaveChangesAsync();
        }
    }
}
