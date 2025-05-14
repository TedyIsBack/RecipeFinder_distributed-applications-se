using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly BaseRepository<Ingredient> _ingredientRepository;

        public IngredientService(BaseRepository<Ingredient> ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public async Task<ResponseIngredientDto> CreateIngredientAsync(CreateIngredientDto createIngredientDto)
        {

            var existingIngredient = await _ingredientRepository.FirstOrDefault(x => x.Name == createIngredientDto.Name);

            if (existingIngredient != null)
            {
                throw new InvalidOperationException("This ingredient already exists.");
            }

            Ingredient ingredient = new Ingredient()
            {
                Name = createIngredientDto.Name,
                ImgUrl = createIngredientDto.ImgUrl.IsNullOrEmpty() ?
                "https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png"
                : createIngredientDto.ImgUrl,
                CaloriesPer100g = createIngredientDto.CaloriesPer100g,
                Unit = createIngredientDto.Unit,
                IsAllergen = createIngredientDto.IsAllergen
            };

            await _ingredientRepository.AddAsync(ingredient);

            return new ResponseIngredientDto()
            {
                Id = ingredient.IngredientId,
                Name = ingredient.Name,
                ImgUrl = ingredient.ImgUrl,
                CaloriesPer100g = ingredient.CaloriesPer100g,
                Unit = ingredient.Unit,
                IsAllergen = ingredient.IsAllergen
            };
        }

        public async Task<ResponseIngredientDto> GetIngredientByIdAsync(string id)
        {
            Ingredient ingredient = await _ingredientRepository.FirstOrDefault(x => x.IngredientId == id);
            ResponseIngredientDto responseIngredientDto = new ResponseIngredientDto();
            if (ingredient != null)
            {
                responseIngredientDto = new ResponseIngredientDto()
                {
                    Id = ingredient.IngredientId,
                    Name = ingredient.Name,
                    ImgUrl = ingredient.ImgUrl,
                    CaloriesPer100g = ingredient.CaloriesPer100g,
                    Unit = ingredient.Unit,
                    IsAllergen = ingredient.IsAllergen
                };
            }

            return responseIngredientDto;
        }

        public async Task<ResponseIngredientDto> UpdateIngredientAsync(string id,UpdateIngredientDto updateIngredientDto)
        {
            var ingredient = await _ingredientRepository.FirstOrDefault(x => x.IngredientId == id);

            ingredient.Name = updateIngredientDto.Name;
            ingredient.ImgUrl = updateIngredientDto.ImgUrl.IsNullOrEmpty() ?
                "https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png"
                : updateIngredientDto.ImgUrl;
            ingredient.CaloriesPer100g = updateIngredientDto.CaloriesPer100g;
            ingredient.Unit = updateIngredientDto.Unit;
            ingredient.IsAllergen = updateIngredientDto.IsAllergen;

            await _ingredientRepository.UpdateAsync(ingredient);

            return new ResponseIngredientDto()
            {
                Id = ingredient.IngredientId,
                Name = ingredient.Name,
                ImgUrl = ingredient.ImgUrl,
                CaloriesPer100g = ingredient.CaloriesPer100g,
                Unit = ingredient.Unit,
                IsAllergen = ingredient.IsAllergen
            };
        }

        public async Task<bool> DeleteIngredientAsync(string id)
        {
            var ingredient = await _ingredientRepository.FirstOrDefault(x => x.IngredientId == id);
            if (ingredient == null)
            {
                return false;
            }
            await _ingredientRepository.DeleteAsync(ingredient);
            return true;
        }

        public async Task<PagedResult<ResponseIngredientDto>> GetAllIngredientAsync(
            Expression<Func<Ingredient, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10)
        {
            var ingredients = await _ingredientRepository.GetAllAsync(_ingredientRepository.Query(),filter, page, itemsPerPage);

            var response = ingredients.Items.Select(x => new ResponseIngredientDto()
            {
                Id = x.IngredientId,
                Name = x.Name,
                ImgUrl = x.ImgUrl,
                CaloriesPer100g = x.CaloriesPer100g,
                Unit = x.Unit,
                IsAllergen = x.IsAllergen
            }).ToList();

            return new PagedResult<ResponseIngredientDto>()
            {
                Items = response,
                TotalCount = ingredients.TotalCount,
                Page = ingredients.Page,
                itemsPerPage = ingredients.itemsPerPage
            };
        }


    }
}
