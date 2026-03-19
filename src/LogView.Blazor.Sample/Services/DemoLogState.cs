namespace LogView.Blazor.Sample.Services;

public sealed class DemoLogState
{
    public string LastMessage { get; set; } = "Waiting for log generation...";
    public string PlainLogPath { get; set; } = string.Empty;
    public string StructuredLogPath { get; set; } = string.Empty;
}
