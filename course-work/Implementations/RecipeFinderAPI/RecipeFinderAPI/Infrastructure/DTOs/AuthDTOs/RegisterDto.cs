using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.AuthDTOs
{
    public class RegisterDto
    {
        [Required]
        //[EmailAddress]
        //[StringLength(100, MinimumLength = 5)]
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}
