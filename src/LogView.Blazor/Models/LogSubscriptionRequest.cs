namespace LogView.Blazor.Models;

public sealed record LogSubscriptionRequest(
    int MaxBatchSize = 100,
    bool Follow = true,
    string? Query = null,
    DateTimeOffset? Since = null);
