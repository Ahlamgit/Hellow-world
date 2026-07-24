# KHADAMATI V1 — Design System Tokens

**Document ID:** KHAD-V1-DESIGN-TOKENS  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Ready for approval — color authority is [`COLOR_REFERENCE.md`](./COLOR_REFERENCE.md) (**Draft** vs **Approved**; Approved empty)  
**Related:** [`BRAND_IDENTITY_SPECIFICATION.md`](./BRAND_IDENTITY_SPECIFICATION.md) · [`UI_UX_SPECIFICATION.md`](./UI_UX_SPECIFICATION.md) · [`DESIGN_APPROVAL_RECORD.md`](./DESIGN_APPROVAL_RECORD.md)

```text
Tokens are a design foundation. DO NOT implement UI components yet.
Do NOT guess production colors.
Use COLOR_REFERENCE Approved values only after Product + Design approval.
After BLOCKER-001 COMPLETED, token changes require Design Change Request.
```

---

## 1. Color Tokens

### 1.1 Brand core (from logo)

| Token | Provisional hex | Role |
|-------|-----------------|------|
| `color.brand.orange` | `#F15A24` | **KHADAMATI Orange** — primary action |
| `color.brand.orange-hover` | `#D94E1C` | Primary pressed / hover |
| `color.brand.orange-light` | `#FFE8DE` | Soft fills, selected chips, tinted surfaces |
| `color.brand.navy` | `#0F2747` | **Secondary navy** — trust, headings, chrome |
| `color.brand.navy-soft` | `#1C3A5F` | Elevated navy, dark header bands |

> **Action required:** Replace provisional hex with values sampled from the official logo master once committed to [`assets/`](./assets/).

### 1.2 Neutrals

| Token | Provisional hex | Role |
|-------|-----------------|------|
| `color.neutral.900` | `#0B1220` | Primary text on light |
| `color.neutral.700` | `#3A4558` | Secondary text |
| `color.neutral.500` | `#6B7689` | Tertiary / placeholders |
| `color.neutral.300` | `#C5CCD8` | Borders |
| `color.neutral.200` | `#E4E8F0` | Dividers |
| `color.neutral.100` | `#F2F4F8` | Subtle fills |
| `color.neutral.0` | `#FFFFFF` | Pure white |

### 1.3 Background & surface (light theme — required)

| Token | Provisional hex | Role |
|-------|-----------------|------|
| `color.bg.app` | `#F7F8FB` | App / portal canvas |
| `color.bg.surface` | `#FFFFFF` | Cards / sheets / panels |
| `color.bg.surface-muted` | `#F2F4F8` | Nested regions |
| `color.bg.overlay` | `rgba(15,39,71,0.48)` | Modal scrim (navy-based) |

### 1.4 Semantic

| Token | Provisional hex | Role |
|-------|-----------------|------|
| `color.semantic.error` | `#D92D20` | Errors, destructive |
| `color.semantic.error-bg` | `#FEE4E2` | Error soft |
| `color.semantic.success` | `#099250` | Success, completed |
| `color.semantic.success-bg` | `#DCFAE6` | Success soft |
| `color.semantic.warning` | `#DC6803` | Warnings (distinct from brand orange) |
| `color.semantic.warning-bg` | `#FEF0C7` | Warning soft |
| `color.semantic.info` | `#175CD3` | Informational |
| `color.semantic.info-bg` | `#D1E9FF` | Info soft |

### 1.5 Dark theme slots (conditional — ADR-018)

Light theme is **required** for V1. Full dark UI ships only if Product expands approved dark UI assets.  

Reserved slots (do not invent a parallel brand):

| Token | Intent |
|-------|--------|
| `color.bg.app.dark` | Near-navy canvas |
| `color.bg.surface.dark` | Elevated navy-soft |
| `color.text.primary.dark` | Near-white |
| `color.brand.orange` | **Unchanged** — primary CTA on dark |

Logo **dark-background version** is specified in Brand Identity even if full app dark theme is deferred.

---

## 2. Typography Tokens

### 2.1 Font families

| Token | Recommendation | Use |
|-------|----------------|-----|
| `font.family.ar` | **IBM Plex Sans Arabic** (fallback: Noto Sans Arabic) | Arabic UI |
| `font.family.en` | **IBM Plex Sans** | English UI — pairs with Arabic family |
| `font.family.mono` | IBM Plex Mono | Admin IDs, money debug (rare) |

Rationale: bilingual optical harmony, premium readability, not generic Inter/Roboto defaults.

### 2.2 Type scale (mobile base; web +10–15% for dashboards where noted)

