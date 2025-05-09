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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = Constants.AdminRole)]
        [HttpGet]
        public async Task<IEnumerable<ResponseUserDto>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10)
        {
            return await _userService.GetAllUsersAsync();
        }

        [Auth]
        [HttpGet("account")]
        public async Task<IActionResult> GetLoggedUser()
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId  == null)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(loggedUserId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Roles = Constants.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
                return BadRequest("Invalid user data.");

            var createdUser = await _userService.CreateUserAsync(createUserDto);

            return CreatedAtAction(nameof(GetAllUsers),
                new { id = createdUser.Id },
                createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id != updateUserDto.Id)
                return BadRequest("User ID mismatch.");

            var updatedUser = await _userService.UpdateUserAsync(updateUserDto);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }
    }
}
