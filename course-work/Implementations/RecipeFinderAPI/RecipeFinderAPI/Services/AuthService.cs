using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AuthDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;

namespace RecipeFinderAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly BaseRepository<User> _userRepository;
        public AuthService(BaseRepository<User> userRepository)
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
        public async Task<ResponseUserDto> CreateUserAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.FirstOrDefault(u => u.Email == registerDto.Email || u.Username == registerDto.Username);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email or username already exists.");
            }

            User user = new User()
            {
                Email = registerDto.Email,
                Username = registerDto.Username,
                Password = registerDto.Password,
                Role = Constants.UserRole,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

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
    }
   
}
