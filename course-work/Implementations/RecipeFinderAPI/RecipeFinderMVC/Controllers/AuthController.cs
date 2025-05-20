using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderMVC.VIewModels.Auth;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace RecipeFinderMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly HttpClient _httpClient;

        public AuthController(ILogger<AuthController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("Auth/login", content);

            Console.WriteLine("Login response: " + response.Headers);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "User with this username/password doesn't exist");
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var token = jsonDoc.RootElement.GetProperty("token").GetString();

            HttpContext.Session.SetString("JWT", token);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = new List<Claim>();

            foreach (var claim in jwtToken.Claims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
            }

            var identity = new ClaimsIdentity(claims, "CookieLogin");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieLogin", principal);

            return RedirectToAction("Index", "Home");
        }




        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWT");
            HttpContext.Session.Remove("UserId");

            HttpContext.Session.Remove("Role");

            await HttpContext.SignOutAsync("CookieLogin");

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

    }
}
