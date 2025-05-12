using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }


        [HttpGet]
        [Authorize]
        public async Task<PagedResult<ResponseIngredientDto>> GetAllIngredients(
            [FromQuery] string name = null,
            [FromQuery] bool? isAllergen = null,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {

            Expression<Func<Ingredient, bool>> filter = x =>
              (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
              (!isAllergen.HasValue || x.IsAllergen == isAllergen);

            return await _ingredientService.GetAllIngredientAsync(filter,page,itemsPerPage);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIngredientById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid client request");

            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }


        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto createIngredientDto)
        {
            if (createIngredientDto == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _ingredientService.CreateIngredientAsync(createIngredientDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> UpdateIngredient(string id, UpdateIngredientDto updateIngredientDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(id,updateIngredientDto);

            return Ok(updatedIngredient);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> DeleteIngredient(string id)
        {
            bool isDeleted = await _ingredientService.DeleteIngredientAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
