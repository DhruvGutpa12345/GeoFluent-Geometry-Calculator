---
trigger: model_decision
description: ArtCraft Crafting Suite integration guidelines, MCP execution safety, and asset attribution.
---

# ArtCraft Crafting Suite MCP Integration

## 1. 13-App Rust-Native Crafting Suite Overview
- **Visuals & UI:** `VectorCraft` (vector icons, SVG badges, scalable tiles), `PhotoCraft` (raster graphics, photo edits), `FigCraft` (UI mockups), `LightCraft` (color grading), `DesignCraft` (graphic layout).
- **Documents & Data:** `PdfCraft` (PDF publishing), `WordCraft` (word processing), `DeckCraft` (slides), `GridCraft` (data tables).
- **Media & CAD:** `FilmCraft` (video editing), `EffectCraft` (motion graphics), `SoundCraft` (audio processing), `CADCraft` (precision 3D/CAD modeling).

## 2. MCP Scoping & Safe Automation
- File access must be scoped to the workspace directory:
  `--automation-read-root <workspace> --automation-write-root <workspace>` (or port control via `--control <port>`).
- Execute MCP commands sequentially:
  - Always verify `file.new` / `doc_open` finishes before adding shapes, paths, or text.
  - In `PhotoCraft`, ensure an active raster pixel layer before running filters.
  - Verify exported files exist and check dimensions after export.

## 3. Attribution Mandate
- Any visual, audio, or document asset produced by ArtCraft or external sources must be registered in:
  1. `ASSETS.md` (asset path, source tool, dimensions, license).
  2. `NOTICE` (third-party and open-source attributions).
