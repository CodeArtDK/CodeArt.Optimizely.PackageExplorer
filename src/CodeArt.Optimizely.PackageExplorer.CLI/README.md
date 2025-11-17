# CodeArt.Optimizely.PackageExplorer.CLI

Command-line interface for exploring and exporting Optimizely content packages.

## Features

- 📊 View content, content types, media, categories, and audiences/visitor groups
- 📁 Export data to CSV or JSON formats
- 🚀 Optimized for large package files using streaming XML parsing
- 🎯 Interactive menu mode for easy navigation
- ⌨️ Command-line mode for automation and scripting

## Usage

### Interactive Mode

Start the interactive menu by providing just the package file path:

```bash
PackageExplorer.CLI.exe path/to/package.episerverdata
```

The interactive menu allows you to:
- View content items with summaries
- View content types with property counts
- View media items
- View categories
- View audiences/visitor groups
- Export content to CSV/JSON with custom property selection
- Export content types to CSV/JSON
- Export categories to CSV/JSON

### Command-Line Mode

#### List Content

Display a list of content items with summary statistics:

```bash
PackageExplorer.CLI.exe list --type content package.episerverdata
```

#### List Content Types

Display available content types:

```bash
PackageExplorer.CLI.exe list --type content-types package.episerverdata
```

#### List Media

Display media items:

```bash
PackageExplorer.CLI.exe list --type media package.episerverdata
```

#### List Categories

Display categories:

```bash
PackageExplorer.CLI.exe list --type categories package.episerverdata
```

#### List Audiences

Display visitor groups/audiences:

```bash
PackageExplorer.CLI.exe list --type audiences package.episerverdata
```

#### Export Content to CSV

Export content items with specific properties:

```bash
PackageExplorer.CLI.exe export package.episerverdata \
  --type content \
  --format csv \
  --output content.csv \
  --properties PageName,PageTypeName,PageLink,PageLanguageBranch
```

#### Export Content to JSON

```bash
PackageExplorer.CLI.exe export package.episerverdata \
  --type content \
  --format json \
  --output content.json \
  --properties PageName,PageTypeName,PageLink,PageURLSegment
```

#### Export Content Types

```bash
PackageExplorer.CLI.exe export package.episerverdata \
  --type content-types \
  --format json \
  --output contenttypes.json
```

#### Export Categories

```bash
PackageExplorer.CLI.exe export package.episerverdata \
  --type categories \
  --format csv \
  --output categories.csv
```

## Common Properties

When exporting content, you can specify any properties available in the package. Common properties include:

- `PageName` - The name/title of the content
- `PageTypeName` - The content type name
- `PageLink` - The content reference/ID
- `PageParentLink` - Parent content reference
- `PageLanguageBranch` - Language code
- `PageURLSegment` - URL segment
- `PageStartPublish` - Publish date
- Custom properties specific to your content types

To discover available properties, use the interactive menu which displays all available properties before export.

## Performance Optimization

For large package files (>10,000 content items), the CLI automatically uses streaming XML parsing to minimize memory usage. This allows processing of very large packages without loading the entire XML document into memory.

## Exit Codes

- `0` - Success
- `1` - Error (file not found, invalid arguments, etc.)

## Examples

### Export all content with basic properties
```bash
PackageExplorer.CLI.exe export site-export.episerverdata \
  --type content \
  --format csv \
  --output all-content.csv \
  --properties PageName,PageTypeName,PageLink,PageLanguageBranch,PageURLSegment
```

### Export content types for documentation
```bash
PackageExplorer.CLI.exe export site-export.episerverdata \
  --type content-types \
  --format json \
  --output content-types.json
```

### Quick content overview
```bash
PackageExplorer.CLI.exe list --type content site-export.episerverdata
```

## Build from Source

```bash
dotnet build
dotnet run -- <arguments>
```

## License

MIT License - See LICENSE file for details
