# 📦 GeoFluent Windows 11 Build Packages

This directory contains the production packages generated for Windows 11:

| Package File | Format | Target Platform | Description |
|---|---|---|---|
| `GeoFluent-Setup-1.2.0.exe` | NSIS Executable Installer | Windows 10/11 (x64) | Full installation wizard with Start Menu and Desktop shortcuts (solid LZMA compressed) |
| `GeoFluent-Portable-1.2.0.exe` | Standalone Executable | Windows 10/11 (x64) | Zero-install standalone executable |
| `GeoFluent-1.2.0.msi` | Microsoft Windows Installer | Windows 10/11 (x64) | Enterprise MSI package using Microsoft Cabinet (CAB) compression with GUID component tracking |
| `GeoFluent-1.2.0.appx` | Windows AppX Package | Windows 10/11 (x64) | Universal Windows Platform format for sideloading and Microsoft Store |
| `GeoFluent-1.2.0.msix` | Windows 11 MSIX Package | Windows 11 (x64) | Modern containerized package with 64KB block-level compression for instant streaming without extraction |

### Package Formats & Size Explanation
- **EXE (NSIS)** (~106 MB): Utilizes 7-Zip/LZMA solid archive compression across all binaries and resources, achieving maximum possible compression (~70% reduction) for traditional installations.
- **MSI (Windows Installer)** (~120 MB): Uses Microsoft Cabinet (CAB/LZX) per-component compression required by Windows Installer database schemas for transactional installs and rollbacks.
- **MSIX / AppX** (~156 MB): Uses Microsoft's 64KB block-level Deflate compression with SHA-256 block maps. Unlike EXE/MSI, MSIX is **never extracted to disk**; the Windows kernel mounts and pages blocks directly into memory on demand, enabling sub-2-second installs and differential delta updates from the Microsoft Store.
