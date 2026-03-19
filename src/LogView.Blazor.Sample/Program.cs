using LogView.Blazor.Extensions;
using LogView.Blazor.Feeds;
using LogView.Blazor.Models;
using LogView.Blazor.Sample.Components;
using LogView.Blazor.Sample.Services;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddLogViewBlazor();
builder.Services.AddSingleton<InMemoryLogFeed<ConsoleLogItem>>();
builder.Services.AddSingleton<InMemoryLogFeed<StructuredLogItem>>();
builder.Services.AddSingleton<SerilogViewerSink>();
builder.Services.AddSingleton<DemoLogState>();
builder.Services.AddHostedService<DemoLogGeneratorService>();

var plainLogPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "demo.log");
var jsonLogPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "demo-structured.json");
Directory.CreateDirectory(Path.GetDirectoryName(plainLogPath)!);

builder.Services.AddSingleton(new TextFileTailFeed(plainLogPath, TimeSpan.FromMilliseconds(250)));
builder.Services.AddSingleton(new SerilogFileFeed(jsonLogPath, TimeSpan.FromMilliseconds(250)));

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.File(plainLogPath, rollingInterval: RollingInterval.Infinite, shared: true)
        .WriteTo.File(new CompactJsonFormatter(), jsonLogPath, rollingInterval: RollingInterval.Infinite, shared: true)
        .WriteTo.Sink(services.GetRequiredService<SerilogViewerSink>());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
