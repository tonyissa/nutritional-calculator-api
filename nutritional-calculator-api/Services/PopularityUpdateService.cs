using nutritional_calculator_api.Data;

namespace nutritional_calculator_api.Services;

public class PopularityUpdateService : BackgroundService
{
    private readonly PopularityTracker _popularityTracker;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PopularityUpdateService> _logger;

    public PopularityUpdateService(PopularityTracker popularityTracker, IServiceScopeFactory scopeFactory, ILogger<PopularityUpdateService> logger)
    {
        _popularityTracker = popularityTracker;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);

            _logger.LogInformation($"Starting popularity update service at {DateTime.Now}");

            var changes = _popularityTracker.GetAndResetChanges();

            if (changes.Count != 0)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<NutritionContext>();

                foreach (var change in changes)
                {
                    var food = await context.Foods.FindAsync(change.Key, CancellationToken.None);

                    var oldPopularity = food.Popularity;
                    food.Popularity += change.Value;

                    _logger.LogInformation($"Food ID: {change.Key} Old popularity: {oldPopularity} New Popularity: {food.Popularity}");
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            _logger.LogInformation($"Stopping popularity update service at {DateTime.Now}");
        }
    }
}