using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderMVC.VIewModels.Accounts;
using System.Threading.Tasks;

namespace RecipeFinderMVC.Controllers
{
    [Authorize(Roles =Constants.UserRole)]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index(IndexAccountVM model)
        {
            var response = await _httpClient.GetAsync("account");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("CustomError", "Failed to load this account.");
            }

            var account = await response.Content.ReadFromJsonAsync<IndexAccountVM>();

            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _httpClient.GetAsync($"account/{id}");

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("CustomError", "Failed to load this account.");
            }

            var account = await response.Content.ReadFromJsonAsync<EditAccountVM>();

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAccountVM model)
        {
            //var response = await _httpClient.PutAsJsonAsync($"account/{model.Id}",model);
            var response = await _httpClient.PutAsJsonAsync($"account", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("CustomError", "Failed to load this account.");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
