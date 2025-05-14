using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet("created")]
        public async Task<IActionResult> GetCreatedRecipes(
          [FromQuery] string name,
          [FromQuery] int page = 1,
          [FromQuery] int itemsPerPage = 10)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedUserId == null)
                return Unauthorized();

            Expression<Func<Recipe, bool>> filter = x =>
         ((string.IsNullOrEmpty(name) || x.Name.Contains(name)) && (x.CreatedBy == loggedUserId));


            var created = await _recipeService.GetAllUserRecipesAsync(loggedUserId, filter, page, itemsPerPage);

            if (created == null)
                return NotFound();

            return Ok(created);
        }

        
    }
}
