using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.FavoriteDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
    /// <summary>
    /// Managing user favorite recipes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.UserRole)]
    [Produces("application/json")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IRecipeService _recipeService;
        public FavoritesController(IFavoriteService favoriteService, IRecipeService recipeService)
        {
            _favoriteService = favoriteService;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Returns logged user's favorites recipes. Supports filtering by recipe name, and if recipe is vegan or vegetarian.
        /// </summary>
        /// <param name="name">Optional: Filter recipe by name (partial match).</param>
        /// <param name="isVegan">Optional: Filter recipe by vegan status (true/false).</param>
        /// <param name="isVegetarian">Optional: Filter recipe by vegetarian status (true/false).</param>
        /// <param name="page">Page number .</param>
        /// <param name="itemsPerPage">Number of items per page .</param>
        /// <response code="200">Returns logged user's favorite recipes</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedResult<ResponseFavoriteDto>))]
        public async Task<IActionResult> GetFavorites(
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
                (string.IsNullOrEmpty(name) || x.Name.Contains(name));

            var favorites = await _favoriteService.GetUserFavoriteRecipesAsync(loggedUserId, filter, page, itemsPerPage);

            foreach (var recipe in favorites.Items)
            {
                recipe.IsFavorite = await _favoriteService.IsRecipeFavoritedAsync(loggedUserId, recipe.Id);
            }

            return Ok(favorites);
        }

        /// <summary>
        /// Add recipe to favorites
        /// </summary>
        /// <param name="recipeId">recipe's id</param>
        /// <response code="200">Recipe added successfully to favorites</response>
        /// <response code="400">Invalid/missing category id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Recipe with such id doesn't exist</response>
        [HttpPost("{recipeId}")]
        public async Task<IActionResult> AddToFavorites(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var result = await _favoriteService.AddToFavoritesAsync(loggedUserId, recipeId);

            if (!result)
                return BadRequest(new { message = "Recipe already in favorites" });

            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);

            recipe.IsFavorite = await _favoriteService.IsRecipeFavoritedAsync(loggedUserId,recipeId);

            return Ok(new { message = "Recipe added to favorites" });
        }



        /// <summary>
        /// Remove recipe from favorites
        /// </summary>
        /// <param name="recipeId">id of existing recipe</param>
        /// <response code="400">Invalid/missing recipe id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Recipe with this id is not in favorites or doesn't exist</response>
        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> RemoveFromFavorites(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (loggedUserId == null)
                return Unauthorized();

            if(recipeId.IsNullOrEmpty())
                return BadRequest(new { message = "Invalid recipe id" });


            var result = await _favoriteService.RemoveFromFavoritesAsync(loggedUserId, recipeId);

            if (!result)
                return NotFound(new { message = "Recipe not found in favorites" });


            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);

            recipe.IsFavorite = await _favoriteService.IsRecipeFavoritedAsync(loggedUserId, recipeId);


            return Ok();
            //return Ok(new { message = "Recipe removed from favorites" });
        }
    }
}
