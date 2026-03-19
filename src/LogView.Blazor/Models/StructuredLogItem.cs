namespace LogView.Blazor.Models;

public sealed record StructuredLogItem(
    DateTimeOffset Timestamp,
    string Level,
    string Message,
    string? Category,
    string? TraceId,
    string? SpanId,
    IReadOnlyDictionary<string, object?>? Properties);
