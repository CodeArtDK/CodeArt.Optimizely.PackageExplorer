# 🧰 CodeArt.Optimizely.PackageExplorer

**CodeArt.Optimizely.PackageExplorer** is an open-source Blazor WebAssembly tool designed to open, inspect, and edit [Optimizely (Episerver)](https://www.optimizely.com/) content packages (`.episerverdata`, `.episerverpackage` files) — right in your browser.

> Think of it as a local, user-friendly visual explorer for Episerver content packages.

---

## 🚀 What It Does

- ✅ Upload and open `.episerverdata` / `.episerverpackage` files  
- ✅ Inspect their contents in a structured UI  
- ✅ View content, media, content types, categories, visitor groups, and more  
- ✅ Drill down into content details and metadata using tree views and dialogs
- ✅ **Debug malformed packages** with detailed error tracking and ZIP contents inspection
- ✅ **Export content to JSON/CSV** with customizable property selection
- ✅ **Delete content items, media, content types, and categories**
- ✅ **Edit property values** in content items and content type definitions
- ✅ **Download modified packages** after making changes

All powered by [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) and [MudBlazor](https://mudblazor.com/).

---

## ✨ Key Features

### 📦 Package Management
- Open and parse Optimizely content packages
- Debug mode for troubleshooting corrupted or malformed packages
- Detailed error reporting with stack traces and ZIP contents inspection
- Download modified packages after editing

### 🔍 Content Exploration
- Hierarchical tree view of content structure
- Searchable and filterable data grids
- Property-level inspection with syntax highlighting for HTML content
- Media preview for images

### ✏️ Editing Capabilities
- **Edit property values** in content items (inline editing in detail view)
- **Edit content type properties** (EditCaption, IsRequired, IsSearchable, IsLocalizable)
- **Delete content items, media, content types, and categories**
- Visual indicators for modified and deleted items
- Real-time modification tracking

### 📤 Export Options
- Export content to **JSON** or **CSV** formats
- Customizable property selection for exports
- Respects active filters and sorting
- Batch export capabilities

---

## 🛤 Roadmap

### ✅ Completed
- Open and parse Episerver content packages
- Display structured UI with tabs and tree views
- Debug mode for malformed packages
- Export content to JSON/CSV
- Delete content items, media, content types, and categories
- Edit property values in content and content types
- Download modified packages

### 🛠 Coming Soon
- 🔁 Convert packages to other formats (e.g., XML, cross-CMS import formats)
- 📦 Create new packages from scratch
- 🔍 Compare packages (diff support)
- 🤖 CLI support for automation and scripting
- 🎨 Bulk editing operations
- 📊 Package statistics and analytics

---

## 📷 Screenshots

> Coming soon — UI screenshots and demo GIFs will be added as the app evolves.

---

## 🧪 Try It Locally

```bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name/src
dotnet run --project CodeArt.Optimizely.PackageExplorer
```

Then open https://localhost:5001 in your browser.

---

## 🌐 Live Demo

> Try it out on Github Pages [https://codeartdk.github.io/CodeArt.Optimizely.PackageExplorer/](https://codeartdk.github.io/CodeArt.Optimizely.PackageExplorer/)

---

## 🤝 Contributing

Contributions are welcome! 🎉

Whether it's a bug fix, feature idea, or UX suggestion, feel free to:

- Open an [issue](https://github.com/your-username/your-repo-name/issues)
- Submit a pull request
- Share the project with the Optimizely/Blazor community

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 💡 Inspiration

This project was created to make life easier for Optimizely developers, content editors, and system integrators who often work with `.episerverdata` files — but want better tooling to explore and manipulate them.

---

## 🔗 Related Projects

- [Optimizely CMS](https://github.com/optimizely)
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [MudBlazor](https://mudblazor.com/)

---

Built with ❤️ by CodeArt ApS.
