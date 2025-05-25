using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.Models.Recipes;
using RecipeFinderMVC.Models;
using RecipeFinderMVC.Models.Ingredients;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderMVC.Models.Categories;
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
        public async Task<IActionResult> Index(IndexRecipesModel model)
        {
            model.Pager ??= new PagerModel();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"recipes?Name={model.Name}&isVegan={model.isVegan}&isVegetarian={model.isVegetarian}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load recipes");
                return View(model);
            }



            var data = await response.Content.ReadFromJsonAsync<PagedResultModel<IndexRecipeModel>>();

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
        public async Task<IActionResult> MyRecipes(IndexRecipesModel model)
        {
            model.Pager ??= new PagerModel();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"recipes/created?Name={model.Name}&isVegan={model.isVegan}&isVegetarian={model.isVegetarian}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load recipes");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultModel<IndexRecipeModel>>();

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
            var recipe = await response.Content.ReadFromJsonAsync<IndexRecipeModel>();
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
            var responseCategories = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexCategoryModel>>($"categories?itemsPerPage={int.MaxValue}");
            var categories = responseCategories?.Items ?? new List<IndexCategoryModel>();

            var responseIngredients = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexIngredientModel>>($"ingredients?itemsPerPage={int.MaxValue}");
            var ingredients = responseIngredients?.Items ?? new List<IndexIngredientModel>();

            var model = new CreateRecipeModel
            {
                AvailableCategories = categories,
                AvailableIngredients = ingredients
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeModel model)
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
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexCategoryModel>>($"categories?itemsPerPage={int.MaxValue}");
                var categories = pagedCategories?.Items ?? new List<IndexCategoryModel>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexIngredientModel>>($"ingredients?itemsPerPage={int.MaxValue}");
                var ingredients = pagedIngredients?.Items ?? new List<IndexIngredientModel>();

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
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
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
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexCategoryModel>>($"categories?itemsPerPage={int.MaxValue}");
                var categories = pagedCategories?.Items ?? new List<IndexCategoryModel>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexIngredientModel>>($"ingredients?itemsPerPage={int.MaxValue}");
                var ingredients = pagedIngredients?.Items ?? new List<IndexIngredientModel>();

                model.AvailableCategories = categories;
                model.AvailableIngredients = ingredients;

                return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {

            var response = await _httpClient.GetAsync($"recipes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var recipe = await response.Content.ReadFromJsonAsync<EditRecipeModel>();


            var responseCheck = await _httpClient.GetAsync($"recipes/{id}");
            var recipeCheck = await responseCheck.Content.ReadFromJsonAsync<IndexRecipeModel>();

            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId != recipeCheck.CreatedBy)
                return Forbid();

            if (recipe == null)
                return NotFound();


            var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexCategoryModel>>($"categories?itemsPerPage={int.MaxValue}");
            recipe.AvailableCategories = pagedCategories?.Items ?? new List<IndexCategoryModel>();

            var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexIngredientModel>>($"ingredients?itemsPerPage={int.MaxValue}");
            recipe.AvailableIngredients = pagedIngredients?.Items ?? new List<IndexIngredientModel>();

            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRecipeModel model)
        {

            if (!ModelState.IsValid)
            {
                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexCategoryModel>>($"categories?itemsPerPage={int.MaxValue}");
                model.AvailableCategories = pagedCategories?.Items ?? new List<IndexCategoryModel>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexIngredientModel>>($"ingredients?itemsPerPage={int.MaxValue}");
                model.AvailableIngredients = pagedIngredients?.Items ?? new List<IndexIngredientModel>();

                return View(model);
            }

            // Ако има нова снимка - качва се локално
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

            var response = await _httpClient.PutAsync($"recipes/{model.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to update recipe.");

                var pagedCategories = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexCategoryModel>>($"categories?itemsPerPage={int.MaxValue}");
                model.AvailableCategories = pagedCategories?.Items ?? new List<IndexCategoryModel>();

                var pagedIngredients = await _httpClient.GetFromJsonAsync<PagedResultModel<IndexIngredientModel>>($"ingredients?itemsPerPage={int.MaxValue}");
                model.AvailableIngredients = pagedIngredients?.Items ?? new List<IndexIngredientModel>();

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
