using CsvHelper.Configuration.Attributes;

namespace nutritional_calculator_api.Models;

public class Food
{
    [Index(0)]
    public int Id { get; set; }
    [Index(2)]
    public string Description { get; set; }
    [Ignore]
    public int Popularity { get; set; } = 0;
    [Index(3)]
    public int CategoryId { get; set; }
    [Ignore]
    public Category Category { get; set; }
    [Ignore]
    public List<FoodNutrient> Nutrients { get; set; } = new();
}