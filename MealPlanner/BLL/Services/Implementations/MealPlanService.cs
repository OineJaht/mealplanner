// MealPlanner/BLL/Services/Implementations/MealPlanService.cs
using AutoMapper;
using MealPlanner.BLL.DTO;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;
using System.Linq;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Meal plan service implementation
    /// </summary>
    public class MealPlanService : BaseService<DTO.MealPlan, Models.Inventory.MealPlan, int>, IMealPlanService
    {
        /// <summary>
        /// Meal plan repository
        /// </summary>
        private readonly IMealPlanRepository _mealPlanRepository;
        
        /// <summary>
        /// Meal plan item repository
        /// </summary>
        private readonly IRepository<Models.Inventory.MealPlanItem> _mealPlanItemRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public MealPlanService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper, unitOfWork.MealPlans)
        {
            _mealPlanRepository = unitOfWork.MealPlans;
            _mealPlanItemRepository = unitOfWork.MealPlanItems;
        }

        /// <summary>
        /// Apply user filter to query
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="userId">User ID</param>
        /// <returns>Filtered query</returns>
        protected override IQueryable<Models.Inventory.MealPlan> ApplyUserFilter(IQueryable<Models.Inventory.MealPlan> query, string? userId)
        {
            if (userId != null)
            {
                query = query.Where(mp => mp.UserId == userId);
            }
            return query;
        }

        /// <summary>
        /// Verify if user owns this entity
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if user owns this entity</returns>
        protected override bool VerifyOwnership(Models.Inventory.MealPlan entity, string userId)
        {
            return entity.UserId == userId;
        }

        /// <summary>
        /// Set owner of entity
        /// </summary>
        /// <param name="entity">Entity to set owner</param>
        /// <param name="userId">User ID</param>
        protected override void SetOwner(Models.Inventory.MealPlan entity, string userId)
        {
            entity.UserId = userId;
        }

        /// <summary>
        /// Get meal plan item by ID with ownership verification
        /// </summary>
        public async Task<BLL.DTO.MealPlanItem?> GetMealPlanItemAsync(int itemId, string userId)
        {
            var mealPlanItem = await _mealPlanItemRepository.GetByIdAsync(itemId);
            if (mealPlanItem == null) return null;
    
            var mealPlan = await _mealPlanRepository.GetByIdAsync(mealPlanItem.MealPlanId);
            if (mealPlan == null || mealPlan.UserId != userId) return null;
    
            return Mapper.Map<BLL.DTO.MealPlanItem>(mealPlanItem);
        }

        /// <summary>
        /// Get meal plan that contains a specific item
        /// </summary>
        public async Task<BLL.DTO.MealPlan?> GetMealPlanByItemIdAsync(int itemId, string userId)
        {
            var mealPlanItem = await _mealPlanItemRepository.GetByIdAsync(itemId);
            if (mealPlanItem == null) return null;
    
            var mealPlan = await _mealPlanRepository.GetByIdAsync(mealPlanItem.MealPlanId);
            if (mealPlan == null || mealPlan.UserId != userId) return null;
    
            return Mapper.Map<BLL.DTO.MealPlan>(mealPlan);
        }

        /// <summary>
        /// Get meal plan with all items
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Meal plan with items or null</returns>
        public async Task<DTO.MealPlanDetail?> GetWithItemsAsync(int id, string userId)
        {
            Console.WriteLine($"BLL DEBUG: Getting meal plan {id} for user {userId}");
    
            var mealPlan = await _mealPlanRepository.GetMealPlanWithItemsAsync(id);
    
            if (mealPlan == null)
            {
                Console.WriteLine($"BLL DEBUG: Meal plan {id} not found");
                return null;
            }
    
            Console.WriteLine($"BLL DEBUG: Found meal plan, checking ownership. Plan UserId: {mealPlan.UserId}, Requested UserId: {userId}");
    
            // Verify ownership
            if (mealPlan.UserId != userId)
            {
                Console.WriteLine($"BLL DEBUG: Ownership check failed - returning null");
                return null;
            }
    
            Console.WriteLine($"BLL DEBUG: Before mapping - meal plan has {mealPlan.MealPlanItems?.Count ?? 0} items");
    
            var mapped = Mapper.Map<DTO.MealPlanDetail>(mealPlan);
    
            Console.WriteLine($"BLL DEBUG: After mapping - DTO has {mapped.MealPlanItems?.Count ?? 0} items");
    
            return mapped;
        }

        /// <summary>
        /// Get current meal plan for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Current meal plan or null</returns>
        public async Task<DTO.MealPlanDetail?> GetCurrentAsync(string userId)
        {
            var mealPlan = await _mealPlanRepository.GetCurrentMealPlanAsync(userId);
            
            if (mealPlan == null)
            {
                return null;
            }
            
            return Mapper.Map<DTO.MealPlanDetail>(mealPlan);
        }

        /// <summary>
        /// Add item to meal plan
        /// </summary>
        /// <param name="mealPlanId">Meal plan ID</param>
        /// <param name="item">Item to add</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Added item with ID</returns>
        public async Task<DTO.MealPlanItem> AddItemAsync(int mealPlanId, DTO.MealPlanItem item, string userId)
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(mealPlanId);
            
            if (mealPlan == null)
            {
                throw new ArgumentException($"Meal plan with ID {mealPlanId} not found");
            }
            
            // Verify ownership
            if (mealPlan.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own meal plan with ID {mealPlanId}");
            }
            
            var mealPlanItem = Mapper.Map<Models.Inventory.MealPlanItem>(item);
            mealPlanItem.MealPlanId = mealPlanId;
            
            
            UnitOfWork.MealPlanItems.Add(mealPlanItem);
            await UnitOfWork.SaveChangesAsync();
            
            return Mapper.Map<DTO.MealPlanItem>(mealPlanItem);
        }

        /// <summary>
        /// Update meal plan item
        /// </summary>
        /// <param name="item">Item with updated values</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated item</returns>
        public async Task<DTO.MealPlanItem> UpdateItemAsync(DTO.MealPlanItem item, string userId)
        {
            var mealPlanItem = await _mealPlanItemRepository.GetByIdAsync(item.Id);
            
            if (mealPlanItem == null)
            {
                throw new ArgumentException($"Meal plan item with ID {item.Id} not found");
            }
            
            var mealPlan = await _mealPlanRepository.GetByIdAsync(mealPlanItem.MealPlanId);
            
            if (mealPlan == null)
            {
                throw new ArgumentException($"Meal plan with ID {mealPlanItem.MealPlanId} not found");
            }
            
            // Verify ownership
            if (mealPlan.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own meal plan with ID {mealPlanItem.MealPlanId}");
            }
            
            Mapper.Map(item, mealPlanItem);
            _mealPlanItemRepository.Update(mealPlanItem);
            await UnitOfWork.SaveChangesAsync();
            
            return Mapper.Map<DTO.MealPlanItem>(mealPlanItem);
        }

        /// <summary>
        /// Remove meal plan item
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>True if item was removed</returns>
        public async Task<bool> RemoveItemAsync(int itemId, string userId)
        {
            var mealPlanItem = await _mealPlanItemRepository.GetByIdAsync(itemId);
            
            if (mealPlanItem == null)
            {
                return false;
            }
            
            var mealPlan = await _mealPlanRepository.GetByIdAsync(mealPlanItem.MealPlanId);
            
            if (mealPlan == null)
            {
                return false;
            }
            
            // Verify ownership
            if (mealPlan.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own meal plan with ID {mealPlanItem.MealPlanId}");
            }
            
            _mealPlanItemRepository.Remove(mealPlanItem);
            await UnitOfWork.SaveChangesAsync();
            
            return true;
        }
        
        /// <summary>
        /// Get nutrition summary for all meal plans
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of meal plans with nutrition data</returns>
        public async Task<List<BLL.DTO.MealPlanDetail>> GetNutritionSummaryAsync(string userId)
        {
            // Get all meal plans for user
            var mealPlans = await _mealPlanRepository.GetUserMealPlansAsync(userId);
            var mealPlanDetails = new List<BLL.DTO.MealPlanDetail>();
    
            // For each meal plan, load the detailed information including recipes with nutrition
            foreach (var mealPlan in mealPlans)
            {
                // Get meal plan with items
                var mealPlanWithItems = await _mealPlanRepository.GetMealPlanWithItemsAsync(mealPlan.Id);
                if (mealPlanWithItems != null)
                {
                    var mealPlanDetail = Mapper.Map<BLL.DTO.MealPlanDetail>(mealPlanWithItems);
            
                    // For each meal plan item, load the recipe with nutrition
                    foreach (var item in mealPlanDetail.MealPlanItems)
                    {
                        var recipe = await UnitOfWork.Recipes.GetRecipeWithDetailsAsync(item.RecipeId);
                        if (recipe != null)
                        {
                            // Create a simple recipe with nutrition object
                            item.Recipe = new BLL.DTO.RecipeWithNutrition
                            {
                                Id = recipe.Id,
                                Name = recipe.Name,
                                DefaultServings = recipe.DefaultServings,
                                Nutrition = recipe.RecipeNutrition != null ? 
                                    Mapper.Map<BLL.DTO.RecipeNutrition>(recipe.RecipeNutrition) : null
                            };
                        }
                    }
            
                    mealPlanDetails.Add(mealPlanDetail);
                }
            }
    
            return mealPlanDetails;
        }
    }
}