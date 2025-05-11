using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.FavoriteRecipesDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace RecipeFinderAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly BaseRepository<User> _userRepository;

        public AccountService(BaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseAccountDto> GetUserByIdAsync(string loggedUserId)
        {
            User user = await _userRepository
             .Query()
             .Where(u => u.UserId == loggedUserId)
             .Include(u => u.FavoriteRecipe)
             .ThenInclude(fr => fr.Recipe)
             .ThenInclude(r => r.Category)
             .FirstOrDefaultAsync();

            ResponseAccountDto responseAccountDto = new ResponseAccountDto();

            if (user != null)
            {
                responseAccountDto = new ResponseAccountDto()
                {
                    Id = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt.ToLongTimeString(),
                    favoriteRecipes = user.FavoriteRecipe.Select(x => new ResponseFavoriteRecipeDto
                    {
                        Id = x.RecipeId,
                        RecipeId = x.RecipeId,
                        Name = x.Recipe.Name,
                        Description = x.Recipe.Description,
                        PreparationTime = x.Recipe.PreparationTime,
                        Calories = x.Recipe.Calories,
                        IsVegan = x.Recipe.IsVegan,
                        IsVegetarian = x.Recipe.IsVegetarian,
                        CategoryId = x.Recipe.CategoryId,
                        CategoryName = x.Recipe.Category.Name
                    }).ToList()
                };
            }
            return responseAccountDto;
        }

        public async Task<ResponseAccountDto> UpdateUserAsync(UpdateAccountDto updateAccountDto)
        {
            User user = await _userRepository
            .Query()
            .Where(u => u.UserId == updateAccountDto.Id)
            .Include(u => u.FavoriteRecipe)
            .ThenInclude(fr => fr.Recipe)
            .ThenInclude(r => r.Category)
            .FirstOrDefaultAsync();

            user.Username = updateAccountDto.Username;

            await _userRepository.UpdateAsync(user);

            return new ResponseAccountDto()
            {
                Id = user.UserId,
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt.ToLongTimeString(),
                favoriteRecipes = user.FavoriteRecipe.Select(x => new ResponseFavoriteRecipeDto
                {
                    Id = x.RecipeId,
                    RecipeId = x.RecipeId,
                    Name = x.Recipe.Name,
                    Description = x.Recipe.Description,
                    PreparationTime = x.Recipe.PreparationTime,
                    Calories = x.Recipe.Calories,
                    IsVegan = x.Recipe.IsVegan,
                    IsVegetarian = x.Recipe.IsVegetarian,
                    CategoryId = x.Recipe.CategoryId,
                    CategoryName = x.Recipe.Category.Name
                }).ToList()
            };
        }
    }
}
