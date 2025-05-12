using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using System.Linq.Expressions;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ResponseCategoryDto> GetCategoryByIdAsync(string id);
        Task<ResponseCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<ResponseCategoryDto> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(string id);
        Task<PagedResult<ResponseCategoryDto>> GetAllCategoryAsync(
            Expression<Func<Category, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);
    }
}
