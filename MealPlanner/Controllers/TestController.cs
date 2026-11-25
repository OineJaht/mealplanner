using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                message = "Authentication successful!", 
                username = User.Identity?.Name,
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            });
        }
    }
}