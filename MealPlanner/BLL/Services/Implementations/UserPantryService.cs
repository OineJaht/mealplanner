// MealPlanner/BLL/Services/Implementations/UserPantryService.cs
using AutoMapper;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;


namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// User pantry service implementation
    /// </summary>
    public class UserPantryService : BaseService<DTO.UserPantryItem, Models.Inventory.UserPantryItem, int>, IUserPantryService
    {
        /// <summary>
        /// User pantry repository
        /// </summary>
        private readonly IUserPantryRepository _userPantryRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public UserPantryService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper, unitOfWork.UserPantry)
        {
            _userPantryRepository = unitOfWork.UserPantry;
        }

        /// <summary>
        /// Apply user filter to query
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="userId">User ID</param>
        /// <returns>Filtered query</returns>
        protected override IQueryable<Models.Inventory.UserPantryItem> ApplyUserFilter(IQueryable<Models.Inventory.UserPantryItem> query, string? userId)
        {
            if (userId != null)
            {
                query = query.Where(p => p.UserId == userId);
            }
            return query;
        }

        /// <summary>
        /// Verify if user owns this entity
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if user owns this entity</returns>
        protected override bool VerifyOwnership(Models.Inventory.UserPantryItem entity, string userId)
        {
            return entity.UserId == userId;
        }

        /// <summary>
        /// Set owner of entity
        /// </summary>
        /// <param name="entity">Entity to set owner</param>
        /// <param name="userId">User ID</param>
        protected override void SetOwner(Models.Inventory.UserPantryItem entity, string userId)
        {
            entity.UserId = userId;
        }

        /// <summary>
        /// Get user pantry items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of pantry items</returns>
        public async Task<List<DTO.UserPantryItem>> GetUserItemsAsync(string userId)
        {
            var items = await _userPantryRepository.GetUserPantryItemsAsync(userId);
            return items.Select(i => Mapper.Map<DTO.UserPantryItem>(i)).ToList();
        }

        /// <summary>
        /// Get expiring items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="daysThreshold">Days until expiration</param>
        /// <returns>List of expiring items</returns>
        public async Task<List<DTO.UserPantryItem>> GetExpiringItemsAsync(string userId, int daysThreshold = 7)
        {
            var items = await _userPantryRepository.GetExpiringItemsAsync(userId, daysThreshold);
            return items.Select(i => Mapper.Map<DTO.UserPantryItem>(i)).ToList();
        }

        /// <summary>
        /// Get low stock items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="threshold">Amount threshold</param>
        /// <returns>List of low stock items</returns>
        public async Task<List<DTO.UserPantryItem>> GetLowStockItemsAsync(string userId, decimal threshold = 5)
        {
            var items = await _userPantryRepository.GetLowStockItemsAsync(userId, threshold);
            return items.Select(i => Mapper.Map<DTO.UserPantryItem>(i)).ToList();
        }

        /// <summary>
        /// Get all pantry ingredients for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of ingredients in pantry</returns>
        public async Task<List<DTO.Ingredient>> GetUserIngredientsAsync(string userId)
        {
            var ingredients = await _userPantryRepository.GetUserPantryIngredientsAsync(userId);
            return ingredients.Select(i => Mapper.Map<DTO.Ingredient>(i)).ToList();
        }
    }
}