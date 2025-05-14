using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<ResponseIngredientDto> GetIngredientByIdAsync(string ingredientId);
        Task<ResponseIngredientDto> CreateIngredientAsync(CreateIngredientDto createIngredientDto);
        Task<ResponseIngredientDto> UpdateIngredientAsync(string ingredientId, UpdateIngredientDto updateIngredientDto);
        Task<bool> DeleteIngredientAsync(string ingredientId);
        Task<PagedResult<ResponseIngredientDto>> GetAllIngredientAsync(Expression<Func<Ingredient, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);

        public Task<List<ResponseIngredientDto>> GetIngredientsByIdsAsync(List<string> ingredientIds);

    }
}
