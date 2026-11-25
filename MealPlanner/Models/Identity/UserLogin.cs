using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealPlanner.Models.Inventory
{
    public class UserLogin
    {
        [StringLength(255)]
        public string LoginProvider { get; set; } = String.Empty;
            
        [StringLength(255)]
        public string ProviderKey { get; set; } = String.Empty;
            
        [StringLength(255)]
        public string ProviderDisplayName { get; set; } = String.Empty;
            
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = String.Empty;

        // Navigation property
        [ForeignKey("UserId")] 
        public virtual ApplicationUser User { get; set; } = null!;
    }
}