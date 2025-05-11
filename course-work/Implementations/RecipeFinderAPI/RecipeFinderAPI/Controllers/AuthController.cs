using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs.AuthDTOs;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> PostAsync([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid client request");

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

        [HttpPut("login")]
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
