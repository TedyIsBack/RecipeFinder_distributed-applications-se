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
        /* private readonly RecipeFinderDbContext _context;
         private readonly DbSet<User> _users;*/

        private BaseRepository<User> _userRepository;
        public UserService(BaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> ValidateUserCredentialsAsync(string username, string password)
        {
            User user = await _userRepository.FirstOrDefault(u => u.Username == username && u.Password == password && u.IsActive);

           /* ResponseUserDto responseUserDto = new ResponseUserDto();

            if (user != null)
            {
                responseUserDto = new ResponseUserDto()
                {
                    Id = user.UserId,
                    Username = user.Username,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };
            }*/
            return user;
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
                    Username = user.Username,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };
            }
            return responseUserDto;
        }

        public async Task<ResponseUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            User user = new User()
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password,
                Role = createUserDto.Role,
                IsActive = createUserDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new ResponseUserDto()
            {
                Id = user.UserId,
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<ResponseUserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            User user = new User()
            {
                Username = updateUserDto.Username,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.UpdateAsync(user);

            return new ResponseUserDto()
            {
                Id = user.UserId,
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            User user = await _userRepository.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            user.IsActive = false;

            await _userRepository.UpdateAsync(user);

            return true;
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
                Username = x.Username,
                Role = x.Role,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            User user = await _userRepository.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}
