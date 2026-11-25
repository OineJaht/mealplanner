namespace MealPlanner.Dto
{
    public class RecipeIngredientDto
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal Amount { get; set; }
        public string? Unit { get; set; }
        public bool IsAvailable { get; set; }
    }
}