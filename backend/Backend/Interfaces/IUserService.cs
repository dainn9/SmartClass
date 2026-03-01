using Backend.Entities;
using Google.Apis.Auth;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User> CreateFromGoogleAsync(GoogleJsonWebSignature.Payload payload);
        Task UpdateGoogleInfoAsync(User user, GoogleJsonWebSignature.Payload payload);
        Task<User?> FindByIdAsync(Guid id);
    }
}
