# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Blazor WebAssembly tool for exploring and modifying Optimizely (Episerver) content packages (`.episerverdata` / `.episerverpackage` files). Runs entirely in the browser. Deployed as a PWA to GitHub Pages.

Live: https://codeartdk.github.io/CodeArt.Optimizely.PackageExplorer/

## Build & Run Commands

```bash
# Restore and run locally (opens at https://localhost:5001)
dotnet run --project src/CodeArt.Optimizely.PackageExplorer

# Build the full solution
dotnet build src/CodeArt.Optimizely.PackageExplorer.sln

# Production publish (GitHub Pages deployment)
dotnet publish src/CodeArt.Optimizely.PackageExplorer -c Release -o release
```

No test project exists yet. The `Samples/` directory contains `.episerverdata` files for manual testing.

## Solution Structure

Three projects in `src/CodeArt.Optimizely.PackageExplorer.sln`, all targeting .NET 8.0:

- **CodeArt.Optimizely.PackageExplorer** — Blazor WASM UI. Uses MudBlazor for components. Contains Razor pages/components, and UI-level services (`PackageService`, `ExportService`).
- **CodeArt.Optimizely.PackageExplorer.Core** — Domain models and parsing logic. No UI dependencies. All package reading/writing/parsing lives here.
- **CodeArt.Optimizely.PackageExplorer.CLI** — Console app stub, references Core. Not yet implemented.

## Architecture

### Data Flow

Upload → `PackageReader` (reads ZIP via `ZipPackage`) → Parsers extract models → `PackageService` holds state → Blazor components render → `PackageWriter` exports modified packages

### Core Layer (`Core/Services/`)

- **`ZipPackage`** — Wraps `ZipArchive`. Handles XML sanitization (invalid character references). Entry point for all file access within packages.
- **`PackageReader`** — Orchestrates parsing. Reads `epix.xml` for content items, `epiDefinition.xml` for types/tabs/categories, `handleddata/` for visitor groups.
- **`ContentItemParser`** — Parses `TransferContentData` XML elements into `ContentItem` models.
- **`ContentTypeParser`** — Parses content types, tabs, and categories from definition XML.
- **`AudienceParser`** — Parses visitor groups from `handleddata/handlermap.xml` and referenced data files.
- **`ContentItemEnricher`** — Builds parent-child hierarchy from flat content items using ContentLink/ParentLink.
- **`PackageWriter`** — Creates modified ZIP packages with deletions applied. Filters XML nodes by ID/GUID, preserves non-XML entries (blobs).

### UI Layer (`Services/`)

- **`PackageService`** — Scoped service managing loaded package state. Tracks deletions via `HashSet<string>` collections (`DeletedContentIds`, `DeletedContentTypeGuids`, `DeletedCategoryIds`, `DeletedTabIds`). Deletions are non-destructive until export.
- **`ExportService`** — CSV/JSON export with dynamic property selection and date formatting.

### Key Design Decisions

- **Non-destructive editing**: The original package stream is buffered. Deletions are tracked in sets and only applied when writing a new package via `PackageWriter`.
- **Memory optimization**: Uses `RecyclableMemoryStreamManager`. Large XML files (>16MB) are parsed with `XmlReader` streaming instead of loading the full string.
- **XML sanitization**: `ZipPackage.ReadXmlFile` strips invalid XML numeric character references and control characters that Optimizely sometimes produces.

## UI Components (`Components/`)

Main page is `Pages/Home.razor` with tab navigation. Key components: `ContentTable` (MudDataGrid with filtering/sorting/export), `ContentTree` (hierarchical view), `ContentTypeView`, `MediaView`, `AudienceView`, `TabView`, `CategoryView`, `Overview`, `PackageDebug` (debug mode for malformed packages).

## Deployment

GitHub Actions (`.github/workflows/deploy.yml`) auto-deploys to GitHub Pages on push to `main`. The workflow patches `<base href>` in `index.html` for the GitHub Pages subdirectory path.
