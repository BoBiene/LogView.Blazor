using System.Runtime.CompilerServices;
using System.Threading.Channels;
using LogView.Blazor.Abstractions;
using LogView.Blazor.Models;

namespace LogView.Blazor.Feeds;

public sealed class InMemoryLogFeed<T> : ILogFeed<T>
{
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();
    private readonly List<T> _history = [];
    private readonly object _syncRoot = new();

    public void Publish(T item)
    {
        lock (_syncRoot)
        {
            _history.Add(item);
        }

        _channel.Writer.TryWrite(item);
    }

    public void PublishBatch(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Publish(item);
        }
    }

    public async IAsyncEnumerable<IReadOnlyList<T>> SubscribeAsync(
        LogSubscriptionRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<T> history;
        lock (_syncRoot)
        {
            history = _history.ToList();
        }

        if (history.Count > 0)
        {
            yield return history.TakeLast(Math.Max(1, request.MaxBatchSize)).ToArray();
        }

        if (!request.Follow)
        {
            yield break;
        }

        var batch = new List<T>(request.MaxBatchSize);
        while (!cancellationToken.IsCancellationRequested)
        {
            var item = await _channel.Reader.ReadAsync(cancellationToken);
            batch.Add(item);

            while (batch.Count < request.MaxBatchSize && _channel.Reader.TryRead(out var buffered))
            {
                batch.Add(buffered);
            }

            yield return batch.ToArray();
            batch.Clear();
        }
    }
}
