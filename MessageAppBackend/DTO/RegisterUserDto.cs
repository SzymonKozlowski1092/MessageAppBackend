using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string DisplayName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter and one number.")]
        public string Password { get; set; } = null!;
        
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
