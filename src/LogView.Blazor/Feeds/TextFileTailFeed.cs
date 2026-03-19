using System.Runtime.CompilerServices;
using LogView.Blazor.Abstractions;
using LogView.Blazor.Models;

namespace LogView.Blazor.Feeds;

public sealed class TextFileTailFeed : ILogFeed<ConsoleLogItem>
{
    private readonly Func<string> _pathFactory;
    private readonly TimeSpan _pollInterval;
    private readonly Func<string, ConsoleLogItem> _lineParser;

    public TextFileTailFeed(string filePath, TimeSpan? pollInterval = null, Func<string, ConsoleLogItem>? lineParser = null)
        : this(() => filePath, pollInterval, lineParser)
    {
    }

    public TextFileTailFeed(Func<string> pathFactory, TimeSpan? pollInterval = null, Func<string, ConsoleLogItem>? lineParser = null)
    {
        _pathFactory = pathFactory;
        _pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(300);
        _lineParser = lineParser ?? (line => new ConsoleLogItem(line, DateTimeOffset.UtcNow, "file"));
    }

    public async IAsyncEnumerable<IReadOnlyList<ConsoleLogItem>> SubscribeAsync(
        LogSubscriptionRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? currentPath = null;
        long position = 0;
        DateTime lastWrite = DateTime.MinValue;

        while (!cancellationToken.IsCancellationRequested)
        {
            var path = _pathFactory();
            if (!File.Exists(path))
            {
                await Task.Delay(_pollInterval, cancellationToken);
                continue;
            }

            var fileInfo = new FileInfo(path);
            var fileChanged = !string.Equals(currentPath, path, StringComparison.Ordinal)
                || fileInfo.Length < position
                || fileInfo.LastWriteTimeUtc < lastWrite;

            if (fileChanged)
            {
                position = 0;
                currentPath = path;
            }

            if (fileInfo.Length > position)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                stream.Seek(position, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                var items = new List<ConsoleLogItem>();
                while (!reader.EndOfStream && items.Count < request.MaxBatchSize)
                {
                    var line = await reader.ReadLineAsync(cancellationToken);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        items.Add(_lineParser(line));
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
