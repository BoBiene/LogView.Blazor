# Copilot Instructions for LogView.Blazor

## Purpose

This repository contains a public, production-ready .NET 8 Blazor component library for live log visualization.

Primary goals:

- keep the public API compact and stable
- prefer maintainable architecture over fast but brittle implementation
- optimize for NuGet distribution
- keep the core package UI-framework-independent
- support incremental delivery with small, reviewable commits

## Product structure

The repository is expected to contain:

- `src/LogView.Blazor`
- `src/LogView.Blazor.DevExpress`
- `src/LogView.Blazor.Sample`
- `src/LogView.Blazor.Tests`
- `.github/workflows`
- `docs`
- `.copilot`

Keep package layout compact:

- `LogView.Blazor` = core + components
- `LogView.Blazor.DevExpress` = optional integration package

Do not split the solution into many small NuGet packages.

## General engineering rules

- Use .NET 8.
- Prefer C# and Blazor best practices.
- Keep methods short and readable.
- Prefer small, composable classes and components.
- Avoid tight coupling between UI, data acquisition, parsing, and rendering.
- Keep public APIs minimal.
- Treat every public type/member as a long-term compatibility decision.
- Prefer explicit models and interfaces over magic behavior.
- Prefer deterministic behavior over implicit framework tricks.
- Avoid unnecessary abstractions, but design extension points intentionally.
- Do not introduce Aspire-specific dependencies or concepts.
- Do not add external UI framework dependencies to the core package.
- Use English for code, identifiers, XML docs, comments, commit messages, and test names.

## Output and execution discipline

You must work incrementally and persist progress early.

### Mandatory workflow

1. Inspect current repository state before changing anything.
2. Reuse existing files and code where possible.
3. Do not restart from scratch unless the repository is clearly empty or broken beyond repair.
4. Create the smallest possible working slice first.
5. Build early.
6. Commit early.
7. Continue in small, safe increments.

### Mandatory persistence rules

- Commit within the first few minutes after creating or stabilizing the initial solution structure.
- Never work for a long time without committing.
- Prefer multiple small commits over one large commit.
- If a task is large, commit partial but buildable progress.
- If something is incomplete but blocking progress, stub it with a clear TODO and continue.

### Timeout protection

Because agent runs may time out, you must optimize for recoverability:

- always create a buildable baseline first
- always commit as soon as the solution structure is valid
- always commit again after first successful build
- always commit again after fixing major compile issues
- never spend the full session on uncommitted work

## Build-first policy

A working build is more important than feature completeness.

Priorities:

1. solution restores
2. solution builds
3. tests pass
4. sample app runs
5. feature completeness
6. polish and refactoring

If forced to choose between completeness and a working build, choose the working build.

## Required local validation flow

After meaningful changes, run:

```bash
dotnet restore
dotnet build
dotnet test
