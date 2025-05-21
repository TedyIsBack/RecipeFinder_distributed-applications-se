using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.VIewModels.Recipes;
using RecipeFinderMVC.VIewModels;

namespace RecipeFinderMVC.Controllers
{
    [Authorize(Roles = Constants.UserRole)]
    public class RecipesController : Controller
    {
        private readonly HttpClient _httpClient;

        public RecipesController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index(IndexRecipesVM model)
        {
            model.Pager ??= new PagerVM();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"recipes?Name={model.Name}&isVegan={model.isVegan}&isVegetarian={model.isVegetarian}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load recipes");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultVM<IndexRecipeVM>>();

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

    }
}
