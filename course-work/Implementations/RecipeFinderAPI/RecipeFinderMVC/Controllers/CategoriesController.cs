using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.Models;
using RecipeFinderMVC.Models.Categories;
using RecipeFinderMVC.Models.Users;

namespace RecipeFinderMVC.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class CategoriesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }


        [HttpGet]
        public async Task<IActionResult> Index(IndexCategoriesModel model)
        {
            model.Pager ??= new PagerModel();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;



            var query = $"categories?Name={model.Name}&IsSeasonal={model.IsSeasonal}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ErrorMessage", "Unable to load categories");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultModel<IndexCategoryModel>>();

            model.Items = data.Items;
            model.Pager.TotalCount = data.TotalCount;
            model.Pager.PagesCount = data.PagesCount;
            model.Pager.Page = data.Page;
            model.Pager.ItemsPerPage = data.itemsPerPage;
            model.Name = model.Name;
            model.IsSeasonal = model.IsSeasonal;

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("categories", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("Existing Category", "Unable to create category. Already exist category with this code. Short code should be unique");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<IndexCategoryModel>();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _httpClient.GetAsync($"categories/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var category = await response.Content.ReadFromJsonAsync<EditCategoryModel>();
            if (category == null)
                return NotFound();

            return View(category);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Изпращаме актуализирания модел към API-то
            var response = await _httpClient.PutAsJsonAsync($"categories/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("Existing Category", "Unable to create category. Already exist category with this code. Short code should be unique");
                return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"categories/{id}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["ErrorMessage"] = "Unable to delete category. Category is used in a recipe.";
                    return RedirectToAction("Index");
                }
            }
                return RedirectToAction("Index");
        }
    }
}
