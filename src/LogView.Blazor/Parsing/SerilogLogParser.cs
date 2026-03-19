using System.Text.Json;
using LogView.Blazor.Models;

namespace LogView.Blazor.Parsing;

public static class SerilogLogParser
{
    public static StructuredLogItem ParseStructured(string line)
    {
        if (TryParseStructured(line, out var item))
        {
            return item;
        }

        return ParseText(line);
    }

    public static bool TryParseStructured(string line, out StructuredLogItem item)
    {
        item = default!;

        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(line);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            var timestamp = root.TryGetProperty("@t", out var t)
                ? t.GetDateTimeOffset()
                : DateTimeOffset.UtcNow;
            var level = root.TryGetProperty("@l", out var l)
                ? l.GetString() ?? "Information"
                : "Information";
            var message = root.TryGetProperty("@m", out var m)
                ? m.GetString() ?? string.Empty
                : root.TryGetProperty("Message", out var messageElement)
                    ? messageElement.GetString() ?? string.Empty
                    : line;
            var category = TryReadString(root, "SourceContext") ?? TryReadString(root, "CategoryName");
            var traceId = TryReadString(root, "TraceId");
            var spanId = TryReadString(root, "SpanId");
            var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in root.EnumerateObject())
            {
                if (property.Name.StartsWith('@'))
                {
                    continue;
                }

                properties[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString(),
                    JsonValueKind.Number => property.Value.ToString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => property.Value.ToString()
                };
            }

            item = new StructuredLogItem(timestamp, level, message, category, traceId, spanId, properties);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static StructuredLogItem ParseText(string line)
    {
        var timestamp = DateTimeOffset.UtcNow;
        var level = "Information";
        var message = line.Trim();

        var firstSeparator = message.IndexOf(' ');
        if (firstSeparator > 0 && DateTimeOffset.TryParse(message[..firstSeparator], out var parsedTimestamp))
        {
            timestamp = parsedTimestamp;
            message = message[(firstSeparator + 1)..].TrimStart();
        }

        if (message.StartsWith("[", StringComparison.Ordinal) && message.Contains(']'))
        {
            var end = message.IndexOf(']');
            level = message[1..end];
            message = message[(end + 1)..].TrimStart();
        }

        return new StructuredLogItem(timestamp, level, message, null, null, null, null);
    }

    public static ConsoleLogItem ParseConsole(string line)
    {
        if (TryParseStructured(line, out var structured))
        {
            return new ConsoleLogItem(
                structured.Message,
                structured.Timestamp,
                structured.Category,
                structured.Level.Contains("Error", StringComparison.OrdinalIgnoreCase) || structured.Level.Contains("Fatal", StringComparison.OrdinalIgnoreCase));
        }

        var text = ParseText(line);
        return new ConsoleLogItem(
            text.Message,
            text.Timestamp,
            text.Category,
            text.Level.Contains("Error", StringComparison.OrdinalIgnoreCase));
    }

    private static string? TryReadString(JsonElement root, string propertyName)
        => root.TryGetProperty(propertyName, out var value) ? value.ToString() : null;
}
