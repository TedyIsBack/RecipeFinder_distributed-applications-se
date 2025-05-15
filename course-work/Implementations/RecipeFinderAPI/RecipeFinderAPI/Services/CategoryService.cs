using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly BaseRepository<Category> _categoryRepository;

        public CategoryService(BaseRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ResponseCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var existingCategory = await _categoryRepository.FirstOrDefault(x => x.Name == createCategoryDto.Name);

            if (existingCategory != null)
                throw new InvalidOperationException("This category already exists.");

            Category category = new Category()
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                ShortCode = createCategoryDto.ShortCode,
                IsSeasonal = createCategoryDto.IsSeasonal,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            return new ResponseCategoryDto
            {
                Id = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ShortCode = category.ShortCode,
                IsSeasonal = category.IsSeasonal,
            };

        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _categoryRepository.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
            {
                return false;
            }
            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<PagedResult<ResponseCategoryDto>> GetAllCategoryAsync(
            Expression<Func<Category, bool>> filter = null, 
            int page = 1, 
            int itemsPerPage = 10)
        {
            var categories = await _categoryRepository.GetAllAsync(_categoryRepository.Query(),filter, page, itemsPerPage);

            var reposnse = categories.Items.Select(x => new ResponseCategoryDto()
            {
                Id = x.CategoryId,
                Name = x.Name,
                Description = x.Description,
                ShortCode = x.ShortCode,
                IsSeasonal = x.IsSeasonal,
            }).ToList();

            return new PagedResult<ResponseCategoryDto>()
            {
                Items = reposnse,
                TotalCount = categories.TotalCount,
                Page = categories.Page,
                itemsPerPage = categories.itemsPerPage
            };
        }

        public async Task<ResponseCategoryDto> GetCategoryByIdAsync(string id)
        {
            Category category = await _categoryRepository.FirstOrDefault(x => x.CategoryId == id);

            if(category == null)
            {
                return null;
            }

            ResponseCategoryDto responseCategoryDto = new ResponseCategoryDto();

            if(category != null)
            {
                responseCategoryDto.Id = category.CategoryId;
                responseCategoryDto.Name = category.Name;
                responseCategoryDto.Description = category.Description;
                responseCategoryDto.ShortCode = category.ShortCode;
                responseCategoryDto.IsSeasonal = category.IsSeasonal;
            }

            return responseCategoryDto;
        }

        public async Task<ResponseCategoryDto> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.FirstOrDefault(x => x.CategoryId == id);

            var existingCategory = await _categoryRepository.FirstOrDefault(x => x.Name == updateCategoryDto.Name);
            if (existingCategory != null && existingCategory.CategoryId != id)
            {
                throw new InvalidOperationException("This category already exists.");
            }

            category.Description = updateCategoryDto.Description;
            category.Name = updateCategoryDto.Name;
            category.ShortCode = updateCategoryDto.ShortCode;
            category.IsSeasonal = updateCategoryDto.IsSeasonal;

            await _categoryRepository.UpdateAsync(category);

            return new ResponseCategoryDto
            {
                Id = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ShortCode = category.ShortCode,
                IsSeasonal = category.IsSeasonal,
            };
        }
    }
}
