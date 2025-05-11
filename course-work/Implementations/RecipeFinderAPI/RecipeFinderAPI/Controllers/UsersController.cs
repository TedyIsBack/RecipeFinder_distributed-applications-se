using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.Reflection.Metadata;
using System.Security.Claims;

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
        public async Task<IEnumerable<ResponseUserDto>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            return await _userService.GetAllUsersAsync();
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
