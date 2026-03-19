namespace LogView.Blazor.Models;

public sealed record LogTailEntry(
    int? LineNumber,
    ConsoleLogItem? LogItem,
    LogPauseMarker? PauseMarker)
{
    public bool IsPauseMarker => PauseMarker is not null;
}
