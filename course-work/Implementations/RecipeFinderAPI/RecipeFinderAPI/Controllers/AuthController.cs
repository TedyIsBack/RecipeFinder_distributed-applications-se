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
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseUserDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostAsync([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _authService.CreateUserAsync(registerDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">User login data (username and password).</param>
        /// <returns>JWT token if valid login.</returns>
        [HttpPut("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutAsync([FromBody] LoginDto loginDto, [FromServices] TokenService tokenService)
        {
            User loggedUser = await _authService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            if (loggedUser == null)
                return Unauthorized();

            string jwt = tokenService.GenerateToken(loggedUser.UserId, loggedUser.Role);

            return Ok(new { success = true, token = jwt });
        }
    }

}
