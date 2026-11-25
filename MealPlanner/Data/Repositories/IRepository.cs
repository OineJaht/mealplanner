using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    /// <summary>
    /// Base repository interface for domain entities (keeping old interface for backward compatibility)
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // Read operations
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> expression);
        Task<T?> GetByIdAsync(int id);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> ToListAsync();
        Task<List<T>> ToListAsync(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);

        // Write operations
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        
        // Async Save
        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// New DTO-based repository interface
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface IDtoRepository<TDto, TKey> where TDto : class
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(TKey id);
        Task<TDto> AddAsync(TDto dto);
        Task<TDto> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(TKey id);
        Task<int> SaveChangesAsync();
    }
}