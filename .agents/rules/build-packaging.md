---
trigger: glob
glob: "**/*.{csproj,props,targets,ps1,appxmanifest}"
description: MSBuild pipeline, central package management, automatic versioning, and dual output packaging.
---

# Build & Packaging Rules

## 1. Central Package Management
- All dependencies must be pinned centrally in `Directory.Packages.props`.
- Do not specify package versions inside `.csproj` files (`<PackageReference Include="..." />` without `Version`).
- Run live online version checks before bumping or adding any package.

## 2. Automatic Version Increment Pipeline
- Target `AutoIncrementVersionOnBuild` in `Directory.Build.targets` automatically increments `RevisionVersion` on every build.
- Synchronizes `AssemblyVersion`, `FileVersion`, `InformationalVersion` in `Version.props`.
- Updates `<Identity Version="..." />` in `Package.appxmanifest`.
- Display live build version in Settings / About page from `Assembly.GetExecutingAssembly().GetName().Version`.

## 3. Dual Packaging Requirements
1. **Portable Unpackaged `.exe`:**
   - Properties: `<WindowsPackageType>None</WindowsPackageType>` and `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>`.
   - Generates standalone zero-install `.exe` in `dist/portable/`.
2. **Microsoft Store `.msix`:**
   - Properties: `<WindowsPackageType>MSIX</WindowsPackageType>`.
   - Targets: `MinVersion="10.0.17763.0"` and `MaxVersionTested="10.0.26100.0"`.
   - Store Identity:
     - Name: `DhruvGupta.GeoFluent-GeometryCalculator`
     - Publisher: `CN=CB32F928-2859-4967-B8F7-396A9964D1FB`
     - DisplayName: `GeoFluent - Geometry Calculator`
   - Generates Store-ready package in `dist/msix/`.
