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
        private readonly IFavoriteService _favoriteService;
        public RecipesController(IRecipeService recipeService, IFavoriteService favoriteService)
        {
            _recipeService = recipeService;
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Returns recipes created by logged user. Supports filtering by recipe name, and if recipe is vegan or vegetarian.
        /// </summary>
        /// <param name="name">Optional: Filter recipe by name (partial match).</param>
        /// <param name="isVegan">Optional: Filter recipe by vegan status (true/false).</param>
        /// <param name="isVegetarian">Optional: Filter recipe by vegetarian status (true/false).</param>
        /// <param name="page">Page number .</param>
        /// <param name="itemsPerPage">Number of items per page .</param>
        /// <response code="200">Returns all recipes created by logged user</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet("created")]
        [ProducesResponseType(typeof(PagedResult<ResponseRecipeDto>), 200)]
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
                (string.IsNullOrEmpty(name) || x.Name.Contains(name))
                    && (!isVegan.HasValue || x.IsVegan == isVegan)
                    && (!isVegetarian.HasValue || x.IsVegetarian == isVegetarian)
                    && (x.CreatedBy == loggedUserId);

            var recipes = await _recipeService.GetAllRecipesAsync(filter, page, itemsPerPage);


            foreach (var recipe in recipes.Items)
            {
                recipe.IsFavorite = await _favoriteService.IsRecipeFavoritedAsync(loggedUserId, recipe.Id);
            }

            return Ok(recipes);
        }

        /// <summary>
        /// Returns all recipes. Supports filtering by recipe name, and if recipe is vegan or vegetarian.
        /// </summary>
        /// <param name="name">Optional: Filter recipe by name (partial match).</param>
        /// <param name="isVegan">Optional: Filter recipe by vegan status (true/false).</param>
        /// <param name="isVegetarian">Optional: Filter recipe by vegetarian status (true/false).</param>
        /// <param name="page">Page number .</param>
        /// <param name="itemsPerPage">Number of items per page .</param>
        /// <response code="200">Returns all recipes</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ResponseRecipeDto>), 200)]
        public async Task<IActionResult> GetAllRecipes(
            [FromQuery] string? name,
            [FromQuery] bool? isVegan,
            [FromQuery] bool? isVegetarian,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            Expression<Func<Recipe, bool>> filter = x =>
                    (string.IsNullOrEmpty(name) || x.Name.Contains(name))
                    && (!isVegan.HasValue || x.IsVegan == isVegan)
                    && (!isVegetarian.HasValue || x.IsVegetarian == isVegetarian);

            var recipes = await _recipeService.GetAllRecipesAsync(filter, page, itemsPerPage);
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var recipe in recipes.Items)
            {
                recipe.IsFavorite = await _favoriteService.IsRecipeFavoritedAsync(loggedUserId, recipe.Id);
            }


            return Ok(recipes);
        }



        /// <summary>
        /// Get recipe by id.
        /// </summary>
        /// <param name="recipeId">id of existing recipe</param>
        /// <response code="200">Returns recipe</response>
        /// <response code="400">Invalid/missing recipe id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Recipe with this id doesn't exist</response>
        [HttpGet("{recipeId}")]
        [ProducesResponseType(typeof(ResponseRecipeDto), 200)]
        public async Task<IActionResult> GetRecipeById(string recipeId)
        {
            var loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);

            if (recipe == null)
                return NotFound();

            recipe.IsFavorite = await _favoriteService.IsRecipeFavoritedAsync(loggedUserId, recipe.Id);

            return Ok(recipe);
        }

        /// <summary>
        /// Create new recipe. Only logged user can create it.
        /// </summary>
        /// <param name="createRecipeDto">Data to create recipe</param>
        /// <response code="200">Recipe is created successfully</response>
        /// <response code="400">Invalid/missing recipe id or invalid data</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRecipeDto), 200)]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto createRecipeDto)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (loggedUserId == null)
                return Unauthorized();

            var created = await _recipeService.CreateRecipeAsync(loggedUserId, createRecipeDto);

            if (created == null)
                return BadRequest(new { message = "Recipe already exists" });

            return Ok(created);
        }



        /// <summary>
        /// Edit existing recipe by id. Only the recipe's creator can edit it.
        /// </summary>
        /// <param name="recipeId">id of existing recipe</param>
        /// <param name="updateRecipeDto">Recipe data which only recipe's creator can edit</param>
        /// <response code="200">Recipe is updated successfully</response>
        /// <response code="400">Invalid/missing recipe id or wrong data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only recipe's creator can perform this action.</response>
        /// <response code="404">Recipe with this id doesn't exist</response>

        [HttpPut("{recipeId}")]
        [ProducesResponseType(typeof(ResponseRecipeDto), 200)]
        public async Task<IActionResult> UpdateRecipe(string recipeId, [FromBody] UpdateRecipeDto updateRecipeDto)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var updated = await _recipeService.UpdateRecipeAsync(loggedUserId, recipeId, updateRecipeDto);

            if (updated == null)
                return NotFound();

            if (updated.CreatedBy != loggedUserId)
                return Forbid();

            return Ok(updated);
        }



        /// <summary>
        /// Delete existing recipe by id. Only the recipe's creator can delete it.
        /// </summary>
        /// <param name="recipeId">id of existing recipe</param>
        /// <response code="400">Invalid/missing recipe id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only recipe's creator can perform this action.</response>
        /// <response code="404">Recipe with this id doesn't exist</response>
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> DeleteRecipe(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);

            if (recipe == null)
                return NotFound();

            if (recipe.CreatedBy != loggedUserId)
                return Forbid();

            bool deleted = await _recipeService.DeleteRecipeAsync(recipeId);
            if (!deleted)
                return NotFound("Recipe not found");

            return Ok();
        }
    }
}
