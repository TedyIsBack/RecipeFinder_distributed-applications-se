using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure.DTOs;
using RecipeFinderAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeFinderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {

        private readonly IUserService _userService;

        public TokenController(IUserService userService)
        {
            this._userService = userService;
        }
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] LoginDto loginDto)
        {
            User loggedUser = await _userService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            if (loggedUser == null)
                return Unauthorized();

            var claims = new[]
               {
                    new Claim(ClaimTypes.NameIdentifier, loggedUser.UserId.ToString()),
                    new Claim(ClaimTypes.Role, loggedUser.Role)
                };
            /*  var claims = new[]
              {
                  new Claim("LoggedUserId", loggedUser.UserId.ToString()),
                  new Claim(ClaimTypes.Role, loggedUser.Role)
              };
  */
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHuykfVJNR6be0WxykqLXj+QxSTDlsFvleAXwPNL6pjFBdIM9r04IbXPjOXzcRo1"));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "meal.finder",
                "projectmanagement.aspnet.restapi",
                claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string jwt = tokenHandler.WriteToken(token);

            return Ok(new { success = true, token = jwt });
        }
    }
}
