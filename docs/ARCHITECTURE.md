# GeoFluent System Architecture & Engineering Specifications

GeoFluent is an open-source, high-performance geometry computation and visualization application designed natively for Windows 11.

---

## 1. High-Level Architecture

```
+--------------------------------------------------------------------------+
|                            GeoFluent Application                         |
+--------------------------------------------------------------------------+
|  Presentation Layer (WinUI 3 / Windows App SDK 2.x)                      |
|  - TitleBar (ExtendsContentIntoTitleBar = true)                          |
|  - Window MicaBackdrop & SystemBackdropElement                           |
|  - NavigationView & Virtualized ItemsRepeater                            |
|  - Compiled {x:Bind} Data Binding Ramp                                   |
+--------------------------------------------------------------------------+
|  Application & ViewModel Layer (CommunityToolkit.Mvvm)                  |
|  - GeometryViewModel (Reactive property derivations)                     |
|  - CalculationHistoryViewModel (Async SQLite/JSON local state)           |
|  - UnitConverterViewModel (High-precision unit matrices)                 |
+--------------------------------------------------------------------------+
|  Computation Engine Layer (.NET 10 LTS / C# 14 Native)                    |
|  - Formulas2D & Formulas3D (Zero-allocation mathematical engines)         |
|  - UnitMatrix (High-precision double/decimal conversion algorithms)      |
|  - DerivationStepsFormatter (Step-by-step mathematical proofs)           |
+--------------------------------------------------------------------------+
|  Platform Interop & Packaging (.NET 10 AOT / ReadyToRun)                 |
|  - Windows.Storage.Pickers (Modern unpackaged & packaged storage)        |
|  - WinRT Interop (CsWinRT 2.3+ Source Generation)                        |
|  - Dual Packaging: Portable Unpackaged .exe + Microsoft Store .msix      |
+--------------------------------------------------------------------------+
```

---

## 2. Zero-Bloat Mandate & Native Performance

1. **Elimination of Web Wrappers:**
   - Previous versions built on web engines (Electron) incurred high startup latency and large distribution footprints (~160MB).
   - The native WinUI 3 architecture boots in sub-100ms and utilizes native DirectX rendering.
2. **Zero-Allocation Math:**
   - Coordinate processing, vector calculations, and vertex transformations utilize `Span<T>` and `ReadOnlySpan<T>`.
3. **Threading Model:**
   - Mathematical calculations and derivations execute asynchronously on background threads, keeping the UI thread running at 120Hz.

---

## 3. MSBuild Pipeline & Versioning

- **Central Versioning:** Controlled via `Version.props`.
- **Automatic Increment:** On every compilation, `Directory.Build.targets` executes `AutoIncrementVersionOnBuild`, incrementing `RevisionVersion` and synchronizing `Package.appxmanifest`.
- **Central Package Management:** Centralized in `Directory.Packages.props`.
