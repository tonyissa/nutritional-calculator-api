using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using nutritional_calculator_api.Data;
using nutritional_calculator_api.Models;
using nutritional_calculator_api.Services;

namespace nutritional_calculator_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class FoodsController : ControllerBase
{
    private readonly NutritionContext _context;
    private readonly IMemoryCache _cache;
    private readonly PopularityTracker _popularityTracker;

    public FoodsController(NutritionContext context, IMemoryCache cache, PopularityTracker popularityTracker)
    {
        _context = context;
        _cache = cache;
        _popularityTracker = popularityTracker;
    }

    /// <summary>
    /// Returns a list of foods that match the user input
    /// </summary>
    /// <returns> A list of food </returns>
    /// <param name="input">User query to search against database</param>
    /// <param name="skip">For pagination, how many entries to skip before taking</param>
    [HttpGet]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<IEnumerable<Food>>> GetFoods(string input, int skipCount = 0)
    {
        input = input.ToLower().Replace(",", "");
        var cacheKey = $"Search_{input}";

        if (!_cache.TryGetValue(input, out List<Food> foodsList))
        {
            foodsList = await _context.Foods
                .Where(f => f.Description.ToLower().Replace(",", "").Contains(input))
                .Include(f => f.Category)
                .OrderBy(f => f.Category.Importance)
                .ThenByDescending(f => f.Popularity)
                .Skip(skipCount)
                .Take(10)
                .ToListAsync();

            if (foodsList.Count != 0)
                _cache.Set(cacheKey, foodsList, TimeSpan.FromDays(1));
        }

        if (foodsList!.Count == 0)
            return new EmptyResult();

        return foodsList;
    }

    /// <summary>
    /// Returns the food specified by the ID
    /// </summary>
    /// <returns> Food details </returns>
    /// <param name="id">Request's payload, matching ID of food in database</param>
    [HttpGet("{id}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<Food>> GetFoods(int id)
    {
        var food = await _context.Foods.FirstOrDefaultAsync(f => f.Id == id);

        if (food == null)
            return NotFound();

        _popularityTracker.ChangePopularity(id);

        return food;
    }
}