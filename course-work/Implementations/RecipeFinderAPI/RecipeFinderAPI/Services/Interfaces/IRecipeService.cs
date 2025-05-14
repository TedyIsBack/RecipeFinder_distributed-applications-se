using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipesDTOs;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<ResponseRecipeDto> GetRecipeByIdAsync(string id);
        Task<ResponseRecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto);
        Task<ResponseRecipeDto> UpdateRecipeAsync(string id, UpdateRecipeDto updateRecipeDto);
        Task<bool> DeleteRecipeAsync(string id);
        Task<PagedResult<ResponseRecipeDto>> GetAllRecipeAsync(Expression<Func<Recipe, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);

        Task<PagedResult<ResponseRecipeDto>> GetAllUserRecipesAsync(
            string userId,
            Expression<Func<Recipe, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);
    }
}
