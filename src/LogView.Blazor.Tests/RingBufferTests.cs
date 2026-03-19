using LogView.Blazor.Collections;

namespace LogView.Blazor.Tests;

public sealed class RingBufferTests
{
    [Fact]
    public void AddsAndEvictsOldestItemsWhenCapacityIsReached()
    {
        var buffer = new RingBuffer<int>(3);
        buffer.AddRange([1, 2, 3, 4]);

        Assert.Equal([2, 3, 4], buffer.Snapshot());
    }
}
