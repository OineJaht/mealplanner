using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Models.Inventory
{
    public class MealPlanItem
    {
        [Key]
        public int Id { get; set; }
        
        public int MealPlanId { get; set; }
        
        public int RecipeId { get; set; }
        
        public DateTime PlannedDate { get; set; }

        [Required] [StringLength(50)] public string MealType { get; set; } = null!;
        
        public int Servings { get; set; }

        // Navigation properties
        [ForeignKey("MealPlanId")]
        public virtual MealPlan MealPlan { get; set; } = null!;
        
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; } = null!;
    }
}

