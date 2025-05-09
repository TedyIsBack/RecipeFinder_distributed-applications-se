using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs
{
    public class CreateUserDto
    {

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }


        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } //admin or user
        public bool IsActive { get; set; } = true;

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
