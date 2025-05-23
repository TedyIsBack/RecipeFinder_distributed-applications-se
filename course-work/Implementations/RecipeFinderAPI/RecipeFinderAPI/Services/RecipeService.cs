using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipesDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly BaseRepository<Recipe> _recipeRepository;
        private readonly IIngredientService _ingredientService;
        private readonly BaseRepository<User> _userRepository;
        private readonly BaseRepository<Category> _categoryRepository;
        public RecipeService(BaseRepository<Recipe> baseRepository,
            BaseRepository<User> userRepository,
            IIngredientService ingredientService,
            BaseRepository<Category> categoryRepository)
        {
            _recipeRepository = baseRepository;
            _ingredientService = ingredientService;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<ResponseRecipeDto> CreateRecipeAsync(string userId, CreateRecipeDto createRecipeDto)
        {
            var existingRecipe = await _recipeRepository.FirstOrDefault(x => x.Name == createRecipeDto.Name);
            if (existingRecipe != null)
                return null;
                //throw new InvalidOperationException("Recipe already exists");

            var user = await _userRepository.FirstOrDefault(x => x.UserId == userId);
            var category = await _categoryRepository.FirstOrDefault(x => x.CategoryId == createRecipeDto.CategoryId);

            var newRecipe = new Recipe
            {
                Name = createRecipeDto.Name,
                Description = createRecipeDto.Description,
                ImgUrl = string.IsNullOrEmpty(createRecipeDto.ImgUrl)
                    ? Constants.DefaultRecipeImage
                    : createRecipeDto.ImgUrl,
                PreparationTime = createRecipeDto.PreparationTime,
                IsVegan = createRecipeDto.IsVegan,
                IsVegetarian = createRecipeDto.IsVegetarian,
                CreatedBy = userId,
                Category = category,
                User = user,
                CategoryId = createRecipeDto.CategoryId,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            var recipeIngredients = createRecipeDto.RecipeIngredients
                .Select(ri => new RecipeIngredient
                {
                    RecipeIngredientId = Guid.NewGuid().ToString(),
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity
                }).ToList();

            newRecipe.RecipeIngredients = recipeIngredients;

             var ingredientIds = createRecipeDto.RecipeIngredients.Select(ri => ri.IngredientId).ToList();

            var responseIngredients = await _ingredientService.GetIngredientsByIdsAsync(ingredientIds);

            var ingredientsDict = responseIngredients.ToDictionary(ingredient => ingredient.Id);

            var recipeIngredientsDto = createRecipeDto.RecipeIngredients
                .Where(ri => ingredientsDict.ContainsKey(ri.IngredientId))
                .Select((ri, index) => new ResponseRecipeIngredientDto
                {
                    RecipeIngredientId = recipeIngredients[index].RecipeIngredientId,  
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Name = ingredientsDict[ri.IngredientId].Name,
                    ImgUrl = ingredientsDict[ri.IngredientId].ImgUrl,
                    CaloriesPer100g = ingredientsDict[ri.IngredientId].CaloriesPer100g,
                    IsAllergen = ingredientsDict[ri.IngredientId].IsAllergen,
                    Unit = ingredientsDict[ri.IngredientId].Unit
                })
                .ToList();

            var totalCalories = recipeIngredientsDto
                .Sum(ri => (ri.Quantity / 100) * ri.CaloriesPer100g);

            newRecipe.Calories = totalCalories;

            await _recipeRepository.AddAsync(newRecipe);

            return new ResponseRecipeDto
            {
                Id = newRecipe.RecipeId,
                Name = newRecipe.Name,
                Description = newRecipe.Description,
                ImgUrl = newRecipe.ImgUrl,
                PreparationTime = newRecipe.PreparationTime,
                IsVegan = newRecipe.IsVegan,
                IsVegetarian = newRecipe.IsVegetarian,
                CategoryId = newRecipe.CategoryId,
                Category = new ResponseCategoryDto()
                {
                    Id = newRecipe.Category.CategoryId,
                    Name = newRecipe.Category.Name,
                    Description = newRecipe.Category.Description,
                    IsSeasonal = newRecipe.Category.IsSeasonal,
                    ShortCode = newRecipe.Category.ShortCode,
                },
                RecipeIngredients = recipeIngredientsDto,
                CreatedBy = userId,
                CreatedByUser = new ResponseAccountDto()
                {
                    Id = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt.ToLongDateString() + " " + user.CreatedAt.ToLongTimeString(),
                },
                Calories =newRecipe.Calories
            };
        }

        public async Task<bool> DeleteRecipeAsync(string id)
        {
            var existingRecipe = await _recipeRepository.FirstOrDefault(x => x.RecipeId == id);
            if (existingRecipe == null)  // Ако рецептата не съществува, не може да се изтрие
                return false;

            // Изтриване на рецептата
            await _recipeRepository.DeleteAsync(existingRecipe);
            return true;
        }

        public async Task<PagedResult<ResponseRecipeDto>> GetAllRecipesAsync(Expression<Func<Recipe, bool>> filter = null, int page = 1, int itemsPerPage = 10)
        {
            var query = _recipeRepository.Query()
     .Include(r => r.User)
     .Include(r => r.Category)
     .Include(r => r.RecipeIngredients)
         .ThenInclude(ri => ri.Ingredient);


            var result = await _recipeRepository.GetAllAsync(query, filter, page, itemsPerPage);
            var response = result.Items.Select(recipe => new ResponseRecipeDto
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                Description = recipe.Description,
                ImgUrl = recipe.ImgUrl,
                Calories = recipe.Calories,
                PreparationTime = recipe.PreparationTime,
                IsVegan = recipe.IsVegan,
                IsVegetarian = recipe.IsVegetarian,
                CreatedBy = recipe.CreatedBy,
                CreatedByUser = new ResponseAccountDto()
                {
                    CreatedAt = recipe.User.CreatedAt.ToLongDateString(),
                    Email = recipe.User.Email,
                    Username = recipe.User.Username,
                    Id = recipe.User.UserId

                },
                CategoryId = recipe.CategoryId,
                Category = new ResponseCategoryDto
                {
                    Id = recipe.CategoryId,
                    Name = recipe.Category.Name,
                    Description = recipe.Category.Description,
                    ShortCode = recipe.Category.ShortCode,
                    IsSeasonal = recipe.Category.IsSeasonal,
                },

                RecipeIngredients = recipe.RecipeIngredients.Select(ri => new ResponseRecipeIngredientDto
                {
                    RecipeIngredientId = ri.RecipeIngredientId,
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Name = ri.Ingredient.Name,
                    ImgUrl = ri.Ingredient.ImgUrl,
                    CaloriesPer100g = ri.Ingredient.CaloriesPer100g,
                    Unit = ri.Ingredient.Unit,
                    IsAllergen = ri.Ingredient.IsAllergen
                }).ToList()
            }).ToList();

            return new PagedResult<ResponseRecipeDto>
            {
                Items = response,
                TotalCount = result.TotalCount,
                Page = result.Page,
                itemsPerPage = result.itemsPerPage,
                PagesCount = result.PagesCount
            };
        }

        public async Task<ResponseRecipeDto> GetRecipeByIdAsync(string recipeId)
        {
            var recipe = await _recipeRepository.Query()
             .Include(r => r.User)
             .Include(r => r.Category)
             .Include(r => r.RecipeIngredients)
                 .ThenInclude(ri => ri.Ingredient)
             .FirstOrDefaultAsync(x => x.RecipeId == recipeId);


            if (recipe == null)
                throw new Exception("Recipe not found");

            var recipeIngredients = recipe.RecipeIngredients.Select(ri => new ResponseRecipeIngredientDto
            {
                RecipeIngredientId = ri.RecipeIngredientId,
                IngredientId = ri.IngredientId,
                Quantity = ri.Quantity
            }).ToList();


            var responseRecipe = new ResponseRecipeDto
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                Description = recipe.Description,
                ImgUrl = recipe.ImgUrl,
                PreparationTime = recipe.PreparationTime,
                IsVegan = recipe.IsVegan,
                IsVegetarian = recipe.IsVegetarian,
                CreatedBy = recipe.CreatedBy,
                CreatedByUser = new ResponseAccountDto()
                {
                    CreatedAt = recipe.User.CreatedAt.ToLongDateString(),
                    Email = recipe.User.Email,
                    Username = recipe.User.Username,
                    Id = recipe.User.UserId

                },
                CategoryId = recipe.CategoryId,
                Category = new ResponseCategoryDto
                {
                    Name = recipe.Category.Name,
                    Description = recipe.Category.Description,
                    ShortCode = recipe.Category.ShortCode,
                    IsSeasonal = recipe.Category.IsSeasonal,
                },

                RecipeIngredients = recipe.RecipeIngredients.Select(ri => new ResponseRecipeIngredientDto
                {
                    RecipeIngredientId = ri.RecipeIngredientId,
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Name = ri.Ingredient.Name,
                    ImgUrl = ri.Ingredient.ImgUrl,
                    CaloriesPer100g = ri.Ingredient.CaloriesPer100g,
                    Unit = ri.Ingredient.Unit,
                    IsAllergen = ri.Ingredient.IsAllergen
                }).ToList()
            };

            return responseRecipe;
        }


        public async Task<ResponseRecipeDto> UpdateRecipeAsync(string userId, string recipeId, UpdateRecipeDto updateRecipeDto)
        {
            // Зареждаме рецептата с включени съставки и техните детайли
            var recipe = await _recipeRepository.Query()
                     .Include(r => r.RecipeIngredients)
                         .ThenInclude(ri => ri.Ingredient)
                     .Include(r => r.User)            // <-- нужният include за създателя
                     .Include(r => r.Category)        // <-- нужният include за категорията
                     .FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.CreatedBy == userId);


            if (recipe == null)
                throw new Exception("Recipe not found");

            // Обновяване на основните свойства
            recipe.Name = updateRecipeDto.Name;
            recipe.Description = updateRecipeDto.Description;
            recipe.ImgUrl = string.IsNullOrEmpty(updateRecipeDto.ImgUrl)
                ? Constants.DefaultRecipeImage
                : updateRecipeDto.ImgUrl;
            recipe.PreparationTime = updateRecipeDto.PreparationTime;
            recipe.IsVegan = updateRecipeDto.IsVegan;
            recipe.IsVegetarian = updateRecipeDto.IsVegetarian;
            recipe.CategoryId = updateRecipeDto.CategoryId;

            // Обработка на съставките
            var existingIngredients = recipe.RecipeIngredients ?? new List<RecipeIngredient>();
            var updatedIngredients = updateRecipeDto.RecipeIngredients ?? new List<CreateRecipeIngredientDto>();

            // Премахваме съставки, които вече не съществуват
            var toRemove = existingIngredients
                .Where(ei => !updatedIngredients.Any(ui => ui.IngredientId == ei.IngredientId))
                .ToList();

            foreach (var remove in toRemove)
            {
                recipe.RecipeIngredients.Remove(remove);
            }

            // Обновяваме количествата на съществуващите съставки
            foreach (var existing in existingIngredients)
            {
                var match = updatedIngredients.FirstOrDefault(ui => ui.IngredientId == existing.IngredientId);
                if (match != null)
                {
                    existing.Quantity = match.Quantity;
                }
            }

            // Добавяме нови съставки
            var toAdd = updatedIngredients
                .Where(ui => !existingIngredients.Any(ei => ei.IngredientId == ui.IngredientId))
                .Select(ui => new RecipeIngredient
                {
                    RecipeIngredientId = Guid.NewGuid().ToString(),
                    IngredientId = ui.IngredientId,
                    Quantity = ui.Quantity
                }).ToList();

            foreach (var add in toAdd)
            {
                recipe.RecipeIngredients.Add(add);
            }

           

            recipe = await _recipeRepository.Query()
                .Include(r => r.User)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(x => x.RecipeId == recipeId);


            // Зареждаме детайлите за съставките, за да изчислим калориите
            var ingredientIds = recipe.RecipeIngredients.Select(ri => ri.IngredientId).ToList();
            var ingredients = await _ingredientService.GetIngredientsByIdsAsync(ingredientIds);
            var ingredientsDict = ingredients.ToDictionary(i => i.Id);

            var recipeIngredientsDto = recipe.RecipeIngredients.Select(ri => new ResponseRecipeIngredientDto
            {
                RecipeIngredientId = ri.RecipeIngredientId,
                IngredientId = ri.IngredientId,
                Quantity = ri.Quantity,
                Name = ingredientsDict[ri.IngredientId].Name,
                ImgUrl = ingredientsDict[ri.IngredientId].ImgUrl,
                CaloriesPer100g = ingredientsDict[ri.IngredientId].CaloriesPer100g,
                IsAllergen = ingredientsDict[ri.IngredientId].IsAllergen,
                Unit = ingredientsDict[ri.IngredientId].Unit
            }).ToList();

            recipe.Calories = recipeIngredientsDto.Sum(ri => (ri.Quantity / 100) * ri.CaloriesPer100g); ;
            // Записваме промените
            await _recipeRepository.UpdateAsync(recipe);
            // Връщаме отговора
            return new ResponseRecipeDto
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                Description = recipe.Description,
                ImgUrl = recipe.ImgUrl,
                PreparationTime = recipe.PreparationTime,
                IsVegan = recipe.IsVegan,
                IsVegetarian = recipe.IsVegetarian,
                CreatedBy = recipe.CreatedBy,

                CreatedByUser = new ResponseAccountDto()
                {
                    CreatedAt = recipe.User.CreatedAt.ToLongDateString(),
                    Email = recipe.User.Email,
                    Username = recipe.User.Username,
                    Id = recipe.User.UserId
                },
                CategoryId = recipe.CategoryId,
                Category = new ResponseCategoryDto()
                {
                    Id = recipe.Category.CategoryId,
                    Name = recipe.Category.Name,
                    Description = recipe.Category.Description,
                    IsSeasonal = recipe.Category.IsSeasonal,
                    ShortCode = recipe.Category.ShortCode,
                },
            
                RecipeIngredients = recipeIngredientsDto,
                Calories = recipe.Calories
            };
        }

    }
}


