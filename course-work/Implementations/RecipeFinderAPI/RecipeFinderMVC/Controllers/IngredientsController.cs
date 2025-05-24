using Microsoft.AspNetCore.Mvc;
using RecipeFinderMVC.Models.Ingredients;
using RecipeFinderMVC.Models;
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
        public async Task<IActionResult> Index(IndexIngredientsModel model)
        {
            model.Pager ??= new PagerModel();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"ingredients?Name={model.Name}&isAllergen={model.isAllergen}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load ingredients");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultModel<IndexIngredientModel>>();

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
        public async Task<IActionResult> Create(CreateIngredientModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                model.ImgUrl = "/images/" + fileName;
            }

            var response = await _httpClient.PostAsJsonAsync("ingredients", model);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict) // 409
                {
                    // Чети съобщението за грешка от API-то (JSON с { message = "..." })
                    var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (errorObj != null && errorObj.TryGetValue("message", out var errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Ingredient already exists.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to create ingredient.");
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _httpClient.GetAsync($"ingredients/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var ingredient = await response.Content.ReadFromJsonAsync<EditIngredientModel>();
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditIngredientModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                model.ImgUrl = "/images/" + fileName;
            }

            var response = await _httpClient.PutAsJsonAsync($"ingredients/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to update ingredient.");
                return View(model);
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"ingredients/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

                if (errorObj != null && errorObj.TryGetValue("message", out var errorMessage))
                {
                    TempData["ErrorMessage"] = errorMessage;
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to delete ingredient.";
                }

                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Ingredient not found.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
