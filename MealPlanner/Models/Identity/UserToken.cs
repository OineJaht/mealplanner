using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealPlanner.Models.Inventory
{
    public class UserToken
    {
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
            
        [StringLength(255)]
        public string LoginProvider { get; set; } = string.Empty;
            
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
            
        [StringLength(255)]
        public string Value { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("UserId")] 
        public virtual ApplicationUser User { get; set; } = null!;
    }
}