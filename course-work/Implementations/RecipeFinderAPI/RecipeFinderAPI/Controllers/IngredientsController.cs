using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Controllers
{
    /// <summary>
    /// Managing ingredients.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        /// <summary>
        /// Returns all ingredients. Supports filtering by name and allergen status.
        /// </summary>
        /// <param name="name">Optional: Filter ingredients by name (partial match).</param>
        /// <param name="isAllergen">Optional: Filter ingredients by allergen status (true/false).</param>
        /// <param name="page">Page number .</param>
        /// <param name="itemsPerPage">Number of items per page .</param>
        /// <response code="200">Returns all ingredients</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedResult<ResponseIngredientDto>))]
        public async Task<IActionResult> GetAllIngredients(
            [FromQuery] string? name = null,
            [FromQuery] bool? isAllergen = null,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            Expression<Func<Ingredient, bool>> filter = x =>
                (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
                (!isAllergen.HasValue || x.IsAllergen == isAllergen);

            var result = await _ingredientService.GetAllIngredientAsync(filter, page, itemsPerPage);
            return Ok(result);
        }



        /// <summary>
        /// Get ingredient by id.
        /// </summary>
        /// <param name="ingredientId">id of ingredient</param>
        /// <response code="200">Returns ingredient</response>
        /// <response code="400">Invalid/missing ingredient id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Ingredient with this id doesn't exist</response>
        [HttpGet("{ingredientId}")]
        [ProducesResponseType(200, Type = typeof(ResponseIngredientDto))]
        [Authorize]
        public async Task<IActionResult> GetIngredientById(string ingredientId)
        {
            if (string.IsNullOrEmpty(ingredientId))
                return BadRequest(new { message = "Invalid client request" });

            var ingredient = await _ingredientService.GetIngredientByIdAsync(ingredientId);

            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }



        /// <summary>
        /// Creates a new ingredient. Accessible only to admin.
        /// </summary>
        /// <param name="createIngredientDto">Object containing the new ingredient details.</param>
        /// <response code="200">Ingredient is created successfully</response>
        /// <response code="400">Invalid/missing ingredient id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(200, Type = typeof(ResponseIngredientDto))]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto createIngredientDto)
        {
            if (createIngredientDto == null)
                return BadRequest(new { message = "Invalid client request" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _ingredientService.CreateIngredientAsync(createIngredientDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the ingredient." });
            }
        }



        /// <summary>
        /// Edit existing ingredient by id. Accessible only to admin.
        /// </summary>
        /// <param name="ingredientId">ingredient's id</param>
        /// <param name="updateIngredientDto">Ingredient data admin can change</param>
        /// <response code="200">Ingredient is updated successfully</response>
        /// <response code="400">Invalid/missing ingredient id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        /// <response code="404">Ingredient with this id doesn't exist</response>
        [HttpPut("{ingredientId}")]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(200, Type = typeof(ResponseIngredientDto))]
        public async Task<IActionResult> UpdateIngredient(string ingredientId, [FromBody] UpdateIngredientDto updateIngredientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedIngredient = await _ingredientService.UpdateIngredientAsync(ingredientId, updateIngredientDto);

                if (updatedIngredient == null)
                    return NotFound();
                return Ok(updatedIngredient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {message = "This ingredient already exists." });
            }
        }



        /// <summary>
        /// Deletes an ingredient by its ID. Accessible only to admins.
        /// </summary>
        /// <param name="ingredientId">The ID of the ingredient to delete.</param>
        /// <response code="400">Invalid/missing ingredient id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        /// <response code="404">Ingredient with this id doesn't exist</response>
        [HttpDelete("{ingredientId}")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> DeleteIngredient(string ingredientId)
        {
            try
            {
                bool isDeleted = await _ingredientService.DeleteIngredientAsync(ingredientId);

                if (!isDeleted)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "This ingredient is used in a recipe and cannot be deleted" });
            }
        }
    }

}
