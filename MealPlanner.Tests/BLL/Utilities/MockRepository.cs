// MealPlanner.Tests/Utilities/MockRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MealPlanner.Data.Repositories;

namespace MealPlanner.Tests.Utilities
{
    /// <summary>
    /// In-memory mock repository for testing
    /// </summary>
    public class MockRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _entities = new List<T>();
        private int _nextId = 1;

        public IQueryable<T> GetAll()
        {
            return _entities.AsQueryable();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _entities.AsQueryable().Where(expression);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null) return null;

            return await Task.FromResult(_entities.FirstOrDefault(e => 
                idProperty.GetValue(e)?.Equals(id) == true));
        }

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.FromResult(_entities.AsQueryable().SingleOrDefault(expression));
        }

        public async Task<List<T>> ToListAsync()
        {
            return await Task.FromResult(_entities.ToList());
        }

        public async Task<List<T>> ToListAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.FromResult(_entities.AsQueryable().Where(expression).ToList());
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.FromResult(_entities.AsQueryable().Any(expression));
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.FromResult(_entities.AsQueryable().Count(expression));
        }

        public void Add(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && idProperty.GetValue(entity)?.Equals(0) == true)
            {
                idProperty.SetValue(entity, _nextId++);
            }
            _entities.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public void Update(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null) return;

            var id = idProperty.GetValue(entity);
            var existingEntity = _entities.FirstOrDefault(e => 
                idProperty.GetValue(e)?.Equals(id) == true);

            if (existingEntity != null)
            {
                var index = _entities.IndexOf(existingEntity);
                _entities[index] = entity;
            }
        }

        public void Remove(T entity)
        {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _entities.Remove(entity);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Task.FromResult(1);
        }

        // Helper methods for testing
        public void Clear()
        {
            _entities.Clear();
            _nextId = 1;
        }

        public int Count => _entities.Count;

        public void Seed(IEnumerable<T> entities)
        {
            _entities.AddRange(entities);
        }
    }
}