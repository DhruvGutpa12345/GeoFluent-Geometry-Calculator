---
name: artcraft-assets
description: Procedure for generating Windows 11 Fluent vector badges, icons, and tile assets using the ArtCraft suite via MCP.
---

# ArtCraft Asset Generation Skill

This skill documents the repeatable procedure for generating scalable vector graphics and pixel-perfect Windows 11 tile assets using VectorCraft and PhotoCraft.

## Supported MCP Tools
- **VectorCraft:** `vectorcraft-cli mcp` (default bridge at `127.0.0.1:7979`)
  - Commands: `file.new`, `draw_shape`, `draw_path`, `set_paint`, `export`
- **PhotoCraft:** `photocraft-cli mcp`
  - Commands: raster filtering, format conversions

## Procedure

### Step 1: Create Master Vector Canvas
Use VectorCraft MCP to initialize a 512×512 document:
```json
{
  "command": "file.new",
  "params": { "width": 512, "height": 512 }
}
```

### Step 2: Draw Precision Geometry Layers
1. **Background Squircle:** Rounded rectangle (`#13171F`, radius 100).
2. **3D Solid Base:** Isometric cube faces (`#3B82F6`, `#1D4ED8`, `#1E3A8A`).
3. **2D Plane Overlay:** Intersecting cyan equilateral triangle (`#00F0FF`).
4. **Precision Mathematical Markings:** Dashed golden inscribed circle (`#F59E0B`), altitude lines, angle arcs, and vertex measurement nodes.

### Step 3: Export Master Visuals
Export SVG and high-resolution PNG:
- Master Vector: `assets/icon.svg`
- Master Raster: `assets/icon.png` (512×512)

### Step 4: Generate Windows 11 Tile Matrix
Generate all required on-device tile resolutions into `build/appx/` and `assets/store/`:
- `Square150x150Logo.png` (150×150 px)
- `Square44x44Logo.png` (44×44 px)
- `Wide310x150Logo.png` (310×150 px)
- `Square310x310Logo.png` / `LargeTile.png` (310×310 px)
- `Square71x71Logo.png` / `SmallTile.png` (71×71 px)
- `StoreLogo.png` (50×50 px)
- `SplashScreen.png` (620×300 px)
- `BadgeLogo.png` (24×24 px)

### Step 5: Log Asset Metadata
Record all generated files in `ASSETS.md` and `NOTICE`.
