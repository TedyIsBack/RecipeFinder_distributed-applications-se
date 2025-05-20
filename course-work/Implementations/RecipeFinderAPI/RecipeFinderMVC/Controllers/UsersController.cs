using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using RecipeFinderMVC.VIewModels;
using RecipeFinderMVC.VIewModels.Users;
using System.Net.Http;
using System.Net.Http.Json;

namespace RecipeFinderMVC.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index(IndexUsersVM model)
        {
            model.Pager ??= new PagerVM();

            model.Pager.Page = model.Pager.Page == 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage == 0 ? 10 : model.Pager.ItemsPerPage;


            var query = $"Users?Username={model.Username}&IsActive={model.IsActive}&page={model.Pager.Page}&itemsPerPage={model.Pager.ItemsPerPage}";

            var response = await _httpClient.GetAsync(query);

            /*Console.WriteLine("BaseAddress: " + _httpClient.BaseAddress);
            Console.WriteLine("Response Url: " + response.RequestMessage.RequestUri);*/
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to load users.");
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResultVM<IndexUserVM>>();

            if (data == null)
            {
                ModelState.AddModelError(string.Empty, "No data.");
                return View(model);
            }

            model.Items = data.Items;
            model.Pager.TotalCount = data.TotalCount;
            model.Pager.PagesCount = data.PagesCount;
            model.Pager.Page = data.Page;
            model.Pager.ItemsPerPage = data.itemsPerPage;
            model.Username = model.Username;
            model.IsActive = model.IsActive;


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {

            var response = await _httpClient.GetAsync($"users/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var user = await response.Content.ReadFromJsonAsync<EditUserVM>();
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var updateDto = new UpdateUserDto
            {
                Username = model.Username,
                Role = model.Role
            };

            var response = await _httpClient.PutAsJsonAsync($"users/{model.Id}", updateDto);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
                //ModelState.AddModelError(string.Empty, "Error updating user.");
                //return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {

            var response = await _httpClient.DeleteAsync($"users/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            return RedirectToAction("Index");
        }
    }
}
