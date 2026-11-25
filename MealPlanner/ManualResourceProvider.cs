using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace MealPlanner
{
    public class ManualStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resources;
        
        public ManualStringLocalizer()
        {
            _resources = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            
            // Add English resources
            var enResources = new ConcurrentDictionary<string, string>();
            AddEnglishResources(enResources);
            _resources["en"] = enResources;
            
            // Add Russian resources
            var ruResources = new ConcurrentDictionary<string, string>();
            AddRussianResources(ruResources);
            _resources["ru"] = ruResources;
        }
        
        private void AddEnglishResources(ConcurrentDictionary<string, string> resources)
        {
            resources["Ingredients"] = "Ingredients";
            resources["CreateNew"] = "Create New";
            resources["SearchIngredientsPlaceholder"] = "Search ingredients...";
            resources["Search"] = "Search";
            resources["AllCategories"] = "All Categories";
            resources["Name"] = "Name";
            resources["Category"] = "Category";
            resources["Nutrition"] = "Nutrition";
            resources["Actions"] = "Actions";
            resources["Available"] = "Available";
            resources["NotAvailable"] = "Not Available";
            resources["Edit"] = "Edit";
            resources["Details"] = "Details";
            resources["Delete"] = "Delete";
            resources["AddNutrition"] = "Add Nutrition";
            resources["ViewNutrition"] = "View Nutrition";
            resources["NoIngredientsFound"] = "No ingredients found. Create your first ingredient to get started.";
            resources["Home"] = "Home";
            resources["Recipes"] = "Recipes";
            resources["MealPlanner"] = "Meal Planner";
            resources["Food"] = "Food";
            resources["WhatCanICook"] = "What Can I Cook?";
            resources["Planning"] = "Planning";
            resources["MealPlans"] = "Meal Plans";
            resources["Pantry"] = "Pantry";
            resources["ShoppingLists"] = "Shopping Lists";
            resources["Privacy"] = "Privacy";
            resources["AddIngredient"] = "Add Ingredient";
            resources["EditIngredient"] = "Edit Ingredient";
            resources["IngredientDetails"] = "Ingredient Details";
            resources["DeleteIngredient"] = "Delete Ingredient";
            resources["Save"] = "Save";
            resources["Cancel"] = "Cancel";
            resources["BackToList"] = "Back to List";
            
            // Model validation
            resources["NameRequired"] = "Name is required";
            resources["NameLength"] = "Name must be between 2 and 100 characters";
            resources["CategoryLength"] = "Category must be 50 characters or less";
        }
        
        private void AddRussianResources(ConcurrentDictionary<string, string> resources)
        {
            resources["Ingredients"] = "Ингредиенты";
            resources["CreateNew"] = "Создать новый";
            resources["SearchIngredientsPlaceholder"] = "Поиск ингредиентов...";
            resources["Search"] = "Поиск";
            resources["AllCategories"] = "Все категории";
            resources["Name"] = "Название";
            resources["Category"] = "Категория";
            resources["Nutrition"] = "Питательная ценность";
            resources["Actions"] = "Действия";
            resources["Available"] = "Доступно";
            resources["NotAvailable"] = "Недоступно";
            resources["Edit"] = "Редактировать";
            resources["Details"] = "Подробности";
            resources["Delete"] = "Удалить";
            resources["AddNutrition"] = "Добавить питательную ценность";
            resources["ViewNutrition"] = "Просмотр питательной ценности";
            resources["NoIngredientsFound"] = "Ингредиенты не найдены. Создайте свой первый ингредиент, чтобы начать.";
            resources["Home"] = "Главная";
            resources["Recipes"] = "Рецепты";
            resources["MealPlanner"] = "Планировщик питания";
            resources["Food"] = "Еда";
            resources["WhatCanICook"] = "Что я могу приготовить?";
            resources["Planning"] = "Планирование";
            resources["MealPlans"] = "Планы питания";
            resources["Pantry"] = "Кладовая";
            resources["ShoppingLists"] = "Списки покупок";
            resources["Privacy"] = "Конфиденциальность";
            resources["AddIngredient"] = "Добавить ингредиент";
            resources["EditIngredient"] = "Редактировать ингредиент";
            resources["IngredientDetails"] = "Детали ингредиента";
            resources["DeleteIngredient"] = "Удалить ингредиент";
            resources["Save"] = "Сохранить";
            resources["Cancel"] = "Отмена";
            resources["BackToList"] = "Вернуться к списку";
            
            // Model validation
            resources["NameRequired"] = "Необходимо указать название";
            resources["NameLength"] = "Название должно содержать от 2 до 100 символов";
            resources["CategoryLength"] = "Категория должна содержать не более 50 символов";
        }

        public LocalizedString this[string name]
        {
            get
            {
                var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
                
                if (_resources.TryGetValue(culture, out var cultureResources) && 
                    cultureResources.TryGetValue(name, out var value))
                {
                    return new LocalizedString(name, value);
                }
                
                // Try English as fallback
                if (culture != "en" && _resources.TryGetValue("en", out var enResources) && 
                    enResources.TryGetValue(name, out var enValue))
                {
                    return new LocalizedString(name, enValue);
                }
                
                return new LocalizedString(name, name, resourceNotFound: true);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var value = this[name];
                return value.ResourceNotFound
                    ? value
                    : new LocalizedString(name, string.Format(value.Value, arguments), false);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            
            if (_resources.TryGetValue(culture, out var cultureResources))
            {
                foreach (var pair in cultureResources)
                {
                    yield return new LocalizedString(pair.Key, pair.Value, false);
                }
            }
            
            // Include English resources if requested and current culture is not English
            if (includeParentCultures && culture != "en" && 
                _resources.TryGetValue("en", out var enResources))
            {
                foreach (var pair in enResources)
                {
                    if (!cultureResources?.ContainsKey(pair.Key) ?? false)
                    {
                        yield return new LocalizedString(pair.Key, pair.Value, false);
                    }
                }
            }
        }
    }

    public class ManualStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ManualStringLocalizer _localizer = new();
        
        public IStringLocalizer Create(Type resourceSource)
        {
            return _localizer;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return _localizer;
        }
    }
}