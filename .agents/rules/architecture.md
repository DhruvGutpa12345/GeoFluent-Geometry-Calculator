---
trigger: always_on
description: Core architectural standards, zero-bloat mandate, and native performance rules.
---

# Architecture & Zero-Bloat Mandate

## 1. Native Windows Framework Only
- Target framework MUST be `<TargetFramework>net10.0-windows10.0.26100.0</TargetFramework>` with C# 14 (`<LangVersion>14.0</LangVersion>`).
- UI Framework is **WinUI 3** via the **Windows App SDK 2.x** (`Microsoft.WindowsAppSDK 2.5.1+`).
- **Forbidden Technologies:** NEVER use Electron, WebView2, CefSharp, Blazor Hybrid, React Native, Tauri, or any HTML rendering engine.

## 2. Compile-Time Performance & Source Generation
- Use C# source generators exclusively:
  - Interop: `[LibraryImport]` (never use legacy runtime `[DllImport]`).
  - Serialization: `JsonSerializerContext` with compile-time code generation.
  - Regular Expressions: `[GeneratedRegex]` for all regex matching.
  - Data Binding: Compile-time `{x:Bind}` bindings (never runtime reflection `{Binding}`).
- Enable Native AOT / ReadyToRun with CsWinRT source generation:
  - `<PublishReadyToRun>true</PublishReadyToRun>`
  - `<PublishTrimmed>true</PublishTrimmed>`
  - Use `[GeneratedWinRTExposedType]` for any custom WinRT-exposed classes.

## 3. UI Thread Non-Blocking Mandate
- Keep the UI DispatcherQueue 100% responsive (targeting 120Hz refresh).
- Offload mathematical calculations, file I/O, and geometry derivations to background tasks via `Task.Run` or `ValueTask`.
- Use `Span<T>` and `ReadOnlySpan<T>` for high-throughput coordinate processing to prevent heap allocations.
- Implement UI virtualization using `ItemsRepeater` or `ListView` for long lists (e.g. calculation history).
