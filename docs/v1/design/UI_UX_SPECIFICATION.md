# KHADAMATI V1 — UI/UX Design Specification

**Document ID:** KHAD-V1-UI-UX-SPEC  
**Version:** 1.1  
**Date:** 2026-07-24  
**Status:** **READY FOR APPROVAL** (BLOCKER-001) — not COMPLETED until assets + colors + signatures  
**Role:** Design Governance Lead  
**ADR:** ADR-023 · ADR-010 · ADR-018 · ADR-028  

**Companions:**  
[`BRAND_IDENTITY_SPECIFICATION.md`](./BRAND_IDENTITY_SPECIFICATION.md) · [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md) · [`COLOR_REFERENCE.md`](./COLOR_REFERENCE.md) · [`DESIGN_APPROVAL_RECORD.md`](./DESIGN_APPROVAL_RECORD.md) · [`assets/LOGO_ASSET_PACKAGE.md`](./assets/LOGO_ASSET_PACKAGE.md) · [`../FINAL_SCOPE_BASELINE.md`](../FINAL_SCOPE_BASELINE.md)

**Design freeze:** After COMPLETED, this spec becomes an implementation reference; later changes need Design Change Request + impact review + approval ([`DESIGN_APPROVAL_RECORD.md`](./DESIGN_APPROVAL_RECORD.md) §7).

```text
DO NOT implement UI.
DO NOT create Flutter or React screens.
DO NOT build components in code.
DO NOT change architecture or V1 scope.
This document is the design foundation before implementation.
```

---

## Blocker Status (BLOCKER-001)

| Item | State |
|------|-------|
| Official logo / brand reference received | ☑ **Yes** |
| Brand Identity Specification | ☑ Ready for approval |
| Design System Tokens | ☑ Ready for approval (colors Draft pending extraction) |
| Color Reference | ☑ Created — **Draft** until master extraction |
| Logo asset package structure | ☑ Verified required layout — **binaries missing** |
| UI/UX Specification (this doc) | ☑ **READY FOR APPROVAL** |
| Spec **approved** / BLOCKER-001 COMPLETED | ☐ Requires assets + colors + Design + Product approval |
| UI implementation authorized | ☐ Blocked until Gate **A** |

---

## 0. Asset Intake Register

| Required input | Received | Notes |
|----------------|----------|-------|
| Logo (official) | ☑ | Home + tools; خدماتي / KHADAMATI; orange + navy |
| Brand identity | ☑ | See Brand Identity Spec |
| Color palette | ☑ | From logo → tokens (confirm hex sample) |
| Typography | ☑ | Recommendations defined |
| Existing UI designs | ☐ Optional | Inspiration only (ADR-010) |
| Design video / screens | ☐ Optional | Improve, do not clone |
| Reference applications | ☐ Optional | Marketplace feeling — not copies |

**Master files:** commit to [`assets/`](./assets/) when binary exports are available.

---

# 1. Design Principles

KHADAMATI UI must feel:

| Principle | Meaning in product |
|-----------|-------------------|
| Modern | Clean surfaces, decisive orange actions, service-first IA |
| Premium | Navy trust chrome, restrained elevation, quality type |
| Trustworthy | Verification badges, clear ratings, calm confirmation states |
| Simple | Minimum booking steps; one job per screen section |
| Professional | Especially Craftsman + Admin — no playful clutter |
| Fast | Persistent search, obvious CTAs, short paths to book |

**Reference feeling:** modern marketplace applications.  
**Do not** copy existing apps. Create KHADAMATI identity from the official logo.

### Visual composition rules (product surfaces)

- First viewport = one composition (not a dashboard) on marketing/customer home  
- Brand is a hero-level signal where branding appears  
- No hero overlay stickers / promo chips on media  
- Cards only when they aid interaction; avoid card-for-everything  
- Service-first discovery (ADR-028): no mandatory Craftsman vs Store chooser  

---

# 2. Visual Analysis

### 2.1 Design language

**Signature:** Orange action on navy trust, anchored by the home+tools mark.  
Whitespace and cool neutrals keep crafts/home-service warmth without cream/terracotta clichés.  
Bilingual wordmarks reinforce local + professional identity.

### 2.2 Visual hierarchy

1. Primary CTA (orange)  
2. Titles (navy / neutral-900)  
3. Trust signals (badges, ratings)  
4. Supporting meta (price, distance, availability)  
5. Chrome / navigation  

### 2.3 Layout principles

- **Mobile:** single-column; sticky primary search on Customer home  
- **Web dashboards:** left nav (RTL-mirrored) + content + optional utility rail  
- **Rhythm:** 8-point spacing; consistent section headers + one supporting sentence  
- **Density:** Customer airy; Craftsman efficient; Admin denser tables  

### 2.4 Component patterns

- List rows for scan speed; cards for bookable entities (service / provider / booking)  
- Bottom sheets for mobile filters & confirmations  
- Dialogs for destructive / finance confirms on web  
- Progress stepper for booking  

---

# 3. UX Analysis

### 3.1 Customer journey (Flutter)

