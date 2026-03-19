namespace LogView.Blazor.Models;

public sealed record StructuredLogQuery(
    string? SearchText = null,
    string? Resource = null,
    string? MinimumLevel = null,
    string? TraceId = null,
    string? SpanId = null,
    IReadOnlyDictionary<string, string>? PropertyFilters = null)
{
    public static StructuredLogQuery Empty { get; } = new();
}
