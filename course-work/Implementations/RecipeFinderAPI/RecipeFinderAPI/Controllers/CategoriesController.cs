using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;
using System.Data.Common;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Controllers
{

    /// <summary>
    ///Maintains recipe's categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
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
        /// <param name="name">Optional: Filter categories by name (partial match).</param>
        /// <param name="IsSeasonal">Optional: Filter ingredients by allergen status (true/false).</param>
        /// <param name="page">Page number .</param>
        /// <param name="itemsPerPage">Number of items per page .</param>
        /// <response code="200">Returns all categories</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        //[Authorize]
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
        /// <param name="categoryId">id of existing category</param>
        /// <response code="200">Returns category</response>
        /// <response code="400">Invalid/missing category id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Category with this id doesn't exist</response>
        [HttpGet("{categoryId}")]
        //[Authorize]
        [ProducesResponseType(typeof(ResponseCategoryDto), 200)]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
                return BadRequest("Invalid client request");

            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Creates a new category. Accessible only to admin.
        /// </summary>
        /// <param name="createCategoryDto">Required data to create new category</param>
        /// <response code="200">Returns created category</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin ca create category</response>
        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(typeof(ResponseCategoryDto), 200)]
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
        /// <param name="categoryId">id of existing category</param>
        /// <param name="updateCategoryDto">Category info you can change</param>
        /// <response code="200">Returns category</response>
        /// <response code="400">Invalid/missing category id or wrong data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin perform this action.</response>
        /// <response code="404">Category with this id doesn't exist</response>
        [HttpPut("{categoryId}")]
        //[Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(typeof(ResponseCategoryDto), 200)]
        public async Task<IActionResult> UpdateCategory(string categoryId, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCategory = await _categoryService.UpdateCategoryAsync(categoryId, updateCategoryDto);

            if (updatedCategory == null)
                return NotFound();

            return Ok(updatedCategory);
        }


        /// <summary>
        /// Delete existing category by id. Accessible only to admin.
        /// </summary>
        /// <param name="categoryId">id of existing category</param>
        /// <response code="400">Invalid/missing category id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        /// <response code="404">Category with this id doesn't exist</response>
        [HttpDelete("{categoryId}")]
        //[Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            try
            {
                bool isDeleted = await _categoryService.DeleteCategoryAsync(categoryId);

                if (!isDeleted)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to delete category. Category is used in a recipe.");
            }
        }

    }
}
