# KHADAMATI — Design Approval Package

| Field | Value |
|-------|-------|
| **Document ID** | EVD-001-PKG-001 |
| **Blocker** | BLOCKER-001 — Design Approval |
| **Version** | 1.0 |
| **Status** | **READY FOR APPROVAL** (signatures and assets pending) |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Product Design Governance Manager |
| **Date** | 2026-07-25 |
| **Closure artifact (on completion)** | `DESIGN_APPROVAL_SIGNOFF_v1.0.md` |

---

## Authority and Purpose

This package is the formal design approval instrument required before **any** KHADAMATI UI implementation (Customer App, Craftsman App, Store Dashboard, Admin Portal).

**Source of truth:**

- `BRAND_IDENTITY_SPECIFICATION.md`
- `UI_UX_SPECIFICATION.md`
- `DESIGN_SYSTEM_TOKENS.md`
- `FINAL_SCOPE_BASELINE.md`
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- ADR-023 — UI blocked until design assets approved
- ADR-028 — Unified Provider Capability Model
- `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`

| Dimension | Status |
|-----------|--------|
| Architecture | **APPROVED** |
| Scope | **FROZEN** |
| Design assets & sign-off | **NOT APPROVED** (this package) |
| UI implementation | **BLOCKED** (per ADR-023) |

**Explicit exclusions:** No production code, Flutter screens, React components, architecture changes, or scope changes in this deliverable.

**Cross-reference:** Per ADR-023, engineering **must not** implement UI until this blocker is **Closed** with approved assets archived.

---

## Instructions to Approvers

1. Review §1–§8 against brand, UX, and design system specifications
2. Confirm final assets are filed under `docs/v1/governance/evidence/BLOCKER-001-design/`
3. Sign §9 — verbal approval is insufficient
4. Return to Product Design Governance Manager for `DESIGN_APPROVAL_SIGNOFF_v1.0` completion

Until §9 signatures exist **and** assets are approved, **BLOCKER-001 remains open**.

---

## 1. Brand Identity Approval

### 1.1 Official KHADAMATI logo usage — approval requested

Confirm approved usage per `BRAND_IDENTITY_SPECIFICATION.md`:

| Asset / usage | Specification reference | Asset on file | Approval |
|---------------|----------------------|---------------|----------|
| **Primary logo** | Master mark for marketing and web headers | ☐ Pending | ☐ Approved |
| **Compact logo** | Constrained layouts, app bars | ☐ Pending | ☐ Approved |
| **Mobile app icon** | iOS/Android launcher icons | ☐ Pending | ☐ Approved |
| **Web usage** | Favicon, header, auth surfaces | ☐ Pending | ☐ Approved |
| **Light theme usage** | Logo on light backgrounds | ☐ Pending | ☐ Approved |
| **Dark theme usage** | Logo on dark backgrounds | ☐ Pending | ☐ Approved |

**Asset path (on approval):** `evidence/BLOCKER-001-design/logo/`

### 1.2 Brand positioning — confirmation requested

Logo and identity confirm KHADAMATI represents:

| Brand attribute | Confirmation |
|-----------------|--------------|
| Home / service marketplace | ☐ Confirmed |
| Professional services | ☐ Confirmed |
| Trust | ☐ Confirmed |
| Modern digital platform | ☐ Confirmed |

### 1.3 Brand approval

| Role | Decision | Date | Signature |
|------|----------|------|-----------|
| Product Owner | **Pending** | | |
| Design Owner | **Pending** | | |
| Business Owner | **Pending** | | |

---

## 2. Design System Approval

### 2.1 Shared system scope — validation requested

Confirm a **single shared design system** applies across:

| Surface | Shared system | Validation |
|---------|---------------|------------|
| Customer App | Yes | ☐ Validated |
| Craftsman App | Yes | ☐ Validated |
| Store Dashboard | Yes | ☐ Validated |
| Admin Portal | Yes | ☐ Validated |

**Reference:** `DESIGN_SYSTEM_TOKENS.md`

### 2.2 Design system elements — checklist

