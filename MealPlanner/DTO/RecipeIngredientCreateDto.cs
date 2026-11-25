namespace MealPlanner.Dto
{
    public class RecipeIngredientCreateDto
    {
        public int IngredientId { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}