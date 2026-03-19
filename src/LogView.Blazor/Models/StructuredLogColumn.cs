namespace LogView.Blazor.Models;

public sealed record StructuredLogColumn(
    string Key,
    string Title,
    Func<StructuredLogItem, string?> ValueSelector,
    bool IsVisible = true);
