using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.UserRole)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoggedUserInfo()
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();

            var user = await _accountService.GetUserByIdAsync(loggedUserId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLoggedUserInfo([FromBody] UpdateAccountDto updateAccountDto)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == null)
                return Unauthorized();


            var updatedUser = await _accountService.UpdateUserAsync(loggedUserId, updateAccountDto);

            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }
    }

}
