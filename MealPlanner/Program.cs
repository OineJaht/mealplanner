using System.Globalization;
using MealPlanner;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MealPlanner.Data;
using MealPlanner.Data.Repositories;
using MealPlanner.Data.UnitOfWork;

using System.Text;
using MealPlanner.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MealPlanner.MappingProfiles;
using MealPlanner.Security;
using AutoMapper;
using MealPlanner.Models.Inventory;
using MealPlanner.Services;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// CRITICAL: Add environment variables support
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register JWT service
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<MealPlanner.Services.IAdminPasswordResetService, MealPlanner.Services.AdminPasswordResetService>();

// Configure AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfiles).Assembly);

// Configure DbContext with Postgres
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => 
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        .SetPostgresVersion(new Version(13, 0))
        .EnableRetryOnFailure()));

// Configure PostgresSQL to handle DateTime correctly
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? 
                                       throw new InvalidOperationException("JWT Secret Key not configured"))),
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddPolicyScheme("AuthScheme", "JWT or Cookie", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            string? authorization = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return JwtBearerDefaults.AuthenticationScheme;
            
            if (context.Request.Cookies.TryGetValue("jwt_auth", out string? jwtCookie) && 
                !string.IsNullOrEmpty(jwtCookie))
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }
            
            if (context.Request.Method == "POST" && 
                context.Request.HasFormContentType &&
                context.Request.Form.TryGetValue("JwtToken", out var formToken) &&
                !string.IsNullOrEmpty(formToken))
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }
                
            return IdentityConstants.ApplicationScheme;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ModeratorOrAdmin", policy => 
        policy.RequireRole("Admin", "Moderator"));
    
    options.AddPolicy("PremiumOrAdmin", policy => 
        policy.RequireRole("Admin", "Premium", "Moderator"));
    
    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("CanModerateContent", policy =>
        policy.RequireRole("Admin", "Moderator"));
    
    options.AddPolicy("CanViewSystemStats", policy =>
        policy.RequireRole("Admin", "Moderator"));
    
    options.AddPolicy("CanManageAllRecipes", policy =>
        policy.RequireRole("Admin", "Moderator"));
    
    options.AddPolicy("CanAccessAdvancedFeatures", policy =>
        policy.RequireRole("Admin", "Premium"));
});

// Add role seeding service
builder.Services.AddScoped<IRoleSeederService, RoleSeederService>();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register the Unit of Work and repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meal Planner API",
        Version = "v1",
        Description = @"
            A comprehensive meal planning and nutrition tracking API that helps users:
            
            • Plan meals with recipes and ingredients
            • Track nutritional information
            • Manage pantry inventory  
            • Generate shopping lists automatically
            • Find recipes based on available ingredients
            
            **Authentication:** This API uses JWT Bearer tokens. Include your token in the Authorization header.
        ",
        Contact = new OpenApiContact
        {
            Name = "Konstantin Morševitski"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme. 
                       Enter 'Bearer' [space] and then your token in the text input below.
                       Example: 'Bearer 12345abcdef'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.DocInclusionPredicate((docName, description) => true);

    c.OrderActionsBy(apiDesc => 
        $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
});

// Add localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizerFactory, DebugStringLocalizerFactory>();

// Add MVC with localization support
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            if (type == typeof(Ingredient) || 
                type.FullName?.StartsWith("MealPlanner.Models.ViewModels.Ingredient") == true)
            {
                return factory.Create(typeof(MealPlanner.Resources.Models.Ingredient));
            }
            return factory.Create(typeof(MealPlanner.Resources.SharedResource));
        };
    });

// Configure supported cultures
var supportedCultures = new[] 
{ 
    new CultureInfo("en"),
    new CultureInfo("ru")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    
    options.RequestCultureProviders.Clear();
    options.AddInitialRequestCultureProvider(new CookieRequestCultureProvider
    {
        CookieName = CookieRequestCultureProvider.DefaultCookieName
    });
});

// Add API Versioning support
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new UrlSegmentApiVersionReader()
    );
}).AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Configure CORS using environment variables
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration["AllowedOrigins"] 
                             ?? "http://localhost:3000,http://localhost:8080,https://localhost:3000";
        
        var origins = allowedOrigins.Split(',').Select(o => o.Trim()).ToArray();
        
        policy.WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Register BLL services
builder.Services.AddScoped<MealPlanner.BLL.Services.IAppBLL, MealPlanner.BLL.Services.Implementations.AppBLL>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meal Planner API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Meal Planner API Documentation";
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
    c.EnableValidator();
    c.InjectStylesheet("/swagger-ui/custom.css");
    c.DefaultModelsExpandDepth(2);
    c.DefaultModelExpandDepth(2);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseStatusCodePagesWithReExecute("/Error/{0}");  
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        context.Database.Migrate();
        
        string[] roles = { "Admin", "Moderator", "Premium" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        var adminPassword = app.Configuration["ADMIN_PASSWORD"];
        var adminUser = await userManager.FindByEmailAsync("admin@mealplanner.com");
        if (adminUser == null && string.IsNullOrWhiteSpace(adminPassword))
        {
            services.GetRequiredService<ILogger<Program>>()
                .LogWarning("ADMIN_PASSWORD is not set; the admin user was not created.");
        }
        else if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@mealplanner.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                Created = DateTime.UtcNow
            };
            
            var result = await userManager.CreateAsync(adminUser, adminPassword!);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Admin user created successfully");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while setting up roles and admin user.");
    }
}

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

// JWT middleware setup
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        if (context.Request.Cookies.TryGetValue("jwt_auth", out string? jwtCookie) && 
            !string.IsNullOrEmpty(jwtCookie))
        {
            context.Request.Headers.Append("Authorization", "Bearer " + jwtCookie);
        }
    }
    
    if (context.Request.Method == "POST" && 
        context.Request.HasFormContentType &&
        context.Request.Form.TryGetValue("JwtToken", out var formToken) &&
        !string.IsNullOrEmpty(formToken) &&
        !context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Request.Headers.Append("Authorization", "Bearer " + formToken.ToString());
    }
    
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Admin}/{action=Dashboard}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Use CORS
app.UseCors();

app.Run();

public partial class Program { }