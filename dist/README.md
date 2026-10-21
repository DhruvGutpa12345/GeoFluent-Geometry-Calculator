# 📦 GeoFluent - Geometry Calculator Windows 11 Build Packages

Native Windows 11 Desktop Architecture engineered with **C# 14**, **WinUI 3** (Windows App SDK 2.5.1), and **.NET 10 LTS** (`net10.0-windows10.0.26100.0`). Zero web wrappers (0% Electron, 0% WebView2).

---

## Production Dual Packaging Outputs

| Target Directory | File | Format | Description |
|---|---|---|---|
| [`dist/portable/`](./portable/) | `GeoFluent.exe` | Standalone Unpackaged Executable | Standalone portable executable signed with code-signing certificate to satisfy Windows Application Control & Smart App Control. No installation required. |
| [`dist/msix/`](./msix/) | `GeoFluent.msix` | Microsoft Store MSIX Package | Containerized Store-ready package matching Microsoft Partner Center reserved identity and custom tile matrix. |

---

## Microsoft Partner Center Package Identity

Configured per Microsoft Partner Center reserved identity credentials:

- **Package/Identity Name**: `DhruvGupta.GeoFluent-GeometryCalculator`
- **Package Family Name**: `DhruvGupta.GeoFluent-GeometryCalculator_126rw96g2j56a`
- **Publisher ID**: `CN=CB32F928-2859-4967-B8F7-396A9964D1FB`
- **Publisher Display Name**: `Dhruv Gupta`
- **Product Display Name**: `GeoFluent - Geometry Calculator`
- **MinVersion**: `10.0.17763.0`
- **MaxVersionTested**: `10.0.26100.0` (Windows 11 24H2)
- **Tile Assets**: Bespoke high-resolution Fluent vector branding generated via ArtCraft MCP (Zero default/placeholder assets).

---

## How to Run & Test

### 1. Portable Unpackaged (.exe)
Double-click `dist/portable/GeoFluent.exe`. The app starts with sub-100ms latency with native Mica backdrop and interactive Fluent controls.

### 2. Microsoft Store Submission (.msix)
Upload `dist/msix/GeoFluent.msix` directly to your submission in Microsoft Partner Center.
