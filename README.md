# 📐 GeoFluent — Geometry Calculator for Windows 11

[![Windows 11](https://img.shields.io/badge/Windows-11-0078D4?style=for-the-badge&logo=windows11&logoColor=white)](https://microsoft.com)
[![Fluent Design 2](https://img.shields.io/badge/Design-Fluent%202%20Mica-60CDFF?style=for-the-badge&logo=windows&logoColor=white)](https://fluent2.microsoft.design/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)
[![Microsoft Store Ready](https://img.shields.io/badge/Store-MSIX%20%2F%20AppX%20Ready-00A4EF?style=for-the-badge&logo=microsoft)](https://partner.microsoft.com/)
[![Built with Electron & Three.js](https://img.shields.io/badge/Engine-Three.js%20%7C%20WebGL-black?style=for-the-badge&logo=three.js&logoColor=white)](https://threejs.org/)

**GeoFluent** is an open-source, high-precision geometry computation application engineered specifically for **Windows 11**. It adheres strictly to the Microsoft Fluent 2 Design Language, featuring native Mica materials, subtle acrylic surfaces, Segoe UI Variable typography, interactive 2D vector diagrams, and real-time 3D WebGL solid visualizations.

Whether you are calculating simple areas or solving complex multi-variable geometric systems, GeoFluent supports **multiple calculation methods**, **per-parameter input units**, **dynamic output unit conversion**, **step-by-step mathematical derivations**, and **full derived parameter extraction**.

---

## ✨ Key Features

- 🎨 **Authentic Windows 11 Fluent 2 Design**:
  - Native Mica backdrop effect and acrylic glassmorphism.
  - Light and Dark modes with automatic system synchronization.
  - Windows 11 title bar controls and fluid transitions.
- 📐 **Comprehensive 2D & 3D Shape Engine**:
  - Over 24 geometric shapes with multi-method parametric calculations.
- 🔄 **Multiple Calculation Methods**:
  - Calculate shapes based on different input parameters (e.g. Triangle via Heron's formula, SAS, ASA, Base/Height, Right-Angled legs, or Equilateral side).
- 🔢 **Full Derived Parameter Extraction**:
  - Automatically computes *every single derived parameter* possible (e.g., for a triangle: all 3 sides, all 3 angles, altitudes $h_a, h_b, h_c$, medians $m_a, m_b, m_c$, inradius $r$, circumradius $R$, and triangle classification).
- 🌐 **Flexible Units Engine**:
  - **Input Units**: Choose individual units (Metric & Imperial) for each input field (mm, cm, m, km, in, ft, yd, mi; angles: degrees, radians, gradians).
  - **Output Units**: Change viewing units on the fly for all lengths, areas, volumes, and angles simultaneously without re-entering parameters.
- 📊 **Step-by-Step Mathematical Derivations**:
  - Shows the exact formulas and substituted numerical values with unit preservation.
- 🧊 **Interactive Visualizers**:
  - **2D Dynamic Diagrams**: Auto-scaling vector diagrams with vertex markers, dimension callouts, inradius/circumradius guides, and angle arcs.
  - **3D WebGL Viewport**: Powered by Three.js with full 360° orbital drag rotation, mouse wheel zoom, wireframe toggle, realistic materials, and auto-rotation.
- 🛠️ **Universal Unit Converter**:
  - Built-in converter tool for Length, Area, Volume, and Angle.
- 📜 **History & Export**:
  - Local history of past calculations with instant reload and Markdown report export.
- 📦 **Multi-Format Packaging**:
  - Packages available in `dist/`: `.exe` (Installer & Portable), `.msi` (Windows Installer), and `.msix` / `.appx` (Microsoft Store).

---

## 📐 Supported Shapes & Calculation Methods

### 🟢 2D Plane Shapes

| Shape | Available Calculation Methods | All Derived Outputs |
|---|---|---|
| **Triangle** | • 3 Sides ($a, b, c$ - Heron's)<br>• Base & Height ($b, h$)<br>• Two Sides & Included Angle (SAS)<br>• Equilateral Triangle ($a$)<br>• Right-Angled (Legs $a, b$) | Perimeter, Semi-perimeter, Area, All 3 angles ($\alpha, \beta, \gamma$), Altitudes ($h_a, h_b, h_c$), Medians ($m_a, m_b, m_c$), Inradius ($r$), Circumradius ($R$), Shape Classification |
| **Square** | • Side Length ($a$)<br>• Diagonal ($d$)<br>• Area ($A$) | Side, Diagonal, Perimeter, Area, Inradius ($a/2$), Circumradius ($d/2$) |
| **Rectangle** | • Length & Width ($l, w$)<br>• Diagonal & Side ($d, l$) | Length, Width, Diagonal, Perimeter, Area, Aspect Ratio, Circumradius |
| **Circle** | • Radius ($r$)<br>• Diameter ($d$)<br>• Area ($A$) | Radius, Diameter, Circumference, Area |
| **Ellipse** | • Semi-Major & Semi-Minor Axes ($a, b$) | Major/Minor axes, Linear Eccentricity ($c$), Eccentricity ($e$), Focal Distance ($2c$), Ramanujan Perimeter, Area |
| **Parallelogram** | • Sides & Included Angle ($a, b, \theta$) | Base, Side, Height, Diagonals ($d_1, d_2$), Angles, Perimeter, Area |
| **Rhombus** | • Two Diagonals ($d_1, d_2$) | Side ($a$), Diagonals, Height, Interior Angles, Perimeter, Area, Inradius |
| **Trapezoid** | • Parallel Bases & Height ($a, b, h$) | Bases, Legs, Height, Midline/Median ($m$), Perimeter, Area |
| **Regular Polygon** | • Number of Sides ($n$) & Side Length ($s$) | Sides ($n$), Side length, Apothem/Inradius ($r$), Circumradius ($R$), Interior/Exterior Angles, Total Diagonals, Perimeter, Area |
| **Circular Sector** | • Radius & Central Angle ($r, \theta$) | Radius, Angle, Arc Length, Chord Length, Sector Area, Triangle Area, Segment Area, Perimeter |
| **Annulus (Ring)** | • Outer & Inner Radii ($R, r$) | Inner/Outer radii, Ring Width ($w$), Circumferences, Annulus Area |

### 🔷 3D Solid Shapes

| Shape | Available Calculation Methods | All Derived Outputs |
|---|---|---|
| **Sphere** | • Radius ($r$)<br>• Diameter ($d$)<br>• Volume ($V$) | Radius, Diameter, Great Circle Circumference/Area, Total Surface Area, Volume |
| **Hemisphere** | • Radius ($r$) | Radius, Diameter, Base Area, Curved Surface Area, Total Surface Area, Volume, Centroid Height |
| **Cube** | • Side Length ($a$)<br>• Volume ($V$) | Side, Face Diagonal, Space Diagonal, Base Area/Perimeter, Lateral Area, Total Surface Area, Volume, Inscribed/Circumscribed Sphere Radii |
| **Cuboid** | • Length, Width & Height ($l, w, h$) | Length, Width, Height, Face Diagonals, Space Diagonal, Base Area/Perimeter, Lateral Area, Total Surface Area, Volume |
| **Cylinder** | • Radius & Height ($r, h$) | Radius, Diameter, Height, Base Perimeter, Single/Both Base Area, Lateral Surface Area, Total Surface Area, Volume |
| **Cone** | • Radius & Height ($r, h$)<br>• Radius & Slant Height ($r, l$) | Radius, Diameter, Vertical Height, Slant Height ($l$), Vertex Angle ($2\theta$), Base Area, Lateral Surface Area, Total Surface Area, Volume |
| **Frustum of Cone** | • Top Radius, Bottom Radius & Height ($r, R, h$) | Top/Bottom Radii, Height, Slant Height, Top/Bottom Base Areas, Lateral Surface Area, Total Surface Area, Volume |
| **Square Pyramid** | • Base Side & Vertical Height ($a, h$) | Base Side, Height, Slant Height, Lateral Edge, Base Area/Perimeter, Lateral Surface Area, Total Surface Area, Volume |
| **Triangular Prism** | • Equilateral Base Side & Length ($a, L$) | Base Side, Base Perimeter, Base Area, Lateral Surface Area, Total Surface Area, Volume |
| **Torus (Donut)** | • Major Radius & Tube Radius ($R, r$) | Major Radius, Tube Radius, Inner Hole Diameter, Outer Diameter, Aspect Ratio, Surface Area, Volume |
| **Ellipsoid** | • Three Semi-Axes ($a, b, c$) | Semi-Axes ($a, b, c$), Volume, Knudsen High-Precision Surface Area, Principal Cross-Sectional Areas |
| **Capsule** | • Radius & Cylinder Length ($r, L$) | Radius, Diameter, Cylinder Length, Overall Length, Cylinder/Sphere Areas, Total Surface Area, Cylinder/Sphere Volumes, Total Volume |

---

## 📦 Packages & Distribution (`dist/`)

The application can be packaged into the `dist/` directory in multiple standard Windows distribution formats:

| Format | File Pattern | Description |
|---|---|---|
| **EXE (Setup)** | `dist/GeoFluent-Setup-1.0.0.exe` | Standard Windows NSIS interactive installer with start menu & desktop shortcuts |
| **EXE (Portable)**| `dist/GeoFluent-Portable-1.0.0.exe` | Standalone zero-install portable executable |
| **MSI** | `dist/GeoFluent-1.0.0.msi` | Microsoft Windows Installer package for enterprise deployment |
| **MSIX / APPX** | `dist/GeoFluent-1.0.0.msix` | Modern Windows 11 package ready for sideloading and Microsoft Store submission |

---

## 🚀 Building from Source

### Prerequisites
- Windows 10/11 (64-bit)
- [Node.js](https://nodejs.org/) (v18 or newer)
- Git

### 1. Clone the repository
```bash
git clone https://github.com/DhruvGutpa12345/GeoFluent-Geometry-Calculator.git
cd GeoFluent-Geometry-Calculator
```

### 2. Install dependencies
```bash
npm install
```

### 3. Generate high-resolution icons & assets
```powershell
powershell -ExecutionPolicy Bypass -File scripts\Run-Builder.ps1
```

### 4. Run in development mode
```bash
npm start
```

### 5. Package for Windows (`dist/`)
```bash
# Build all packages (EXE, Portable, MSI, MSIX/APPX)
npm run build

# Or build individual packages:
npm run pack:exe
npm run pack:msi
npm run pack:appx
```

---

## 🏪 Microsoft Store Submission Plan

GeoFluent is configured for submission to the **Microsoft Store**:
1. **Package Identity**:
   - `Identity Name`: `DhruvGupta.GeoFluentGeometryCalculator`
   - `Publisher`: `CN=DhruvGupta`
   - `App ID`: `GeoFluent`
2. **AppxManifest.xml**:
   - Full Trust Capability declared (`runFullTrust`).
   - Targets Windows 10/11 Desktop Family (`MinVersion="10.0.17763.0"`, `MaxVersionTested="10.0.26100.0"`).
3. **Store Assets**:
   - `Square44x44Logo.png` (App list icon)
   - `Square150x150Logo.png` (Start menu medium tile)
   - `Square310x310Logo.png` (Start menu large tile)
   - `Wide310x150Logo.png` (Start menu wide tile)
   - `StoreLogo.png` (Store listing logo)
4. **Partner Center Steps**:
   - Reserve app name in Microsoft Partner Center.
   - Upload the generated `.msix` / `.appx` package from `dist/`.
   - Fill in store descriptions, keywords, and screenshots.

---

## 🤝 Contributing

Contributions, bug reports, and feature suggestions are welcome!
1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/NewFeature`)
3. Commit your Changes (`git commit -m 'Add new geometric shape'`)
4. Push to the Branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the **MIT License**. See [`LICENSE`](LICENSE) for more information.
