using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class ShoppingListItemCreateDto
    {
        public int ShoppingListId { get; set; }
        public int IngredientId { get; set; }
        public decimal Amount { get; set; }
        public bool Purchased { get; set; } = false;
    }
}