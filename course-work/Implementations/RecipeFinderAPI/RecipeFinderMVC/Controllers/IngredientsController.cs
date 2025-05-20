using Microsoft.AspNetCore.Mvc;
using RecipeFinderMVC.VIewModels.Ingredients;
using RecipeFinderMVC.VIewModels;
using Microsoft.AspNetCore.Authorization;
using RecipeFinderAPI.Common;

namespace RecipeFinderMVC.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class IngredientsController : Controller
    {
        private readonly HttpClient _httpClient;

        public IngredientsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        [HttpGet]
        public async Task<IActionResult> Index(IndexIngredientsVM model)
        {
            model.Pager ??= new PagerVM();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"ingredients?Name={model.Name}&isAllergen={model.isAllergen}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load ingredients");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultVM<IndexIngredientVM>>();

            model.Items = data.Items;
            model.Pager.TotalCount = data.TotalCount;
            model.Pager.PagesCount = data.PagesCount;
            model.Pager.Page = data.Page;
            model.Pager.ItemsPerPage = data.itemsPerPage;
            model.Name = model.Name;
            model.isAllergen = model.isAllergen;

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateIngredientVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("ingredients", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to create ingredients. Already exist");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<IndexIngredientVM>();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _httpClient.GetAsync($"ingredients/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var category = await response.Content.ReadFromJsonAsync<EditIngredientVM>();
            if (category == null)
                return NotFound();

            return View(category);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditIngredientVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Изпращаме актуализирания модел към API-то
            var response = await _httpClient.PutAsJsonAsync($"ingredients/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to update ingredients");
                return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"ingredients/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            return RedirectToAction("Index");
        }
    }
}
