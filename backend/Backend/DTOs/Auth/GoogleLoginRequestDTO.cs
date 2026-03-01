using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Auth
{
    public class GoogleLoginRequestDTO
    {
        [Required]
        public string IdToken { get; set; } = default!;
    }
}
