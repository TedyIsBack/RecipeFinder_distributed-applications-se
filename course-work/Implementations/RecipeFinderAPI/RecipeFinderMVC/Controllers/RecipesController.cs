using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.VIewModels.Recipes;
using RecipeFinderMVC.VIewModels;
using RecipeFinderMVC.VIewModels.Ingredients;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderMVC.VIewModels.Categories;
using System.Text;
using System.Text.Json;
using RecipeFinderAPI.Entities;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> MyRecipes(IndexRecipesVM model)
        {
            model.Pager ??= new PagerVM();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"recipes/created?Name={model.Name}&isVegan={model.isVegan}&isVegetarian={model.isVegetarian}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

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

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var response = await _httpClient.GetAsync($"recipes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var recipe = await response.Content.ReadFromJsonAsync<IndexRecipeVM>();
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Зареждаме категории и сътавки при отваряне на формата
            var responseCategories = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexCategoryVM>>($"categories?itemsPerPage={int.MaxValue}");
            var categories = responseCategories?.Items ?? new List<IndexCategoryVM>();

            var responseIngredients = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexIngredientVM>>($"ingredients?itemsPerPage={int.MaxValue}");
            var ingredients = responseIngredients?.Items ?? new List<IndexIngredientVM>();

            var model = new CreateRecipeVM
            {
                AvailableCategories = categories,
                AvailableIngredients = ingredients
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeVM model)
        {
            /*if (!model.RecipeIngredients.Any())
            {
                ModelState.AddModelError("RecipeIngredients", "Please select at least one ingredient.");
            }

            if (!model.AvailableCategories.Any())
            {
                ModelState.AddModelError("AvailableCategories", "Please select category.");
            }
*/
            if (!ModelState.IsValid)
            {
                // Зареждаме отново категориите и съставките при грешка
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexCategoryVM>>($"categories?itemsPerPage={int.MaxValue}");
                var categories = pagedCategories?.Items ?? new List<IndexCategoryVM>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexIngredientVM>>($"ingredients?itemsPerPage={int.MaxValue}");
                var ingredients = pagedIngredients?.Items ?? new List<IndexIngredientVM>();

                model.AvailableCategories = categories;
                model.AvailableIngredients = ingredients;

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

            var json = JsonSerializer.Serialize(model);

            // Създаваме съдържание с правилен Content-Type
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Изпращаме POST заявка
            var response = await _httpClient.PostAsync("recipes", content);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (errorObj != null && errorObj.TryGetValue("message", out var errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Recipe already exists.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to create recipe.");
                }

                // Зареждаме категориите и съставките отново при грешка
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexCategoryVM>>($"categories?itemsPerPage={int.MaxValue}");
                var categories = pagedCategories?.Items ?? new List<IndexCategoryVM>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexIngredientVM>>($"ingredients?itemsPerPage={int.MaxValue}");
                var ingredients = pagedIngredients?.Items ?? new List<IndexIngredientVM>();

                model.AvailableCategories = categories;
                model.AvailableIngredients = ingredients;

                return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            // Вземаме рецептата по id от API
            var response = await _httpClient.GetAsync($"recipes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var recipe = await response.Content.ReadFromJsonAsync<EditRecipeVM>();

            if (recipe == null)
            {
                return NotFound();
            }

            // Зареждаме наличните категории и съставки
            var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexCategoryVM>>($"categories?itemsPerPage={int.MaxValue}");
            recipe.AvailableCategories = pagedCategories?.Items ?? new List<IndexCategoryVM>();

            var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexIngredientVM>>($"ingredients?itemsPerPage={int.MaxValue}");
            recipe.AvailableIngredients = pagedIngredients?.Items ?? new List<IndexIngredientVM>();

            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRecipeVM model)
        {
            if (!ModelState.IsValid)
            {
                // При грешка презареждаме категориите и съставките
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexCategoryVM>>($"categories?itemsPerPage={int.MaxValue}");
                model.AvailableCategories = pagedCategories?.Items ?? new List<IndexCategoryVM>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexIngredientVM>>($"ingredients?itemsPerPage={int.MaxValue}");
                model.AvailableIngredients = pagedIngredients?.Items ?? new List<IndexIngredientVM>();

                return View(model);
            }

            // Ако има нова снимка - качваме я локално
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

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Изпращаме PUT заявка към API-то (предполагам, че API-то има PUT за обновяване)
            var response = await _httpClient.PutAsync($"recipes/{model.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to update recipe.");

                // При грешка презареждаме категориите и съставките
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexCategoryVM>>($"categories?itemsPerPage={int.MaxValue}");
                model.AvailableCategories = pagedCategories?.Items ?? new List<IndexCategoryVM>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultVM<IndexIngredientVM>>($"ingredients?itemsPerPage={int.MaxValue}");
                model.AvailableIngredients = pagedIngredients?.Items ?? new List<IndexIngredientVM>();

                return View(model);
            }

            return RedirectToAction("MyRecipes");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"recipes/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("CustomError", "Unable to delete recipe.");
                return RedirectToAction("MyRecipes");
            }


            return RedirectToAction("MyRecipes");
        }

    }
}
