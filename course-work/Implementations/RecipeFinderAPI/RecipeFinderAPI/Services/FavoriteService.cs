using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Infrastructure.DTOs.FavoriteDTOs;

namespace RecipeFinderAPI.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly BaseRepository<FavoriteRecipe> _favoriteRepo;

        public FavoriteService(BaseRepository<FavoriteRecipe> favoriteRepo)
        {
            _favoriteRepo = favoriteRepo;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, string recipeId)
        {
            var exists = await _favoriteRepo.FirstOrDefault(f => f.UserId == userId && f.RecipeId == recipeId);
            if (exists != null)
                return false; 

            
            await _favoriteRepo.AddAsync(new FavoriteRecipe
            {
                UserId = userId,
                RecipeId = recipeId
            });

            return true; 
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, string recipeId)
        {
            var favorite = await _favoriteRepo.FirstOrDefault(f => f.UserId == userId && f.RecipeId == recipeId);

            if (favorite == null)
                return false; 

            
            await _favoriteRepo.DeleteAsync(favorite);
            return true; 
        }

      
        public async Task<PagedResult<ResponseFavoriteDto>> GetUserFavoriteRecipesAsync(
      string userId,
      Expression<Func<Recipe, bool>> filter = null,
      int page = 1,
      int itemsPerPage = 10)
        {
            var query = _favoriteRepo.Query()
                .Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.Category)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient);

            // Преобразуваме filter от Recipe -> bool в FavoriteRecipe -> bool
            Expression<Func<FavoriteRecipe, bool>> favoriteFilter = null;
            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(FavoriteRecipe), "f");
                var body = Expression.Invoke(filter, Expression.PropertyOrField(parameter, "Recipe"));
                favoriteFilter = Expression.Lambda<Func<FavoriteRecipe, bool>>(body, parameter);
            }

            // Използваме BaseRepository
            var pagedResult = await _favoriteRepo.GetAllAsync(query, favoriteFilter, page, itemsPerPage);

            // Преобразуване към ResponseFavoriteDto
            var responseItems = pagedResult.Items.Select(fav => new ResponseFavoriteDto
            {
               // FavoritesId = fav.FavoriteRecipeId,
                Id = fav.Recipe.RecipeId,
                Name = fav.Recipe.Name,
                Description = fav.Recipe.Description,
                ImgUrl = fav.Recipe.ImgUrl,
                PreparationTime = fav.Recipe.PreparationTime,
                Calories = fav.Recipe.Calories,
                IsVegan = fav.Recipe.IsVegan,
                IsVegetarian = fav.Recipe.IsVegetarian,
                CategoryId = fav.Recipe.CategoryId,
                Category = new ResponseCategoryDto
                {
                    Id = fav.Recipe.CategoryId,
                    Name = fav.Recipe.Category.Name,
                    Description = fav.Recipe.Category.Description,
                    ShortCode = fav.Recipe.Category.ShortCode,
                    IsSeasonal = fav.Recipe.Category.IsSeasonal,
                },
                RecipeIngredients = fav.Recipe.RecipeIngredients.Select(ri =>  new ResponseRecipeIngredientDto
                {
                    IngredientId = ri.IngredientId,
                    Name = ri.Ingredient.Name,
                    ImgUrl = ri.Ingredient.ImgUrl,
                    CaloriesPer100g = ri.Ingredient.CaloriesPer100g,
                    Unit = ri.Ingredient.Unit,
                    IsAllergen = ri.Ingredient.IsAllergen
                }).ToList()
            }).ToList();

            return new PagedResult<ResponseFavoriteDto>
            {
                Items = responseItems,
                TotalCount = pagedResult.TotalCount,
                Page = page,
                itemsPerPage = itemsPerPage,
                PagesCount = pagedResult.PagesCount
            };
        }

        public async Task<bool> IsRecipeFavoritedAsync(string userId, string recipeId)
        {
            var item = await _favoriteRepo.FirstOrDefault(f => f.UserId == userId && f.RecipeId == recipeId);

            return item != null; 
        }
    }

}
