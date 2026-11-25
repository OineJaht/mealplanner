namespace MealPlanner.Dto
{
    public class RecipeIngredientUpdateDto
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}