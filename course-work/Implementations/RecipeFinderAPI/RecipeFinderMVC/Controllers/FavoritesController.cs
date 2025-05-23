using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.VIewModels;
using RecipeFinderMVC.VIewModels.Favorites;

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

        [HttpGet]
        public async Task<IActionResult> Index(IndexFavoritesVM model)
        {
            model.Pager ??= new PagerVM();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"favorites?Name={model.Name}&isVegan={model.isVegan}&isVegetarian={model.isVegetarian}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load user's favorite recipes");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultVM<IndexFavoriteVM>>();

            model.Items = data.Items;
            model.Pager.TotalCount = data.TotalCount;
            model.Pager.PagesCount = data.PagesCount;
            model.Pager.Page = data.Page;
            model.Pager.ItemsPerPage = data.itemsPerPage;
            model.Name = model.Name;
            model.isVegan = model.isVegan;
            model.isVegetarian = model.isVegetarian;


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string id)
        {

            var response = await _httpClient.PostAsync($"favorites/{id}",null);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }
            var model = await response.Content.ReadFromJsonAsync<IndexFavoriteVM>();

           // return RedirectToAction("Details", "Recipes", new { id });
           return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {
            var response = await _httpClient.DeleteAsync($"favorites/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }
            var model = await response.Content.ReadFromJsonAsync<IndexFavoriteVM>();

            //return RedirectToAction("Details", "Recipes", new { id });
            return RedirectToAction("Index");
        }
    }
}
