using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class CookingStepCreateDto
    {
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string? Instruction { get; set; }
        
        public int TimingMinutes { get; set; }
    }
}