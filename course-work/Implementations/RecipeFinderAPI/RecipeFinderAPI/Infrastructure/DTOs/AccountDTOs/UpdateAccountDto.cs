using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs
{
    public class UpdateAccountDto
    {
       // public string Id { get; set; }

        //public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; }
    }
}
