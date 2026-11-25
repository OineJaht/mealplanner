// Services/IRoleSeederService.cs - UPDATED VERSION
using Microsoft.AspNetCore.Identity;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Services
{
    public interface IRoleSeederService
    {
        Task SeedRolesAsync();
        Task CreateDefaultAdminAsync();
    }

}