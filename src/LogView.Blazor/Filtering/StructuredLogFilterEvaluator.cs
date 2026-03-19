using LogView.Blazor.Models;

namespace LogView.Blazor.Filtering;

public static class StructuredLogFilterEvaluator
{
    private static readonly Dictionary<string, int> LevelOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Trace"] = 0,
        ["Debug"] = 1,
        ["Information"] = 2,
        ["Warning"] = 3,
        ["Error"] = 4,
        ["Critical"] = 5,
        ["Fatal"] = 5
    };

    public static bool Matches(StructuredLogItem item, StructuredLogFilter filter)
        => Matches(item, new StructuredLogQuery(filter.Text, filter.Resource, filter.Level));

    public static bool Matches(StructuredLogItem item, StructuredLogQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.MinimumLevel) && !MatchesMinimumLevel(item.Level, query.MinimumLevel))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var text = query.SearchText.Trim();
            var inMessage = item.Message.Contains(text, StringComparison.OrdinalIgnoreCase);
            var inCategory = item.Category?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false;
            var inProperties = item.Properties?.Any(property =>
                property.Key.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                (property.Value?.ToString()?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)) ?? false;

            if (!inMessage && !inCategory && !inProperties)
            {
                return false;
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Resource))
        {
            var resource = query.Resource.Trim();
            if (!string.Equals(item.Category, resource, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        if (!string.IsNullOrWhiteSpace(query.TraceId) && !ContainsValue(item.TraceId, query.TraceId))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(query.SpanId) && !ContainsValue(item.SpanId, query.SpanId))
        {
            return false;
        }

        if (query.PropertyFilters is not null)
        {
            foreach (var filter in query.PropertyFilters)
            {
                if (item.Properties is null ||
                    !item.Properties.TryGetValue(filter.Key, out var value) ||
                    !ContainsValue(value?.ToString(), filter.Value))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool ContainsValue(string? candidate, string expected)
        => candidate?.Contains(expected.Trim(), StringComparison.OrdinalIgnoreCase) ?? false;

    private static bool MatchesMinimumLevel(string itemLevel, string minimumLevel)
    {
        if (!LevelOrder.TryGetValue(itemLevel, out var itemRank))
        {
            itemRank = int.MaxValue;
        }

        if (!LevelOrder.TryGetValue(minimumLevel, out var minimumRank))
        {
            minimumRank = int.MaxValue;
        }

        return itemRank >= minimumRank;
    }
}
