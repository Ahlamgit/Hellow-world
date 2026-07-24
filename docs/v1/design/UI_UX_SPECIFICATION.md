# KHADAMATI V1 — UI/UX Design Specification

**Document ID:** KHAD-V1-UI-UX-SPEC  
**Version:** 0.1-BLOCKED  
**Date:** 2026-07-24  
**Status:** **BLOCKED — awaiting design assets**  
**ADR:** ADR-023 · ADR-010 · ADR-018  

**Related:** [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md) · BLOCKER-001  

```text
DO NOT implement UI until this document is completed and approved.
DO NOT invent brand visuals without uploaded assets.
```

---

## Blocker Status

| Item | State |
|------|-------|
| Design assets received | ☐ **No** |
| Visual analysis complete | ☐ |
| UX analysis complete | ☐ |
| Design system defined | ☐ |
| Spec approved | ☐ |
| BLOCKER-001 closed | ☐ |

**Fill all sections below only after assets are available.** Until then, leave `TBD` / unchecked.

---

## 0. Asset Intake Register

| Required input | Received | File / link | Reviewer |
|----------------|----------|-------------|----------|
| Logo | ☐ | | |
| Brand identity | ☐ | | |
| Color palette | ☐ | | |
| Typography | ☐ | | |
| Existing UI designs | ☐ | | |
| Design video / screens | ☐ | | |
| Reference applications | ☐ | | |

Inspiration rule (ADR-010): improve on references; do **not** pixel-clone.

Dark theme (ADR-018): light required; dark **only if** assets include it.

---

## 1. Visual Analysis

*(Complete after assets)*

### 1.1 Design language
TBD

### 1.2 Visual hierarchy
TBD

### 1.3 Layout principles
TBD

### 1.4 Component patterns
TBD

---

## 2. UX Analysis

*(Complete after assets; journeys must align to Scope Baseline — no new features)*

### 2.1 Customer journey
Registration → service-first discovery → listing/detail → compare → availability → booking → pay → track → chat (booking-scoped) → complete → review  

Notes / improvements: TBD

### 2.2 Provider journey (Craftsman + Store)

**Craftsman:** onboarding/verification → services → availability calendar → bookings/jobs → chat → earnings/withdrawals → subscription  

**Store:** services + promotional catalog advertising (no e-commerce) → bookings → promotions → analytics  

Notes / improvements: TBD

### 2.3 Admin journey (Web only)
Users · verification · categories · subscriptions · promotions · ads · finance policies · notifications · reports · audit  

Notes / improvements: TBD

### 2.4 Navigation improvements
TBD — service-first entry (ADR-028); no mandatory Craftsman/Store chooser

---

## 3. Design System

### 3.1 Colors
| Token | Light | Dark (if in scope) | Usage |
|-------|-------|--------------------|-------|
| TBD | | | |

### 3.2 Typography
| Role | Font | Size / weight | Notes |
|------|------|---------------|-------|
| TBD | | | |

### 3.3 Spacing system
TBD (base unit, scale)

### 3.4 Elevation
TBD

### 3.5 Components inventory

| Component | Spec | States | Notes |
|-----------|------|--------|-------|
| Buttons | ☐ | default / hover / disabled / loading | |
| Inputs | ☐ | focus / error / disabled | |
| Cards | ☐ | Use only where interaction requires | Per design rules |
| Lists | ☐ | | |
| Tables | ☐ | Admin / Store primarily | |
| Dialogs | ☐ | | |
| Bottom sheets | ☐ | Mobile | |
| Navigation | ☐ | App bars / tabs / drawers | |
| Loading states | ☐ | | |
| Empty states | ☐ | | |
| Error states | ☐ | | |
| Skeleton loaders | ☐ | | |
| Notifications | ☐ | toast / in-app / banners | |

---

## 4. Localization & Theme Support

| Capability | Required | Spec complete |
|------------|----------|---------------|
| Arabic RTL | Yes | ☐ |
| English LTR | Yes | ☐ |
| Light theme | Yes | ☐ |
| Dark theme | Only if assets include (ADR-018) | ☐ / N/A |

---

## 5. Screen Hierarchy & Navigation

| App | Primary IA | Spec |
|-----|------------|------|
| Customer (Flutter) | TBD after assets | ☐ |
| Craftsman (Flutter) | TBD after assets | ☐ |
| Store Dashboard (React) | TBD after assets | ☐ |
| Admin Portal (React, web only) | TBD after assets | ☐ |

Responsive rules: TBD  
Accessibility rules: TBD (WCAG target TBD with Design/Compliance)

---

## 6. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Design Lead | | | ☐ Approve / ☐ Reject |
| Product Owner | | | ☐ Approve / ☐ Reject |
| Solution Architect | | | ☐ Acknowledge (scope/ADR alignment) |

**UI implementation authorized only after approval above and Implementation Gate → A.**

---

**End of UI/UX Specification (blocked shell)**
