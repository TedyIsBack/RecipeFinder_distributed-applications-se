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

    /// <summary>
    ///Maintains recipe's categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")] // 👈 Винаги връща JSON
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Returns all categories. Supports filtering by name and seasonal status.
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(PagedResult<ResponseCategoryDto>), 200)]
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

        /// <summary>
        /// Get category by id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseCategoryDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid client request");

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Creates a new category. Accessible only to admin.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(typeof(ResponseCategoryDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return Ok(result);
        }

        /// <summary>
        /// Edit existing category by id. Accessible only to admin.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(typeof(ResponseCategoryDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            return Ok(updatedCategory);
        }

        /// <summary>
        /// Delete existing category by id. Accessible only to admin.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            bool isDeleted = await _categoryService.DeleteCategoryAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }

}
