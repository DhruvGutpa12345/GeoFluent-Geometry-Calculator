---
trigger: glob
glob: "src/**/*.{cs,xaml}"
description: Windows 11 Fluent 2 Design System and Windows App SDK 2.x standards.
---

# Windows 11 Fluent 2 Design Guidelines

## 1. Windowing, TitleBar & Backdrops
- **TitleBar:** Use the native WinUI `<TitleBar>` control (`Microsoft.UI.Xaml.Controls.TitleBar`) in XAML. Set `ExtendsContentIntoTitleBar = true` on the Window.
- **Window Backdrop:** Apply window-level `MicaBackdrop` (default) or `DesktopAcrylicBackdrop`.
- **In-App Layering:** Use the WinAppSDK 2.0+ `<SystemBackdropElement>` with `CornerRadius` for layered Mica/Acrylic cards and floating toolbars.
- **Popups:** Anchor flyouts and relative popups using `PopupAnchor` (`Microsoft.UI.Content`) for exact positioning relative to UI elements.

## 2. Modern Storage Pickers
- Always use `Microsoft.Windows.Storage.Pickers` (`FileOpenPicker`, `FileSavePicker`, `FolderPicker`) from Windows App SDK 2.x.
- Do NOT use legacy `Windows.Storage.Pickers` which requires `InitializeWithWindow` HWND interop boilerplate in unpackaged modes.

## 3. Styling, Layout & Typography
- **Typography Ramp:** Use Windows 11 Fluent typography resources:
  - Header: `TitleLargeTextBlockStyle` (40px/28px)
  - Section Title: `SubtitleTextBlockStyle` (20px)
  - Body: `BodyTextBlockStyle` (14px)
  - Caption: `CaptionTextBlockStyle` (12px)
- **Spatial Grid:** Strict adherence to 4px/8px layout grid for margins, paddings, and control spacing.
- **Icons:** Use `Segoe Fluent Icons` font glyphs exclusively via `<FontIcon FontFamily="{StaticResource SymbolThemeFontFamily}" Glyph="..." />`.
- **Theming:** Full support for Light, Dark, and High-Contrast themes via `ThemeDictionaries` in `App.xaml`.
- **Conditional XAML:** Use `IXamlCondition` (replacing deprecated `IXamlPredicate`) for platform capability testing.