| Element | Defined in spec | Approved |
|---------|-----------------|----------|
| Colour system | `DESIGN_SYSTEM_TOKENS.md` | ☐ Pending |
| Typography | `DESIGN_SYSTEM_TOKENS.md` | ☐ Pending |
| Spacing system | `DESIGN_SYSTEM_TOKENS.md` | ☐ Pending |
| Components (library) | Design system spec | ☐ Pending |
| Buttons | Component spec | ☐ Pending |
| Inputs | Component spec | ☐ Pending |
| Cards | Component spec | ☐ Pending |
| Tables | Component spec | ☐ Pending |
| Dialogs | Component spec | ☐ Pending |
| Bottom sheets | Component spec | ☐ Pending |
| Navigation patterns | UI/UX spec | ☐ Pending |
| Loading states | UI/UX spec | ☐ Pending |
| Error states | UI/UX spec | ☐ Pending |
| Empty states | UI/UX spec | ☐ Pending |
| Notifications | UI/UX spec | ☐ Pending |
| Animation rules | UI/UX spec | ☐ Pending |

**Asset path (on approval):** `evidence/BLOCKER-001-design/design-system/`

---

## 3. UX Principles Approval

### 3.1 Experience principles — confirmation requested

KHADAMATI experience follows:

| Principle | Confirmation |
|-----------|--------------|
| Modern | ☐ Confirmed |
| Premium | ☐ Confirmed |
| Minimal | ☐ Confirmed |
| Fast | ☐ Confirmed |
| Professional | ☐ Confirmed |
| Accessible | ☐ Confirmed |

**Reference:** `UI_UX_SPECIFICATION.md`

### 3.2 UX rules — approval requested

| Rule | Approval |
|------|----------|
| Few clicks to booking | ☐ Approved |
| Persistent search | ☐ Approved |
| Smart filtering | ☐ Approved |
| Availability visibility | ☐ Approved |
| Clear booking progress | ☐ Approved |
| Strong feedback states (loading, error, empty, success) | ☐ Approved |

**Attestation (§3):** ☐ Product Owner ☐ Design Owner ☐ Business Owner

---

## 4. Customer App Approval

### 4.1 Discovery journey — approval requested

```
Service → Category → Location → Availability → Provider Capability → Listing
```

| Step | In frozen scope | Screen inventory approved | Approval |
|------|-----------------|---------------------------|----------|
| Service discovery | Yes | ☐ Pending | ☐ Approved |
| Category browse | Yes | ☐ Pending | ☐ Approved |
| Location filter | Yes | ☐ Pending | ☐ Approved |
| Availability filter | Yes | ☐ Pending | ☐ Approved |
| Provider capability view | Yes (ADR-028) | ☐ Pending | ☐ Approved |
| Listing detail | Yes | ☐ Pending | ☐ Approved |

### 4.2 Booking journey — approval requested

```
Request → Provider confirmation → Payment → Service execution
        → Completion acknowledgement → Review
```

| Stage | Screen flows approved | Approval |
|-------|----------------------|----------|
| Request | ☐ Pending | ☐ Approved |
| Provider confirmation | ☐ Pending | ☐ Approved |
| Payment | ☐ Pending | ☐ Approved |
| Service execution | ☐ Pending | ☐ Approved |
| Completion acknowledgement | ☐ Pending | ☐ Approved |
| Review | ☐ Pending | ☐ Approved |

**Asset path (on approval):** `evidence/BLOCKER-001-design/screens/customer-app/`

---

## 5. Craftsman App Approval

Approve journeys and screen inventory for:

| Journey / module | Screen inventory | Approval |
|------------------|------------------|----------|
| Registration | ☐ Pending | ☐ Approved |
| Verification | ☐ Pending | ☐ Approved |
| Profile | ☐ Pending | ☐ Approved |
| Capabilities (ADR-028) | ☐ Pending | ☐ Approved |
| Services / listings | ☐ Pending | ☐ Approved |
| Availability calendar | ☐ Pending | ☐ Approved |
| Booking management | ☐ Pending | ☐ Approved |
| Earnings | ☐ Pending | ☐ Approved |
| Withdrawal request | ☐ Pending | ☐ Approved |

**Asset path (on approval):** `evidence/BLOCKER-001-design/screens/craftsman-app/`

---

## 6. Store Dashboard Approval

### 6.1 Store capabilities — confirmation requested

Stores can:

| Capability | In scope | Design approved |
|------------|----------|-----------------|
| Provide services | Yes | ☐ Pending |
| Manage service listings | Yes | ☐ Pending |
| Advertise services | Yes | ☐ Pending |
| Advertise product catalogue items (display only) | Yes | ☐ Pending |

