using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        [Authorize]
        public async Task<PagedResult<ResponseCategoryDto>> GetAllCategories(
            [FromQuery] string? name = null,
            [FromQuery] bool? IsSeasonal = null,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            Expression<Func<Category, bool>> filter = x =>
             (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
            (!IsSeasonal.HasValue || x.IsSeasonal == IsSeasonal);

            return await _categoryService.GetAllCategoryAsync(filter, page, itemsPerPage);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid client request");

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }


        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _categoryService.CreateCategoryAsync(createCategoryDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
                return Ok(updatedCategory);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            bool isDeleted = await _categoryService.DeleteCategoryAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
