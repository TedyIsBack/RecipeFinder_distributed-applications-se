using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipesDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
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

        [HttpGet("created")]
        public async Task<IActionResult> GetCreatedRecipes(
          [FromQuery] string? name,
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

        [HttpGet]
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

            var recipes = await _recipeService.GetAllRecipesAsync(filter,page, itemsPerPage);

            if (recipes == null)
                return NotFound();

            return Ok(recipes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto createRecipeDto)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var created = await _recipeService.CreateRecipeAsync(loggedUserId, createRecipeDto);

            if (created == null)
                return BadRequest("Recipe could not be created.");

            return Ok(created);

            //return CreatedAtAction(nameof(GetCreatedRecipes), new { id = created.Id }, created);
        }

        [HttpPut("{recipeId}")]
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

        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> DeleteRecipe(string recipeId)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized("User is not logged in.");

            // Проверка дали рецептата съществува и дали потребителят е собственик на нея
            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
                return NotFound("Recipe not found.");

            if (recipe.CreatedBy != loggedUserId)
                return Unauthorized("You are not authorized to delete this recipe.");

            // Изтриване на рецептата
            bool deleted = await _recipeService.DeleteRecipeAsync(recipeId);
            if (!deleted)
                return NotFound("Failed to delete recipe.");

            return Ok("Recipe successfully deleted.");
        }
    }
}