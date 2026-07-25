# BLOCKER-001 — Design Asset Reference

| Field | Value |
|-------|-------|
| **Document ID** | EVD-001-ASSETS-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-001 — Design |
| **Evidence status** | **Approved** — evidence archival pending |
| **Approver (business)** | Project Owner / Business Owner |

---

## 1. Approved reference assets

| Asset | Filename | Purpose | Archive path | On file |
|-------|----------|---------|--------------|---------|
| Design theme video | `theme(1).mp4` | Approved mobile application visual direction | `reference-assets/theme(1).mp4` | Pending upload |
| Brand logo reference | `ic-khadamati(1).jpg` | Official KHADAMATI brand identity reference | `reference-assets/ic-khadamati(1).jpg` | Pending upload |

**Rule:** Do **not** redesign the logo in governance — document refinement requirements only (`LOGO_REFINEMENT_SPECIFICATION.md`).

---

## 2. Color system (direction)

| Element | Direction |
|---------|-----------|
| Primary brand | **Orange** — marketplace identity per approved theme |
| Neutrals | Clean minimal palette aligned with `theme(1).mp4` |
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
