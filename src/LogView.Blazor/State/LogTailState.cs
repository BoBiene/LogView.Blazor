using LogView.Blazor.Collections;
using LogView.Blazor.Models;

namespace LogView.Blazor.State;

public sealed class LogTailState
{
    private RingBuffer<LogTailEntry> _entries;
    private int _nextLineNumber = 1;
    private LogPauseMarkerBuilder? _activePause;

    public LogTailState(int capacity)
    {
        _entries = new RingBuffer<LogTailEntry>(capacity);
    }

    public int Capacity => _entries.Capacity;

    public IReadOnlyList<LogTailEntry> Snapshot() => _entries.Snapshot();

    public void Resize(int capacity)
    {
        if (capacity == _entries.Capacity)
        {
            return;
        }

        var replacement = new RingBuffer<LogTailEntry>(capacity);
        replacement.AddRange(_entries.Snapshot().TakeLast(capacity));
        _entries = replacement;
    }

    public void AppendLogs(IEnumerable<ConsoleLogItem> items)
    {
        foreach (var item in items)
        {
            _entries.Add(new LogTailEntry(_nextLineNumber++, item, null));
        }
    }

    public void Pause(IEnumerable<ConsoleLogItem> skippedItems)
    {
        foreach (var item in skippedItems)
        {
            _activePause ??= new LogPauseMarkerBuilder(item.Timestamp ?? DateTimeOffset.UtcNow, item.Source);
            _activePause.Include(item);
        }
    }

    public void Resume()
    {
        if (_activePause is null)
        {
            return;
        }

        _entries.Add(new LogTailEntry(null, null, _activePause.Build()));
        _activePause = null;
    }

    private sealed class LogPauseMarkerBuilder(DateTimeOffset startedAt, string? source)
    {
        private DateTimeOffset? _endedAt;
        private int _skippedCount;

        public void Include(ConsoleLogItem item)
        {
            _skippedCount++;
            _endedAt = item.Timestamp ?? DateTimeOffset.UtcNow;
        }

        public LogPauseMarker Build() => new(startedAt, _endedAt, _skippedCount, source);
    }
}
