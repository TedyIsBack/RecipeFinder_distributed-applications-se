using RecipeFinderAPI.Entities;

namespace RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs
{
    public class ResponseUserDto
    {

        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string CreatedAt { get; set; }

    }
}
