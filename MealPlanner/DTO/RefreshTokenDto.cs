using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class RefreshTokenDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
    
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
