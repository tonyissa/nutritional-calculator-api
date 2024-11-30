namespace nutritional_calculator_api.Services;

public class PopularityTracker
{
    private readonly Dictionary<int, int> _popularityChanges = [];
    private readonly object _lock = new();

    public void ChangePopularity(int foodId)
    {
        lock (_lock)
        {
            if (_popularityChanges.ContainsKey(foodId))
            {
                _popularityChanges[foodId]++;
            }
            else
            {
                _popularityChanges[foodId] = 1;
            }
        }
    }

    public Dictionary<int, int> GetAndResetChanges()
    {
        lock (_lock)
        {
            var changes = new Dictionary<int, int>(_popularityChanges);
            _popularityChanges.Clear();

            return changes;
        }
    }
}