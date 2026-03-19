using LogView.Blazor.Abstractions;
using Microsoft.AspNetCore.Components;

namespace LogView.Blazor.Rendering;

public sealed class DefaultLogViewRenderer : ILogViewRenderer
{
    public RenderFragment RenderStructured<T>(IEnumerable<T> items) => builder =>
    {
        var seq = 0;
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "lv-renderer-list");
        foreach (var item in items)
        {
            builder.OpenElement(seq++, "pre");
            builder.AddAttribute(seq++, "class", "lv-renderer-item");
            builder.AddContent(seq++, item?.ToString());
            builder.CloseElement();
        }

        builder.CloseElement();
    };
}
