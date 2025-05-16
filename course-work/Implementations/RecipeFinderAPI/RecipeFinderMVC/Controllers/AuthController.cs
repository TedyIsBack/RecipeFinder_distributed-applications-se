using Microsoft.AspNetCore.Mvc;

namespace RecipeFinderMVC.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
