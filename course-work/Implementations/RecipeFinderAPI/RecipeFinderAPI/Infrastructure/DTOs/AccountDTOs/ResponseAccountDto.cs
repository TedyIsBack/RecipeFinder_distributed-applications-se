using RecipeFinderAPI.Infrastructure.DTOs.FavoriteRecipesDTOs;

namespace RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs
{
    public class ResponseAccountDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public string CreatedAt { get; set; }

        public List<ResponseFavoriteRecipeDto> favoriteRecipes { get; set; } = new List<ResponseFavoriteRecipeDto>();
    }
}
