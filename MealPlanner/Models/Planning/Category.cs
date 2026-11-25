using System.ComponentModel.DataAnnotations;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Models.Inventory
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = String.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = String.Empty;

        // Navigation properties
        public virtual ICollection<RecipeCategory> RecipeCategories { get; set; } = new HashSet<RecipeCategory>();
    }
}
