# KHADAMATI V1 — Color Reference

**Document ID:** KHAD-V1-COLOR-REF  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** **Draft** — pending extraction from master logo asset  
**Governance:** Design Governance Lead  

**Related:** [`assets/LOGO_ASSET_PACKAGE.md`](./assets/LOGO_ASSET_PACKAGE.md) · [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md) · [`BRAND_IDENTITY_SPECIFICATION.md`](./BRAND_IDENTITY_SPECIFICATION.md)

```text
Do NOT guess final production colors.
Use color extraction from the approved master logo asset only.
All values remain Draft until Product approval.
```

---

## 1. Extraction Status

| Item | State |
|------|-------|
| Master logo present in `assets/master/` | ☐ **Not deposited in repository** |
| Color extraction performed | ☐ **Not performed** |
| Values status | **Draft** |
| Product color approval | ☐ Pending |

Until the official master file is deposited, **no extracted HEX values are claimed**.

Provisional values previously listed in [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md) are **not** production colors and **must not** be treated as extracted.

---

## 2. Extraction Method (when master is available)

1. Open `assets/master/khadamati-logo-original.*` (or transparent SVG/PNG).  
2. Sample **primary orange** from the dominant orange fill of the home/tools mark (average of 3 solid pixels; avoid anti-aliased edges).  
3. Sample **secondary navy** from navy fills/strokes in the mark or wordmark.  
4. Record tool used (e.g. Figma eyedropper / ImageMagick) + date + sampler name.  
5. Derive hover/light supporting tones systematically from extracted primaries (document formula).  
6. Update this file → sync tokens → Product approves.

### Suggested extraction command (after PNG master exists)

```bash
# Example only — run when master PNG is present; do not invent colors.
# identify assets/master/khadamati-logo-transparent.png
# Convert/sample solid regions via design tool preferred over CLI guessing.
```

---

## 3. Brand Core Colors

| Role | Token | Extracted HEX | Source region | Status |
|------|-------|---------------|---------------|--------|
| Primary orange | `color.brand.orange` | *PENDING_EXTRACTION* | Logo orange fill | **Draft** |
| Primary hover | `color.brand.orange-hover` | *PENDING_DERIVATION* | Darken extracted orange ~8–12% | **Draft** |
| Primary light | `color.brand.orange-light` | *PENDING_DERIVATION* | Tint of extracted orange | **Draft** |
| Secondary navy | `color.brand.navy` | *PENDING_EXTRACTION* | Logo navy / wordmark | **Draft** |
| Secondary navy soft | `color.brand.navy-soft` | *PENDING_DERIVATION* | Lifted navy | **Draft** |

---

## 4. Supporting Colors

| Role | Token | HEX | Status |
|------|-------|-----|--------|
| Error | `color.semantic.error` | *PENDING_DESIGN_SET* | **Draft** |
| Success | `color.semantic.success` | *PENDING_DESIGN_SET* | **Draft** |
| Warning | `color.semantic.warning` | *PENDING_DESIGN_SET* (must remain distinct from brand orange) | **Draft** |
| Info | `color.semantic.info` | *PENDING_DESIGN_SET* | **Draft** |
| Neutrals | `color.neutral.*` | *PENDING_DESIGN_SET* (cool undertone aligned to navy) | **Draft** |

Semantic/neutral systems may be completed after brand core extraction; they still require Product approval before implementation.

---

## 5. Background Colors

| Role | Token | HEX | Status |
|------|-------|-----|--------|
| App canvas | `color.bg.app` | *PENDING_DESIGN_SET* | **Draft** |
| Surface | `color.bg.surface` | `#FFFFFF` candidate only if logo is white-BG compatible — **confirm** | **Draft** |
| Surface muted | `color.bg.surface-muted` | *PENDING_DESIGN_SET* | **Draft** |
| Overlay scrim | `color.bg.overlay` | Derived from navy @ ~48% opacity | **Draft** |

---

## 6. Contrast Notes (post-extraction)

| Pair | Requirement | Result |
|------|-------------|--------|
| Orange CTA text on orange | Prefer white label; verify AA for size | ☐ |
| Navy text on white | AA for body | ☐ |
| White wordmark on navy band | AA | ☐ |
| Orange symbol on navy (dark logo version) | Visible at small size | ☐ |

---

## 7. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Design Lead | | | ☐ Approve extracted values |
| Product Owner | | | ☐ Approve production colors |

**Values remain Draft until both approvals are recorded.**

---

**End of Color Reference v1.0**
