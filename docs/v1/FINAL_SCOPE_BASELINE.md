# KHADAMATI V1 — Final Scope Baseline

**Document ID:** KHAD-V1-SCOPE-BASELINE  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Official V1 scope reference  

**Controlling sources:** Master Prompt v1.0 · Architecture Readiness · Decisions Complete (ADR-001–028) · Implementation Gate Report · Feature Traceability Matrix · Approved business model  

**Production code / UI / application files:** Not authorized by this document.

---

## Scope Status

# A) Scope Frozen

Product and feature scope for KHADAMATI V1 is **frozen** as defined herein.

**Implementation remains blocked** until:

- Scope approved (this baseline signed)  
- Design assets available (ADR-023)  
- Vendors approved (ADR-025)  
- Cloud decision completed (ADR-024)  
- Finance/compliance configuration approved (ADR-013/022/026)  

---

# 1. Product Vision

KHADAMATI is a **service-first marketplace platform** connecting customers with **verified service providers**.

The platform supports:

- **Individual professionals** (Craftsman provider type)  
- **Service companies / stores** (Store provider type)  

Customers search by **service need**, not by provider type.

Examples of needs: AC repair · Plumbing · Solar installation · Painting · Appliance repair  

---

# 2. V1 Included Features

Every included feature must remain traceable in [`FEATURE_TRACEABILITY_MATRIX.md`](./FEATURE_TRACEABILITY_MATRIX.md).

## 2.1 Customer Application (Flutter)

| Area | Included |
|------|----------|
| Auth | Registration / login (incl. OTP) |
| Profile | Profile management |
| Discovery | Service discovery, search, filtering |
| Providers | Provider listings, service details, provider comparison |
| Trust | Verification status / trust badges (not type-first entry) |
| Scheduling | Availability viewing (provider calendar) |
| Booking | Booking request, confirmation tracking |
| Payment | Online payment (Areeba IXOPAY Payment.js) |
| Tracking | Booking / job status tracking |
| Chat | **Booking-scoped** chat only |
| Completion | Service completion confirmation |
| Feedback | Reviews and ratings |
| Notifications | In-app / push (and channels per templates) |
| Other | Account deletion request flow; lightweight disputes/complaints |

**UX rule (ADR-028):** No mandatory Craftsman vs Store chooser before search.

---

## 2.2 Craftsman Provider Application (Flutter)

| Area | Included |
|------|----------|
| Auth | Registration / login |
| Profile | Profile management |
| Trust | Verification onboarding, document submission |
| Supply | Service creation & management (`CanCreateServices`) |
| Calendar | Availability calendar (`CanManageAvailability`) |
| Demand | Booking management, job lifecycle (`CanAcceptBookings`) |
| Chat | Booking-scoped chat |
| Money | Earnings visibility, withdrawal requests (`CanReceivePayments`) |
| Monetization | Subscription management |
| Field IDV | GPS, selfie, QR/OTP job verification |

---

## 2.3 Store Provider Dashboard (React Web)

Stores are:

- **Service providers**  
- **Service advertisers**  
- **Product catalog advertisers**  

### Store services (bookable)

- Create / manage services (descriptions, pricing, categories)  
- Manage availability  
- Receive and manage bookings  
- Staff / affiliation where capabilities allow (`CanManageTeam`)  

### Product catalog advertising (promotional only — ADR-027)

**Allowed:** catalog entries, images, descriptions, categories, brands/models, promotional info, inquiry/contact CTA  

**Purpose:** Showcase, advertise, generate inquiries — **not** sell online  

**NOT included:** cart, checkout, product payment, product orders, inventory, delivery workflow  

### Store promotions

- Subscription-based promotions  
- Featured services  
- Product catalog promotion  
- Advertisement placements  

Subject to: active **Admin-managed** store subscription plan + Admin promotion rules (`CanCreatePromotions` / `CanAdvertiseProducts`).

### Store analytics

Profile views · Service views · Catalog views · Ad impressions · Customer inquiries · Booking conversions  

---

## 2.4 Administration Portal (Web Only — ADR-006)

**Channel:** Web Administration Portal only. No admin login or admin APIs on mobile.

| Area | Included |
|------|----------|
| Users | User / customer / craftsman / store management |
| Trust | Provider verification, onboarding approval |
| Catalog | Service / category management |
| Monetization | Subscription plans (craftsman + **store**), promotions, advertisement configuration |
| Comms | Notification templates, channel/trigger config |
| Insight | Reports, analytics |
| Governance | Audit logs, global settings, retention policy config |
| Trust ops | Ratings moderation, restriction review, dispute review, support chat access (audited) |

### Financial administration (ADR-013 / 026)

All **configurable** — **no hardcoded business values**:

- Commission configuration  
- Cancellation rules  
- Refund rules  
- Withdrawal configuration  
- Settlement rules  

