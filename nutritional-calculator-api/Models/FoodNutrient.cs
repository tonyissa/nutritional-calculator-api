using CsvHelper.Configuration.Attributes;

namespace nutritional_calculator_api.Models;

public class FoodNutrient
{
    [Index(0)]
    public int Id { get; set; }
    [Index(1)]
    public int FoodId { get; set; }
    [Index(2)]
    public int NutrientId { get; set; }
    [Index(3)]
    public float Amount { get; set; }
    [Ignore]
    public Food Food { get; set; }
    [Ignore]
    public Nutrient Nutrient { get; set; }
}