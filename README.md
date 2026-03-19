# LogView.Blazor

LogView.Blazor is a production-ready .NET 8 Blazor component library starter for live log visualization.

## Repository layout

```text
/src
  LogView.Blazor
  LogView.Blazor.DevExpress
  LogView.Blazor.Sample
  LogView.Blazor.Tests
/docs
.github/workflows
.copilot
```

## Core capabilities

- `LogTailViewer` for console-style live tailing with pause/resume, follow mode, wrapping, and bounded buffering.
- `StructuredLogViewer` for virtualized structured log inspection with filters and optional details rendering.
- Pluggable collectors for text files, Serilog files, and in-process Serilog sinks.
- RenderFragment templates and renderer abstraction for extensible UI integration.

## Getting started

```csharp
builder.Services.AddLogViewBlazor();
```

Reference the stylesheet in your host app:

```html
<link rel="stylesheet" href="_content/LogView.Blazor/logview.blazor.css" />
```
