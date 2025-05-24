using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Auth
{
    public class RegisterUserModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "Email address is too long.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Passowrd")]
        public string Password { get; set; }
    }
}
