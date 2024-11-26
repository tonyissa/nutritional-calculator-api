namespace nutritional_calculator_api.Options;

public class DataOptions
{
    public const string DataPaths = "DataPaths";
    public string FoodPath { get; set; } = string.Empty;
    public string NutrientPath { get; set; } = string.Empty;
    public string FoodNutrientPath { get; set; } = string.Empty;
    public string CategoryPath { get; set; } = string.Empty;
    public string MeasurePath { get; set; } = string.Empty;
    public string CommonNutrientPath { get; set; } = string.Empty;
}