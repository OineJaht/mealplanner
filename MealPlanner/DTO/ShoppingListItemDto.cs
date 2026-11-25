namespace MealPlanner.Dto
{
    public class ShoppingListItemDto
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal Amount { get; set; }
        public bool Purchased { get; set; }
        
    }
}