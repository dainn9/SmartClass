using Backend.DTOs.User;

namespace Backend.DTOs.Auth
{
    public class GoogleLoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public UserDTO User { get; set; } = default!;
    }
}
