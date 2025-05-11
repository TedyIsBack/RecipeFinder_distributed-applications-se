using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto createIngredientDto)
        {
            if (createIngredientDto == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _ingredientService.CreateIngredientAsync(createIngredientDto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
