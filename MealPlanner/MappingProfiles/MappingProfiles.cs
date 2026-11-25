// MappingProfiles.cs

using AutoMapper;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory;

namespace MealPlanner.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // =========================================================
            // Domain model <-> BLL DTO mappings
            // =========================================================

            // Category mappings
            CreateMap<Models.Inventory.Category, BLL.DTO.Category>().ReverseMap();

            // Category direct mappings (for recipe creation)
            CreateMap<Models.Inventory.Category, CategoryDto>().ReverseMap();

            // Recipe direct mappings (for meal plan selection)
            CreateMap<Models.Inventory.Recipe, RecipeDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Category)))
                .ReverseMap();

            // Ingredient direct mappings (for pantry ingredient names)
            CreateMap<Models.Inventory.Ingredient, IngredientDto>().ReverseMap();

            // IngredientNutrition direct mappings
            CreateMap<Models.Inventory.IngredientNutrition, IngredientNutritionDto>().ReverseMap();

            // Recipe mappings
            CreateMap<Models.Inventory.Recipe, BLL.DTO.Recipe>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Category)))
                .ReverseMap();

            CreateMap<Models.Inventory.Recipe, BLL.DTO.RecipeDetail>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Category)))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.CookingSteps, opt => opt.MapFrom(src => src.CookingSteps))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.RecipeNutrition))
                .ReverseMap();

            // RecipeCategory mappings
            CreateMap<Models.Inventory.RecipeCategory, BLL.DTO.RecipeCategory>().ReverseMap();

            // RecipeIngredient mappings
            CreateMap<Models.Inventory.RecipeIngredient, BLL.DTO.RecipeIngredient>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ReverseMap();

            // CookingStep mappings
            CreateMap<Models.Inventory.CookingStep, BLL.DTO.CookingStep>().ReverseMap();

            // RecipeNutrition mappings
            CreateMap<Models.Inventory.RecipeNutrition, BLL.DTO.RecipeNutrition>().ReverseMap();

            // Ingredient mappings
            CreateMap<Models.Inventory.Ingredient, BLL.DTO.Ingredient>().ReverseMap();
            CreateMap<Models.Inventory.Ingredient, BLL.DTO.IngredientDetail>()
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.IngredientNutrition))
                .ReverseMap();

            // IngredientNutrition mappings
            CreateMap<Models.Inventory.IngredientNutrition, BLL.DTO.IngredientNutrition>().ReverseMap();

            // MealPlan mappings
            CreateMap<Models.Inventory.MealPlan, BLL.DTO.MealPlan>().ReverseMap();
            CreateMap<Models.Inventory.MealPlan, BLL.DTO.MealPlanDetail>()
                .ForMember(dest => dest.MealPlanItems, opt => opt.MapFrom(src => src.MealPlanItems))
                .ReverseMap();

            // MealPlanItem mappings
            CreateMap<Models.Inventory.MealPlanItem, BLL.DTO.MealPlanItem>()
                .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src =>
                    src.Recipe != null ? src.Recipe.Name : "Unknown"))
                .ForMember(dest => dest.Recipe, opt => opt.MapFrom((src, dest, _, context) =>
                    src.Recipe != null
                        ? new BLL.DTO.RecipeWithNutrition
                        {
                            Id = src.Recipe.Id,
                            Name = src.Recipe.Name,
                            DefaultServings = src.Recipe.DefaultServings,
                            Nutrition = src.Recipe.RecipeNutrition != null
                                ? context.Mapper.Map<BLL.DTO.RecipeNutrition>(src.Recipe.RecipeNutrition)
                                : null
                        }
                        : null))
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());
            
            // ShoppingList mappings
            CreateMap<Models.Inventory.ShoppingList, BLL.DTO.ShoppingList>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap();

            CreateMap<Models.Inventory.ShoppingList, BLL.DTO.ShoppingListDetail>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap();

            CreateMap<Models.Inventory.ShoppingListItem, ShoppingListItemDto>()
                .ForMember(dest => dest.IngredientName,
                    opt => opt.MapFrom(src => src.Ingredient!.Name ?? "Unknown"))
                .ReverseMap();

            CreateMap<Models.Inventory.ShoppingList, ShoppingListDto>()
                .ForMember<List<ShoppingListItemDto>>(dest => dest.Items,
                    opt => opt.MapFrom(src => src.Items))
                .ReverseMap();

            // ShoppingListItem mappings
            CreateMap<Models.Inventory.ShoppingListItem, BLL.DTO.ShoppingListItem>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ReverseMap();
            // Add this mapping in the "BLL DTO <-> Controller DTO mappings" section
            CreateMap<BLL.DTO.ShoppingListItemDto, ShoppingListItemDto>().ReverseMap();

            // UserPantryItem mappings
            CreateMap<Models.Inventory.UserPantryItem, BLL.DTO.UserPantryItem>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ForMember(dest => dest.IngredientCategory, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Category : null))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.IngredientNutrition : null))
                .ReverseMap()
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore()) // IGNORE - don't create new ingredient
                .ForMember(dest => dest.User, opt => opt.Ignore()); // IGNORE - don't create new user

            CreateMap<Models.Inventory.UserPantryItem, UserPantryItemDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ForMember(dest => dest.IngredientCategory, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Category : null))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.IngredientNutrition : null));

            // =========================================================
            // BLL DTO <-> Controller DTO mappings
            // =========================================================

            // Category mappings
            CreateMap<BLL.DTO.Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryCreateDto, BLL.DTO.Category>();
            CreateMap<CategoryUpdateDto, BLL.DTO.Category>();
            CreateMap<BLL.DTO.Category, CategoryUpdateDto>();

            // Recipe mappings
            CreateMap<BLL.DTO.Recipe, RecipeDto>().ReverseMap();
            CreateMap<BLL.DTO.RecipeDetail, RecipeDetailDto>().ReverseMap();
            CreateMap<RecipeCreateDto, BLL.DTO.Recipe>();
            CreateMap<RecipeUpdateDto, BLL.DTO.Recipe>();
            CreateMap<BLL.DTO.Recipe, RecipeUpdateDto>();
            CreateMap<BLL.DTO.RecipeWithNutrition, RecipeWithNutritionDto>().ReverseMap();

            // RecipeIngredient mappings
            CreateMap<BLL.DTO.RecipeIngredient, RecipeIngredientDto>().ReverseMap();
            CreateMap<RecipeIngredientCreateDto, BLL.DTO.RecipeIngredient>();
            CreateMap<RecipeIngredientUpdateDto, BLL.DTO.RecipeIngredient>();

            // CookingStep mappings
            CreateMap<BLL.DTO.CookingStep, CookingStepDto>().ReverseMap();
            CreateMap<CookingStepCreateDto, BLL.DTO.CookingStep>();
            CreateMap<CookingStepUpdateDto, BLL.DTO.CookingStep>();

            // RecipeNutrition mappings
            CreateMap<BLL.DTO.RecipeNutrition, RecipeNutritionDto>().ReverseMap();
            CreateMap<RecipeNutritionUpdateDto, BLL.DTO.RecipeNutrition>();

            // Ingredient mappings
            CreateMap<BLL.DTO.Ingredient, IngredientDto>().ReverseMap();
            CreateMap<BLL.DTO.IngredientDetail, IngredientDetailDto>().ReverseMap();
            CreateMap<IngredientCreateDto, BLL.DTO.Ingredient>();
            CreateMap<IngredientUpdateDto, BLL.DTO.Ingredient>();
            CreateMap<BLL.DTO.Ingredient, IngredientUpdateDto>();

            // IngredientNutrition mappings
            CreateMap<BLL.DTO.IngredientNutrition, IngredientNutritionDto>().ReverseMap();
            CreateMap<IngredientNutritionCreateDto, BLL.DTO.IngredientNutrition>();
            CreateMap<IngredientNutritionUpdateDto, BLL.DTO.IngredientNutrition>();

            // MealPlan mappings
            CreateMap<BLL.DTO.MealPlan, MealPlanDto>().ReverseMap();
            CreateMap<BLL.DTO.MealPlanDetail, MealPlanDetailDto>().ReverseMap();
            CreateMap<MealPlanCreateDto, BLL.DTO.MealPlan>();
            CreateMap<MealPlanUpdateDto, BLL.DTO.MealPlan>();
            CreateMap<BLL.DTO.MealPlan, MealPlanUpdateDto>();
            CreateMap<Models.Inventory.MealPlan, MealPlanDto>().ReverseMap();

            // MealPlanItem mappings
            CreateMap<BLL.DTO.MealPlanItem, MealPlanItemDto>().ReverseMap();
            CreateMap<MealPlanItemCreateDto, BLL.DTO.MealPlanItem>();
            CreateMap<MealPlanItemUpdateDto, BLL.DTO.MealPlanItem>();

            // ShoppingList mappings
            CreateMap<BLL.DTO.ShoppingList, ShoppingListDto>().ReverseMap();
            CreateMap<BLL.DTO.ShoppingListDetail, ShoppingListDetailDto>().ReverseMap();
            CreateMap<ShoppingListCreateDto, BLL.DTO.ShoppingList>();
            CreateMap<ShoppingListUpdateDto, BLL.DTO.ShoppingList>();
            CreateMap<BLL.DTO.ShoppingList, ShoppingListUpdateDto>();

            // ShoppingListItem mappings
            CreateMap<BLL.DTO.ShoppingListItem, ShoppingListItemDto>().ReverseMap();
            CreateMap<ShoppingListItemCreateDto, BLL.DTO.ShoppingListItem>();
            CreateMap<ShoppingListItemUpdateDto, BLL.DTO.ShoppingListItem>();

            // UserPantryItem mappings
            CreateMap<Models.Inventory.UserPantryItem, UserPantryItemDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ForMember(dest => dest.IngredientCategory, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Category : null))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.IngredientNutrition : null));
            
            CreateMap<BLL.DTO.UserPantryItem, UserPantryItemDto>().ReverseMap();
            CreateMap<UserPantryItemCreateDto, BLL.DTO.UserPantryItem>();
            CreateMap<UserPantryItemUpdateDto, BLL.DTO.UserPantryItem>();
            CreateMap<BLL.DTO.UserPantryItem, UserPantryItemUpdateDto>();

            // =========================================================
            // Domain model <-> Controller DTO direct mappings
            // =========================================================
            // Note: These could be gradually removed as you transition to using BLL
            // exclusively, but are kept for backwards compatibility
            
            

            // Recipe direct mappings
            CreateMap<Models.Inventory.Recipe, RecipeDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Category)));

            CreateMap<Models.Inventory.Recipe, RecipeDetailDto>()
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.RecipeCategories));

            CreateMap<Models.Inventory.Recipe, RecipeWithNutritionDto>()
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.RecipeNutrition));

            CreateMap<RecipeCreateDto, Models.Inventory.Recipe>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeCategories, opt => opt.Ignore());

            CreateMap<RecipeUpdateDto, Models.Inventory.Recipe>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeCategories, opt => opt.Ignore());

            // Other direct mappings - these can be kept for existing controllers
            // until they're migrated to use BLL DTOs

            // RecipeIngredient direct mappings
            CreateMap<Models.Inventory.RecipeIngredient, RecipeIngredientDto>()
                .ForMember(dest => dest.IngredientName, opt =>
                    opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : "Unknown"));

            // Ingredient direct mappings
            CreateMap<Models.Inventory.Ingredient, IngredientDto>();
            CreateMap<Models.Inventory.Ingredient, IngredientDetailDto>();

            // And other direct mappings that are still needed for backwards compatibility
            // with controllers not yet migrated to use BLL services
            
            
             // =========================================================
            // Domain model <-> DAL DTO mappings (for repository layer)
            // =========================================================
            CreateMap<Recipe, DAL.DTO.RecipeDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // Exclude User navigation

            CreateMap<RecipeCategory, DAL.DTO.RecipeCategoryDto>()
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<RecipeIngredient, DAL.DTO.RecipeIngredientDto>()
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<CookingStep, DAL.DTO.CookingStepDto>()
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<RecipeNutrition, DAL.DTO.RecipeNutritionDto>()
                .ReverseMap()
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<Category, DAL.DTO.CategoryDto>().ReverseMap();

            CreateMap<Ingredient, DAL.DTO.IngredientDto>().ReverseMap();

            CreateMap<IngredientNutrition, DAL.DTO.IngredientNutritionDto>()
                .ReverseMap()
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore());

            CreateMap<MealPlan, DAL.DTO.MealPlanDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<MealPlanItem, DAL.DTO.MealPlanItemDto>()
                .ReverseMap()
                .ForMember(dest => dest.MealPlan, opt => opt.Ignore())
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<ShoppingList, DAL.DTO.ShoppingListDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<ShoppingListItem, DAL.DTO.ShoppingListItemDto>()
                .ReverseMap()
                .ForMember(dest => dest.ShoppingList, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore());

            CreateMap<UserPantryItem, DAL.DTO.UserPantryItemDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredient, opt => opt.Ignore());

            // =========================================================
            // DAL DTO <-> BLL DTO mappings (for service layer)
            // =========================================================
            
            // Recipe mappings
            CreateMap<DAL.DTO.RecipeDto, BLL.DTO.Recipe>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => 
                    src.RecipeCategories.Select(rc => rc.Category)))
                .ReverseMap()
                .ForMember(dest => dest.RecipeCategories, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.CookingSteps, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeNutrition, opt => opt.Ignore())
                .ForMember(dest => dest.MealPlanItems, opt => opt.Ignore());

            CreateMap<DAL.DTO.RecipeDto, BLL.DTO.RecipeDetail>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => 
                    src.RecipeCategories.Select(rc => rc.Category)))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.CookingSteps, opt => opt.MapFrom(src => src.CookingSteps))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.RecipeNutrition))
                .ReverseMap();

            // Category mappings
            CreateMap<DAL.DTO.CategoryDto, BLL.DTO.Category>().ReverseMap();

            // Ingredient mappings
            CreateMap<DAL.DTO.IngredientDto, BLL.DTO.Ingredient>().ReverseMap();
            CreateMap<DAL.DTO.IngredientDto, BLL.DTO.IngredientDetail>()
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.IngredientNutrition))
                .ReverseMap();

            // Recipe ingredient mappings
            CreateMap<DAL.DTO.RecipeIngredientDto, BLL.DTO.RecipeIngredient>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ReverseMap();

            // Cooking step mappings
            CreateMap<DAL.DTO.CookingStepDto, BLL.DTO.CookingStep>().ReverseMap();

            // Nutrition mappings
            CreateMap<DAL.DTO.RecipeNutritionDto, BLL.DTO.RecipeNutrition>().ReverseMap();
            CreateMap<DAL.DTO.IngredientNutritionDto, BLL.DTO.IngredientNutrition>().ReverseMap();

            // Meal plan mappings
            CreateMap<DAL.DTO.MealPlanDto, BLL.DTO.MealPlan>().ReverseMap();
            CreateMap<DAL.DTO.MealPlanDto, BLL.DTO.MealPlanDetail>()
                .ForMember(dest => dest.MealPlanItems, opt => opt.MapFrom(src => src.MealPlanItems))
                .ReverseMap();

            CreateMap<DAL.DTO.MealPlanItemDto, BLL.DTO.MealPlanItem>()
                .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src =>
                    src.Recipe != null ? src.Recipe.Name : "Unknown"))
                .ForMember(dest => dest.Recipe, opt => opt.MapFrom((src, dest, _, context) =>
                    src.Recipe != null
                        ? new BLL.DTO.RecipeWithNutrition
                        {
                            Id = src.Recipe.Id,
                            Name = src.Recipe.Name,
                            DefaultServings = src.Recipe.DefaultServings,
                            Nutrition = src.Recipe.RecipeNutrition != null
                                ? context.Mapper.Map<BLL.DTO.RecipeNutrition>(src.Recipe.RecipeNutrition)
                                : null
                        }
                        : null))
                .ReverseMap();

            // Shopping list mappings
            CreateMap<Models.Inventory.ShoppingList, BLL.DTO.ShoppingList>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap();

            CreateMap<Models.Inventory.ShoppingList, BLL.DTO.ShoppingListDetail>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap();

