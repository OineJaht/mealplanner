namespace MealPlanner.Dto
{
    public class MealPlanItemDto
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string? RecipeName { get; set; }
        public DateTime PlannedDate { get; set; }
        public string? MealType { get; set; }
        public int Servings { get; set; }
        
        // Add the Recipe property with full details including nutrition
        public RecipeWithNutritionDto? Recipe { get; set; }
    }
    
    // New DTO class specifically for recipes with nutrition data
    public class RecipeWithNutritionDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int DefaultServings { get; set; }
        public RecipeNutritionDto? Nutrition { get; set; }
    }
}