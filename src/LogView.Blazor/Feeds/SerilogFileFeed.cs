using System.Runtime.CompilerServices;
using LogView.Blazor.Abstractions;
using LogView.Blazor.Models;
using LogView.Blazor.Parsing;

namespace LogView.Blazor.Feeds;

public sealed class SerilogFileFeed : ILogFeed<StructuredLogItem>
{
    private readonly string _filePath;
    private readonly TimeSpan _pollInterval;

    public SerilogFileFeed(string filePath, TimeSpan? pollInterval = null)
    {
        _filePath = filePath;
        _pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(300);
    }

    public async IAsyncEnumerable<IReadOnlyList<StructuredLogItem>> SubscribeAsync(
        LogSubscriptionRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long position = 0;
        DateTime lastWrite = DateTime.MinValue;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!File.Exists(_filePath))
            {
                await Task.Delay(_pollInterval, cancellationToken);
                continue;
            }

            var fileInfo = new FileInfo(_filePath);
            if (fileInfo.Length < position || fileInfo.LastWriteTimeUtc < lastWrite)
            {
                position = 0;
            }

            if (fileInfo.Length > position)
            {
                using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                stream.Seek(position, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                var items = new List<StructuredLogItem>();
                while (!reader.EndOfStream && items.Count < request.MaxBatchSize)
                {
                    var line = await reader.ReadLineAsync(cancellationToken);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        items.Add(SerilogLogParser.ParseStructured(line));
                    }
                }

                position = stream.Position;
                lastWrite = fileInfo.LastWriteTimeUtc;

                if (items.Count > 0)
                {
                    yield return items;
                }
            }

            if (!request.Follow)
            {
                yield break;
            }

            await Task.Delay(_pollInterval, cancellationToken);
        }
    }
}