Finance Admin / Super Admin · MFA · audit · change history  

---

# 3. Core Marketplace Architecture (Frozen)

```text
Provider
  → Provider Type
  → Provider Capabilities
  → Listings / Services
  → Bookings
  → Payments
  → Ledger
  → Settlement
```

Canonical references: ADR-003, ADR-028, ADR-004, ADR-005, ADR-019.

---

# 4. Provider Model Rules (Frozen)

### Types
- Craftsman (individual / independent professional)  
- Store (company / service center / business provider)  

### Capabilities (examples)
`CanCreateServices` · `CanAcceptBookings` · `CanReceivePayments` · `CanAdvertiseProducts` · `CanManageTeam` · `CanCreatePromotions` · `CanManageAvailability`

Type may seed defaults; runtime gates use **capabilities**.

### Customer entry
Search by: **Service · Category · Location · Availability · Rating** (and price, etc.).  

Provider type = trust indicator and **optional** filter only  
(“Verified Professional” / “Verified Company”).

---

# 5. Booking Rules (Frozen)

```text
Customer request
  → Availability validation (provider calendar)
  → Provider confirmation
  → Payment
  → Service execution (incl. field verification where required)
  → Completion
  → Review
```

- Engine is **identical** for craftsman and store providers  
- Accept/fulfill gated by **`CanAcceptBookings`**, not type  
- Cancellation/refund via **admin-configured policies**  

---

# 6. Payment & Financial Scope (Frozen)

```text
Payment.js → Payment Service → Gateway Adapter → Areeba IXOPAY
  → Webhook → Ledger (+ escrow-ready holds)
```

Included:

- Escrow-ready architecture  
- Commission engine (admin rules)  
- Settlement (admin rules)  
- Withdrawals (admin methods/config)  
- **Unified** provider financial accounts (no separate craftsman vs store money pipelines)  

V1 payment gateway implementation: **Areeba IXOPAY Payment.js only**, behind abstraction.

---

# 7. Communication Scope (Frozen)

| In | Out |
|----|-----|
| Booking-scoped chat: Customer ↔ Provider | Open marketplace messaging |
| Optional Admin support access + audit | Social messaging |
| Notifications (email/SMS/push/in-app per templates) | |

---

# 8. Explicitly Excluded From V1

## E-commerce
- Product checkout · Shopping cart · Product payments · Product ordering  
- Inventory management · Delivery / warehouse / fulfillment  

## Advanced marketplace
- Open provider messaging  
- Full arbitration / automated legal dispute resolution  
- Complex workforce / franchise management platforms  
- Self-serve advertising **marketplace** (vs subscription-gated + admin-configured promotions)  

## Other
- Admin on mobile apps  
- Additional payment gateways (beyond IXOPAY adapter)  
- Any feature **not** mapped as Included in the Feature Traceability Matrix  

---

# 9. Future Roadmap (Not V1)

Track separately — **do not implement in V1**:

- Advanced provider teams / workforce management  
- Franchise support  
- Advanced logistics  
- Full product commerce (transactional)  
- Automated multi-party dispute resolution  
- Advanced AI recommendations / dynamic pricing  
- Multi-gateway routing beyond config switch  
- White-label multi-tenant SaaS  

---

# 10. Change Control Process

After this baseline is signed, **any new feature** requires:

### 1. Business justification
Why now? Who benefits? What fails if deferred?

### 2. Impact analysis
Architecture · Database · API · Security · Cost · Timeline · FTM update · ADR if architectural  

### 3. Approval decision (exactly one)
- **Add to V1** (exception — requires Product + Architecture + Eng lead)  
- **Move to future roadmap**  
- **Reject**  

Unauthorized scope additions during implementation are **out of process** and must be refused or escalated.

---

# 11. Traceability & Governance Links

| Artifact | Role |
|----------|------|
| [`FEATURE_TRACEABILITY_MATRIX.md`](./FEATURE_TRACEABILITY_MATRIX.md) | Feature ↔ module ↔ entity ↔ API ↔ UI |
| [`adr/`](./adr/README.md) | Architectural decisions ADR-001–028 |
| [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) | Implementation readiness (currently **NOT READY**) |
| [`MASTER_IMPLEMENTATION_PROMPT_v1.0.md`](./MASTER_IMPLEMENTATION_PROMPT_v1.0.md) | Controlling brief |

---

# 12. Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| Product Owner | | | ☐ Freeze / ☐ Reject |
| Solution Architect | | | ☐ Freeze / ☐ Reject |
| Engineering Lead | | | ☐ Acknowledge |
| Finance | | | ☐ Acknowledge financial scope |
| Design | | | ☐ Acknowledge (assets still required for UI) |

---

**End of Final Scope Baseline v1.0**
