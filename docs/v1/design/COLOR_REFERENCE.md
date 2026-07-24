# KHADAMATI V1 — Color Reference

**Document ID:** KHAD-V1-COLOR-REF  
**Version:** 1.1  
**Date:** 2026-07-24  
**Status:** **Draft values only** — Approved values empty until master extraction + Product approval  
**Governance:** Design Governance Lead  

**Related:** [`assets/LOGO_ASSET_PACKAGE.md`](./assets/LOGO_ASSET_PACKAGE.md) · [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md) · [`DESIGN_APPROVAL_RECORD.md`](./DESIGN_APPROVAL_RECORD.md)

```text
Production HEX values must come ONLY from approved master logo extraction.
No guessed colors.
Draft values ≠ Approved values.
```

---

## 1. Extraction & Approval Status

| Item | State |
|------|-------|
| Master logo in `assets/master/original/` (or transparent) | ☐ **Not deposited** |
| Color extraction performed | ☐ **Not performed** |
| Draft section | Active (placeholders only) |
| Approved section | **Empty** — not yet authorized for implementation |
| Product color approval | ☐ Pending |
| Design color approval | ☐ Pending |

Until extraction + dual approval, implementation must **not** hardcode colors as production truth.

Token file provisional HEX (if any) are **Draft context only** — not Approved.

---

## 2. Extraction Method (required before Approved values)

1. Open approved master from `assets/master/original/` or `assets/master/transparent/`.  
2. Sample **Primary Orange** from solid orange fill (avoid anti-aliased edges; average 3 samples).  
3. Sample **Secondary Navy** from navy fill/stroke / wordmark.  
4. Record tool, sampler, date.  
5. Derive hover/light and neutrals systematically; document formulas.  
6. Copy confirmed values into **§4 Approved values** only after Product + Design sign-off.  

---

## 3. Draft Values

> **Draft** = working placeholders / pending extraction. **Not** production authority.

### 3.1 Primary Orange

| Role | Token | Draft HEX | Notes |
|------|-------|-----------|-------|
| Primary Orange | `color.brand.orange` | *PENDING_EXTRACTION* | From master logo only |
| Primary hover | `color.brand.orange-hover` | *PENDING_DERIVATION* | After orange extracted |
| Primary light | `color.brand.orange-light` | *PENDING_DERIVATION* | After orange extracted |

### 3.2 Secondary Navy

| Role | Token | Draft HEX | Notes |
|------|-------|-----------|-------|
| Secondary Navy | `color.brand.navy` | *PENDING_EXTRACTION* | From master logo only |
| Navy soft | `color.brand.navy-soft` | *PENDING_DERIVATION* | After navy extracted |

### 3.3 Neutral palette

| Role | Token | Draft HEX | Status |
|------|-------|-----------|--------|
| Neutral 900 | `color.neutral.900` | *PENDING_DESIGN_SET* | **Draft** |
| Neutral 700 | `color.neutral.700` | *PENDING_DESIGN_SET* | **Draft** |
| Neutral 500 | `color.neutral.500` | *PENDING_DESIGN_SET* | **Draft** |
| Neutral 300 | `color.neutral.300` | *PENDING_DESIGN_SET* | **Draft** |
| Neutral 200 | `color.neutral.200` | *PENDING_DESIGN_SET* | **Draft** |
| Neutral 100 | `color.neutral.100` | *PENDING_DESIGN_SET* | **Draft** |
| Neutral 0 | `color.neutral.0` | *PENDING_DESIGN_SET* | **Draft** |

### 3.4 Background palette

| Role | Token | Draft HEX | Status |
|------|-------|-----------|--------|
| App background | `color.bg.app` | *PENDING_DESIGN_SET* | **Draft** |
| Surface | `color.bg.surface` | *PENDING_DESIGN_SET* | **Draft** |
| Surface muted | `color.bg.surface-muted` | *PENDING_DESIGN_SET* | **Draft** |
| Overlay | `color.bg.overlay` | *PENDING_DERIVATION* | **Draft** |

### 3.5 Status colors

| Role | Token | Draft HEX | Status |
|------|-------|-----------|--------|
| Error | `color.semantic.error` | *PENDING_DESIGN_SET* | **Draft** |
| Success | `color.semantic.success` | *PENDING_DESIGN_SET* | **Draft** |
| Warning | `color.semantic.warning` | *PENDING_DESIGN_SET* | **Draft** — must stay distinct from brand orange |
| Info | `color.semantic.info` | *PENDING_DESIGN_SET* | **Draft** |

---

## 4. Approved Values

> Fill **only** after master extraction and Product + Design approval.  
> Until then, this section remains intentionally empty of production HEX.

### 4.1 Primary Orange — Approved

| Role | Token | Approved HEX | Approved by | Date |
|------|-------|--------------|-------------|------|
| Primary Orange | `color.brand.orange` | — | | |
| Primary hover | `color.brand.orange-hover` | — | | |
| Primary light | `color.brand.orange-light` | — | | |

### 4.2 Secondary Navy — Approved

| Role | Token | Approved HEX | Approved by | Date |
|------|-------|--------------|-------------|------|
| Secondary Navy | `color.brand.navy` | — | | |
| Navy soft | `color.brand.navy-soft` | — | | |

### 4.3 Neutral palette — Approved

| Role | Token | Approved HEX | Approved by | Date |
|------|-------|--------------|-------------|------|
| Neutral 900…0 | `color.neutral.*` | — | | |

### 4.4 Background palette — Approved

| Role | Token | Approved HEX | Approved by | Date |
|------|-------|--------------|-------------|------|
| Backgrounds | `color.bg.*` | — | | |

### 4.5 Status colors — Approved

| Role | Token | Approved HEX | Approved by | Date |
|------|-------|--------------|-------------|------|
| Error / Success / Warning / Info | `color.semantic.*` | — | | |

**Approved section authority:** Empty = no production color freeze yet.

---

## 5. Contrast Checks (after Approved values exist)

| Pair | Requirement | Result |
|------|-------------|--------|
| Orange CTA label on orange | Prefer white; AA where applicable | ☐ |
| Navy text on white | AA body | ☐ |
| White wordmark on navy | AA | ☐ |
| Orange symbol on dark variation | Small-size visible | ☐ |

---

## 6. Color Approval Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| Design Lead | | | ☐ Approve extracted → Approved section |
| Product Owner | | | ☐ Approve production colors |

---

**End of Color Reference v1.1**
