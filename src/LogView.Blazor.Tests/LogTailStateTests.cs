using LogView.Blazor.State;

namespace LogView.Blazor.Tests;

public sealed class LogTailStateTests
{
    [Fact]
    public void AddsPauseMarkerWhenResumingAfterSkippedLogs()
    {
        var state = new LogTailState(10);
        state.AppendLogs([new ConsoleLogItem("before", DateTimeOffset.Parse("2026-03-19T10:00:00Z"), "demo")]);
        state.Pause([new ConsoleLogItem("missed-1", DateTimeOffset.Parse("2026-03-19T10:00:01Z"), "demo")]);
        state.Pause([new ConsoleLogItem("missed-2", DateTimeOffset.Parse("2026-03-19T10:00:02Z"), "demo")]);

        state.Resume();

        var entries = state.Snapshot();
        Assert.Equal(2, entries.Count);
        Assert.True(entries[1].IsPauseMarker);
        Assert.Equal(2, entries[1].PauseMarker!.SkippedCount);
    }
}
