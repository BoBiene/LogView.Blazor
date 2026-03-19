namespace LogView.Blazor.Tests;

public sealed class LogTailViewerTests : BunitContext
{
    public LogTailViewerTests()
    {
        var module = JSInterop.SetupModule("./_content/LogView.Blazor/logView.js");
        module.SetupVoid("scrollToBottom", _ => true);
    }

    [Fact]
    public void RendersRowsFromFeed()
    {
        var feed = new InMemoryLogFeed<ConsoleLogItem>();
        var cut = Render<LogTailViewer>(parameters => parameters
            .Add(parameter => parameter.Feed, feed));

        feed.Publish(new ConsoleLogItem("hello", DateTimeOffset.UtcNow, "test"));

        cut.WaitForAssertion(() => Assert.Contains("hello", cut.Markup));
    }

    [Fact]
    public void UsesCustomRowTemplate()
    {
        var feed = new InMemoryLogFeed<ConsoleLogItem>();
        RenderFragment<ConsoleLogItem> template = item => builder => builder.AddMarkupContent(0, $"<div class='custom'>{item.Text}</div>");

        var cut = Render<LogTailViewer>(parameters => parameters
            .Add(parameter => parameter.Feed, feed)
            .Add(parameter => parameter.RowTemplate, template));

        feed.Publish(new ConsoleLogItem("templated", DateTimeOffset.UtcNow, "test"));

        cut.WaitForAssertion(() => Assert.Contains("custom", cut.Markup));
    }
}
