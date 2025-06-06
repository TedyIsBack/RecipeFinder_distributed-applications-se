﻿using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs
{
    public class UpdateUserDto
    {

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Role name must be at least 3 characters long.")]
        public string Role { get; set; }
    }
}
