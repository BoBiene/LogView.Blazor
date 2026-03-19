# Versioning

LogView.Blazor follows Semantic Versioning.

- **MAJOR** for breaking API or behavioral changes.
- **MINOR** for backward-compatible feature additions.
- **PATCH** for backward-compatible fixes, documentation updates, or internal improvements.

## Package policy

- `LogView.Blazor` carries the primary public API surface.
- `LogView.Blazor.DevExpress` should track compatible `LogView.Blazor` versions.
- Avoid package fragmentation; add new features to the existing packages unless a dependency boundary clearly requires a separate package.
