using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipesDTOs;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<ResponseRecipeDto> GetRecipeByIdAsync(string recipeId);
        Task<ResponseRecipeDto> CreateRecipeAsync(string userId, CreateRecipeDto createRecipeDto);
        Task<ResponseRecipeDto> UpdateRecipeAsync(string userId, string recipeId, UpdateRecipeDto updateRecipeDto);
        Task<bool> DeleteRecipeAsync(string recipeId);
        Task<PagedResult<ResponseRecipeDto>> GetAllRecipesAsync(Expression<Func<Recipe, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);
    }
}
