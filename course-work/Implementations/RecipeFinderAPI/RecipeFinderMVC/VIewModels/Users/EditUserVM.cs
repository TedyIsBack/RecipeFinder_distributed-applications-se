using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs
{
    public class EditUserVM
    {
        public string Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Role { get; set; }
    }
}
