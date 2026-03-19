# LogView.Blazor

LogView.Blazor is a .NET 8 Blazor component library for live log visualization with production-oriented extension points.

## Packages

- `LogView.Blazor`: core components, collectors, and renderer abstractions.
- `LogView.Blazor.DevExpress`: optional renderer integration point for DevExpress-oriented experiences.

## Features

- Console-style live tailing with pause, follow, wrapping, and bounded buffering.
- Structured log visualization with client-side filtering and virtualization.
- Pluggable collectors for files, Serilog file output, and in-process Serilog sinks.
- RenderFragment-based templates for rows, details, and toolbars.
