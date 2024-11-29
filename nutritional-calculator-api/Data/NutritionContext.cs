using Microsoft.EntityFrameworkCore;
using nutritional_calculator_api.Models;

namespace nutritional_calculator_api.Data;

public class NutritionContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Nutrient> Nutrients { get; set; }
    public DbSet<FoodNutrient> FoodNutrients { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Measure> Measures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .Navigation(c => c.Foods)
            .AutoInclude(false);

        modelBuilder.Entity<Food>()
            .Navigation(f => f.Nutrients)
            .AutoInclude(false);
    }
}