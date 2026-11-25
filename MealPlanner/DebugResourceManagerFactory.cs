using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MealPlanner
{
    public class DebugStringLocalizer : IStringLocalizer
    {
        private readonly string _baseName;
        private readonly Assembly _assembly;
        private readonly ILogger _logger;
        private ResourceManager? _resourceManager;

        public DebugStringLocalizer(string baseName, Assembly assembly, ILogger logger)
        {
            _baseName = baseName;
            _assembly = assembly;
            _logger = logger;
    
            try
            {
                _resourceManager = new ResourceManager(baseName, assembly);
                // Commented out debug logs - uncomment if needed for debugging
                // _logger.LogInformation($"Successfully created ResourceManager for {baseName}");
        
                // Test both English and Russian resource loading
                // var testValueEn = _resourceManager.GetString("Ingredients", new CultureInfo("en"));
                // _logger.LogInformation($"Test fetch for 'Ingredients' in en: {testValueEn ?? "NULL"}");
        
                // var testValueRu = _resourceManager.GetString("Ingredients", new CultureInfo("ru"));
                // _logger.LogInformation($"Test fetch for 'Ingredients' in ru: {testValueRu ?? "NULL"}");
        
                // Also test with current UI culture
                // var testValue = _resourceManager.GetString("Ingredients", CultureInfo.CurrentUICulture);
                // _logger.LogInformation($"Test fetch for 'Ingredients' in {CultureInfo.CurrentUICulture.Name}: {testValue ?? "NULL"}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating ResourceManager for {baseName}: {ex.Message}");
                _resourceManager = null;
            }
        }

        public LocalizedString this[string name]
        {
            get
            {
                var currentCulture = CultureInfo.CurrentUICulture;
                // Changed to LogTrace to reduce noise
                _logger.LogTrace($"Getting string '{name}' for culture: {currentCulture.Name}");
                if (_resourceManager == null)
                {
                    _logger.LogWarning($"ResourceManager is null, returning name as value for {name}");
                    return new LocalizedString(name, name, resourceNotFound: true);
                }
                
                string? value = null;
                bool notFound = true;
                
                try
                {
                    value = _resourceManager.GetString(name, CultureInfo.CurrentUICulture);
                    notFound = value == null;
                    
                    // Changed to LogTrace to reduce noise
                    _logger.LogTrace($"GetString({name}, {CultureInfo.CurrentUICulture.Name}) returned: {value ?? "NULL"}, notFound: {notFound}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error getting string for {name}: {ex.Message}");
                }
                
                return new LocalizedString(name, value ?? name, notFound);
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
            if (_resourceManager == null)
            {
                _logger.LogWarning("ResourceManager is null, returning empty collection for GetAllStrings");
                return Enumerable.Empty<LocalizedString>();
            }
            
            var resourceSet = _resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (resourceSet == null)
            {
                _logger.LogWarning($"Could not get ResourceSet for culture {CultureInfo.CurrentUICulture.Name}");
                return Enumerable.Empty<LocalizedString>();
            }
            
            return resourceSet.Cast<System.Collections.DictionaryEntry>()
                .Select(e => new LocalizedString(
                    e.Key?.ToString() ?? string.Empty, 
                    e.Value?.ToString() ?? string.Empty, 
                    false))
                .ToList();
        }
    }

    public class DebugStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly string _resourcesPath;
        private readonly ILogger<DebugStringLocalizerFactory> _logger;
        private readonly Assembly _assembly;

        public DebugStringLocalizerFactory(
            IOptions<LocalizationOptions> localizationOptions,
            ILogger<DebugStringLocalizerFactory> logger)
        {
            _resourcesPath = localizationOptions.Value.ResourcesPath;
            _logger = logger;
            _assembly = Assembly.GetExecutingAssembly();
            
            // Commented out debug logs - uncomment if needed for debugging
            // Log available resources
            // var resourceNames = _assembly.GetManifestResourceNames();
            // _logger.LogInformation($"Available embedded resources in assembly {_assembly.FullName}:");
            // foreach (var name in resourceNames)
            // {
            //     _logger.LogInformation($"  {name}");
            // }
            
            // Log current culture
            // _logger.LogInformation($"Current culture: {CultureInfo.CurrentCulture.Name}");
            // _logger.LogInformation($"Current UI culture: {CultureInfo.CurrentUICulture.Name}");
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var baseName = GetResourceBaseName(resourceSource);
            // Changed to LogTrace to reduce noise
            _logger.LogTrace($"Create(Type): Creating localizer for {baseName}");
            return new DebugStringLocalizer(baseName, _assembly, _logger);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var fullBaseName = location.IsNullOrEmpty() ? baseName : $"{NormalizeName(location)}.{baseName}";
            // Changed to LogTrace to reduce noise
            _logger.LogTrace($"Create(string): Creating localizer for {fullBaseName}");
            return new DebugStringLocalizer(fullBaseName, _assembly, _logger);
        }

        private string GetResourceBaseName(Type resourceSource)
        {
            var typeName = resourceSource.Name;
    
            if (resourceSource.IsNested && resourceSource.DeclaringType != null)
            {
                typeName = $"{resourceSource.DeclaringType.Name}.{typeName}";
            }
    
            var assemblyName = resourceSource.Assembly.GetName().Name ?? string.Empty;
            var namespaceName = resourceSource.Namespace ?? string.Empty;
    
            // Return directly what we know works for your specific case
            if (resourceSource.FullName == "MealPlanner.Resources.SharedResource")
            {
                return "MealPlanner.Resources.SharedResource";
            }
    
            return $"{namespaceName}.{typeName}";
        }
        
        private string TryFixResourcePath(string resourcePath, string resourcesPath)
        {
            var normalizedPath = resourcesPath.Replace(Path.DirectorySeparatorChar, '.')
                                             .Replace(Path.AltDirectorySeparatorChar, '.');
            
            // Handle the case where the resource path already contains the resources path
            if (resourcePath.Contains(normalizedPath, StringComparison.OrdinalIgnoreCase))
            {
                return resourcePath;
            }
            
            // Replace the resource path with the resources path if it's already there
            var segmentsToReplace = normalizedPath.Split('.');
            var resourceSegments = resourcePath.Split('.');
            
            var segmentsToKeep = resourceSegments.Except(segmentsToReplace, 
                StringComparer.OrdinalIgnoreCase).ToList();
            
            return string.Join('.', segmentsToKeep);
        }
        
        private static string NormalizeName(string name)
        {
            return name.Replace(Path.DirectorySeparatorChar, '.')
                       .Replace(Path.AltDirectorySeparatorChar, '.');
        }
    }
    
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}