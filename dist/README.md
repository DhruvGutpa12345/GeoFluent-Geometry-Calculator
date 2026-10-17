# 📦 GeoFluent - Geometry Calculator Windows 11 Build Packages

This directory contains the production packages generated for Windows 11:

| Package File | Format | Target Platform | Description |
|---|---|---|---|
| `GeoFluent-Setup-1.4.0.exe` | NSIS Executable Installer | Windows 10/11 (x64) | Full installation wizard with Start Menu and Desktop shortcuts (solid LZMA compressed) |
| `GeoFluent-Portable-1.4.0.exe` | Standalone Executable | Windows 10/11 (x64) | Zero-install standalone executable |
| `GeoFluent-1.4.0.msi` | Microsoft Windows Installer | Windows 10/11 (x64) | Enterprise MSI package using Microsoft Cabinet (CAB) compression with GUID component tracking |
| `GeoFluent-1.4.0.appx` | Windows AppX Package | Windows 10/11 (x64) | Universal Windows Platform format for sideloading and Microsoft Store |
| `GeoFluent-1.4.0.msix` | Windows 11 MSIX Package | Windows 11 (x64) | Modern containerized package signed with developer certificate (`MinVersion 10.0.17763.0`) |
| `GeoFluentDev.cer` | Security Certificate | Windows 10/11 | Developer public code-signing certificate for sideloading MSIX |
| `Install-GeoFluent.bat` | 1-Click Installer | Windows 10/11 | Automatically trusts the certificate and installs the MSIX package |

---

### How to Install the MSIX Package Locally (Resolving Error 0x800B010A / 0x800B0109)

When installing an `.msix` outside the Microsoft Store, Windows security requires the package's signing certificate to be trusted on the machine:

#### Option 1: 1-Click Automated Install (Recommended)
Simply double-click **`Install-GeoFluent.bat`** (or right-click `Install-App.ps1` -> "Run with PowerShell"). It will trust `GeoFluentDev.cer` and install the package automatically.

#### Option 2: Manual Certificate Trust (20 Seconds)
1. Double-click **`GeoFluentDev.cer`** in this folder.
2. Click **"Install Certificate..."**.
3. Select **"Local Machine"** (or Current User) and click Next.
4. Select **"Place all certificates in the following store"** -> click **Browse** -> select **"Trusted People"** (or "Trusted Root Certification Authorities") -> click OK -> Next -> Finish.
5. Now double-click **`GeoFluent-1.4.0.msix`** — Windows App Installer will display a green verified checkmark and install seamlessly!

#### Option 3: Zero-Certificate Install
If you do not want to configure a sideload certificate, simply run:
- **`GeoFluent-Setup-1.4.0.exe`** (Standard Windows installer)
- **`GeoFluent-1.4.0.msi`** (Windows Installer)
- **`GeoFluent-Portable-1.4.0.exe`** (Standalone executable)

---

### Microsoft Partner Center Submission Guidelines

1. **Target MinVersion Fixed**:
   - The package now explicitly targets `MinVersion="10.0.17763.0"` and `MaxVersionTested="10.0.26100.0"`. This satisfies Partner Center's requirement that packages target a Windows 10/11 version greater than `10.0.17134.0`.

2. **Reserved Name Alignment**:
   - Package identity display name and manifest display name are configured as:
     `GeoFluent - Geometry Calculator`
   - Matching your Microsoft Store reserved product name.

3. **`runFullTrust` Restricted Capability Justification**:
   - In Microsoft Partner Center, go to **Package submissions** -> **Submission options** -> **Notes for certification** (or Submission info).
   - Paste the following justification note for certification review:
     > *"GeoFluent - Geometry Calculator is an Electron desktop application packaged via Desktop Bridge / Centennial. It requires the 'runFullTrust' restricted capability to execute the bundled 64-bit native Windows application executable (GeoFluent.exe) and render WebGL/Chromium geometry visualizations."*

---

### Package Formats & Size Explanation
- **EXE (NSIS)** (~111 MB): Utilizes 7-Zip/LZMA solid archive compression across all binaries and resources, achieving maximum possible compression (~70% reduction) for traditional installations.
- **MSI (Windows Installer)** (~126 MB): Uses Microsoft Cabinet (CAB/LZX) per-component compression required by Windows Installer database schemas for transactional installs and rollbacks.
- **MSIX / AppX** (~164 MB): Uses Microsoft's 64KB block-level Deflate compression with SHA-256 block maps. Unlike EXE/MSI, MSIX is **never extracted to disk**; the Windows kernel mounts and pages blocks directly into memory on demand, enabling sub-2-second installs and differential delta updates from the Microsoft Store.
