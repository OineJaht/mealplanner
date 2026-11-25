using System;
using System.Collections.Generic;
using MealPlanner.Models.Inventory;
using Microsoft.AspNetCore.Identity;

namespace MealPlanner.Models.Inventory
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow; // Changed from DateTime.Now to DateTime.UtcNow
        
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        
        // Navigation properties
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
        public virtual ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
        public virtual ICollection<UserPantryItem> PantryItems { get; set; } = new List<UserPantryItem>();
        public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
        public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
        public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
        
    }
}