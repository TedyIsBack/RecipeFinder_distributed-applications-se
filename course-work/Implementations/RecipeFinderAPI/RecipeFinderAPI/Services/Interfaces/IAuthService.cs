using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AuthDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IAuthService
    {
        
        Task<User> ValidateUserCredentialsAsync(string username, string password);
        Task<ResponseUserDto> CreateUserAsync(RegisterDto registerDto);
    }
}
