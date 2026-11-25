using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Controllers
{
    public class JwtLoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}