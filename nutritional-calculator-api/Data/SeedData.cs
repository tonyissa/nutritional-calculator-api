using CsvHelper;
using nutritional_calculator_api.Models;
using nutritional_calculator_api.Options;
using System.Globalization;

namespace nutritional_calculator_api.Data;

public static class SeedData
{
    public async static Task ExecuteAsync(NutritionContext context, DataOptions dataOptions)
    {
        if (context.Foods.Any())
            return;

        var nutrients = ReadCSVAndReturnList<Nutrient>(dataOptions.NutrientPath);
        var categories = ReadCSVAndReturnList<Category>(dataOptions.CategoryPath);
        var foods = ReadCSVAndReturnList<Food>(dataOptions.FoodPath);
        var foodNutrients = ReadCSVAndReturnList<FoodNutrient>(dataOptions.FoodNutrientPath);
        var measures = ReadCSVAndReturnList<Measure>(dataOptions.MeasurePath);
        var commonNutrients = ReadCSVAndReturnList<CommonNutrient>(dataOptions.CommonNutrientPath);

        context.ChangeTracker.AutoDetectChangesEnabled = false;
        int batchSize = 2000;

        await BatchInsertAsync<Category>(context, categories, batchSize);
        await BatchInsertAsync<Nutrient>(context, nutrients, batchSize, (batch, i) =>
        {
            return batch.Where(n => commonNutrients.Any(cn => n.Code == cn.Id)).Skip(i).Take(batchSize);
        });
        await BatchInsertAsync<Food>(context, foods, batchSize);
        await BatchInsertAsync<FoodNutrient>(context, foodNutrients, batchSize, (batch, i) =>
        {
            return batch.Where(fn => context.Nutrients.Any(n => fn.NutrientId == n.Id)).Skip(i).Take(batchSize);
        });
        await BatchInsertAsync<Measure>(context, measures, batchSize);
    }

    private static List<TEntity> ReadCSVAndReturnList<TEntity>(string path)
        where TEntity : class
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<TEntity>().ToList();
    }

    private static async Task BatchInsertAsync<TEntity>(
        NutritionContext context,
        List<TEntity> list,
        int batchSize,
        Func<List<TEntity>, int, IEnumerable<TEntity>>? optionalQueryAction = null)
        where TEntity : class
    {
        for (int i = 0; i < list.Count; i += batchSize)
        {
            IEnumerable<TEntity> batch;

            if (optionalQueryAction != null)
            {
                batch = optionalQueryAction(list, i);
            }
            else
            {
                batch = list.Skip(i).Take(batchSize);
            }

            await context.Set<TEntity>().AddRangeAsync(batch);
            context.ChangeTracker.DetectChanges();
            await context.SaveChangesAsync();
        }
    }
}