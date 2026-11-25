using System.Collections.Generic;

namespace MealPlanner.Public.DTO.v1
{
    /// <summary>
    /// View model for the Recipe Finder page
    /// </summary>
    public class RecipeFinderViewModel
    {
        /// <summary>
        /// List of ingredients in the user's pantry
        /// </summary>
        public List<Ingredient> PantryIngredients { get; set; } = new List<Ingredient>();
        
        /// <summary>
        /// List of all available ingredients
        /// </summary>
        public List<Ingredient> AllIngredients { get; set; } = new List<Ingredient>();
        
        /// <summary>
        /// IDs of ingredients selected by the user for recipe search
        /// </summary>
        public List<int> SelectedIngredientIds { get; set; } = new List<int>();
    }
}