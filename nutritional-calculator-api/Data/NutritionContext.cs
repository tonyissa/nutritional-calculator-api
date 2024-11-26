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
}