using LogView.Blazor.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace LogView.Blazor.Tests;

public sealed class StructuredLogViewerTests : BunitContext
{
    public StructuredLogViewerTests()
    {
        Services.AddScoped<ILogViewRenderer, DefaultLogViewRenderer>();
    }

    [Fact]
    public void RendersStructuredItemsUsingFilterParameter()
    {
        var feed = new InMemoryLogFeed<StructuredLogItem>();
        feed.Publish(new StructuredLogItem(DateTimeOffset.UtcNow, "Information", "Healthy", "Health", null, null, null));
        feed.Publish(new StructuredLogItem(DateTimeOffset.UtcNow, "Error", "Failure", "Storage", "trace-1", "span-1", null));

        var cut = Render<StructuredLogViewer>(parameters => parameters
            .Add(parameter => parameter.Feed, feed)
            .Add(parameter => parameter.Query, new StructuredLogQuery(MinimumLevel: "Error", TraceId: "trace-1")));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Failure", cut.Markup);
            Assert.DoesNotContain("Healthy", cut.Markup);
            Assert.Contains("trace-1", cut.Markup);
        });
    }
}
