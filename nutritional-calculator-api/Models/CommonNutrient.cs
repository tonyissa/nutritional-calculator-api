using CsvHelper.Configuration.Attributes;

namespace nutritional_calculator_api.Models;

public class CommonNutrient
{
    [Index(0)]
    public int Id { get; set; }
}