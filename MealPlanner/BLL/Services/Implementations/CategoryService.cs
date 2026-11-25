using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MealPlanner.BLL.DTO;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Category service implementation
    /// </summary>
    public class CategoryService : BaseService<DTO.Category, Models.Inventory.Category, int>, ICategoryService
    {
        /// <summary>
        /// Category repository
        /// </summary>
        private readonly ICategoryRepository _categoryRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper, unitOfWork.Categories)
        {
            _categoryRepository = unitOfWork.Categories;
        }

        /// <summary>
        /// Get all category names
        /// </summary>
        /// <returns>List of category names</returns>
        public async Task<List<string>> GetAllNamesAsync()
        {
            return await _categoryRepository.GetAllCategoryNamesAsync();
        }

        /// <summary>
        /// Get count of recipes in a category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Number of recipes in the category</returns>
        public async Task<int> GetRecipeCountAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryWithRecipesAsync(categoryId);
            if (category == null)
            {
                return 0;
            }
            
            return category.RecipeCategories?.Count ?? 0;
        }

        /// <summary>
        /// Get all recipes in a category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of recipes in the category</returns>
        public async Task<List<BLL.DTO.Recipe>> GetRecipesAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryWithRecipesAsync(categoryId);
            if (category == null)
            {
                return new List<BLL.DTO.Recipe>();
            }

            var recipes = category.RecipeCategories
                .Select(rc => rc.Recipe)
                .ToList();

            return recipes.Select(r => Mapper.Map<BLL.DTO.Recipe>(r)).ToList();
        }
    }
}