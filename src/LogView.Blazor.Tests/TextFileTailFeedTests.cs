namespace LogView.Blazor.Tests;

public sealed class TextFileTailFeedTests
{
    [Fact]
    public async Task ReadsAppendedLinesAndHandlesRotation()
    {
        var tempDir = Directory.CreateTempSubdirectory();
        var path = Path.Combine(tempDir.FullName, "app.log");
        await File.WriteAllTextAsync(path, "first" + Environment.NewLine);
        var feed = new TextFileTailFeed(path, TimeSpan.FromMilliseconds(50), line => new ConsoleLogItem(line, null, null));
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        var results = new List<IReadOnlyList<ConsoleLogItem>>();
        var subscription = Task.Run(async () =>
        {
            await foreach (var batch in feed.SubscribeAsync(new LogSubscriptionRequest(10, true), cts.Token))
            {
                results.Add(batch);
                if (results.Count == 2)
                {
                    cts.Cancel();
                }
            }
        }, cts.Token);

        await Task.Delay(120, CancellationToken.None);
        await File.AppendAllTextAsync(path, "second" + Environment.NewLine, CancellationToken.None);
        await Task.Delay(120, CancellationToken.None);
        await File.WriteAllTextAsync(path, "rotated" + Environment.NewLine, CancellationToken.None);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await subscription);
        Assert.Contains(results.SelectMany(batch => batch), item => item.Text == "first");
        Assert.Contains(results.SelectMany(batch => batch), item => item.Text is "second" or "rotated");
    }
}
