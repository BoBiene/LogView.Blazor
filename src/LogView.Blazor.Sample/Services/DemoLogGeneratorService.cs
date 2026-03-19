using LogView.Blazor.Sample.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LogView.Blazor.Sample.Services;

public sealed class DemoLogGeneratorService : BackgroundService
{
    private readonly ILogger<DemoLogGeneratorService> _logger;
    private readonly DemoLogState _state;
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _levels = ["INF", "WRN", "ERR"];

    public DemoLogGeneratorService(ILogger<DemoLogGeneratorService> logger, DemoLogState state, IWebHostEnvironment environment)
    {
        _logger = logger;
        _state = state;
        _environment = environment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var plainPath = Path.Combine(_environment.ContentRootPath, "App_Data", "demo.log");
        var structuredPath = Path.Combine(_environment.ContentRootPath, "App_Data", "demo-structured.json");
        _state.PlainLogPath = plainPath;
        _state.StructuredLogPath = structuredPath;

        var index = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            index++;
            var level = _levels[index % _levels.Length];
            var message = $"{DateTimeOffset.UtcNow:u} [{(level == "ERR" ? "Error" : level == "WRN" ? "Warning" : "Information")}] Sample event #{index}";
            _state.LastMessage = message;

            if (level == "ERR")
            {
                _logger.LogError("Structured pipeline error event {Index} for demo dashboard", index);
            }
            else if (level == "WRN")
            {
                _logger.LogWarning("Structured pipeline warning event {Index} for demo dashboard", index);
            }
            else
            {
                _logger.LogInformation("Structured pipeline information event {Index} for demo dashboard", index);
            }

            await File.AppendAllTextAsync(plainPath, message + Environment.NewLine, stoppingToken);
            await Task.Delay(900, stoppingToken);
        }
    }
}
