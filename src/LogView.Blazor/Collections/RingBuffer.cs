using System.Collections;

namespace LogView.Blazor.Collections;

public sealed class RingBuffer<T> : IReadOnlyCollection<T>
{
    private readonly Queue<T> _items;

    public RingBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        Capacity = capacity;
        _items = new Queue<T>(capacity);
    }

    public int Capacity { get; }

    public int Count => _items.Count;

    public void Add(T item)
    {
        if (_items.Count == Capacity)
        {
            _items.Dequeue();
        }

        _items.Enqueue(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public IReadOnlyList<T> Snapshot() => _items.ToArray();

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
