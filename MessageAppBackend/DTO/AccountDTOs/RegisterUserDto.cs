using Azure.Messaging;
using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.AccountDTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Username cannot be empty")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "User display name cannot be empty")]
        public string DisplayName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
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
