using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Data;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services
{
    public class UserService : IUserService
    {

        private readonly BaseRepository<User> _userRepository;
        private readonly BaseRepository<Recipe> _recipeRepository;
        public UserService(BaseRepository<User> userRepository, BaseRepository<Recipe> recipeRepository)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<ResponseUserDto> GetUserByIdAsync(string id)
        {
            User user = await _userRepository.FirstOrDefault(u => u.UserId == id);
            ResponseUserDto responseUserDto = new ResponseUserDto();

            if (user != null)
            {
                responseUserDto = new ResponseUserDto()
                {
                    Id = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt.ToLongDateString() + " " + user.CreatedAt.ToLongTimeString()
                };
            }
            return responseUserDto;
        }

        public async Task<ResponseUserDto> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.FirstOrDefault(u => u.UserId == id);


            user.Username = updateUserDto.Username;
            user.Role = updateUserDto.Role;

            await _userRepository.UpdateAsync(user);

            return new ResponseUserDto()
            {
                Id = user.UserId,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt.ToLongDateString() + " " + user.CreatedAt.ToLongTimeString()
            };
        }

        public async Task<PagedResult<ResponseUserDto>> GetAllUsersAsync(
            Expression<Func<User, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10)
        {
            var users = await _userRepository.GetAllAsync(_userRepository.Query(), filter, page, itemsPerPage);
            var responseUsers = users.Items.Select(u => new ResponseUserDto()
            {
                Id = u.UserId,
                Email = u.Email,
                Username = u.Username,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt.ToLongDateString() + " " + u.CreatedAt.ToLongTimeString()
            }).ToList();

            return new PagedResult<ResponseUserDto>()
            {
                Items = responseUsers,
                TotalCount = users.TotalCount,
                Page = users.Page,
                itemsPerPage = users.itemsPerPage,
                PagesCount = users.PagesCount
            };
        }


        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userRepository.FirstOrDefault(u => u.UserId == userId);


            if (user == null)
                return false;

            Expression<Func<Recipe, bool>> filter = x =>
                x.CreatedBy == userId;

            var recipes = await _recipeRepository
                .Query()
                .Where(filter)
                .ToListAsync();

            foreach (var recipe in recipes)
            {
                recipe.CreatedBy = Constants.UndefinedUserId;
            }

            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}
