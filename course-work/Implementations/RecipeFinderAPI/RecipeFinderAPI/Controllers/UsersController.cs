using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Controllers
{
    /// <summary>
    /// Managing users. Admin only.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.AdminRole)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users. Supports filtering by username and active status.
        /// </summary>
        /// <param name="Username">Optional: Filter users by username (partial match).</param>
        /// <param name="IsActive">Optional: Filter users by active status (default is true).</param>
        /// <param name="page">Page number .</param>
        /// <param name="itemsPerPage">Number of items per page .</param>
        /// <response code="200">Returns all users</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ResponseUserDto>), 200)]
        public async Task<PagedResult<ResponseUserDto>> GetAllUsers(
            [FromQuery] string? Username = null,
            [FromQuery] bool IsActive = true,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            Expression<Func<User, bool>> filter = x =>
                (string.IsNullOrEmpty(Username) || x.Username.Contains(Username)) &&
                (x.IsActive == IsActive);
            return await _userService.GetAllUsersAsync(filter, page, itemsPerPage);
        }


        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="userId">id of existing user</param>
        /// <response code="200">Returns user</response>
        /// <response code="404">user with this id doesn't exist</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ResponseUserDto), 200)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Update existing user details by id.
        /// </summary>
        /// <param name="userId">id of existing user</param>
        /// <param name="updateUserDto">user data which admin can edit (username,role)</param>
        /// <response code="200">User is updates successfully</response>
        /// <response code="400">Invalid/missing user id or invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        /// <response code="404">User with this id doesn't exist</response>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(ResponseUserDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);

            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }



        /// <summary>
        /// Delete existing user by id. Admin only.
        /// </summary>
        /// <param name="userId">id of existing user</param>
        /// <response code="400">Invalid/missing user id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden. Only admin can perform this action.</response>
        /// <response code="404">User with this user doesn't exist</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            bool isDeleted = await _userService.DeleteUserAsync(userId);

            if (!isDeleted)
                return NotFound();

            return Ok();
        }
    }
}
