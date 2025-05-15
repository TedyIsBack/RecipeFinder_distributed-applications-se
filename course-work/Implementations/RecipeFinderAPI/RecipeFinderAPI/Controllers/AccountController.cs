using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Security.Claims;

namespace RecipeFinderAPI.Controllers
{
    /// <summary>
    /// Access to logged user account information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.UserRole)]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Logged user can check his/her information.
        /// </summary>
        /// <returns>User account data.</returns>
        /// <response code="200">Returns the user data</response>
        /// <response code="401">User is not authorized</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseAccountDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Logged user can update his/her information.
        /// </summary>
        /// <param name="updateAccountDto">Information that the logged user can update.</param>
        /// <returns>Updated user data.</returns>
        /// <response code="200">Returns the updated user data</response>
        /// <response code="401">User is not authorized</response>
        /// <response code="404">User not found</response>
        [HttpPut]
        [ProducesResponseType(typeof(UpdateAccountDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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
