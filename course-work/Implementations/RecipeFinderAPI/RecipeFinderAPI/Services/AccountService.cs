using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace RecipeFinderAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly BaseRepository<User> _userRepository;

        public AccountService(BaseRepository<User> userRepository, IFavoriteService favoriteService)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseAccountDto> GetUserByIdAsync(string loggedUserId)
        {
           /* User user = await _userRepository
             .Query()
             .Where(u => u.UserId == loggedUserId)
             .Include(u => u.FavoriteRecipe)
             .ThenInclude(fr => fr.Recipe)
             .ThenInclude(r => r.Category)
             .FirstOrDefaultAsync();*/

            User user = await _userRepository.FirstOrDefault(u => u.UserId == loggedUserId);

            ResponseAccountDto responseAccountDto = new ResponseAccountDto();
           
            return new ResponseAccountDto()
            {
                Id = user.UserId,
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt.ToLongDateString() + " " + user.CreatedAt.ToLongTimeString(),
            };
        }

    

        public async Task<ResponseAccountDto> UpdateUserAsync(string id, UpdateAccountDto updateAccountDto)
        {
            /* User user = await _userRepository
             .Query()
             .Where(u => u.UserId == id)
             .Include(u => u.FavoriteRecipe)
             .ThenInclude(fr => fr.Recipe)
             .ThenInclude(r => r.Category)
             .FirstOrDefaultAsync();*/

            User user = await _userRepository.FirstOrDefault(u => u.UserId == id);

            user.Username = updateAccountDto.Username;

            await _userRepository.UpdateAsync(user);

            return new ResponseAccountDto()
            {
                Id = user.UserId,
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt.ToLongDateString() + " " + user.CreatedAt.ToLongTimeString(),
            };
        }
    }
}
