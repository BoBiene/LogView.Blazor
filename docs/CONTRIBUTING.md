# Contributing

## Coding guidelines

- Use English for code, comments, docs, tests, and commit messages.
- Prefer small, composable components and focused services.
- Keep async streaming code cancellation-aware and allocation-conscious.

## API stability rules

- Avoid breaking public APIs after release.
- Add new capabilities through optional parameters or new abstractions.
- Keep the core package free of large UI framework dependencies.

## Adding collectors

1. Implement `ILogFeed<T>`.
2. Model subscription semantics through `LogSubscriptionRequest`.
3. Keep parsing isolated from UI concerns.
4. Add unit tests for batching, rotation, and cancellation behavior.

## Extending the UI

- Prefer `RenderFragment<T>` templates before introducing new inheritance points.
- Use `ILogViewRenderer` for external renderer integrations.
- Keep CSS customization based on documented variables.

## Branching strategy

- Use short-lived feature branches.
- Rebase or merge frequently from the default branch.
- Keep pull requests focused and reviewable.
