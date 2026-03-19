namespace LogView.Blazor.Models;

public sealed record ConsoleLogItem(
    string Text,
    DateTimeOffset? Timestamp,
    string? Source,
    bool IsError = false);
