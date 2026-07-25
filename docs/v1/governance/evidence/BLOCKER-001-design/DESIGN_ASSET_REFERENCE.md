# BLOCKER-001 — Design Asset Reference

| Field | Value |
|-------|-------|
| **Document ID** | EVD-001-ASSETS-001 |
| **Version** | 1.2 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-001 — Design |
| **Evidence status** | **Approved** — archive filenames standardized |
| **Approver (business)** | Project Owner / Business Owner |

---

## 1. Approved reference assets (repository archive names)

| Asset | Archive filename | Archive path | Original delivery name |
|-------|------------------|--------------|------------------------|
| Design theme video | `theme.mp4` | `reference-assets/theme.mp4` | `theme.mp4` (PO/BO approved) |
| Brand logo reference | `khadamatiLogo.jpg` | `reference-assets/khadamatiLogo.jpg` | `khadamatiLogo.jpg` (PO/BO approved) |

**Same approved content** — filenames standardized for repository and tooling. Identity and visual direction unchanged.

**Rule:** Do **not** redesign the logo — refine only (`LOGO_REFINEMENT_SPECIFICATION.md`).

---

## 2. Color system (direction)

| Element | Direction |
|---------|-----------|
| Primary brand | **Orange** — marketplace identity per approved theme |
| Neutrals | Clean minimal palette aligned with `theme.mp4` |
| Semantic colors | Trust-focused; accessible contrast for mobile |
| Configuration | Design tokens — not hardcoded business values |

**Design Lead** to finalize token values in design system package.

---

## 3. Typography direction

| Requirement | Detail |
|-------------|--------|
| Arabic | RTL-optimized type scale and line height |
| English | LTR-optimized type scale |
| Hierarchy | Mobile-first heading and body scales |
| Consistency | Shared tokens across Customer / Provider / Admin surfaces |

---

## 4. Localization & layout

| Requirement | Mandate |
|-------------|---------|
| Arabic | **RTL** — primary launch language support |
| English | **LTR** — secondary language support |
| Mobile-first | Customer and Provider apps primary; Admin web responsive |
| Multi-country | Architecture multi-region ready; Lebanon defaults configurable |

---

## 5. Related evidence

| Document | ID |
|----------|-----|
| `LOGO_REFINEMENT_SPECIFICATION.md` | EVD-001-LOGO-001 |
| `DESIGN_APPROVAL_RECORD.md` | EVD-001-DESIGN-APPROVAL-001 |
| `DESIGN_GOVERNANCE_RECORD.md` | EVD-001-GOV-001 |
| `BUSINESS_APPROVAL_RECORD.md` | EVD-001-BUSINESS-001 |

**BLOCKER-001 NOT CLOSED** until checklist complete per GOV-BEMF-001.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.1 | 2026-07-25 | Initial asset reference |
| 1.2 | 2026-07-25 | Standardized archive filenames: `theme.mp4`, `khadamatiLogo.jpg` |
