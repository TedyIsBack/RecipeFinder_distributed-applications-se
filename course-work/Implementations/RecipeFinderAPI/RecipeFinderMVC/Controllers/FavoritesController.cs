using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.VIewModels.Favorites;
using System.Threading.Tasks;

namespace RecipeFinderMVC.Controllers
{
    [Authorize(Roles = Constants.UserRole)]
    public class FavoritesController : Controller
    {
        private readonly HttpClient _httpClient;

        public FavoritesController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("ApiClient");
        }
        [HttpPost]
        public async Task<IActionResult> Add(string id)
        {
            var loggedUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var response = await _httpClient.GetAsync($"favorites/{id}");

            var model = await response.Content.ReadFromJsonAsync<IndexFavoritesVM>();
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            return View();

        }
    }
}
