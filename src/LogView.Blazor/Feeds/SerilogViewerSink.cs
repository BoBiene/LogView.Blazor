using LogView.Blazor.Models;
using Serilog.Core;
using Serilog.Events;

namespace LogView.Blazor.Feeds;

public sealed class SerilogViewerSink : ILogEventSink
{
    private readonly InMemoryLogFeed<StructuredLogItem> _structuredFeed;
    private readonly InMemoryLogFeed<ConsoleLogItem> _consoleFeed;

    public SerilogViewerSink(InMemoryLogFeed<StructuredLogItem> structuredFeed, InMemoryLogFeed<ConsoleLogItem> consoleFeed)
    {
        _structuredFeed = structuredFeed;
        _consoleFeed = consoleFeed;
    }

    public void Emit(LogEvent logEvent)
    {
        var properties = logEvent.Properties.ToDictionary(
            pair => pair.Key,
            pair => (object?)pair.Value.ToString());

        var structured = new StructuredLogItem(
            logEvent.Timestamp,
            logEvent.Level.ToString(),
            logEvent.RenderMessage(),
            properties.TryGetValue("SourceContext", out var sourceContext) ? sourceContext?.ToString() : null,
            properties.TryGetValue("TraceId", out var traceId) ? traceId?.ToString() : null,
            properties.TryGetValue("SpanId", out var spanId) ? spanId?.ToString() : null,
            properties);

        _structuredFeed.Publish(structured);
        _consoleFeed.Publish(new ConsoleLogItem(
            structured.Message,
            structured.Timestamp,
            structured.Category,
            logEvent.Level >= LogEventLevel.Error));
    }
}
