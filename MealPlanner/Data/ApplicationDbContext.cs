using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MealPlanner.Models.ViewModels;
using System;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Recipe> Recipes { get; set; } = null!;
        public DbSet<RecipeCategory> RecipeCategories { get; set; } = null!;
        public DbSet<RecipeNutrition> RecipeNutritions { get; set; } = null!;
        public DbSet<CookingStep> CookingSteps { get; set; } = null!;
        public DbSet<Ingredient> Ingredients { get; set; } = null!;
        public DbSet<IngredientNutrition> IngredientNutritions { get; set; } = null!;
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;
        public DbSet<MealPlan> MealPlans { get; set; } = null!;
        public DbSet<MealPlanItem> MealPlanItems { get; set; } = null!;
        public DbSet<UserPantryItem> UserPantryItems { get; set; } = null!;
        public DbSet<ShoppingList> ShoppingLists { get; set; } = null!;
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; } = null!;
        public DbSet<UserLogin> UserLogins { get; set; } = null!;
        public DbSet<UserToken> UserTokens { get; set; } = null!;
        public DbSet<UserClaim> UserClaims { get; set; } = null!;
        public override int SaveChanges()
{
    ConvertDateTimePropertiesToUtc();
    return base.SaveChanges();
}

public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    ConvertDateTimePropertiesToUtc();
    return base.SaveChangesAsync(cancellationToken);
}

private void ConvertDateTimePropertiesToUtc()
{
    var entities = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

    foreach (var entity in entities)
    {
        foreach (var property in entity.Properties)
        {
            if (property.Metadata.ClrType == typeof(DateTime))
            {
                if (property.CurrentValue is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
                {
                    property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                }
            }
            else if (property.Metadata.ClrType == typeof(DateTime?))
            {
                if (property.CurrentValue != null)
                {
                    var nullableDate = (DateTime?)property.CurrentValue;
                    if (nullableDate.HasValue && nullableDate.Value.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(nullableDate.Value, DateTimeKind.Utc);
                    }
                }
            }
        }
    }
}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite keys for identity-related entities
            modelBuilder.Entity<UserLogin>()
                .HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            modelBuilder.Entity<UserToken>()
                .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            // Configure composite keys and relationships
            
            // RecipeCategory relationships
            modelBuilder.Entity<RecipeCategory>()
                .HasOne(rc => rc.Recipe)
                .WithMany(r => r.RecipeCategories)
                .HasForeignKey(rc => rc.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeCategory>()
                .HasOne(rc => rc.Category)
                .WithMany(c => c.RecipeCategories)
                .HasForeignKey(rc => rc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // RecipeNutrition relationship
            modelBuilder.Entity<RecipeNutrition>()
                .HasOne(rn => rn.Recipe)
                .WithOne(r => r.RecipeNutrition)
                .HasForeignKey<RecipeNutrition>(rn => rn.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // CookingStep relationship
            modelBuilder.Entity<CookingStep>()
                .HasOne(cs => cs.Recipe)
                .WithMany(r => r.CookingSteps)
                .HasForeignKey(cs => cs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // IngredientNutrition relationship
            modelBuilder.Entity<IngredientNutrition>()
                .HasOne(n => n.Ingredient)
                .WithOne(i => i.IngredientNutrition)
                .HasForeignKey<IngredientNutrition>(n => n.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // RecipeIngredient relationships
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // MealPlanItem relationships
            modelBuilder.Entity<MealPlanItem>()
                .HasOne(mpi => mpi.MealPlan)
                .WithMany(mp => mp.MealPlanItems)
                .HasForeignKey(mpi => mpi.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealPlanItem>()
                .HasOne(mpi => mpi.Recipe)
                .WithMany(r => r.MealPlanItems)
                .HasForeignKey(mpi => mpi.RecipeId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascading deletes from both sides

            // UserPantryItem relationships
            modelBuilder.Entity<UserPantryItem>()
                .HasOne(upi => upi.User)
                .WithMany(u => u.PantryItems)
                .HasForeignKey(upi => upi.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPantryItem>()
                .HasOne(upi => upi.Ingredient)
                .WithMany(i => i.UserPantryItems)
                .HasForeignKey(upi => upi.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoppingListItem relationships
            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.ShoppingList)
                .WithMany(sl => sl.Items)
                .HasForeignKey(sli => sli.ShoppingListId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.Ingredient)
                .WithMany(i => i.ShoppingListItems)
                .HasForeignKey(sli => sli.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // UserClaim relationship
            modelBuilder.Entity<UserClaim>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserClaims)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // UserLogin relationship
            modelBuilder.Entity<UserLogin>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLogins)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // UserToken relationship
            modelBuilder.Entity<UserToken>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTokens)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Add this to OnModelCreating in ApplicationDbContext.cs
// Set precision for decimal properties in IngredientNutrition
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.CaloriesPer100g).HasPrecision(10, 2);
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.ProteinG).HasPrecision(10, 2);
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.CarbsG).HasPrecision(10, 2);
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.FatG).HasPrecision(10, 2);
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.FiberG).HasPrecision(10, 2);
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.SugarG).HasPrecision(10, 2);
            modelBuilder.Entity<IngredientNutrition>()
                .Property(n => n.SodiumMg).HasPrecision(10, 2);
    
// Set precision for decimal properties in RecipeNutrition
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalCalories).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalProteinG).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalCarbsG).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalFatG).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalFiberG).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalSugarG).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.TotalSodiumG).HasPrecision(10, 2);
            modelBuilder.Entity<RecipeNutrition>()
                .Property(n => n.CaloriesPerServing).HasPrecision(10, 2);

// Set precision for other decimal properties
            modelBuilder.Entity<RecipeIngredient>()
                .Property(ri => ri.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<ShoppingListItem>()
                .Property(sli => sli.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<UserPantryItem>()
                .Property(upi => upi.AmountInStock).HasPrecision(10, 2);
        }
    }
}