# KHADAMATI V1 — Brand Identity Specification

**Document ID:** KHAD-V1-BRAND-ID  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Asset received — specification in progress (BLOCKER-001)  
**Role:** Lead Product Designer & UX Architect  

**Related:** [`UI_UX_SPECIFICATION.md`](./UI_UX_SPECIFICATION.md) · [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md) · ADR-010 · ADR-023  

```text
DO NOT replace the logo concept.
DO NOT create a different brand identity.
Refinement only — preserve Home + tools + services meaning.
DO NOT implement UI or generate application screens.
```

---

## 1. Official Brand Asset

The uploaded logo is the **official KHADAMATI logo** and the primary visual identity reference.

### Brand names

| Language | Name |
|----------|------|
| Arabic | **خدماتي** |
| English | **KHADAMATI** |

### Symbol meaning

| Element | Meaning |
|---------|---------|
| Home / house silhouette | Household & local services marketplace |
| Wrench | Professional crafts / repair |
| Paint brush | Finishing & home improvement trades |
| Electrical plug | Technical / electrical services |
| Orange primary | Energy, accessibility, action |
| Dark navy secondary | Trust, reliability, professionalism |

### Brand attributes (locked)

Trust · Home services · Professional craftsmen · Reliability · Fast service access · Local marketplace identity  

### Asset custody

| Item | Guidance |
|------|----------|
| Master file location | Place official masters under [`assets/`](./assets/) (`khadamati-logo-primary.*`, icon exports) |
| Concept | **Locked** — Home + tools + bilingual wordmark |
| Redesign | **Forbidden** for V1 |
| Color sampling | Final hex values in tokens must be **color-picked from the master logo file** when committed to `assets/` |

---

## 2. Logo Usage

### 2.1 Primary logo version

**Use:** Marketing, onboarding splash, web headers (wide), store/admin login brand lockups  

**Composition:** Home + tools symbol + Arabic wordmark (خدماتي) + English wordmark (KHADAMATI)  

**Background:** Prefer white / light surfaces (logo designed for white-background compatibility)  

**Clear space:** Minimum clear space = height of the home symbol’s roof peak (or ≥ 0.5× symbol height on all sides)  

**Minimum width (full lockup):** ≥ 140 CSS px (digital); do not compress below readability of Arabic script  

---

### 2.2 Compact mobile icon version

**Use:** App bars, tab contexts where space is limited, map pins overlays (if needed), dense lists  

**Composition:** **Symbol only** (home + tools) — omit wordmarks when width < ~96 px  

**Requirement:** Tools remain recognizable as a set; do not crop the house silhouette  

---

### 2.3 App icon version

**Use:** Android launcher · iOS home screen · store listings  

See §5 Mobile App Icon Specification (also mirrored in UI/UX Spec).  

**Composition:** Centered symbol on orange field **or** symbol with orange accent on navy field — choose one primary system after export test at 48 / 72 / 1024 px (prefer orange field for marketplace recognition).  

**Do not** place full bilingual wordmark inside the app icon.  

---

### 2.4 Web header version

**Use:** Admin Portal · Store Dashboard · marketing site header  

**Composition:** Horizontal lockup — symbol left (LTR) / symbol right-aware in RTL · wordmarks beside symbol  

**Height:** 32–40 px symbol height in product chrome; scale wordmarks optically to match  

**Behavior:** On scroll-compact headers, collapse to compact symbol if needed  

---

### 2.5 Dark background version

**Use:** Navy / dark charcoal surfaces, splash on dark, premium promo bands  

| Element | Treatment |
|---------|-----------|
| Symbol | Prefer **orange** home+tools (or white outline only if contrast fails — avoid inventing a new mark) |
| Wordmarks | **White** or light neutral |
| Do not | Recolor tools to low-contrast gray on navy |

---

### 2.6 Light background version

**Use:** Default product UI, white cards, login, certificates  

| Element | Treatment |
|---------|-----------|
| Symbol | Official orange + navy as in master |
| Wordmarks | Navy (or near-black) for English/Arabic |
| Background | White / off-white |

---

### 2.7 Usage matrix

| Context | Version | Notes |
|---------|---------|-------|
| Customer app splash | Primary or symbol + Arabic first | Arabic-default market |
| Customer app bar | Compact symbol | Optional small wordmark on large phones |
| Craftsman app | Same system | Professional, quieter chrome |
| Store web header | Web header lockup | |
| Admin web header | Web header lockup | Neutral density |
| Favicon | Compact symbol | Simplified if needed |
| Push / notification small | Compact / monochrome if OS requires | Preserve silhouette |

### 2.8 Don’ts

