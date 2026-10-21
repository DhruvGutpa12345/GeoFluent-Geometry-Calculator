---
name: dual-packaging
description: Procedure for compiling and packaging dual outputs (Portable Unpackaged .exe and Microsoft Store .msix).
---

# Dual Packaging Skill

This skill documents the repeatable procedure for compiling both the standalone portable `.exe` and the Microsoft Store containerized `.msix`.

## Prerequisites
- .NET 10 SDK (`10.0.13`+ runtime, C# 14).
- Windows App SDK 2.x and Windows 10/11 SDK BuildTools.
- Valid developer code-signing certificate (e.g. `certs/GeoFluentDev.pfx`).

## Procedure

### Step 1: Clean and Prepare Output Directories
```powershell
if (Test-Path "dist") { Remove-Item -Recurse -Force "dist" }
New-Item -ItemType Directory -Force -Path "dist/portable", "dist/msix"
```

### Step 2: Build Portable Unpackaged Executable
```powershell
dotnet publish src/GeoFluent/GeoFluent.csproj `
  -c Release `
  -r win-x64 `
  -p:WindowsPackageType=None `
  -p:WindowsAppSDKSelfContained=true `
  -p:PublishReadyToRun=true `
  -p:PublishSingleFile=true `
  --self-contained true `
  -o dist/portable/
```

### Step 3: Build Microsoft Store MSIX Package
```powershell
dotnet publish src/GeoFluent/GeoFluent.csproj `
  -c Release `
  -r win-x64 `
  -p:WindowsPackageType=MSIX `
  -p:GenerateAppxPackageOnBuild=true `
  -p:AppxPackageDir="$PWD/dist/msix/" `
  -p:AppxBundle=Never `
  -p:UapAppxPackageBuildMode=StoreUpload
```

### Step 4: Verify Output Packages
- Ensure `dist/portable/GeoFluent.exe` runs without requiring any installation or MSIX registration.
- Verify `dist/msix/*.msix` package identity matches Partner Center:
  - Identity: `DhruvGupta.GeoFluent-GeometryCalculator`
  - Publisher: `CN=CB32F928-2859-4967-B8F7-396A9964D1FB`
  - MinVersion: `10.0.17763.0`