### 6.2 Store exclusions — explicit approval requested

**NOT included in V1** — design must not imply:

| Excluded feature | Exclusion acknowledged in design |
|------------------|--------------------------------|
| Product checkout | ☐ Acknowledged |
| Cart | ☐ Acknowledged |
| Inventory management | ☐ Acknowledged |
| Orders | ☐ Acknowledged |
| Delivery | ☐ Acknowledged |

**Asset path (on approval):** `evidence/BLOCKER-001-design/screens/store-dashboard/`

---

## 7. Admin Portal Approval

### 7.1 Platform constraint — confirmation requested

| Constraint | Confirmation |
|------------|--------------|
| Admin is **web only** | ☐ Confirmed |
| **Not** mobile admin application | ☐ Confirmed |

### 7.2 Admin design modules — approval requested

| Module | Design coverage | Approval |
|--------|-----------------|----------|
| User management | ☐ Pending | ☐ Approved |
| Provider approval | ☐ Pending | ☐ Approved |
| Finance policies (Admin configurable) | ☐ Pending | ☐ Approved |
| Subscriptions | ☐ Pending | ☐ Approved |
| Promotions | ☐ Pending | ☐ Approved |
| Reports | ☐ Pending | ☐ Approved |
| Audit | ☐ Pending | ☐ Approved |

**Asset path (on approval):** `evidence/BLOCKER-001-design/screens/admin-portal/`

---

## 8. Localization Approval

### 8.1 Default market presentation

| Setting | Default | Design treatment approved |
|---------|---------|---------------------------|
| **Country** | Lebanon | ☐ Pending |
| **Currency display** | USD | ☐ Pending |
| **Phone** | +961 | ☐ Pending |

### 8.2 Localization support — confirmation requested

| Requirement | Confirmation |
|-------------|--------------|
| **Arabic RTL** layout support | ☐ Confirmed |
| **English LTR** layout support | ☐ Confirmed |
| **Future markets** — no hardcoded locale assumptions in design system | ☐ Confirmed |

**Attestation (§8):** ☐ Product Owner ☐ Design Owner ☐ Business Owner

---

## 9. Design Approval Record

**BLOCKER-001 status:** **READY FOR APPROVAL** — not **Closed** until signatures **and** assets approved.

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| Product Owner | | **Pending** | | |
| Design Owner | | **Pending** | | |
| Business Owner | | **Pending** | | |

### Decision values

- **Approved** — Brand, design system, UX, and screen inventories accepted for implementation
- **Approved with conditions** — Document conditions below; UI blocked until resolved
- **Rejected** — Package returned; BLOCKER-001 remains open

### Comments / conditions

| Role | Comments |
|------|----------|
| Product Owner | |
| Design Owner | |
| Business Owner | |

---

## 10. Blocker Status

| Item | Status |
|------|--------|
| **BLOCKER-001** | **READY FOR APPROVAL** |
| **Evidence package** | `DESIGN_APPROVAL_PACKAGE.md` (this document) |
| **Brand / design assets** | **Pending approval and archival** |
| **BLOCKER-001 Closed** | **No** |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Closed blockers** | **0 / 7** |
| **UI implementation** | **BLOCKED** (ADR-023) |

### Closure criteria (all required)

- [ ] Logo and brand assets approved and filed (`logo/`, `brand/`)
- [ ] Design tokens and component spec approved (`design-system/`)
- [ ] UI/UX specification sign-off (`ui-ux/`)
- [ ] Customer, craftsman, store, admin screen inventories approved (`screens/`)
- [ ] Product Owner — signed §9
- [ ] Design Owner — signed §9
- [ ] Business Owner — signed §9
- [ ] `DESIGN_APPROVAL_SIGNOFF_v1.0.md` completed
- [ ] `READINESS_BLOCKER_CLOSURE_STATUS.md` updated to **Closed**

---

## Document Control

| Version | Date | Author | Change |
|---------|------|--------|--------|
| 1.0 | 2026-07-25 | Product Design Governance Manager | Initial design approval package |

**Distribution:** Product Owner, Design Owner, Business Owner, Program Governance Manager, Engineering Lead (informational — UI blocked per ADR-023)
