using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.VIewModels.Auth
{
    public class LoginVM
    {
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}
