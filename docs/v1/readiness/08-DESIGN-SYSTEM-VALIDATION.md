# Design System Validation (Pre-Implementation)

**Document ID:** KHAD-V1-DESIGN-SYSTEM  
**Status:** Architecture validated; visual tokens pending design assets (ADR-010, ADR-018)

---

## Must Support

| Capability | V1 Requirement |
|------------|----------------|
| Arabic RTL | **Required** (primary) |
| English LTR | **Required** (secondary) |
| Light theme | **Required** |
| Dark theme | **Conditional** — only if design assets include it (ADR-018) |

---

## Component Inventory (Required in Design System)

Buttons · Inputs · Cards · Tables · Charts · Dialogs · Bottom sheets · Navigation (tabs/side/rail) · FAB · Toast · Badges/Chips · Loading · Empty · Error · Skeleton loaders

---

## Design Principles

Premium · Modern · Minimal · Professional · Fast · Accessible

### Visual rules (Master Prompt)

- Corner radius 12–20px  
- Soft shadows  
- 8-point spacing  
- Motion 150–250ms  
- Large touch targets  
- Accessible contrast (WCAG-oriented)  

---

## Engineering Mapping

| Layer | Package |
|-------|---------|
| Web tokens/components | `packages/ui` + MUI theme overrides |
| Flutter tokens/components | `khad_ui` |

No generic template UI. No pixel-clone without analysis.
