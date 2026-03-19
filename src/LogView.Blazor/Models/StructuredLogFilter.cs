namespace LogView.Blazor.Models;

public sealed record StructuredLogFilter(
    string? Level = null,
    string? Text = null,
    string? Resource = null)
{
    public static StructuredLogFilter Empty { get; } = new();
}
