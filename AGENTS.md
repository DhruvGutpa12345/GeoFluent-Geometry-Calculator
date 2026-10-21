# AGENTS.md — Lead Windows Systems & UI Architect Contract

> **Single Source of Truth for AI Coding Agents**  
> *Target Framework:* Native .NET 10 LTS (`net10.0-windows10.0.26100.0`)  
> *Language:* C# 14 | *UI Framework:* WinUI 3 (Windows App SDK 2.x)  
> *Status:* Production OSS & Microsoft Store Architecture

---

## 1. Core Architecture & Zero-Bloat Mandate

All AI coding agents modifying this repository MUST strictly obey the following architecture:

- **Native Windows ONLY:** The application is built 100% natively using **C# 14** and **WinUI 3** via the **Windows App SDK 2.x**.
- **STRICTLY FORBIDDEN:** NEVER introduce Electron, WebView2, CefSharp, Blazor Hybrid, React Native, Tauri, or any HTML rendering wrapper. Any pull request or commit adding web-wrapper dependencies will be rejected.
- **Minimal Dependencies:** Native .NET 10 and Windows App SDK 2.x APIs handle math, threading, rendering, and storage. Every third-party NuGet package must be justified for binary footprint and sub-100ms startup latency.
- **Native AOT & High-Throughput Performance:**
  - High-efficiency CsWinRT AOT source generation (`[GeneratedWinRTExposedType]`) and ReadyToRun (`<PublishReadyToRun>true</PublishReadyToRun>`).
  - Use modern C# source generators exclusively: `[LibraryImport]` (never legacy `[DllImport]`), `JsonSerializerContext` compile-time serialization, `[GeneratedRegex]`, and `{x:Bind}` compiled bindings.
  - Non-blocking UI thread: Use `async`/`await`, `ValueTask`, `Span<T>`, `ReadOnlySpan<T>`, and UI virtualization (`ItemsRepeater` / `ListView`) for 120Hz responsiveness.

---

## 2. Verified Tooling & Central Package Management

Per the live online dependency verification mandate, all NuGet package versions are pinned centrally in `Directory.Packages.props` (`<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`):

| Package / Tool | Pinned Version | Scope & Verification |
|---|---|---|
| **.NET SDK & Runtime** | `10.0.13` (SDK `10.0.402`+) | `.NET 10 LTS`, C# 14, `<TargetFramework>net10.0-windows10.0.26100.0</TargetFramework>` |
| **`Microsoft.WindowsAppSDK`** | `2.5.1` | Windows App SDK 2.x WinUI 3 runtime, backdrops, TitleBar |
| **`Microsoft.Windows.SDK.BuildTools`** | `10.0.28000.2705` | Windows SDK build targets & MakeAppx/MakePri tooling |
| **`Microsoft.Windows.CsWinRT`** | `2.3.1` | CsWinRT WinRT interop source generation |
| **`CommunityToolkit.Mvvm`** | `8.4.2` | Compile-time MVVM source generators (zero runtime overhead) |

*Note: Never target .NET 8 or .NET 9 as they reach End of Support in November 2026.*

---

## 3. Windows 11 Fluent 2 Design Standards

- **Backdrops:** Use window-level `MicaBackdrop` or `DesktopAcrylicBackdrop`, and utilize the WinAppSDK 2.0+ `<SystemBackdropElement>` with `CornerRadius` for in-app layered cards.
- **Windowing & TitleBar:** Use the native WinUI `<TitleBar>` control (`Microsoft.UI.Xaml.Controls.TitleBar`) with `ExtendsContentIntoTitleBar = true`. Relative popups anchor via `PopupAnchor` (`Microsoft.UI.Content`).
- **Modern Storage Pickers:** Use `Microsoft.Windows.Storage.Pickers` (`FileOpenPicker`, `FileSavePicker`, `FolderPicker` from WinAppSDK 2.x) instead of legacy `Windows.Storage.Pickers` to avoid `InitializeWithWindow` HWND boilerplate.
- **Typography & Grid:** Align to the official Windows 11 typography ramp (`TitleLarge`, `Subtitle`, `Body`, `Caption`), 4px/8px spatial layout grid, Segoe Fluent Icons, and full Light/Dark/High-Contrast `ThemeDictionaries`.

---

## 4. Automatic Versioning Pipeline

Every single recompilation automatically increments the four-part version (`Major.Minor.Build.Revision`) via `Directory.Build.targets`:
- State is preserved in `Version.props`.
- Target automatically synchronizes `AssemblyVersion`, `FileVersion`, `InformationalVersion`, and `Package.appxmanifest` (`<Identity Version="..." />`).
- The live build version is rendered in the app's Settings / About view.

---

## 5. Dual Build & Packaging Outputs

The build pipeline (`build.ps1` and `GeoFluent.csproj`) supports two build targets:
1. **Portable Unpackaged `.exe`:** Standalone no-installer execution (`<WindowsPackageType>None</WindowsPackageType>` and `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>`) for instant local testing.
2. **Microsoft Store `.msix`:** Containerized Store package (`<WindowsPackageType>MSIX</WindowsPackageType>`) with Store-ready identity:
   - `Identity Name`: `DhruvGupta.GeoFluent-GeometryCalculator`
   - `Publisher`: `CN=CB32F928-2859-4967-B8F7-396A9964D1FB`
   - `MinVersion`: `10.0.17763.0`
   - `MaxVersionTested`: `10.0.26100.0`

---

## 6. ArtCraft Crafting Suite Integration (All 13 Apps)

When generating vector icons, raster splash screens, documentation, or marketing assets:
- **Available MCP Servers:** `VectorCraft`, `PhotoCraft`, `FigCraft`, `LightCraft`, `DesignCraft`, `PdfCraft`, `WordCraft`, `DeckCraft`, `GridCraft`, `FilmCraft`, `EffectCraft`, `SoundCraft`, `CADCraft`.
- **Execution Rules:** Scope automation roots to workspace (`--automation-read-root` and `--automation-write-root`). Always await document creation/opening before invoking mutations.
- **Asset Attribution:** Log all generated assets in `ASSETS.md` and `NOTICE`.

---

## 7. Modular Rules and Skills Layout

- **Modular Domain Rules:** Stored in `.agents/rules/*.md` with YAML frontmatter triggers.
- **Repeatable Procedures (Skills):** Stored in `.agents/skills/<skill-name>/SKILL.md`. (`.agents/workflows/` is deprecated and permanently disabled).
- **Context Limit:** Maintain rule files under 24,000 bytes per file and under the 20,000-token always-on budget.
