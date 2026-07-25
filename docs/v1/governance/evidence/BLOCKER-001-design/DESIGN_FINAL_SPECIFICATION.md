# BLOCKER-001 — Design Final Specification

| Field | Value |
|-------|-------|
| **Document ID** | EVD-001-FINAL-SPEC-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-001 — Design |
| **Visual reference** | `theme.mp4` |
| **Logo reference** | `khadamatiLogo.jpg` |
| **Business status** | **APPROVED** |
| **Blocker closure** | **NOT CLOSED** — Design Lead sign-off pending |

```text
This specification defines approved V1 design intent.
It does NOT authorize UI implementation (ADR-023).
```

---

## 1. Brand identity

| Element | Value |
|---------|-------|
| Application name (English) | **KHADAMATI** |
| Application name (Arabic) | **خدماتي** |
| Logo | `khadamatiLogo.jpg` — **approved; do not replace or redesign** |
| Theme reference | `theme.mp4` — primary visual direction |
| Market baseline | Lebanon launch · multi-region architecture ready |
| Languages | Arabic **RTL** · English **LTR** |

---

## 2. Design principles

| Principle | Requirement |
|-----------|-------------|
| Mobile-first | Customer and Provider experiences prioritize mobile |
| Premium marketplace | Modern, clean, trustworthy service marketplace feeling |
| Bilingual | Full RTL/LTR parity; no mirrored-afterthought layouts |
| Accessibility-ready | Legible typography, sufficient contrast, clear touch targets |
| Configuration-aware | Regional defaults via administrator configuration — not hardcoded UI assumptions |

---

## 3. Customer mobile application

### 3.1 Approved journeys

| Journey | Scope |
|---------|-------|
| Service discovery | Categories, search, featured providers/services |
| Provider browsing | Profiles, ratings, service listings |
| Booking journey | Request → confirmation → status tracking |
| Payment journey | Initiation toward Areeba IXOPAY Payment.js (flow per BLOCKER-007) |
| Chat experience | Booking-scoped messaging (ADR-020) |
| Profile | Account settings, booking history, language preference |

### 3.2 Localization

- Arabic **RTL** default presentation supported
- English **LTR** full parity
- Copy and layout must support bidirectional switching without breaking navigation

### 3.3 Excluded from V1 UI (scope frozen)

- Map screens · geolocation · distance-based discovery (see `BLOCKER-003` maps exclusion)
- Customer wallet · instant withdrawal

---

## 4. Provider mobile application

### 4.1 Approved journeys

| Journey | Scope |
|---------|-------|
| Provider onboarding | Registration, profile setup, document submission |
| Verification status | Pending / approved / rejected visibility |
| Availability | Schedule and availability management |
| Booking management | Accept, decline, complete, reschedule workflows |
| Earnings visibility | Commission and earnings reporting (no wallet withdrawal UI V1) |
| Subscription status | Plan tier, renewal, upgrade visibility |
| Customer communication | Booking-scoped chat |

---

## 5. Administrator web portal

### 5.1 Approved modules

| Module | Scope |
|--------|-------|
| Dashboard | Platform KPIs, operational overview |
| Provider approval | Onboarding review, verification workflow |
| User moderation | Reports, suspensions, content review |
| Subscription management | Provider plans, advertising subscriptions, pricing configuration |
| Advertisements | Packages, featured listings, campaign approval |
| Categories | Service categories and subcategories |
| Platform configuration | Operational rules, regional settings, finance configuration surfaces |

**Note:** Administrator governs platform operations — not business strategy, revenue model creation, architecture, or scope expansion.

---

## 6. Theme and visual direction

Derived from approved reference `theme.mp4`:

| Attribute | Direction |
|-----------|-----------|
| Style | Modern · clean · trustworthy |
| Feeling | Premium service marketplace |
| Primary brand | Orange identity (preserve logo palette) |
| Density | Comfortable spacing; card-based layouts |
| Trust signals | Clear status, verification badges, professional typography |

Detailed tokens: see `DESIGN_SYSTEM_BASELINE.md`.

---

## 7. Logo refinement (mandatory before closure)

Per `LOGO_REFINEMENT_SPECIFICATION.md` and `LOGO_USAGE_GUIDELINES.md`:

- Refine for digital usage only — **identity preserved**
- Mobile app icon compatibility
- High resolution · clean edges · balanced spacing
- Light/dark background compatibility
- Splash screen · App Store / Play Store readiness
- Arabic identity preservation

---

## 8. Closure dependencies

| Item | Status |
|------|--------|
| `theme.mp4` archived to `reference-assets/` | Pending upload |
| `khadamatiLogo.jpg` archived to `reference-assets/` | Pending upload |
| Refined logo package | Pending Design Lead |
| `DESIGN_APPROVAL_SIGNOFF.md` signed | Pending Design Lead |

**BLOCKER-001 NOT CLOSED** until GOV-BEMF-001 checklist complete.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Final design specification — Gate B evidence finalization |
