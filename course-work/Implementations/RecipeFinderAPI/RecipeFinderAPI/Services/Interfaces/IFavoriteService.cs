using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.FavoriteDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<PagedResult<ResponseFavoriteDto>> GetUserFavoriteRecipesAsync(string userId,
            Expression<Func<Recipe, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);
        Task<bool> AddToFavoritesAsync(string userId, string recipeId);
        Task<bool> RemoveFromFavoritesAsync(string userId, string recipeId);
        //Task<bool> IsRecipeFavoritedAsync(string userId, string recipeId);

    }
}