- Do not stretch, rotate, or recolor arbitrarily  
- Do not separate tools from the home into a new mark  
- Do not replace tools with unrelated icons  
- Do not stack decorative badges on the logo  
- Do not use low-contrast orange-on-orange  

---

## 3. Logo Refinement Analysis

### 3.1 Evaluation (against official concept)

| Criterion | Assessment | Refinement direction |
|-----------|------------|----------------------|
| Shape balance | Home silhouette carries brand; tools add density | Keep house as dominant outer shape; keep tools visually secondary inside |
| Icon proportions | Three tools risk clutter at small sizes | Slightly unify stroke weight; ensure plug/wrench/brush share optical weight |
| Typography alignment | Bilingual lockup must not fight the symbol | Align baselines; prefer Arabic primacy in Lebanon market layouts |
| Arabic / English relationship | Both required for trust & clarity | Arabic above or leading in RTL; English as supportive companion (smaller or secondary line) |
| Small-size readability | Fine tool detail may muddy < 32 px | Provide **simplified symbol** export for ≤ 32 px (fewer internal cuts, thicker strokes) |
| Mobile app icon suitability | Home shape reads well in circle/squircle | Test safe-zone padding; avoid edge clipping of roof |

### 3.2 Allowed improvements (refinement only)

- Simplify internal tool detail for small-screen / app-icon exports  
- Improve spacing between symbol and wordmarks  
- Harmonize stroke consistency across wrench / brush / plug  
- Improve visual balance of bilingual stack  
- Optical kerning for KHADAMATI caps  

### 3.3 Forbidden changes

- Removing home silhouette  
- Removing the tools set (or replacing with unrelated metaphor)  
- New mascot, gradient-only abstract mark, or purple/generic SaaS rebrand  
- Dropping Arabic or English from the **primary** brand lockup system (compact icon may omit text)  

### 3.4 Export checklist (design production — not app code)

| Export | Format | Status |
|--------|--------|--------|
| Primary full color (light BG) | SVG + PNG @2x/@3x | ☐ Pending master commit |
| Primary for dark BG | SVG + PNG | ☐ |
| Compact symbol | SVG + PNG | ☐ |
| Simplified small symbol (≤32 px) | SVG + PNG | ☐ |
| App icon 1024 master | PNG | ☐ |
| Android adaptive layers | Foreground + background | ☐ |
| iOS App Icon set | Per Apple sizes | ☐ |
| Monochrome / single-color | SVG | ☐ Optional |

---

## 4. Brand Color Direction (from logo)

| Role | Direction | Token home |
|------|-----------|------------|
| Primary | **KHADAMATI Orange** (action, CTAs, key accents) | [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md) |
| Secondary | **Dark navy** (trust, text emphasis, headers) | same |
| Neutrals | Cool grays aligned to navy undertone | same |
| Surfaces | White-first; soft cool off-white | same |

Exact hex: sample from master logo file; provisional values documented in tokens until sampling confirmed.

---

## 5. Mobile App Icon Specification

### Goals

- Recognizable at small size (iOS Spotlight / Android mdpi)  
- Uses KHADAMATI **symbol** (home + tools)  
- Orange identity preserved  
- Works on light and dark home-screen wallpapers  

### Android

| Item | Spec |
|------|------|
| Master | 1024×1024 foreground art + solid/adaptive background |
| Adaptive | Safe zone: keep symbol within center ~66% |
| Background | KHADAMATI Orange (preferred) or navy with orange symbol |
| Shape | System mask (circle / squircle / rounded square) — design for all |
| Legacy | mdpi→xxxhdpi mipmaps from master |

### iOS

| Item | Spec |
|------|------|
| Master | 1024×1024 (no transparency for App Store) |
| Corner radius | System-applied — do not bake incorrect radius |
| Content | Symbol centered with padding ≥ 10% |
| Alternate | Optional dark-mode appearance asset if provided later |

### Recognition test

| Size | Pass criteria |
|------|---------------|
| 29–40 px | House silhouette readable |
| 60–80 px | At least two tools implied |
| 1024 px | Full tool set crisp |

---

## 6. Voice of Brand in Product UI (non-copy deck)

| Do | Don’t |
|----|-------|
| Calm navy + decisive orange CTAs | Neon / playful clutter |
| Service-first headlines | Provider-type-first choosers as the hero |
| Clear trust badges | Fake urgency stickers on hero media |
| Short Arabic-first microcopy | Dense English-only admin tone in customer app |

---

## 7. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Design Lead | | | ☐ Approve brand refinement direction |
| Product Owner | | | ☐ Approve (concept preserved) |
| Solution Architect | | | ☐ Acknowledge (no scope/architecture impact) |

---

**End of Brand Identity Specification v1.0**
