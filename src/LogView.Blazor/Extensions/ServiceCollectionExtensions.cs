using LogView.Blazor.Abstractions;
using LogView.Blazor.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace LogView.Blazor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLogViewBlazor(this IServiceCollection services)
    {
        services.AddScoped<ILogViewRenderer, DefaultLogViewRenderer>();
        return services;
    }
}
