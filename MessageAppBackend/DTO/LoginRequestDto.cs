using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username cannot be empty")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; } = string.Empty;
    }
}
