using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseAccountDto> GetUserByIdAsync(string loggedUserId);
        Task<ResponseAccountDto> UpdateUserAsync(UpdateAccountDto updateAccountDto);
    }
}
