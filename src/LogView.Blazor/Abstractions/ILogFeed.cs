using LogView.Blazor.Models;

namespace LogView.Blazor.Abstractions;

public interface ILogFeed<T>
{
    IAsyncEnumerable<IReadOnlyList<T>> SubscribeAsync(
        LogSubscriptionRequest request,
        CancellationToken cancellationToken = default);
}