| Token | Size | Weight | Line height | Use |
|-------|------|--------|-------------|-----|
| `type.display` | 32 / 36 | 700 | 1.2 | Rare marketing |
| `type.h1` | 28 | 700 | 1.25 | Screen titles |
| `type.h2` | 22 | 650–700 | 1.3 | Section titles |
| `type.h3` | 18 | 600 | 1.35 | Card titles |
| `type.body` | 16 | 400–500 | 1.5 | Body |
| `type.body-sm` | 14 | 400–500 | 1.45 | Secondary |
| `type.caption` | 12 | 400–500 | 1.4 | Meta, timestamps |
| `type.button` | 16 | 600 | 1.2 | Buttons |
| `type.button-sm` | 14 | 600 | 1.2 | Compact buttons |
| `type.overline` | 11–12 | 600 | 1.3 | Labels (uppercase EN only; never force Arabic uppercase) |

Arabic: prefer slightly looser line-height (+0.05) for Naskh/Sans Arabic glyphs.

---

## 3. Spacing Tokens

Base unit: **4**

| Token | Value |
|-------|-------|
| `space.0` | 0 |
| `space.1` | 4 |
| `space.2` | 8 |
| `space.3` | 12 |
| `space.4` | 16 |
| `space.5` | 20 |
| `space.6` | 24 |
| `space.8` | 32 |
| `space.10` | 40 |
| `space.12` | 48 |
| `space.16` | 64 |

Screen padding mobile: `space.4` (16). Dashboard content: `space.6`–`space.8`.

---

## 4. Radius Tokens

| Token | Value | Use |
|-------|-------|-----|
| `radius.sm` | 8 | Inputs, chips |
| `radius.md` | 12 | Cards, sheets |
| `radius.lg` | 16 | Large panels, hero search |
| `radius.pill` | 999 | Use sparingly (filters only when needed) |
| `radius.icon` | 12 | App icon mask reference |

Avoid “pill cluster” clutter in marketing heroes (product design rules).

---

## 5. Elevation Tokens

| Token | Value (light) | Use |
|-------|---------------|-----|
| `elevation.0` | none | Flat |
| `elevation.1` | `0 1px 2px rgba(15,39,71,0.06)` | Subtle |
| `elevation.2` | `0 4px 12px rgba(15,39,71,0.08)` | Cards that need lift for interaction |
| `elevation.3` | `0 8px 24px rgba(15,39,71,0.12)` | Sheets / dialogs |

Prefer border + soft elevation over heavy multi-layer shadows.

---

## 6. Motion Tokens

| Token | Duration | Easing | Use |
|-------|----------|--------|-----|
| `motion.fast` | 120ms | standard | Press feedback |
| `motion.base` | 200ms | standard | Tabs, expands |
| `motion.enter` | 280ms | ease-out | Sheets |
| `motion.page` | 320ms | ease-in-out | Screen transitions |

Intentional motions (min 2–3 in visually led flows): search focus expand, booking step progress, sheet present.

---

## 7. Z-Index Tokens

| Token | Value |
|-------|-------|
| `z.base` | 0 |
| `z.sticky` | 10 |
| `z.dropdown` | 20 |
| `z.sheet` | 30 |
| `z.modal` | 40 |
| `z.toast` | 50 |

---

## 8. Iconography Tokens

| Token | Guidance |
|-------|----------|
| `icon.size.sm` | 16 |
| `icon.size.md` | 20–24 |
| `icon.size.lg` | 32 |
| Stroke | 1.5–2 px consistent set |
| Direction | Mirror start/end chevrons in RTL |

Brand mark ≠ UI icon set. UI icons should be simple line icons; do not redraw the logo as every list glyph.

---

## 9. Breakpoints

| Token | Min width | Use |
|-------|-----------|-----|
| `bp.phone` | 0 | Customer / Craftsman default |
| `bp.tablet` | 768 | Large phone / tablet |
| `bp.desktop` | 1024 | Store / Admin |
| `bp.wide` | 1280 | Admin dense tables |

---

## 10. Token Naming Convention (for future implementation)

```text
{category}.{variant}.{state?}
color.brand.orange
color.brand.orange-hover
type.h1
space.4
```

Implementation mapping (Flutter ThemeExtension / MUI theme) is **out of scope** until Implementation Gate → A.

---

## 11. Confirmation Checklist

| Item | State |
|------|-------|
| Hex sampled from official logo master | ☐ |
| Tokens linked in UI/UX Spec | ☑ |
| Dark UI theme approved for V1 ship | ☐ / N/A per ADR-018 |
| Design Lead approve tokens | ☐ |

---

**End of Design System Tokens v1.0**
