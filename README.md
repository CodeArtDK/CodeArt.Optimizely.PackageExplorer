# 🧰 CodeArt.Optimizely.PackageExplorer

**CodeArt.Optimizely.PackageExplorer** is an open-source Blazor WebAssembly tool designed to open, inspect, and eventually edit [Optimizely (Episerver)](https://www.optimizely.com/) content packages (`.episerverdata`, `.episerverpackage` files) — right in your browser.

> Think of it as a local, user-friendly visual explorer for Episerver content packages.

---

## 🚀 What It Does

**Explore & Inspect:**
- ✅ Upload and open `.episerverdata` / `.episerverpackage` files  
- ✅ Inspect their contents in a structured UI  
- ✅ View content, media, content types, categories, tab definitions, and visitor groups/audiences  
- ✅ Drill down into content details and metadata using tree views and dialogs  
- ✅ Debug mode for troubleshooting malformed or corrupted packages

**Modify & Clean:**
- ✅ Delete content items, content types, media, categories, and tab definitions
- ✅ Download modified packages with your changes applied
- ✅ Track modifications with visual indicators

**Export:**
- ✅ Export content to CSV or JSON formats
- ✅ Customize which properties to export with multi-select interface
- ✅ Export respects active filters and sorting

**Developer Tools:**
- ✅ Package validation and error tracking for debugging
- ✅ Browse package structure and view ZIP contents
- ✅ Performance optimizations for handling large packages
- ✅ Progressive Web App (PWA) with automatic updates

All powered by [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) and [MudBlazor](https://mudblazor.com/).

---

## 🛤 Roadmap

### ✅ Version 1.0 (Current - November 2025)
**Explore & Debug:**
- Open and parse Episerver content packages
- Display structured UI with tabs, tree views, and data grids
- Debug mode for troubleshooting malformed packages
- View content, media, content types, categories, tab definitions, and audiences/visitor groups
- Performance optimizations for large packages

**Modify:**
- Delete content items, content types, media, categories, and tab definitions
- Download modified packages
- Track and visualize modifications

**Export:**
- Export content to CSV and JSON formats
- Customizable property selection
- Respect filters and sorting

**Other:**
- Progressive Web App (PWA) support with automatic updates
- Installable as desktop application

### 🛠 Coming Soon
- ✏️ Edit content properties directly in the UI
- 📝 Edit block properties and nested content structures  
- 🤖 **CLI tool** for automation, scripting, and working with very large packages
- 📦 Create new packages from scratch
- 🔍 Compare packages (diff support)
- 🔁 Additional export formats (XML, Excel)

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
