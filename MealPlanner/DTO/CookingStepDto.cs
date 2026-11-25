namespace MealPlanner.Dto
{
    public class CookingStepDto
    {
        public int Id { get; set; }
        public int StepNumber { get; set; }
        public string? Instruction { get; set; }
        public int TimingMinutes { get; set; }
        public int RecipeId { get; set; }
    }
}