```text
Register / Login → Home (service-first search)
  → Results (filters) → Service / Provider detail → Compare (optional)
  → Availability → Booking request → Provider confirm → Payment
  → Track → Chat (booking-scoped) → Complete → Review
```

**Focus:** discovery · booking · trust · simplicity  

**Improvements to encode in UI:**

- Persistent search entry  
- Smart filters (category, location, availability, rating; provider type optional)  
- Trust indicators near provider name (verified professional / company)  
- Availability visibility before request  
- Clear booking progress  
- Minimum steps to request  

### 3.2 Craftsman journey (Flutter)

```text
Onboarding / verification → Services → Availability calendar
  → Booking inbox → Job lifecycle (field verify) → Chat
  → Earnings / withdrawals → Subscription
```

**Focus:** jobs · availability · earnings · professional workflow  

### 3.3 Store dashboard journey (React web)

```text
Login → Services & availability → Bookings
  → Promotional product catalog (advertise only)
  → Promotions / ads (subscription-gated) → Analytics
```

**Focus:** services management · promotions · product catalog advertising  
**Out of UI scope:** cart, checkout, product payment, inventory, delivery  

### 3.4 Admin portal journey (React web only)

```text
MFA Login → Users / Providers → Verification
  → Categories · Subscriptions · Promotions · Ads
  → Finance policies · Notifications · Reports · Audit
```

**Focus:** configuration · monitoring · finance policies · reports  

### 3.5 Navigation improvements

| App | Pattern |
|-----|---------|
| Customer | Tab bar: Home · Bookings · Chat/Inbox · Account (+ FAB or sticky search on Home) |
| Craftsman | Tab bar: Jobs · Calendar · Earnings · Account |
| Store | Side navigation + top bar |
| Admin | Side navigation + top bar; finance section RBAC-gated |

---

# 4. Color System

Based on official logo. Full tokens: [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md).

| Role | Token | Notes |
|------|-------|-------|
| Primary | `color.brand.orange` | **KHADAMATI Orange** — primary actions |
| Primary hover | `color.brand.orange-hover` | |
| Primary light | `color.brand.orange-light` | Soft selection |
| Secondary navy | `color.brand.navy` | Trust, headers |
| Neutrals | `color.neutral.*` | Text, borders |
| Background | `color.bg.app` | Canvas |
| Surface | `color.bg.surface` | Panels |
| Error / Success / Warning | `color.semantic.*` | Warning ≠ brand orange |

**Theme:** Light required. Dark UI conditional (ADR-018). Logo dark-background version still required for navy bands.

---

# 5. Typography

| Need | Recommendation |
|------|----------------|
| Arabic RTL | **IBM Plex Sans Arabic** |
| English LTR | **IBM Plex Sans** |
| Heading scale | `type.h1`–`type.h3` |
| Body scale | `type.body` / `type.body-sm` |
| Caption | `type.caption` |
| Buttons | `type.button` / `type.button-sm` |

Arabic is the **default market language** (Lebanon). Never force Arabic into Latin uppercase patterns.

---

# 6. Applications Covered (same design language)

| Surface | Stack (future) | Design emphasis |
|---------|----------------|-----------------|
| Customer Mobile App | Flutter | Discovery, booking, trust, simplicity |
| Craftsman Mobile App | Flutter | Jobs, calendar, earnings, workflow |
| Store Dashboard | React + MUI | Services, promotions, catalog ads |
| Admin Portal | React + MUI (web only) | Config, monitoring, finance, reports |

Shared: orange CTA, navy trust, tokenized spacing/type, bilingual behavior.

---

# 7. Design System Components

Reusable definitions (spec only — **no code**).

| Component | Guidance |
|-----------|----------|
| **Buttons** | Primary orange filled; Secondary navy outline; Tertiary text; Destructive semantic error. Height 48 mobile / 40 compact. One primary CTA per view. |
| **Inputs** | 48 touch height; labeled; error text below; RTL label alignment. |
| **Search bar** | Persistent on Customer home; leading search icon (mirrored OK); trailing filter affordance. |
| **Cards** | Use for interactive entities only; `radius.md`; light border optional. |
| **Provider cards** | Photo/initial · name · trust badge · rating · distance · starting price · availability cue. Type as badge, not gate. |
| **Service cards** | Category · title · price cue · provider snippet · rating. |
| **Booking cards** | Status chip · service · datetime · counterparty · next action. |
| **Badges** | Verified · Company · Featured · Status (Requested/Confirmed/Paid/…). |
| **Chips** | Filters; multi-select; clear-all. |
| **Dialogs** | Web confirms; destructive finance actions. |
| **Bottom sheets** | Mobile filters, sort, lightweight confirms. |
| **Navigation** | Tab bars (mobile); side menus (web); RTL mirror. |
| **Side menus** | Store/Admin; collapse on tablet. |
| **Tables** | Admin/Store; sticky header; row actions; RTL column order. |
| **Dashboard widgets** | KPI tiles (views, bookings, inquiries) — not customer home. |
| **Charts** | Simple bar/line; navy/orange series; accessible patterns. |
| **Toast notifications** | Short; success/error/info; avoid blocking. |
| **Loading states** | Inline spinner on CTA; full-section sparingly. |
| **Empty states** | Illustration optional; one sentence + one CTA. |
| **Error states** | Plain language; retry; support path if needed. |
| **Skeleton loaders** | Lists/cards placeholders matching final layout. |

