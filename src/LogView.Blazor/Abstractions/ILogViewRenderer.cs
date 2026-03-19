using Microsoft.AspNetCore.Components;

namespace LogView.Blazor.Abstractions;

public interface ILogViewRenderer
{
    RenderFragment RenderStructured<T>(IEnumerable<T> items);
}
