using CsvHelper.Configuration.Attributes;

namespace nutritional_calculator_api.Models;

public class Measure
{
    [Index(0)]
    public int Id { get; set; }
    [Index(1)]
    public int FoodId { get; set; }
    [Index(3)]
    public float Amount { get; set; }
    [Index(6)]
    public string PortionDescription { get; set; }
    [Index(7)]
    public float GramWeight { get; set; }
    [Ignore]
    public Food Food { get; set; }
}