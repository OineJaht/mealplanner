namespace MealPlanner.Dto
{
    public class RecipeDetailDto : RecipeDto
    {
        public string? Instructions { get; set; }
        public string? ImageUrl { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; } = new List<RecipeIngredientDto>();
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<CookingStepDto> CookingSteps { get; set; } = new List<CookingStepDto>();
        public RecipeNutritionDto? Nutrition { get; set; }
    }
}