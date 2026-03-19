namespace LogView.Blazor.Tests;

public sealed class StructuredLogFilterEvaluatorTests
{
    [Fact]
    public void MatchesLegacyLevelAndTextFilters()
    {
        var item = new StructuredLogItem(DateTimeOffset.UtcNow, "Error", "Disk failure", "Storage", "t1", "s1", new Dictionary<string, object?> { ["host"] = "node-1" });

        Assert.True(StructuredLogFilterEvaluator.Matches(item, new StructuredLogFilter("Warning", "Disk", null)));
        Assert.False(StructuredLogFilterEvaluator.Matches(item, new StructuredLogFilter("Critical", "Disk", null)));
    }

    [Fact]
    public void MatchesStructuredQueryWithMinimumLevelAndTraceFilters()
    {
        var item = new StructuredLogItem(DateTimeOffset.UtcNow, "Error", "Disk failure", "Storage", "trace-123", "span-456", new Dictionary<string, object?> { ["host"] = "node-1" });

        Assert.True(StructuredLogFilterEvaluator.Matches(item, new StructuredLogQuery("Disk", "Storage", "Warning", "trace-123", "span-456")));
        Assert.False(StructuredLogFilterEvaluator.Matches(item, new StructuredLogQuery(null, null, "Critical", "trace-123", null)));
    }
}
