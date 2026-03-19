using LogView.Blazor.Abstractions;
using LogView.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LogView.Blazor.DevExpress;

/// <summary>
/// An <see cref="ILogViewRenderer"/> that renders log items using DevExpress Blazor grid CSS
/// conventions (<c>dxbl-grid-*</c> classes), providing visual consistency with DevExpress UI
/// without requiring the <c>DevExpress.Blazor</c> package at compile time.
/// </summary>
/// <remarks>
/// <para>
/// When <c>DevExpress.Blazor</c> is available in the host application, replace the HTML
/// scaffolding in this class with the real <c>DxGrid</c> / <c>DxDataGrid</c> components.
/// See the commented-out instructions in <c>LogView.Blazor.DevExpress.csproj</c>.
/// </para>
/// <para>
/// Register via <see cref="DevExpressServiceCollectionExtensions.AddLogViewBlazorDevExpress"/>.
/// </para>
/// </remarks>
public sealed class DevExpressLogViewRenderer : ILogViewRenderer
{
    /// <summary>
    /// Renders the given log <paramref name="items"/> as a DevExpress-style HTML table.
    /// When <typeparamref name="T"/> is <see cref="StructuredLogItem"/> the renderer emits
    /// full column headers and per-row level badges; other types fall back to a simple list.
    /// </summary>
    public RenderFragment RenderStructured<T>(IEnumerable<T> items) => builder =>
    {
        if (items is IEnumerable<StructuredLogItem> logItems)
        {
            RenderStructuredGrid(builder, logItems);
        }
        else
        {
            RenderFallbackList(builder, items);
        }
    };

    // -------------------------------------------------------------------------
    // Structured-item grid
    // -------------------------------------------------------------------------

    private static void RenderStructuredGrid(RenderTreeBuilder builder, IEnumerable<StructuredLogItem> items)
    {
        var seq = 0;

        // Outer container — mirrors DevExpress DxGrid root element classes
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "dxbl-grid dx-logview-dx-grid");

        // Header row
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "dxbl-grid-header-row");
        foreach (var header in new[] { "Timestamp", "Level", "Category", "Message" })
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-header-cell");
            builder.AddContent(seq++, header);
            builder.CloseElement();
        }
        builder.CloseElement(); // header row

        // Data rows
        foreach (var item in items)
        {
            var levelLower = item.Level.ToLowerInvariant();
            var levelCss = GetLevelCssClass(item.Level);

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", string.IsNullOrEmpty(levelCss)
                ? "dxbl-grid-data-row"
                : $"dxbl-grid-data-row {levelCss}");

            // Timestamp
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-data-cell dx-logview-ts");
            builder.AddContent(seq++, item.Timestamp.ToLocalTime().ToString("HH:mm:ss"));
            builder.CloseElement();

            // Level badge
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-data-cell dx-logview-level");
            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", $"dx-logview-badge dx-logview-badge-{levelLower}");
            builder.AddContent(seq++, item.Level);
            builder.CloseElement(); // span
            builder.CloseElement(); // cell

            // Category
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-data-cell dx-logview-category");
            builder.AddContent(seq++, item.Category ?? string.Empty);
            builder.CloseElement();

            // Message
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-data-cell dx-logview-message");
            builder.AddContent(seq++, item.Message);
            builder.CloseElement();

            builder.CloseElement(); // data row
        }

        builder.CloseElement(); // outer container
    }

    // -------------------------------------------------------------------------
    // Generic fallback list
    // -------------------------------------------------------------------------

    private static void RenderFallbackList<T>(RenderTreeBuilder builder, IEnumerable<T> items)
    {
        var seq = 0;
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "dxbl-grid dx-logview-dx-grid");
        foreach (var item in items)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-data-row");
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dxbl-grid-data-cell");
            builder.AddContent(seq++, item?.ToString());
            builder.CloseElement();
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string GetLevelCssClass(string level) => level.ToUpperInvariant() switch
    {
        "ERROR" or "FATAL" or "CRITICAL" => "dxbl-grid-data-row-danger",
        "WARNING" or "WARN"              => "dxbl-grid-data-row-warning",
        "DEBUG" or "TRACE"               => "dxbl-grid-data-row-muted",
        _                                => string.Empty
    };
}
