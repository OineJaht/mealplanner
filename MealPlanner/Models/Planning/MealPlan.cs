using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealPlanner.Models.Inventory;


namespace MealPlanner.Models.Inventory
{
    public class MealPlan
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")] 
        public virtual ApplicationUser User { get; set; } = null!;
        
        public virtual ICollection<MealPlanItem> MealPlanItems { get; set; } = new HashSet<MealPlanItem>();
    }
}
