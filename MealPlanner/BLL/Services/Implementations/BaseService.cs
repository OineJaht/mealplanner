// MealPlanner.BLL/Services/Implementations/BaseService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Base service implementation for all BLL services
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public abstract class BaseService<TDto, TEntity, TKey> : IBaseService<TDto, TKey>
        where TDto : class
        where TEntity : class
        where TKey : notnull
    {
        /// <summary>
        /// Unit of work
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Mapper
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Repository
        /// </summary>
        protected IRepository<TEntity> Repository { get; }

        /// <summary>
        /// Logger (optional - can be null for services that don't need logging)
        /// </summary>
        protected ILogger? Logger { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="repository">Repository</param>
        /// /// <param name="logger">Optional logger</param>
        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper, IRepository<TEntity> repository, ILogger? logger = null)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            Repository = repository;
            Logger = logger;
        }

        /// <summary>
        /// Apply user filter to query - override in derived classes if needed
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="userId">User ID</param>
        /// <returns>Filtered query</returns>
        protected virtual IQueryable<TEntity> ApplyUserFilter(IQueryable<TEntity> query, string? userId)
        {
            return query; // Default implementation does no filtering
        }

        /// <summary>
        /// Verify if user owns this entity - override in derived classes if needed
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if user owns this entity</returns>
        protected virtual bool VerifyOwnership(TEntity entity, string userId)
        {
            return true; // Default implementation allows all access
        }

        /// <summary>
        /// Set owner of entity - override in derived classes if needed
        /// </summary>
        /// <param name="entity">Entity to set owner</param>
        /// <param name="userId">User ID</param>
        protected virtual void SetOwner(TEntity entity, string userId)
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="userId">Optional user ID to filter by</param>
        /// <returns>List of entities</returns>
        public virtual async Task<List<TDto>> AllAsync(string? userId = null)
        {
            var query = Repository.GetAll();
            query = ApplyUserFilter(query, userId);
            
            var entities = await Repository.ToListAsync();
            return entities.Select(e => Mapper.Map<TDto>(e)).ToList();
        }

        /// <summary>
        /// Find entity by ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="userId">Optional user ID to verify ownership</param>
        /// <returns>Entity or null if not found</returns>
        public virtual async Task<TDto?> FindAsync(TKey id, string? userId = null)
        {
            // Convert TKey to int - this assumes all IDs are integers
            var intId = Convert.ToInt32(id);
            var entity = await Repository.GetByIdAsync(intId);
            
            if (entity == null)
            {
                return null;
            }

            // Verify ownership if userId provided
            if (userId != null && !VerifyOwnership(entity, userId))
            {
                return null;
            }

            return Mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="dto">DTO to add</param>
        /// <param name="userId">User ID who owns this entity</param>
        /// <returns>Added entity with ID</returns>
        // Update your BaseService.cs AddAsync method to be safer
        public virtual async Task<TDto> AddAsync(TDto dto, string userId)
        {
            // Map DTO to entity
            var entity = Mapper.Map<TEntity>(dto);
    
            // Set owner
            SetOwner(entity, userId);
    
            // CRITICAL: For UserPantryItem, ensure we don't have navigation properties set
            if (entity is Models.Inventory.UserPantryItem pantryItem)
            {
                // Ensure navigation properties are null to prevent EF from trying to insert them
                pantryItem.Ingredient = null;
                pantryItem.User = null;
        
                // Only the foreign key IDs should be set
                Logger?.LogInformation("Creating pantry item with IngredientId: {IngredientId}, UserId: {UserId}", 
                    pantryItem.IngredientId, pantryItem.UserId);
            }
    
            Repository.Add(entity);
            await UnitOfWork.SaveChangesAsync();
    
            return Mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Update existing entity
        /// </summary>
        /// <param name="dto">DTO with updated values</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated entity</returns>
        public virtual async Task<TDto> UpdateAsync(TDto dto, string userId)
        {
            // Get the ID from the DTO - assumes DTO has an Id property
            var idProperty = typeof(TDto).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"DTO {typeof(TDto).Name} must have an Id property");
            }

            var id = idProperty.GetValue(dto);
            if (id == null)
            {
                throw new ArgumentException("Entity ID cannot be null");
            }

            var intId = Convert.ToInt32(id);
            var entity = await Repository.GetByIdAsync(intId);
            
            if (entity == null)
            {
                throw new ArgumentException($"Entity with ID {id} not found");
            }

            // Verify ownership
            if (!VerifyOwnership(entity, userId))
            {
                throw new UnauthorizedAccessException($"User {userId} does not own entity with ID {id}");
            }

            Mapper.Map(dto, entity);
            Repository.Update(entity);
            await UnitOfWork.SaveChangesAsync();
            
            return Mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Remove entity by ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>True if entity was removed</returns>
        public virtual async Task<bool> RemoveAsync(TKey id, string userId)
        {
            var intId = Convert.ToInt32(id);
            var entity = await Repository.GetByIdAsync(intId);
            
            if (entity == null)
            {
                return false;
            }

            // Verify ownership
            if (!VerifyOwnership(entity, userId))
            {
                throw new UnauthorizedAccessException($"User {userId} does not own entity with ID {id}");
            }

            Repository.Remove(entity);
            await UnitOfWork.SaveChangesAsync();
            
            return true;
        }
    }
}