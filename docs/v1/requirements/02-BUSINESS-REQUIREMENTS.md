# 2. Business Requirements

**Document ID:** KHAD-V1-BRD  
**Status:** Draft for Approval  

---

## 2.1 Business Vision (V1)

Launch a production-ready **services marketplace MVP+** that:

1. Enables customers to discover, book, and pay for services
2. Enables craftsmen to onboard, verify, subscribe, fulfill jobs, and request withdrawals
3. Enables stores to sell products/services and manage related bookings/orders
4. Enables administrators to govern users, monetization, quality, and communications

V1 prioritizes **clean architecture and operational correctness** over feature breadth.

## 2.2 Business Objectives

| ID | Objective | Success Signal (measurable after go-live) |
|----|-----------|-------------------------------------------|
| BO-001 | Reduce time-to-book for customers | Median booking creation time within target (target TBD Q-KPI-001) |
| BO-002 | Ensure craftsman trust & compliance | % approved craftsmen with completed identity verification |
| BO-003 | Monetize platform | Commission + subscription revenue tracked in settlement overview |
| BO-004 | Improve job quality | Survey response rate + quality score trends |
| BO-005 | Operational control | Admin can suspend actors and audit sensitive actions |
| BO-006 | Payment reliability | Successful capture rate for online payments (target TBD Q-KPI-002) |

## 2.3 Business Capabilities (Capability Map)

```text
Identity & Access
  ├─ Registration / Login
  ├─ RBAC
  └─ Audit

Marketplace Supply
  ├─ Craftsman catalog & onboarding
  ├─ Store products & services
  └─ Advertisements

Marketplace Demand
  ├─ Search / browse
  ├─ Booking engine
  └─ Orders (store)

Trust & Safety
  ├─ Document OCR
  ├─ Face recognition
  ├─ GPS / QR / OTP verification
  └─ Quality scoring & restrictions

Monetization
  ├─ Payments (Areeba IXOPAY)
  ├─ Commissions
  ├─ Subscriptions
  └─ Settlements / withdrawals

Engagement
  ├─ Notifications (Email/SMS/In-app)
  ├─ Ratings & reviews
  └─ Follow-up / reminders
```

## 2.4 Business Rules — Confirmed from Brief

| ID | Rule |
|----|------|
| BR-001 | V1 supports **only** Areeba IXOPAY Payment.js as payment gateway implementation |
| BR-002 | Payment module **must** use an abstraction so future gateways do not change business logic |
| BR-003 | Platform has four primary client surfaces: Admin Portal, Store Dashboard, Customer App, Craftsman App |
| BR-004 | Craftsman onboarding requires approval workflow |
| BR-005 | Identity verification includes GPS proximity, selfie, and QR/OTP job verification |
| BR-006 | Quality layer includes reminders, surveys, scoring, and restriction rules (no permanent auto-block without admin review) |
| BR-007 | Architecture must be multi-language, multi-currency, and multi-region **ready** |
| BR-008 | **Administrators authenticate only via Administration Portal (web). No admin login or admin APIs on mobile.** |
| BR-009 | **Default Market = Lebanon** (USD, +961, Arabic RTL primary, English LTR, Asia/Beirut) — not hardcoded |
| BR-010 | Stores are service providers + advertisers; **promotional product catalog allowed**; **no e-commerce** (ADR-002/027) |
| BR-011 | **Unified Provider + Listing + Capabilities** — service-first UX; booking via CanAcceptBookings; unified finance (ADR-003/028) |
| BR-012 | **Financial ledger is mandatory**; money events immutable |
| BR-013 | Booking sequence: **request → provider confirmation → payment → execution → …** |
| BR-014 | **Admin MFA required** |
| BR-015 | Promotions/ads in V1 are **admin-managed** (no self-serve ads marketplace) |
| BR-016 | **Chat** is in V1 customer scope |
| BR-017 | **Redis + background workers** required |
| BR-018 | **Commission, cancellation, refund, withdrawal, and settlement rules are admin-configurable and must not be hardcoded** (ADR-013) |

## 2.5 Business Rules — Not Confirmed (Do Not Invent)

The following are **required capabilities** but **policy values are undecided**:

| Topic | Why it matters | Question IDs |
|-------|----------------|--------------|
| Commission model | Affects ledger design and store/craftsman net earnings | Q-COM-001..005 |
| Settlement cadence & method | Affects withdrawal and finance ops | Q-SET-001..004 |
| Who pays for ads | Affects ads billing workflow | Q-ADS-001..003 |
| Booking cancellation / refund policy | Affects payment reverse flows | Q-BOOK-001..006 |
| Craftsman subscription gating | Affects entitlement checks | Q-SUB-001..004 |
| Identity thresholds (GPS meters, face match score) | Affects verification pass/fail | Q-IDV-001..005 |
| Primary launch market(s), currencies, languages | Affects defaults and compliance | Q-LOC-001..004 |
| Store vs craftsman relationship model | Affects booking assignment | Q-REL-001..003 |

## 2.6 Personas (Business View)

| Persona | Goal | Pain if unmet |
|---------|------|---------------|
| Customer | Book trusted help quickly and pay securely | No-shows, unclear status, payment friction |
| Craftsman | Get approved, get jobs, get paid | Slow onboarding, opaque earnings |
| Store Operator | Sell offerings and manage demand | Fragmented order/booking tools |
| Platform Admin | Control risk, quality, and revenue | Manual ops, no auditability |
| Finance Ops | Reconcile payments/commissions/settlements | Unmatched ledgers |
| Support Agent | Resolve disputes with evidence | Missing verification/audit data |

> Note: Support Agent role may map to an Admin sub-role — see Q-RBAC-001.

## 2.7 Regulatory / Compliance Business Drivers

| Driver | V1 Implication |
|--------|----------------|
| PCI DSS scope minimization | Card data collected via Payment.js hosted fields; platform stores tokens/transaction refs only |
| Personal data protection | Consent, retention, access control for PII (exact regulation set TBD Q-CMP-001) |
| KYC / identity | Document + biometric checks for craftsmen before full activation (thresholds TBD) |
| Financial auditability | Immutable payment/commission/settlement event history |

## 2.8 Assumptions (Temporary — Must Be Validated)

These are **working assumptions for architecture seams only**, not approved product policy:

1. A single logical backend API serves all clients (multi-module monolith or modular monolith first).
2. V1 deploys primarily to one region but schemas/config support region codes.
3. Money amounts are stored as integer minor units + ISO currency code.
4. Soft-delete is preferred for master data; hard-delete only under retention policy.

Each assumption is reversible without rewriting domain boundaries if challenged.

## 2.9 Business Success Criteria for Architecture Approval

Architecture is considered business-ready when:

1. All P0 capabilities have a module owner and API surface
2. All money-moving flows have an auditable event model
3. All open questions that block schema/API contracts are either answered or explicitly deferred with a safe default seam
4. V1 vs V2 boundaries are agreed
