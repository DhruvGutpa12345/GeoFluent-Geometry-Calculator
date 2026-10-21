# Contributing to GeoFluent

Thank you for your interest in contributing to GeoFluent!

GeoFluent is engineered as a high-performance native Windows 11 application using C# 14 and WinUI 3 (Windows App SDK 2.x).

---

## 1. Architectural Rules for Contributors & AI Agents

Before submitting code, ensure compliance with the repository rules:

1. **Zero-Bloat Framework Policy:**
   - Web wrappers (Electron, WebView2, CefSharp, Blazor, React Native, etc.) are strictly forbidden.
   - All features must use native .NET 10 and WinUI 3 APIs.
2. **Central Package Management:**
   - Pin any dependencies in `Directory.Packages.props`.
   - Never specify versions directly in project files.
3. **High Performance & Threading:**
   - Keep the UI thread non-blocking at all times.
   - Use `{x:Bind}` for compiled bindings.
   - Use `[LibraryImport]` for any P/Invoke native interop.
4. **Design Compliance:**
   - Adhere to the official Windows 11 Fluent 2 Design guidelines and the 4px/8px spatial grid.

---

## 2. Building the Project Locally

### Prerequisites
- Windows 11 (Build 22621 or newer recommended).
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`10.0.13`+ runtime).
- Visual Studio 2022 / 2026 or Windows App SDK Build Tools.

### Running the App (Portable)
```powershell
dotnet run --project src/GeoFluent/GeoFluent.csproj -f net10.0-windows10.0.26100.0
```

### Packaging Outputs
Run the automated packaging script:
```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

---

## 3. Pull Request Guidelines
- Ensure all code compiles cleanly with zero warnings (`dotnet build`).
- Verify the automatic version increment target completes without errors.
- Sign commits and adhere to the MIT License.
