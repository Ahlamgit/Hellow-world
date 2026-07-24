# 8. Complete Module Breakdown

**Document ID:** KHAD-V1-MOD  
**Status:** Draft for Approval  

Modules are logical bounded contexts inside a **modular monolith** for V1 (extractable later). Each module owns its persistence tables, domain services, and API controllers (or facades).

---

## 8.1 Module Catalog

| Module | Code | Responsibility | Primary Clients |
|--------|------|----------------|-----------------|
| Platform Kernel | `platform` | Market, config, i18n keys, clock, idempotency, outbox, Redis ports | All |
| Identity & Access | `iam` | Users, OTP, JWT, MFA, roles, sessions, audience enforcement | All |
| Customer | `customer` | Customer profile, addresses | Customer, Admin |
| Provider | `provider` | Unified provider + craftsman/store profiles, affiliations, availability | Craftsman, Store, Admin, Customer |
| Listing / Catalog | `listing` | Categories, listings, search/proximity projections | Customer, providers, Admin |
| Booking Engine | `booking` | Booking aggregate, lifecycle, reminders hooks | All apps |
| Payments | `payment` | Payment intents, IXOPAY adapter, webhooks | Customer, Craftsman, Admin |
| Ledger | `ledger` | Immutable financial entries, escrow-ready accounts, withdrawals | System, Admin, Craftsman |
| Commissions | `commission` | Rules + commission lines posting to ledger | Admin, Finance |
| Subscriptions | `subscription` | Plans, subscriptions, entitlements | Craftsman, Admin |
| Notifications | `notification` | Templates, dispatcher, providers, in-app inbox | All |
| Chat | `chat` | Booking-scoped conversations/messages | Customer, Provider |
| Trust & Safety / IDV | `identity_verification` | Documents, OCR/face/GPS/QR-OTP cases | Craftsman, Store, Admin |
| Promotions / Ads | `ads` | Admin-managed packages, featured slots, visibility | Admin, Store (read) |
| Trust (ratings/quality) | `trust` | Ratings, reviews, surveys, restrictions (review-gated) | Customer, Admin |
| Reporting | `reporting` | Dashboards/exports | Admin, Store, Craftsman |
| Media | `media` | Presigned uploads, object storage | All |
| Audit | `audit` | Append-only audit events | Admin, system |

> **Removed from V1 module catalog:** product commerce / store orders. Admin ops is an API facade over modules, not a separate domain owning rules.

## 8.2 Module Dependency Rules

Allowed dependencies (high → low):

```text
admin, reporting  →  domain modules
booking           →  customer, craftsman, store, catalog, payment(port), notification(port)
payment           →  platform (no dependency on booking internals; uses payment refs)
commission        →  payment events / booking events (via integration events)
subscription      →  payment(port), craftsman/store
identity_verification → media, craftsman
quality           → booking, rating, notification
ads               → store, media
```

**Forbidden:** circular dependencies; domain modules importing admin UI DTOs; business modules importing IXOPAY SDK types (only `payment` adapter may).

## 8.3 Administration Portal Feature → Module Map

| Portal Feature | Modules |
|----------------|---------|
| Advertisements management | `ads`, `media` |
| User management | `iam`, `admin` |
| Customer management | `customer`, `admin` |
| Craftsman management | `craftsman`, `identity_verification` |
| Store management | `store` |
| Craftsman onboarding approvals | `craftsman`, `identity_verification`, `audit` |
| Notification templates | `notification` |
| Email/SMS/In-app notifications | `notification` |
| Commission management | `commission` |
| Settlement overview | `commission`, `payment`, `reporting` |
| Subscription plans | `subscription` |
| Analytics dashboards | `reporting` |
| Reports | `reporting` |
| Ratings management | `rating` |
| Global settings | `platform`, `admin` |
| Audit logs | `audit` |

## 8.4 Store Dashboard Feature → Module Map

| Feature | Modules |
|---------|---------|
| Store profile | `store`, `media` |
| Products / Services | `store`, `catalog` |
| Orders | `store` |
| Advertisements | `ads` |
| Booking management | `booking` |
| Customers | `store` + limited `customer` projection |
| Reports | `reporting` |

## 8.5 Customer App Feature → Module Map

| Feature | Modules |
|---------|---------|
| Registration/Login/Profile | `iam`, `customer` |
| Search/Browse | `catalog`, `store`, `craftsman` |
| Booking + history/status | `booking` |
| Online payment | `payment` |
| Notifications | `notification` |
| Reviews/Ratings | `rating` |

## 8.6 Craftsman App Feature → Module Map

| Feature | Modules |
|---------|---------|
| Registration/Login/Profile | `iam`, `craftsman` |
| Service catalog | `craftsman`, `catalog` |
| Onboarding + documents | `craftsman`, `media`, `identity_verification` |
| Approval workflow | `craftsman` |
| GPS/Selfie/QR/OTP | `identity_verification`, `booking` |
| Subscriptions | `subscription`, `payment` |
| Dashboard/Earnings/KPIs | `reporting`, `commission`, `booking` |
| Withdrawals | `commission`/`payment` settlement submodule |
| Notifications | `notification` |

## 8.7 Shared Technical Modules

| Concern | Implementation Home |
|---------|---------------------|
| Outbox / integration events | `platform` |
| Idempotency keys | `platform` |
| Feature flags / settings | `platform` |
| OpenAPI / problem details | API layer cross-cutting |
| Observability | infrastructure cross-cutting |

## 8.8 Extraction Strategy (Post-V1)

V1 ships as modular monolith. Extraction candidates after traffic justifies it:

1. `payment` + webhook workers  
2. `notification` dispatcher  
3. `identity_verification` workers  
4. `reporting` read replicas / warehouses  

## 8.9 Questions Requiring Business Decision

- Q-SUB-001: Are stores subscription-billed in V1?  
- Q-REL-001: Can a craftsman belong to a store, be independent, or both?  
- Q-SET-003: Are withdrawals a `commission` concern, `payment` concern, or separate `payout` module?
