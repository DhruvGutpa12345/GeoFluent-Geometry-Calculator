# GeoFluent — Geometry Calculator for Windows 11

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2011%20%7C%20Windows%2010%20(1809%2B)-0078D4.svg)](https://microsoft.com)
[![Framework](https://img.shields.io/badge/Framework-WinUI%203%20%7C%20Windows%20App%20SDK%202.x-60CDFF.svg)](https://github.com/microsoft/WindowsAppSDK)
[![Runtime](https://img.shields.io/badge/Runtime-.NET%2010%20LTS%20%7C%20C%23%2014-512BD4.svg)](https://dotnet.microsoft.com)

**GeoFluent** is an open-source, high-precision geometry computation suite engineered natively for Windows 11 using **C# 14**, **WinUI 3**, and the **Windows App SDK 2.x**.

Designed for students, educators, engineers, and mathematics enthusiasts, GeoFluent solves complex 2D planar and 3D solid geometry problems with step-by-step mathematical derivations, interactive real-time visual canvases, and multi-unit conversions.

---

## 🌟 Key Features

- **17 2D Planar Shapes:** Triangle (5 calculation methods), Square, Rectangle, Circle, Ellipse, Parallelogram, Rhombus, Trapezoid, Regular Polygon (n-gon), Circular Sector & Segment, Annulus, Kite, Isosceles Trapezoid, Star Polygon, and Cardioid.
- **18 3D Solid Shapes:** Cube, Rectangular Cuboid, Sphere, Hemisphere, Cylinder, Hollow Cylinder, Cone, Frustum, Square Pyramid, Triangular Prism, Torus, Ellipsoid, Capsule, Spherical Cap, Regular Tetrahedron, Octahedron, Icosahedron, and Dodecahedron.
- **Step-by-Step Derivations:** Mathematical proofs displaying substituted numerical formulas with dynamic precision rounding.
- **Universal Unit Converter:** Seamless length, area, volume, and angle conversions across metric and imperial systems.
- **Windows 11 Fluent 2 Design:** Native `MicaBackdrop`, `SystemBackdropElement` cards, custom WinUI `TitleBar`, Segoe Fluent Icons, and dark/light/system theming.
- **Zero-Bloat Native Architecture:** 100% native desktop application with zero web wrappers, sub-100ms startup latency, and 120Hz smooth scrolling.

---

## 🚀 Building & Packaging

### Prerequisites
- Windows 10 (Build 17763+) or Windows 11 (Build 22621+ recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`10.0.13` or newer)
- Visual Studio 2022 / 2026 or Windows App SDK Build Tools

### Dual Build Pipeline
To produce both the portable unpackaged executable and the Microsoft Store package:

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1 -Target all
```

- **Portable Unpackaged `.exe`:** Output located in `dist/portable/GeoFluent.exe` (Standalone, zero-installer execution).
- **Microsoft Store `.msix`:** Output located in `dist/msix/` (Store-ready submission package).

---

## 📐 Architecture & Agent Rules

This repository follows strict architectural rules governed by [AGENTS.md](./AGENTS.md). All AI coding agents and human contributors must adhere to:
- Native C# 14 / WinUI 3 (Windows App SDK 2.x) ONLY.
- Central Package Management in `Directory.Packages.props`.
- Automatic version increment on every recompile via `Directory.Build.targets`.
- Modular rules located in `.agents/rules/` and skills in `.agents/skills/`.

---

## 📄 License & Attribution

- Distributed under the [MIT License](LICENSE).
- Visual assets created using **VectorCraft** (ArtCraft Crafting Suite). See [ASSETS.md](ASSETS.md) and [NOTICE](NOTICE).
