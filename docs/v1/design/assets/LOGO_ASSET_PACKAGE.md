# KHADAMATI — Logo Asset Package

**Document ID:** KHAD-V1-LOGO-ASSETS  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Package structure ready — **master binaries pending deposit**  
**Governance:** Design Governance Lead  

```text
DO NOT replace the logo concept.
DO NOT generate a substitute brand mark.
Drop the official approved master files into the paths below.
```

---

## 1. Package Layout

```text
docs/v1/design/assets/
├── LOGO_ASSET_PACKAGE.md          ← this file
├── README.md
├── master/                        ← official source files (required)
│   ├── khadamati-logo-original.*  ← Original logo file
│   ├── khadamati-logo-hires.*     ← High-resolution version
│   └── khadamati-logo-transparent.* ← Transparent background version
├── variations/                    ← derived exports (after refinement)
│   ├── primary/
│   ├── compact/
│   ├── app-icon/
│   ├── dark-background/
│   └── monochrome/
└── exports/                       ← platform-ready sets (Android/iOS/web)
```

---

## 2. Master Logo (required drop-in)

| Asset | Target filename (suggested) | Received | Notes |
|-------|----------------------------|----------|-------|
| Original logo file | `master/khadamati-logo-original.{png\|svg\|pdf}` | ☐ | Source of truth |
| High-resolution version | `master/khadamati-logo-hires.png` (≥2000 px or vector) | ☐ | Print / store listing |
| Transparent background version | `master/khadamati-logo-transparent.png` / `.svg` | ☐ | Preferred for product chrome |

**Concept locked:** Home outline · service tools (wrench, brush, plug) · خدماتي / KHADAMATI · orange + navy.

---

## 3. Logo Variations — Specifications

Derivatives must be produced **from the master** (design tooling). No concept redesign.

### 3.1 Primary logo

| Field | Spec |
|-------|------|
| **Usage** | Website · Marketing · Documents |
| **Composition** | Full lockup: symbol + Arabic (خدماتي) + English (KHADAMATI) |
| **Background** | Transparent preferred; also light-BG preview |
| **Export path** | `variations/primary/` |
| **Formats** | SVG + PNG @1x/@2x/@3x |
| **Min width** | ≥ 140 CSS px digital |
| **Clear space** | ≥ 0.5× symbol height |

### 3.2 Compact logo

| Field | Spec |
|-------|------|
| **Usage** | Mobile headers · Small spaces |
| **Composition** | Symbol-only (home + tools); wordmarks optional only if width allows |
| **Export path** | `variations/compact/` |
| **Formats** | SVG + PNG |
| **Min size** | Symbol ≥ 24 px; prefer simplified strokes ≤ 32 px |

### 3.3 App icon

| Field | Spec |
|-------|------|
| **Usage** | Android · iOS |
| **Composition** | Symbol only, centered; **no** bilingual wordmark |
| **Export path** | `variations/app-icon/` + `exports/` |
| **Master** | 1024×1024 PNG |
| **Android** | Adaptive foreground + background (orange preferred) |
| **iOS** | 1024 App Store master (no transparency) |
| **Test sizes** | 29 / 40 / 60 / 80 / 1024 px readability |

### 3.4 Dark background version

| Field | Spec |
|-------|------|
| **Usage** | Dark theme surfaces · navy bands · dark marketing |
| **Composition** | Orange (or high-contrast) symbol + light wordmarks |
| **Export path** | `variations/dark-background/` |
| **Do not** | Invent a new mark; only contrast-safe treatment of official logo |

### 3.5 Monochrome version

| Field | Spec |
|-------|------|
| **Usage** | Special cases (single-color print, emboss, watermark, legal B/W) |
| **Composition** | Single-color silhouette of official home+tools (+ optional wordmark) |
| **Export path** | `variations/monochrome/` |
| **Variants** | Black-on-transparent · White-on-transparent |

---

## 4. Deposit Checklist

| Step | Owner | State |
|------|-------|-------|
| Place original / hires / transparent masters in `master/` | Design / Product | ☐ |
| Run color extraction → [`../COLOR_REFERENCE.md`](../COLOR_REFERENCE.md) | Design | ☐ |
| Produce variation exports per §3 | Design | ☐ |
| Android / iOS export sets in `exports/` | Design | ☐ |
| Design Lead verifies concept preservation | Design Governance | ☐ |

---

## 5. Forbidden

- AI-generated replacement logos  
- Removing home or tools  
- New color systems unrelated to master extraction  
- Shipping UI code that embeds non-approved marks  

---

**End of Logo Asset Package**
