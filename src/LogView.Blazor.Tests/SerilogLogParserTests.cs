using LogView.Blazor.Parsing;

namespace LogView.Blazor.Tests;

public sealed class SerilogLogParserTests
{
    [Fact]
    public void ParsesCompactJsonIntoStructuredLogItem()
    {
        const string line = "{\"@t\":\"2026-03-19T00:00:00Z\",\"@m\":\"Hello\",\"@l\":\"Warning\",\"SourceContext\":\"Sample\",\"TraceId\":\"abc\"}";

        var item = SerilogLogParser.ParseStructured(line);

        Assert.Equal("Warning", item.Level);
        Assert.Equal("Hello", item.Message);
        Assert.Equal("Sample", item.Category);
        Assert.Equal("abc", item.TraceId);
    }
}
