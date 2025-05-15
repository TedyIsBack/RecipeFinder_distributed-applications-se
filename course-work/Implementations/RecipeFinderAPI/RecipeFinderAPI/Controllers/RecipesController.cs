using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipesDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
    /// <summary>
    /// Access to all recipes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.UserRole)]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        /// <summary>
        /// Returns recipes created by logged user. Supports filtering by recipe name, and if recipe is vegan or vegetarian.
        /// </summary>
        /// <param name="name">Optional: Filter recipe by name (partial match).</param>
        /// <param name="isVegan">Optional: Filter recipe by vegan status (true/false).</param>
        /// <param name="isVegetarian">Optional: Filter recipe by vegetarian status (true/false).</param>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="itemsPerPage">Number of items per page (default is 10).</param>
        [HttpGet("created")]
        [ProducesResponseType(typeof(PagedResult<ResponseRecipeDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCreatedRecipes(
          [FromQuery] string? name,
          [FromQuery] bool? isVegan,
          [FromQuery] bool? isVegetarian,
          [FromQuery] int page = 1,
          [FromQuery] int itemsPerPage = 10)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedUserId == null)
                return Unauthorized();

            Expression<Func<Recipe, bool>> filter = x =>
                (string.IsNullOrEmpty(name) || x.Name.Contains(name)) && (x.CreatedBy == loggedUserId);

            var recipes = await _recipeService.GetAllRecipesAsync(filter, page, itemsPerPage);

            if (recipes == null)
                return NotFound();

            return Ok(recipes);
        }

        /// <summary>
        /// Returns all recipes. Supports filtering by recipe name, and if recipe is vegan or vegetarian.
        /// </summary>
        /// <param name="name">Optional: Filter recipe by name (partial match).</param>
        /// <param name="isVegan">Optional: Filter recipe by vegan status (true/false).</param>
        /// <param name="isVegetarian">Optional: Filter recipe by vegetarian status (true/false).</param>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="itemsPerPage">Number of items per page (default is 10).</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ResponseRecipeDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllRecipes(
            [FromQuery] string? name,
            [FromQuery] bool? isVegan,
            [FromQuery] bool? isVegetarian,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            Expression<Func<Recipe, bool>> filter = null;
            if (!string.IsNullOrEmpty(name) || isVegan.HasValue || isVegetarian.HasValue)
            {
                filter = x =>
                    (string.IsNullOrEmpty(name) || x.Name.Contains(name)) && (x.IsVegan == isVegan) && (x.IsVegetarian == isVegetarian);
            }

            var recipes = await _recipeService.GetAllRecipesAsync(filter, page, itemsPerPage);

            if (recipes == null)
                return NotFound();

            return Ok(recipes);
        }

        /// <summary>
        /// Create new recipe. Only logged user can create it.
        /// </summary>
        /// <param name="createRecipeDto">Data to create recipe</param>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRecipeDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto createRecipeDto)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var created = await _recipeService.CreateRecipeAsync(loggedUserId, createRecipeDto);

            if (created == null)
                return BadRequest("Recipe could not be created.");

            return Ok(created);
        }

        /// <summary>
        /// Edit existing recipe by id. Only the recipe's creator can edit it.
        /// </summary>
        /// <param name="recipeId">id of existing recipe</param>
        /// <param name="updateRecipeDto">Recipe data which only recipe's creator can edit</param>
        [HttpPut("{recipeId}")]
        [ProducesResponseType(typeof(ResponseRecipeDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRecipe(string recipeId, [FromBody] UpdateRecipeDto updateRecipeDto)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var updated = await _recipeService.UpdateRecipeAsync(loggedUserId, recipeId, updateRecipeDto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        /// <summary>
        /// Delete existing recipe by id. Only the recipe's creator can delete it.
        /// </summary>
        /// <param name="recipeId">id of existing recipe</param>
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRecipe(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized("User is not logged in.");

            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
                return NotFound("Recipe not found.");

            if (recipe.CreatedBy != loggedUserId)
                return Unauthorized("You are not authorized to delete this recipe.");

            bool deleted = await _recipeService.DeleteRecipeAsync(recipeId);
            if (!deleted)
                return NotFound("Failed to delete recipe.");

            return Ok("Recipe successfully deleted.");
        }
    }
}
