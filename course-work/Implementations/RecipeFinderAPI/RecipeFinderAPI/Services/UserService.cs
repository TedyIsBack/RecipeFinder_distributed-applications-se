using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Data;
using RecipeFinderAPI.Entities;
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
        public UserService(BaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
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
                    CreatedAt = user.CreatedAt.ToLongTimeString()
                };
            }
            return responseUserDto;
        }

        public async Task<ResponseUserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.FirstOrDefault(u => u.UserId == updateUserDto.Id);


            user.Username = updateUserDto.Username;
            user.Role = updateUserDto.Role;

            await _userRepository.UpdateAsync(user);

            return new ResponseUserDto()
            {
                Id = user.UserId,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt.ToLongTimeString()
            };
        }

        public async Task<List<ResponseUserDto>> GetAllUsersAsync(
            Expression<Func<User, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10)
        {
            List<User> users = await _userRepository.GetAllAsync(filter);

            var pagedUsers = users
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage);

            return pagedUsers.Select(x => new ResponseUserDto
            {
                Id = x.UserId,
                Email = x.Email,
                Username = x.Username,
                Role = x.Role,
                CreatedAt = x.CreatedAt.ToLongTimeString()
            }).ToList();
        }


        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userRepository.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}
