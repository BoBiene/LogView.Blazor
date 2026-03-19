namespace LogView.Blazor.Models;

public sealed record LogPauseMarker(
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    int SkippedCount,
    string? Source = null);
