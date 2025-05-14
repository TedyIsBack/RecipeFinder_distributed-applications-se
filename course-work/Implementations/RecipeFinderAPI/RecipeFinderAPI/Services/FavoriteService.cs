using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs;
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

        public async Task<bool> IsRecipeFavoritedAsync(string userId, string recipeId)
        {
            var favorite = await _favoriteRepo.FirstOrDefault(f => f.UserId == userId && f.RecipeId == recipeId);
            return favorite != null; 
        }

        public async Task<PagedResult<ResponseFavoriteDto>> GetUserFavoriteRecipesAsync(
       string userId,
       Expression<Func<Recipe, bool>> filter = null,
       int page = 1,
       int itemsPerPage = 10)
        {
            var favoriteQuery = _favoriteRepo.Query()
                .Where(x => x.UserId == userId)
                .Include(x => x.Recipe)
                    .ThenInclude(r => r.Category)
                .Include(x => x.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                .Select(x => new { x.FavoriteRecipeId, x.Recipe });

            if (filter != null)
            {
                favoriteQuery = favoriteQuery.Where(x => filter.Compile()(x.Recipe));
            }

            var totalCount = await favoriteQuery.CountAsync();
            var pagedFavorites = await favoriteQuery
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var responseItems = pagedFavorites.Select(fav => new ResponseFavoriteDto
            {
                Id = fav.FavoriteRecipeId, 
                RecipeId = fav.Recipe.RecipeId,
                Name = fav.Recipe.Name,
                Description = fav.Recipe.Description,
                ImgUrl = fav.Recipe.ImgUrl,
                PreparationTime = fav.Recipe.PreparationTime,
                Calories = fav.Recipe.Calories,
                IsVegan = fav.Recipe.IsVegan,
                IsVegetarian = fav.Recipe.IsVegetarian,
                CategoryId = fav.Recipe.CategoryId,
                Category =
                new ResponseCategoryDto
                {
                    Name = fav.Recipe.Category.Name,
                    Description = fav.Recipe.Category.Description,
                    ShortCode = fav.Recipe.Category.ShortCode,
                    IsSeasonal = fav.Recipe.Category.IsSeasonal,
                    CreatedAt = fav.Recipe.Category.CreatedAt.ToString("D")
                },
                RecipeIngredients = fav.Recipe.RecipeIngredients.Select(ri => new ResponseIngredientDto
                {
                    Id = ri.IngredientId,
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
                TotalCount = totalCount,
                Page = page,
                itemsPerPage = itemsPerPage
            };
        }


    }

}
