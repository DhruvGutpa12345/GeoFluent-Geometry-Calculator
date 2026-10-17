# 📦 GeoFluent - Geometry Calculator Windows 11 Build Packages

This directory contains the production packages generated for Windows 11:

| Package File | Format | Target Platform | Description |
|---|---|---|---|
| `GeoFluent-Setup-1.5.0.exe` | NSIS Executable Installer | Windows 10/11 (x64) | Full installation wizard with Start Menu and Desktop shortcuts (solid LZMA compressed) |
| `GeoFluent-Portable-1.5.0.exe` | Standalone Executable | Windows 10/11 (x64) | Zero-install standalone executable |
| `GeoFluent-1.5.0.msi` | Microsoft Windows Installer | Windows 10/11 (x64) | Enterprise MSI package using Microsoft Cabinet (CAB) compression with GUID component tracking |
| `GeoFluent-1.5.0.appx` | Windows AppX Package | Windows 10/11 (x64) | Universal Windows Platform format for sideloading and Microsoft Store |
| `GeoFluent-1.5.0.msix` | Windows 11 MSIX Package | Windows 11 (x64) | Modern containerized package matching Microsoft Partner Center reserved identity |
| `GeoFluentDev.cer` | Security Certificate | Windows 10/11 | Developer public code-signing certificate matching Partner Center Publisher ID for sideloading |
| `Install-GeoFluent.bat` | 1-Click Installer | Windows 10/11 | Automatically trusts the certificate and installs the MSIX package |

---

### Microsoft Partner Center Package Identity

This package is signed and configured with the exact Microsoft Partner Center reserved identity credentials:

- **Package/Identity Name**: `DhruvGupta.GeoFluent-GeometryCalculator`
- **Package Family Name**: `DhruvGupta.GeoFluent-GeometryCalculator_126rw96g2j56a`
- **Publisher**: `CN=CB32F928-2859-4967-B8F7-396A9964D1FB`
- **Publisher Display Name**: `Dhruv Gupta`
- **Product Display Name**: `GeoFluent - Geometry Calculator`
- **MinVersion**: `10.0.17763.0` (Windows 10 Build 1809+)
- **MaxVersionTested**: `10.0.26100.0` (Windows 11 24H2)

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
5. Now double-click **`GeoFluent-1.5.0.msix`** — Windows App Installer will display a green verified checkmark and install seamlessly!

#### Option 3: Zero-Certificate Install
If you do not want to configure a sideload certificate, simply run:
- **`GeoFluent-Setup-1.5.0.exe`** (Standard Windows installer)
- **`GeoFluent-1.5.0.msi`** (Windows Installer)
- **`GeoFluent-Portable-1.5.0.exe`** (Standalone executable)

---

### Microsoft Partner Center Submission Notes

1. **Upload Package**:
   - In Partner Center -> **Packages**, upload `GeoFluent-1.5.0.msix`.
   - Identity name, family name, and publisher ID are completely aligned.

2. **`runFullTrust` Restricted Capability Justification**:
   - In Microsoft Partner Center, go to **Package submissions** -> **Submission options** -> **Notes for certification** (or Submission info).
   - Paste the following justification note for certification review:
     > *"GeoFluent - Geometry Calculator is an Electron desktop application packaged via Desktop Bridge / Centennial. It requires the 'runFullTrust' restricted capability to execute the bundled 64-bit native Windows application executable (GeoFluent.exe) and render WebGL/Chromium geometry visualizations."*
