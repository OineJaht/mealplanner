using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Models.Inventory
{
    public class UserClaim
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string ClaimType { get; set; } = ClaimTypes.Name;
        
        [Required]
        [StringLength(255)]
        public string ClaimValue { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}