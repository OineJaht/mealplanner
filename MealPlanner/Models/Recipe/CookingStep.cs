using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealPlanner.Models.Inventory
{
    public class CookingStep
    {
        [Key]
        public int Id { get; set; }
        
        public int RecipeId { get; set; }
        
        public int StepNumber { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Instruction { get; set; } = String.Empty;
        
        public int TimingMinutes { get; set; }

        // Navigation property
        [ForeignKey("RecipeId")] public virtual Recipe Recipe { get; set; } = null!;
    }
}
