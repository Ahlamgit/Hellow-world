# BLOCKER-001 — Design System Baseline

| Field | Value |
|-------|-------|
| **Document ID** | EVD-001-DS-BASELINE-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Reference** | `theme.mp4` · `khadamatiLogo.jpg` |
| **Status** | **Baseline approved** — token values finalized at implementation under Gate A |
| **ADR** | ADR-023 — UI blocked until BLOCKER-001 Closed |

```text
This baseline defines the approved design system structure.
It does NOT authorize component implementation.
```

---

## 1. Foundations

### 1.1 Brand color (from approved logo)

| Token | Role | Notes |
|-------|------|-------|
| `brand-primary` | Primary actions, key accents | Orange from `khadamatiLogo.jpg` — exact hex from refined asset |
| `brand-primary-dark` | Pressed / emphasis | Derived from primary |
| `brand-primary-light` | Subtle highlights | Derived from primary |
| `neutral-0` → `neutral-900` | Surfaces, text, borders | Light/dark theme scales |
| `semantic-success` | Confirmation, completed states | |
| `semantic-warning` | Pending, attention | |
| `semantic-error` | Errors, destructive emphasis | |
| `semantic-info` | Informational states | |

### 1.2 Typography

| Token | Usage | Direction notes |
|-------|-------|-----------------|
| `font-family-latin` | English UI | Clean sans-serif; premium marketplace tone |
| `font-family-arabic` | Arabic UI | Arabic-optimized sans; RTL-native metrics |
| `text-display` | Hero, splash | |
| `text-heading-lg/md/sm` | Section titles | |
| `text-body-lg/md/sm` | Content | |
| `text-caption` | Metadata, hints | |
| `text-label` | Form labels, chips | |

**Rule:** Arabic and English type scales must maintain visual weight parity in RTL and LTR.

### 1.3 Spacing system

| Token | Value (baseline unit) |
|-------|----------------------|
| `space-1` | 4px |
| `space-2` | 8px |
| `space-3` | 12px |
| `space-4` | 16px |
| `space-5` | 20px |
| `space-6` | 24px |
| `space-8` | 32px |
| `space-10` | 40px |
| `space-12` | 48px |

**Grid:** 4px base unit · mobile-first responsive breakpoints at implementation.

### 1.4 Elevation and radius

| Token | Usage |
|-------|-------|
| `radius-sm` | Chips, small controls |
| `radius-md` | Buttons, inputs |
| `radius-lg` | Cards |
| `radius-xl` | Modals, sheets |
| `shadow-sm/md/lg` | Card hierarchy per `theme.mp4` |

---

## 2. Components (approved inventory)

| Component | Surfaces | States required |
|-----------|----------|-----------------|
| **Buttons** | Primary · secondary · tertiary · destructive · icon | Default · pressed · disabled · loading |
| **Cards** | Service · provider · booking · subscription | Default · selected · disabled |
| **Forms** | Text · phone · OTP · select · checkbox · radio | Default · focus · error · disabled |
| **Navigation** | Bottom tab (mobile) · drawer · top bar · breadcrumbs (admin) | Active · inactive |
| **Lists** | Service list · booking list · notification list | Loading · empty · error |
| **Chips / badges** | Category · status · verification | |
| **Avatars** | User · provider | Placeholder when no image |
| **Modals / sheets** | Confirmation · filters · payment handoff | |
| **Toast / snackbar** | Success · error · info | |
| **Skeleton loaders** | List · card · detail | |
| **Empty states** | No results · no bookings · no messages | Bilingual copy |

---

## 3. Interaction states (mandatory)

| State | Requirement |
|-------|-------------|
| **Loading** | Skeleton or branded spinner; never blank screen |
| **Empty** | Illustration or icon + actionable copy (RTL/LTR) |
| **Error** | Clear message + recovery action |
| **Success** | Confirmation feedback before navigation |
| **Disabled** | Reduced opacity; no interaction |
| **Offline** (where applicable) | Graceful degradation message |

---

## 4. Surface-specific patterns

### 4.1 Customer mobile

- Bottom navigation for primary journeys
- Card-based service discovery
- Step indicator for booking and payment handoff
- Chat thread per booking

### 4.2 Provider mobile

- Dashboard summary cards
- Availability calendar pattern
- Booking action bar (accept / decline / complete)
- Subscription status banner

### 4.3 Administrator web

- Sidebar navigation
- Data tables with filters
- Approval queue pattern
- Configuration forms with validation

---

## 5. Localization rules

| Rule | Detail |
|------|--------|
| RTL | Mirror layout; do not mirror icons that imply direction incorrectly |
| LTR | Standard western layout |
| Numbers | Locale-appropriate formatting at implementation |
| Dates/times | Timezone-aware display |
| Bilingual brand | KHADAMATI / خدماتي per `LOGO_USAGE_GUIDELINES.md` |

---

## 6. Prohibited (V1)

| Item | Reason |
|------|--------|
| Map UI components | BLOCKER-003 maps exclusion |
| Wallet UI | Scope exclusion |
| Hardcoded financial values in UI copy | BLOCKER-005 finance governance |

---

## 7. Related documents

| Document | Purpose |
|----------|---------|
| `DESIGN_FINAL_SPECIFICATION.md` | Surface and journey specification |
| `LOGO_REFINEMENT_SPECIFICATION.md` | Logo deliverables |
| `LOGO_USAGE_GUIDELINES.md` | Logo usage rules |
| `DESIGN_GOVERNANCE_RECORD.md` | Governance alignment |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Design system baseline — Gate B evidence finalization |
