// MealPlanner.DAL.DTO/DalMappingProfile.cs
using AutoMapper;
using MealPlanner.DAL.DTO;
using MealPlanner.Models.Inventory;

namespace MealPlanner.DAL.DTO
{
    /// <summary>
    /// Mapping profile for DAL DTOs - excludes AppUser navigation properties
    /// </summary>
    public class DalMappingProfile : Profile
    {
        public DalMappingProfile()
        {
            // Recipe mappings - exclude User navigation property
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dest => dest.RecipeCategories, opt => opt.MapFrom(src => src.RecipeCategories))
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.CookingSteps, opt => opt.MapFrom(src => src.CookingSteps))
                .ForMember(dest => dest.RecipeNutrition, opt => opt.MapFrom(src => src.RecipeNutrition))
                .ForMember(dest => dest.MealPlanItems, opt => opt.MapFrom(src => src.MealPlanItems))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // Exclude User navigation

            // Recipe Category mappings
            CreateMap<RecipeCategory, RecipeCategoryDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore()); // Will be set by parent

            // Recipe Ingredient mappings
            CreateMap<RecipeIngredient, RecipeIngredientDto>()
                .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => src.Ingredient))
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore()); // Will be set by parent

            // Cooking Step mappings
            CreateMap<CookingStep, CookingStepDto>()
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore()); // Will be set by parent

            // Recipe Nutrition mappings
            CreateMap<RecipeNutrition, RecipeNutritionDto>()
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore()); // Will be set by parent

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.RecipeCategories, opt => opt.MapFrom(src => src.RecipeCategories))
                .ReverseMap();

            // Ingredient mappings
            CreateMap<Ingredient, IngredientDto>()
                .ForMember(dest => dest.IngredientNutrition, opt => opt.MapFrom(src => src.IngredientNutrition))
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.UserPantryItems, opt => opt.MapFrom(src => src.UserPantryItems))
                .ForMember(dest => dest.ShoppingListItems, opt => opt.MapFrom(src => src.ShoppingListItems))
                .ReverseMap();

            // Ingredient Nutrition mappings
            CreateMap<IngredientNutrition, IngredientNutritionDto>()
                .ReverseMap()
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore()); // Will be set by parent

            // Meal Plan mappings - exclude User navigation property
            CreateMap<MealPlan, MealPlanDto>()
                .ForMember(dest => dest.MealPlanItems, opt => opt.MapFrom(src => src.MealPlanItems))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // Exclude User navigation

            // Meal Plan Item mappings
            CreateMap<MealPlanItem, MealPlanItemDto>()
                .ForMember(dest => dest.Recipe, opt => opt.MapFrom(src => src.Recipe))
                .ReverseMap()
                .ForMember(dest => dest.MealPlan, opt => opt.Ignore()) // Will be set by parent
                .ForMember(dest => dest.Recipe, opt => opt.Ignore()); // Navigation property

            // Shopping List mappings - exclude User navigation property
            CreateMap<ShoppingList, ShoppingListDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // Exclude User navigation

            // Shopping List Item mappings
            CreateMap<ShoppingListItem, ShoppingListItemDto>()
                .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => src.Ingredient))
                .ReverseMap()
                .ForMember(dest => dest.ShoppingList, opt => opt.Ignore()) // Will be set by parent
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore()); // Navigation property

            // User Pantry Item mappings - exclude User navigation property
            CreateMap<UserPantryItem, UserPantryItemDto>()
                .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => src.Ingredient))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()) // Exclude User navigation
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore()); // Navigation property
        }
    }
}