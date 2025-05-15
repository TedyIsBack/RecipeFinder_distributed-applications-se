using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="itemsPerPage">Number of items per page (default is 10).</param>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedResult<ResponseIngredientDto>))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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
        /// <param name="id">id of ingredient</param>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ResponseIngredientDto))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Authorize]
        public async Task<IActionResult> GetIngredientById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Invalid client request" });

            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);

            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        /// <summary>
        /// Creates a new ingredient. Accessible only to admin.
        /// </summary>
        /// <param name="createIngredientDto">Object containing the new ingredient details.</param>
        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(200, Type = typeof(ResponseIngredientDto))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto createIngredientDto)
        {
            if (createIngredientDto == null)
                return BadRequest(new { message = "Invalid client request" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _ingredientService.CreateIngredientAsync(createIngredientDto);
            return Ok(result);
        }

        /// <summary>
        /// Edit existing ingredient by id. Accessible only to admin.
        /// </summary>
        /// <param name="id">ingredient's id</param>
        /// <param name="updateIngredientDto">Ingredient data admin can change</param>
        [HttpPut("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(200, Type = typeof(ResponseIngredientDto))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateIngredient(string id, [FromBody] UpdateIngredientDto updateIngredientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(id, updateIngredientDto);
            return Ok(updatedIngredient);
        }

        /// <summary>
        /// Deletes an ingredient by its ID. Accessible only to admins.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteIngredient(string id)
        {
            bool isDeleted = await _ingredientService.DeleteIngredientAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
