using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace nutritional_calculator_api.Models;

public class Nutrient
{
    [Index(0)]
    public int Id { get; set; }
    [Index(1)]
    public string Name { get; set; }
    [Index(2)]
    public string Unit { get; set; }
    [NotMapped]
    [Index(3)]
    public float? Code { get; set; }
}