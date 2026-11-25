using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class JwtAccountController : Controller
    {
        [HttpGet]
        public IActionResult Profile()
        {
            // Forward to the Identity Account Management page
            return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
        }
        
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return RedirectToPage("/Account/Manage/ChangePassword", new { area = "Identity" });
        }
        
        [HttpGet]
        public IActionResult TwoFactorAuth()
        {
            return RedirectToPage("/Account/Manage/TwoFactorAuthentication", new { area = "Identity" });
        }
        
        [HttpGet]
        public IActionResult PersonalData()
        {
            return RedirectToPage("/Account/Manage/PersonalData", new { area = "Identity" });
        }
    }
}