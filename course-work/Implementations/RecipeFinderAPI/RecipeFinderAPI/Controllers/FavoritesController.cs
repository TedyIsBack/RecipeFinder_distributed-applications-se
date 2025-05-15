using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]  // Задължителна автентикация
    [Produces("application/json")]  // Винаги JSON отговор
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Returns logged user's favorites recipes. Supports filtering by recipe name, and if recipe is vegan or vegetarian.
        /// </summary>
        /// <param name="name">Optional: Filter recipe by name (partial match).</param>
        /// <param name="isVegan">Optional: Filter recipe by vegan status (true/false).</param>
        /// <param name="isVegetarian">Optional: Filter recipe by vegetarian status (true/false).</param>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="itemsPerPage">Number of items per page (default is 10).</param>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedResult<ResponseFavoriteDto>))] // 
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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

            if (favorites == null)
                return NotFound();

            return Ok(favorites);
        }

        /// <summary>
        /// Add recipe to favorites
        /// </summary>
        /// <param name="recipeId">recipe's id</param>
        [HttpPost("{recipeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddToFavorites(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var result = await _favoriteService.AddToFavoritesAsync(loggedUserId, recipeId);

            if (!result)
                return BadRequest(new { message = "Recipe already in favorites" });

            return Ok(new { message = "Recipe added to favorites" });
        }

        /// <summary>
        /// Remove recipe from favorites
        /// </summary>
        /// <param name="recipeId">recipe's id</param>
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveFromFavorites(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var result = await _favoriteService.RemoveFromFavoritesAsync(loggedUserId, recipeId);

            if (!result)
                return BadRequest(new { message = "Recipe not found in favorites" });

            return Ok(new { message = "Recipe removed from favorites" });
        }
    }
}