### Elevation & spacing

Use token scales in [`DESIGN_SYSTEM_TOKENS.md`](./DESIGN_SYSTEM_TOKENS.md). Prefer border + soft shadow over heavy stacks.

---

# 8. Mobile App Icon Specification

See also Brand Identity §5.

| Platform | Requirements |
|----------|--------------|
| **Android** | Adaptive icon; KHADAMATI symbol; orange identity; safe zone; all mask shapes |
| **iOS** | 1024 master; symbol centered; no transparency for store; system corners |
| Shared | Recognizable small; home silhouette first; tools simplified if needed; works on light/dark wallpapers |

**Do not** put full bilingual wordmark in the icon.

---

# 9. UX Rules (Optimization Targets)

| Rule | Design implication |
|------|--------------------|
| Minimum booking steps | Collapse optional fields; defer non-essentials post-request |
| Clear booking progress | Visible stepper / status timeline |
| Persistent search | Home search always reachable |
| Smart filtering | Availability, rating, location, category first; type optional |
| Provider trust indicators | Verified state near name; reviews visible |
| Availability visibility | Calendar/slots before submit |
| Accessibility | Contrast AA for text; 48 dp targets; semantic labels |
| Readability | 16 px body minimum mobile; generous Arabic line-height |

### Booking progress (customer)

`Request → Confirmed → Paid → In progress → Completed → Reviewed`  
Map each to status chip colors (navy/neutral + semantic; orange for active CTA only).

---

# 10. RTL / LTR Requirements

### Arabic (default market)

- Full RTL layouts  
- Arabic typography (`font.family.ar`)  
- Correct icon direction (back, chevrons, progress)  
- RTL navigation (tabs order, drawers from start edge)  
- Brand lockup: Arabic-leading where space allows  

### English

- Full LTR layouts  
- English typography (`font.family.en`)  
- International readiness (Market config — not hardcoded copy)  

### Mixed content

Numbers, phone (+961), and prices follow locale rules; do not break RTL containers.

---

# 11. Responsive Requirements

| Surface | Support |
|---------|---------|
| Mobile phones | Primary for Customer + Craftsman |
| Tablets | Readable large layout; optional two-column detail |
| Web dashboards | Store + Admin; `bp.desktop`+ |
| Densities | Comfortable mobile; compact admin tables |

Breakpoints: see tokens `bp.*`.

---

# 12. Screen Hierarchy (logical — not builds)

### Customer (logical screens)

Home/Search · Results · Service detail · Provider profile · Compare · Availability · Booking request · Payment host · Booking detail/track · Chat · Reviews · Profile/Settings · Notifications  

### Craftsman

Home/Jobs · Job detail · Calendar · Services list/edit · Earnings · Withdrawal · Subscription · Verification · Chat · Profile  

### Store

Dashboard · Services · Catalog items (promo) · Bookings · Promotions · Analytics · Subscription · Staff (if capability) · Settings  

### Admin

Users · Providers/Verification · Categories · Subscriptions · Promotions/Ads · Finance policies · Notifications templates · Reports · Audit · Settings  

Exact visual mockups follow Design Lead approval of this spec + asset exports — still **no UI coding**.

---

# 13. Accessibility Rules

| Rule | Target |
|------|--------|
| Contrast | WCAG AA for text/icons on surfaces |
| Touch | Min 48×48 dp interactive |
| Focus | Visible focus on web |
| Screen readers | Labels on icon-only controls |
| Motion | Respect reduced-motion OS setting |
| Errors | Text + color, not color alone |

---

# 14. Out of Scope (design & product)

- E-commerce cart/checkout UI  
- Open marketplace messaging UI  
- Admin on mobile  
- New V1 features not in Scope Baseline / FTM  

New needs → ADR or Change Request.

---

# 15. Approval Section

**Package record:** [`DESIGN_APPROVAL_RECORD.md`](./DESIGN_APPROVAL_RECORD.md)  
**BLOCKER-001 governance status:** **READY FOR APPROVAL** (not Completed until signatures).

## Design review checklist

- [ ] Brand identity approved
- [ ] Color system approved
- [ ] Typography approved
- [ ] Component system approved
- [ ] RTL/LTR approach approved
- [ ] Mobile-first approach approved
- [ ] Customer journey approved
- [ ] Provider journey approved
- [ ] Admin experience approved

## Signatures

| Role | Name | Date | Decision |
|------|------|------|----------|
| Design Lead | | | ☐ Approve UI/UX Spec |
| Product Owner | | | ☐ Approve (scope-aligned) |
| Solution Architect | | | ☐ Acknowledge (no architecture change) |

After checklist + signatures: update [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md) BLOCKER-001 → **Closed** (Completed).  
Until then, status remains **READY FOR APPROVAL** — **not** Completed.  
UI coding still requires Implementation Gate **A**.

---

**End of UI/UX Design Specification v1.1**
