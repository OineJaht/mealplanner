using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Models.Inventory
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required] 
        [StringLength(255)] 
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Instructions { get; set; } = string.Empty;
        
        public int DefaultServings { get; set; }
        
        public int PrepTimeMinutes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        
        public bool IsFavorite { get; set; }

        // Navigation properties
        [ForeignKey("UserId")] 
        public virtual ApplicationUser User { get; set; } = null!;
        
        public virtual RecipeNutrition? RecipeNutrition { get; set; }
        
        public virtual ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
        
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        
        public virtual ICollection<CookingStep> CookingSteps { get; set; } = new List<CookingStep>();
        
        public virtual ICollection<MealPlanItem> MealPlanItems { get; set; } = new List<MealPlanItem>();
    }
}

