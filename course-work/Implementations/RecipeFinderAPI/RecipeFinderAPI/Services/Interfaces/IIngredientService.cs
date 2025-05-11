using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<ResponseIngredientDto> GetIngredientByIdAsync(string id);
        Task<ResponseIngredientDto> CreateIngredientAsync(CreateIngredientDto createIngredientDto);
        Task<ResponseIngredientDto> UpdateIngredientAsync(UpdateIngredientDto updateIngredientDto);
        Task<bool> DeleteIngredientAsync(string id);
        Task<List<ResponseIngredientDto>> GetAllIngredientAsync(Expression<Func<Ingredient, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);
    }
}
