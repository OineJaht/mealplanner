using System.ComponentModel.DataAnnotations;
using MealPlanner.Models.Inventory;


namespace MealPlanner.Models.Inventory
{
    public class Ingredient
    {
        public Ingredient()
        {
            // Initialize collections in constructor, but NOT IngredientNutrition
            RecipeIngredients = new HashSet<RecipeIngredient>();
            UserPantryItems = new HashSet<UserPantryItem>();
            ShoppingListItems = new HashSet<ShoppingListItem>();
        }

        [Key] 
        public int Id { get; set; }

        [Required(ErrorMessage = "NameRequired")] 
        [StringLength(255, ErrorMessage = "NameLength")] 
        [Display(Name = "Name")]
        public string Name { get; set; } = String.Empty;

        [StringLength(255, ErrorMessage = "CategoryLength")] 
        [Display(Name = "Category")]
        public string Category { get; set; } = String.Empty;

        // Navigation properties
        public virtual IngredientNutrition? IngredientNutrition { get; set; } = null; // Make it nullable and initialize to null

        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }

        public virtual ICollection<UserPantryItem> UserPantryItems { get; set; }

        public virtual ICollection<ShoppingListItem> ShoppingListItems { get; set; }
    }
}