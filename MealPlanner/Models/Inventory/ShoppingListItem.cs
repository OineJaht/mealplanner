using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealPlanner.Models.Inventory
{
    public class ShoppingListItem
    {
        [Key]
        public int Id { get; set; }
        
        public int ShoppingListId { get; set; }
        
        public int IngredientId { get; set; }
        
        public decimal Amount { get; set; }
        
        public bool Purchased { get; set; }

        // Navigation properties
        [ForeignKey("ShoppingListId")] public virtual ShoppingList ShoppingList { get; set; } = null!;

        [ForeignKey("IngredientId")] public virtual Ingredient Ingredient { get; set; } = null!;
    }
}
