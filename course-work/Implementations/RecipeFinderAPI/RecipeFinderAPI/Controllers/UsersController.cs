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

        [HttpGet]
        public async Task<PagedResult<ResponseUserDto>> GetAllUsers(
            [FromQuery] string Username = null,
            [FromQuery] bool IsActive = true,
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            Expression<Func<User, bool>> filter = x =>
             (string.IsNullOrEmpty(Username) || x.Username.Contains(Username)) &&
             (x.IsActive == IsActive);
            return await _userService.GetAllUsersAsync(filter,page,itemsPerPage);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromQuery]string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id != updateUserDto.Id)
                return BadRequest("User ID mismatch.");

            var updatedUser = await _userService.UpdateUserAsync(updateUserDto);

            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            bool isDeleted = await _userService.DeleteUserAsync(id);

            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
