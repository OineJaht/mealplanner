namespace MealPlanner.Dto
{
    public class UserPantryItemDto
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal AmountInStock { get; set; }
        public string? Unit { get; set; }
        public string? Location { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? IngredientCategory { get; set; }
        public IngredientNutritionDto Nutrition { get; set; } = new IngredientNutritionDto();
    }
}