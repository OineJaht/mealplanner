using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class MealPlanItemUpdateDto
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public int RecipeId { get; set; }
        public DateTime PlannedDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? MealType { get; set; }
        
        [Range(1, 100)]
        public int Servings { get; set; }
    }
}