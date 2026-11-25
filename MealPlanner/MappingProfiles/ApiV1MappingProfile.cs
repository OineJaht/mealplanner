// MealPlanner/MappingProfiles/ApiV1MappingProfile.cs
using AutoMapper;
using MealPlanner.BLL.DTO;
using MealPlanner.Public.DTO.v1;

namespace MealPlanner.MappingProfiles
{
    /// <summary>
    /// AutoMapper profile for mapping between BLL DTOs and Public API v1 DTOs
    /// </summary>
    public class ApiV1MappingProfile : Profile
    {
        public ApiV1MappingProfile()
        {
            // Category mappings
            CreateMap<BLL.DTO.Category, Public.DTO.v1.Category>();
            CreateMap<Public.DTO.v1.Category, BLL.DTO.Category>();

            // Ingredient mappings
            CreateMap<BLL.DTO.Ingredient, Public.DTO.v1.Ingredient>();
            CreateMap<Public.DTO.v1.Ingredient, BLL.DTO.Ingredient>();

            // Ingredient nutrition mappings
            CreateMap<BLL.DTO.IngredientNutrition, Public.DTO.v1.IngredientNutrition>();
            CreateMap<Public.DTO.v1.IngredientNutrition, BLL.DTO.IngredientNutrition>();

            // Ingredient detail mappings
            CreateMap<BLL.DTO.Ingredient, Public.DTO.v1.IngredientDetail>()
                .IncludeBase<BLL.DTO.Ingredient, Public.DTO.v1.Ingredient>();

            // Recipe mappings
            CreateMap<BLL.DTO.Recipe, Public.DTO.v1.Recipe>();
            CreateMap<Public.DTO.v1.Recipe, BLL.DTO.Recipe>();

            // Recipe detail mappings
            CreateMap<BLL.DTO.Recipe, Public.DTO.v1.RecipeDetail>()
                .IncludeBase<BLL.DTO.Recipe, Public.DTO.v1.Recipe>();

            // Recipe ingredient mappings
            CreateMap<BLL.DTO.RecipeIngredient, Public.DTO.v1.RecipeIngredient>();
            CreateMap<Public.DTO.v1.RecipeIngredient, BLL.DTO.RecipeIngredient>();

            // Cooking step mappings
            CreateMap<BLL.DTO.CookingStep, Public.DTO.v1.CookingStep>();
            CreateMap<Public.DTO.v1.CookingStep, BLL.DTO.CookingStep>();

            // Recipe nutrition mappings
            CreateMap<BLL.DTO.RecipeNutrition, Public.DTO.v1.RecipeNutrition>();
            CreateMap<Public.DTO.v1.RecipeNutrition, BLL.DTO.RecipeNutrition>();

            // Meal plan mappings
            CreateMap<BLL.DTO.MealPlan, Public.DTO.v1.MealPlan>();
            CreateMap<Public.DTO.v1.MealPlan, BLL.DTO.MealPlan>();
            
            CreateMap<BLL.DTO.MealPlanDetail, Public.DTO.v1.MealPlanDetail>()
                .IncludeBase<BLL.DTO.MealPlan, Public.DTO.v1.MealPlan>()
                .ForMember(dest => dest.MealPlanItems, opt => opt.MapFrom(src => src.MealPlanItems));
            
            CreateMap<BLL.DTO.MealPlanItem, Public.DTO.v1.MealPlanItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.RecipeId))
                .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.RecipeName))
                .ForMember(dest => dest.PlannedDate, opt => opt.MapFrom(src => src.PlannedDate))
                .ForMember(dest => dest.MealType, opt => opt.MapFrom(src => src.MealType))
                .ForMember(dest => dest.Servings, opt => opt.MapFrom(src => src.Servings));
            
            CreateMap<BLL.DTO.ShoppingListDetail, Public.DTO.v1.ShoppingListDetail>()
                .IncludeBase<BLL.DTO.ShoppingList, Public.DTO.v1.ShoppingList>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count : 0))
                .ForMember(dest => dest.PurchasedItems, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count(i => i.Purchased) : 0));

            CreateMap<Public.DTO.v1.ShoppingListDetail, BLL.DTO.ShoppingListDetail>()
                .IncludeBase<Public.DTO.v1.ShoppingList, BLL.DTO.ShoppingList>();


            // Meal plan detail mappings
            CreateMap<BLL.DTO.MealPlan, Public.DTO.v1.MealPlanDetail>()
                .IncludeBase<BLL.DTO.MealPlan, Public.DTO.v1.MealPlan>();

            // Meal plan item mappings
            CreateMap<BLL.DTO.MealPlanItem, Public.DTO.v1.MealPlanItem>();
            CreateMap<Public.DTO.v1.MealPlanItem, BLL.DTO.MealPlanItem>();
            
            

            // Shopping list mappings
            CreateMap<BLL.DTO.ShoppingList, Public.DTO.v1.ShoppingList>();
            CreateMap<Public.DTO.v1.ShoppingList, BLL.DTO.ShoppingList>();
            
            CreateMap<Models.Inventory.ShoppingList, BLL.DTO.ShoppingListDetail>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap();

            CreateMap<Models.Inventory.ShoppingListItem, BLL.DTO.ShoppingListItem>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ReverseMap();
            

            // Shopping list detail mappings
            CreateMap<BLL.DTO.ShoppingList, Public.DTO.v1.ShoppingListDetail>()
                .IncludeBase<BLL.DTO.ShoppingList, Public.DTO.v1.ShoppingList>();

            // Shopping list item mappings
            CreateMap<BLL.DTO.ShoppingListItem, Public.DTO.v1.ShoppingListItem>();
            CreateMap<Public.DTO.v1.ShoppingListItem, BLL.DTO.ShoppingListItem>();

            // User pantry item mappings
            CreateMap<BLL.DTO.UserPantryItem, Public.DTO.v1.UserPantryItem>();
            CreateMap<Public.DTO.v1.UserPantryItem, BLL.DTO.UserPantryItem>();

            // Recipe match mappings
            CreateMap<BLL.DTO.RecipeMatch, Public.DTO.v1.RecipeMatch>();
            CreateMap<Public.DTO.v1.RecipeMatch, BLL.DTO.RecipeMatch>();
            
            // Recipe detail mappings
            CreateMap<BLL.DTO.RecipeDetail, Public.DTO.v1.RecipeDetail>()
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.CookingSteps, opt => opt.MapFrom(src => src.CookingSteps))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .IncludeBase<BLL.DTO.Recipe, Public.DTO.v1.Recipe>();
        }
    }
}