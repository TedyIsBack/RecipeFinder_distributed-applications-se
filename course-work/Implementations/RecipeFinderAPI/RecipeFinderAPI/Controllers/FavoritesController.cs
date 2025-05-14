using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }


        [HttpGet]
        public async Task<IActionResult> GetFavorites(
           [FromQuery] string? name,
           [FromQuery] int page = 1,
           [FromQuery] int itemsPerPage = 10)
        {
            string loggedUserId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();
            Expression<Func<Recipe, bool>> filter = x =>
            (string.IsNullOrEmpty(name) || x.Name.Contains(name));

            var favorites = await _favoriteService.GetUserFavoriteRecipesAsync(loggedUserId, filter, page, itemsPerPage);

            if (favorites == null)
                return NotFound();

            return Ok(favorites);
        }


        [HttpPost("{recipeId}")]
        public async Task<IActionResult> AddToFavorites(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var result = await _favoriteService.AddToFavoritesAsync(loggedUserId, recipeId);

            if (!result)
                return BadRequest("Recipe already in favorites");

            return Ok("Recipe added to favorites");
        }

        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> RemoveFromFavorites(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var result = await _favoriteService.RemoveFromFavoritesAsync(loggedUserId, recipeId);

            if (!result)
                return BadRequest("Recipe not found in favorites");

            return Ok("Recipe removed from favorites");
        }
    }
}
