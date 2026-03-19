using LogView.Blazor.Abstractions;
using LogView.Blazor.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LogView.Blazor.DevExpress;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods for LogView.Blazor.DevExpress.
/// </summary>
public static class DevExpressServiceCollectionExtensions
{
    /// <summary>
    /// Registers LogView.Blazor core services and replaces the default
    /// <see cref="ILogViewRenderer"/> with <see cref="DevExpressLogViewRenderer"/>.
    /// </summary>
    /// <remarks>
    /// Call this instead of (or after) <c>AddLogViewBlazor()</c> when using the
    /// DevExpress integration package.
    /// </remarks>
    public static IServiceCollection AddLogViewBlazorDevExpress(this IServiceCollection services)
    {
        services.AddLogViewBlazor();
        services.AddScoped<ILogViewRenderer, DevExpressLogViewRenderer>();
        return services;
    }

    /// <summary>
    /// Registers only the <see cref="DevExpressLogViewRenderer"/> as a scoped
    /// <see cref="ILogViewRenderer"/>.  Use this overload when you have already
    /// called <c>AddLogViewBlazor()</c> separately.
    /// </summary>
    public static IServiceCollection AddDevExpressLogViewRenderer(this IServiceCollection services)
    {
        services.AddScoped<ILogViewRenderer, DevExpressLogViewRenderer>();
        return services;
    }
}