// ShoppingListItem mappings - map to BOTH classes
            CreateMap<Models.Inventory.ShoppingListItem, BLL.DTO.ShoppingListItem>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ReverseMap();

            CreateMap<Models.Inventory.ShoppingListItem, BLL.DTO.ShoppingListItemDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ReverseMap();
            
            CreateMap<BLL.DTO.ShoppingListDetail, Public.DTO.v1.ShoppingListDetail>()
                .IncludeBase<BLL.DTO.ShoppingList, Public.DTO.v1.ShoppingList>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count : 0))
                .ForMember(dest => dest.PurchasedItems, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count(i => i.Purchased) : 0));

            // User pantry mappings
            CreateMap<DAL.DTO.UserPantryItemDto, BLL.DTO.UserPantryItem>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Name : "Unknown"))
                .ForMember(dest => dest.IngredientCategory, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.Category : null))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src =>
                    src.Ingredient != null ? src.Ingredient.IngredientNutrition : null))
                .ReverseMap();
            
            
            CreateMap<Models.Inventory.Recipe, DAL.DTO.RecipeDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Models.Inventory.Ingredient, DAL.DTO.IngredientDto>().ReverseMap();
            CreateMap<Models.Inventory.Category, DAL.DTO.CategoryDto>().ReverseMap();
            CreateMap<Models.Inventory.MealPlan, DAL.DTO.MealPlanDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<Models.Inventory.ShoppingList, DAL.DTO.ShoppingListDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<Models.Inventory.UserPantryItem, DAL.DTO.UserPantryItemDto>()
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());
            
        }
        
        
    }
}