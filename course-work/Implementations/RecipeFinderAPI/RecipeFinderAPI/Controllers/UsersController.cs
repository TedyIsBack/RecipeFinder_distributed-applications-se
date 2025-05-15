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
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="itemsPerPage">Number of items per page (default is 10).</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ResponseUserDto>), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        /// Update existing user details by id.
        /// </summary>
        /// <param name="id">id of existing user</param>
        /// <param name="updateUserDto">user data which admin can edit (username,role)</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        /// <summary>
        /// Delete existing user by id. Admin only.
        /// </summary>
        /// <param name="id">id of existing user</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            bool isDeleted = await _userService.DeleteUserAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
