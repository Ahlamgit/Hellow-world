# KHADAMATI V1 — Final Scope Baseline

| Field | Value |
|-------|-------|
| **Document ID** | KHAD-V1-SCOPE-BASELINE |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **FROZEN — Pending stakeholder sign-off (BLOCKER-002)** |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Implementation** | **NOT AUTHORIZED** |
| **Controlling brief** | `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` |
| **Traceability** | `FEATURE_TRACEABILITY_MATRIX.md` (KHAD-V1-FTM) |
| **Architecture** | ADR-001 → ADR-032 — **Approved & validated** |

```text
SCOPE FROZEN — Changes require formal change control (Execution Plan §4).
This document is the canonical V1 scope baseline for stakeholder approval (BLOCKER-002).
```

---

## 1. Product definition

KHADAMATI V1 is a **service-first marketplace** connecting:

| Actor | Primary surface |
|-------|-----------------|
| **Customers** | Customer mobile application (Flutter) |
| **Craftsmen / individual providers** | Craftsman mobile application (Flutter) |
| **Stores / service businesses** | Store dashboard (React web) |
| **Platform operations** | Administration portal (React web — **web only**, MFA required) |

The platform manages service discovery, booking, payments, commissions, subscriptions, provider verification, quality follow-up, notifications, and booking-scoped chat.

**Marketplace model:** Provider and Listing are **unified** concepts (ADR-003, ADR-028). Customer experience is **service-first** — customers are not required to choose Craftsman vs Store before searching.

---

## 2. Initial market configuration (defaults — not hardcoded)

| Setting | Default launch value |
|---------|---------------------|
| Country | Lebanon |
| Currency | USD |
| Phone format | +961 (E.164) |
| Primary language | Arabic (RTL) |
| Secondary language | English (LTR) |
| Timezone | Asia/Beirut |

Architecture **must** support future: multiple countries · multiple currencies · multiple phone formats · regional configurations · country-specific payment providers and business rules via `Market` configuration.

**No Lebanon-specific hardcoding in application logic.**

---

## 3. Included V1 scope

### 3.1 Customer application

| Capability | Included |
|------------|----------|
| Registration / login / OTP authentication | Yes |
| Profile and address management | Yes |
| Service discovery (category, location, availability, rating, capability) | Yes |
| Search and filters (service-first; optional type filter) | Yes |
| Booking creation (date/time from provider availability, address, notes) | Yes |
| Booking tracking and history | Yes |
| Payment after provider confirmation (Payment.js / IXOPAY) | Yes |
| Simple payment and booking status (no ledger/commission UI) | Yes |
| Service completion confirmation | Yes |
| Ratings and reviews | Yes |
| Notifications (in-app primary; push secondary) | Yes |
| Booking-scoped chat only (ADR-020) | Yes |
| Dispute / complaint (lightweight, ADR-021) | Yes |
| Account deletion request (ADR-022) | Yes |

### 3.2 Provider application (Craftsman)

| Capability | Included |
|------------|----------|
| Registration / login / profile | Yes |
| Skills, services, service areas, pricing | Yes |
| Provider Availability Calendar (ADR-019) | Yes |
| Identity verification workflow (documents, OCR/Face integration points) | Yes |
| Receive / accept / reject bookings | Yes |
| Job lifecycle and daily schedule | Yes |
| Field verification (GPS, selfie, OTP/QR handshake) | Yes |
| Earnings summary and KPIs (not ledger/settlement detail) | Yes |
| Subscription management (craftsman plans) | Yes |
| Withdrawal **requests** (admin-approved process — not instant withdrawal UI) | Yes |
| Notifications | Yes |
| Booking-scoped chat | Yes |

### 3.3 Store / service advertiser capabilities

| Capability | Included |
|------------|----------|
| Store profile, verification, service areas | Yes |
| Service listings management | Yes |
| Provider/staff affiliation | Yes |
| Booking management | Yes |
| **Product catalog advertising** (display only — ADR-027) | Yes |
| Catalog inquiries / leads (non-transactional) | Yes |
| Promotions for services and catalog items (subscription + admin rules) | Yes |
| Store subscriptions (admin-defined plans) | Yes |
| Analytics (views, impressions, inquiries, booking conversions) | Yes |

### 3.4 Booking workflow

```text
Customer request → Provider confirmation → Payment → Service execution
    → Completion acknowledgement → Review
```

| Booking status (booking domain) | Included |
|---------------------------------|----------|
| Requested → Confirmed → Awaiting Payment → Paid → In Progress → Completed / Cancelled | Yes |

Booking owns **service lifecycle only** — separate from payment, ledger, and settlement state (ADR-030).

### 3.5 Payment workflow

| Capability | Included |
|------------|----------|
| Areeba IXOPAY Payment.js (tokenized; no card storage) | Yes |
| Payment Port → IXOPAY Adapter → Gateway (ADR-025) | Yes |
| Payment state: Pending → Processing → Paid / Failed / Cancelled / Refunded | Yes |
| Webhook verification, idempotency, reconciliation (ADR-031) | Yes |
| Ledger as financial source of truth (ADR-004) | Yes |
| Admin-configurable commission, refund, cancellation, withdrawal, settlement rules (ADR-013) | Yes |
| Escrow readiness (internal; no complex escrow screens for customers) | Yes |

