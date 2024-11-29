using CsvHelper.Configuration.Attributes;

namespace nutritional_calculator_api.Models;

public class Category
{
    [Index(0)]
    public int Id { get; set; }
    [Index(2)]
    public string Name { get; set; }
    [Index(3)]
    public int Importance { get; set; }
    [Ignore]
    public List<Food> Foods { get; set; } = new();
}