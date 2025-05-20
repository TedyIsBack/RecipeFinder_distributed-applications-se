using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AuthDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;

namespace RecipeFinderAPI.Controllers
{
    /// <summary>
    /// Handling user authentication and registration.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDto">User registration data (e.g. email, password, etc.).</param>
        /// <returns>The created user or error.</returns>
        /// <response code="400">Invalid user data</response>
        /// <response code="404">User not found</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseUserDto), 200)]
        public async Task<IActionResult> PostAsync([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.CreateUserAsync(registerDto);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">User login data (username and password).</param>
        /// <returns>JWT token if valid login.</returns>
        /// <response code="400">Invalid login user data</response>
        /// <response code="404">User not found</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> PutAsync([FromBody] LoginDto loginDto, [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User loggedUser = await _authService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            if (loggedUser == null)
                return NotFound();

            string jwt = tokenService.GenerateToken(loggedUser.UserId, loggedUser.Role);

            return Ok(new { success = true, token = jwt });
        }
    }

}