**Customer sees:** service · provider · amount · payment status · confirmation · booking progress  
**Customer does NOT see:** ledger · commission · settlement · wallet · financial operations

**Provider sees:** bookings · completed services · earnings summary  
**Provider does NOT manage:** ledger · settlement calculations

**Finance Admin manages:** payments · ledger · commission · settlement policies

### 3.6 Chat

| Rule | V1 |
|------|-----|
| Booking-scoped chat only (ADR-020) | Included |
| Open marketplace messaging | **Excluded** |

### 3.7 Notifications

| Channel | Priority |
|---------|----------|
| In-app | Primary |
| Push | Secondary |
| SMS / Email | Optional (ports/adapters — ADR-032) |

Business modules emit events; notification module delivers. No direct vendor calls from business modules.

### 3.8 Subscriptions

| Area | Included |
|------|----------|
| Craftsman subscription plans (admin-configurable) | Yes |
| Store subscription plans (admin-configurable) | Yes |
| Subscription payment via Payment.js | Yes |

### 3.9 Promotions

| Area | Included |
|------|----------|
| Admin-managed promotions (ADR-007) | Yes |
| Featured listings, promotion slots, provider visibility | Yes |
| Store/service and catalog item promotions (subscription-gated) | Yes |
| Self-service advertising marketplace | **Excluded** (unless separately approved) |

### 3.10 Administration portal

| Area | Included |
|------|----------|
| User management (customers, craftsmen, stores) | Yes |
| Provider onboarding and verification workflow | Yes |
| Advertisement and promotion management | Yes |
| Subscription plan management | Yes |
| Commission, refund, cancellation, withdrawal, settlement policy configuration | Yes |
| Finance reports and settlement views | Yes |
| Notification templates and channel configuration | Yes |
| Analytics and quality/follow-up metrics | Yes |
| Dispute review queue | Yes |
| Audit logs, RBAC, MFA (web only) | Yes |
| Retention policy configuration (values pending BLOCKER-006) | Yes |
| Mobile admin application | **Excluded** |

### 3.11 Reporting

| Area | Included |
|------|----------|
| Provider earnings summaries | Yes |
| Admin financial reports | Yes |
| Store analytics | Yes |
| Customer financial dashboards | **Excluded** |
| Provider wallet UI | **Excluded** |

---

## 4. Excluded V1 scope

The following are **explicitly excluded** from V1. Inclusion requires formal change control.

| Exclusion | Rationale / reference |
|-----------|----------------------|
| E-commerce checkout | Service-first marketplace — ADR-027 |
| Shopping cart | No product commerce |
| Product ordering | Catalog is advertising only |
| Inventory management | No warehouse/stock |
| Product fulfillment / delivery management | Out of V1 |
| Customer wallet | ADR-029 |
| Provider wallet UI | ADR-029 |
| Instant withdrawals | Admin-approved withdrawal process only |
| Gateway split payments | ADR-029 |
| Complex escrow screens (customer/provider) | Internal ledger model only |
| Manual financial adjustment screens (unauthorized) | Finance Admin governed policies only |
| Open marketplace messaging / public chat | ADR-020 |
| Mobile admin application | ADR-006 |
| Self-service advertising marketplace | ADR-007 (admin-managed promotions only) |

---

## 5. Architecture constraints (scope-related)

| Constraint | Reference |
|------------|-----------|
| Unified Provider + capability model | ADR-003, ADR-028 |
| Provider Availability Calendar (not static schedules) | ADR-019 |
| Confirm-then-pay booking flow | ADR-005 |
| Mandatory financial ledger | ADR-004 |
| Booking / payment / ledger / settlement state separation | ADR-030 |
| Payment.js only; ports/adapters for integrations | ADR-025, ADR-029 |
| Event-driven notifications | ADR-032 |
| Design gate before UI implementation | ADR-023 (BLOCKER-001) |

---

## 6. Technology scope (fixed)

| Layer | Approved stack |
|-------|----------------|
| Backend | Spring Boot 3.x, Java 21, Maven |
| Database | PostgreSQL, Flyway |
| Admin + Store web | React, TypeScript, Material UI |
| Customer + Craftsman mobile | Flutter |
| Payments | Areeba IXOPAY Payment.js + abstraction |
| Cache / workers | Redis (ADR-012) |

No conflicting technologies without new ADR and approval.

---

## 7. Change control

| Rule | Detail |
|------|--------|
| Scope frozen | This document version 1.0 |
| Additions | Require change request, impact analysis, stakeholder approval |
| Exclusions | Cannot be added without same process |
| Architecture changes | New ADR + governance update |

---

## 8. Approval record (BLOCKER-002)

**Status:** **CLOSED** — 2026-07-25 (GOV-BLOCKER-002-ROLE-CORR-001)

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| **Project Owner / Business Owner** | Project Owner | **Approved** | 2026-07-25 | Recorded |
| **Administrator** | Administrator | **Approved** | 2026-07-25 | Recorded |

**BLOCKER-002 CLOSED.** Closure artifact: `governance/evidence/BLOCKER-002-stakeholder/BLOCKER_002_CLOSURE_RECORD.md`

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Restored canonical scope baseline from Master Prompt, FTM, ADRs, stakeholder package |

**Distribution:** Product Owner, Business Owner, Operations Owner, Program Governance Manager, Technical Architect
