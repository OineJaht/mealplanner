// MealPlanner/Public/DTO/v1/RecipeWithAvailability.cs
namespace MealPlanner.Public.DTO.v1
{
    /// <summary>
    /// Recipe with ingredient availability information
    /// </summary>
    public class RecipeWithAvailability
    {
        /// <summary>
        /// Recipe details
        /// </summary>
        public RecipeDetail? Recipe { get; set; }
        
        /// <summary>
        /// Recipe ingredients with availability flags
        /// </summary>
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}