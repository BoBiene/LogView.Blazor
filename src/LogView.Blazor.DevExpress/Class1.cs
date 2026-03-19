using LogView.Blazor.Abstractions;
using Microsoft.AspNetCore.Components;

namespace LogView.Blazor.DevExpress;

public sealed class DevExpressLogViewRenderer : ILogViewRenderer
{
    public RenderFragment RenderStructured<T>(IEnumerable<T> items) => builder =>
    {
        var seq = 0;
        builder.OpenElement(seq++, "section");
        builder.AddAttribute(seq++, "class", "dx-logview-renderer");
        foreach (var item in items)
        {
            builder.OpenElement(seq++, "article");
            builder.AddAttribute(seq++, "class", "dx-logview-card");
            builder.AddContent(seq++, item?.ToString());
            builder.CloseElement();
        }

        builder.CloseElement();
    };
}
