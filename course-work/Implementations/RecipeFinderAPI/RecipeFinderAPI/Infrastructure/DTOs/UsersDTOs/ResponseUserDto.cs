using RecipeFinderAPI.Entities;

namespace RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs
{
    public class ResponseUserDto  : UpdateUserDto
    {
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
