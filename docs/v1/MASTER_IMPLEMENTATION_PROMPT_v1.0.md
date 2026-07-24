# KHADAMATI Master Implementation & Architecture Prompt v1.0

**Document ID:** KHAD-V1-MASTER-PROMPT  
**Version:** 1.0  
**Status:** Controlling business & architecture brief (pre-implementation)  
**Supersedes conflicting statements** in earlier draft docs where this prompt is more specific.  
**Companion:** [FEATURE_TRACEABILITY_MATRIX.md](./FEATURE_TRACEABILITY_MATRIX.md) · [ARCHITECTURE_AUDIT_FINAL.md](./ARCHITECTURE_AUDIT_FINAL.md) · [QUESTIONS-REQUIRING-BUSINESS-DECISION.md](./QUESTIONS-REQUIRING-BUSINESS-DECISION.md)

---

## Role

You are acting as the Lead Solution Architect, Senior Software Engineer, Database Architect, Security Architect, and UI/UX System Architect for KHADAMATI.

Your responsibility is to transform KHADAMATI from an MVP concept into a production-ready, scalable marketplace platform.

**Do not start implementation until all architecture, design, database, and feature validation steps are completed.**

---

## 1. Source of Truth

The following documents are mandatory sources:

- KHADAMATI business proposal  
- KHADAMATI architecture documentation (`docs/v1/`)  
- Architecture audit recommendations  
- Approved UI/design assets  
- **Business rules defined in this prompt**

Before coding, create:

### Feature Traceability Matrix

Every business feature must map to:

```text
Business Requirement
        |
Module
        |
Database Entity
        |
API
        |
Frontend/Mobile Screen
        |
Implementation Status
```

No feature from the proposal may disappear silently.

If a feature requires scope clarification, create an Architecture Decision Record (ADR).

---

## 2. Product Definition

KHADAMATI is a marketplace platform connecting:

- Customers  
- Certified Craftsmen  
- Service Stores/Businesses  

through:

- Customer Mobile Application  
- Craftsman Mobile Application  
- Store Dashboard  
- Administration Web Portal  

The platform manages:

- Service discovery  
- Booking  
- Payments  
- Commissions  
- Subscriptions  
- Provider verification  
- Service quality follow-up  
- (and related engagement including chat — see Customer features)

---

## 3. Market Architecture

### Initial Market

KHADAMATI launches in **Lebanon**.

Default configuration:

| Setting | Value |
|---------|-------|
| Country | Lebanon |
| Currency | USD |
| Phone Country Code | +961 |
| Primary Language | Arabic RTL |
| Secondary Language | English LTR |
| Timezone | Lebanon (Asia/Beirut) |

### Future Expansion Requirement

The architecture must support future expansion **without redesign**.

Support:

- Multiple countries  
- Multiple currencies  
- Multiple phone formats  
- Multiple languages  
- Multiple time zones  
- Country-specific payment providers  
- Country-specific business rules  

**Do not hardcode Lebanon logic.** Lebanon is the **default market**, not a limitation. Model via `Market` configuration.

---

## 4. User Roles & Applications

### Customer — Mobile application

Features:

- Registration/login  
- OTP authentication  
- Profile management  
- Service discovery  
- Search  
- Categories  
- Filters  
- Provider proximity search  
- Booking creation (date/time, location/address, notes)  
- Payment  
- Booking tracking  
- Service completion confirmation  
- Ratings  
- Reviews  
- Notifications  
- Chat  

### Craftsman — Mobile application

**Account**

- Registration/login  
- Profile management  
- Skills  
- Services  
- Service areas  
- Pricing  
- Availability  

**Verification**

- Document submission  
- Identity verification workflow  
- Approval status  
- OCR integration point  
- Face verification integration point  

**Job Management**

- Receive bookings  
- Accept/reject requests  
- Daily schedule  
- Job lifecycle  
- Earnings dashboard  
- KPIs  

**Field Verification (Mandatory)**

```text
Arrive → GPS validation → Selfie verification → OTP/QR confirmation
→ Start job → Complete job → Customer confirmation
```

**Financial**

- Subscription management  
- Earnings  
- Withdrawal requests  

### Store Dashboard

**Important:** Stores are:

- Service providers  
- Service advertisers  
- **Product catalog advertisers** (promotional showcase only)

Stores **DO NOT** run e-commerce.

**Out of scope (V1):**

- Product checkout / shopping cart  
- Online product purchasing / product payment  
- Product delivery / warehouse / inventory management  
- Order fulfillment  

**Store features:**

- Store profile (logo, images, business info, location, service areas, contact, verification docs, business status)  
- Service management (offerings, categories, descriptions, pricing, areas, availability)  
- **Product catalog advertising** (name, images, descriptions, categories, brands/models, promo info, inquiry CTA) — no cart/checkout  
- Provider/staff affiliation  
- Booking management  
- Store subscriptions (**Admin-managed** plans: duration, visibility, featured, ad limits, promoted services/catalog caps, search benefits)  
- Advertisements for **services** and **catalog items** (subject to subscription + Admin promotion rules)  
- Analytics (profile/service/catalog views, ad impressions, inquiries, booking conversions)  

See ADR-002 (amended) and **ADR-027**.

### Administration Portal

#### IMPORTANT RULE

Admins **ONLY** access through the Web Administration Portal.

No admin login exists in Android/iOS apps.

No admin functionality should be exposed through mobile APIs.

#### Admin features

**User Management** — Customers, Craftsmen, Stores  

**Provider Management** — Craftsman/Store onboarding, verification workflow, approval/rejection, status management  

**Advertisement Management** — Advertisement packages, featured listings, promotion slots, provider visibility  

V1 supports **admin-managed promotions**. Do **not** create a complex self-service advertising marketplace unless separately approved.

**Subscription Management** — Plans, duration, pricing, status, expiry, payments  

**Commission Management** — Admin-configurable commission rules (not hardcoded), settlement views, financial reports  

**Cancellation / Refund / Withdrawal / Settlement Policy Management** — Admin-configurable policies evaluated dynamically by booking/payment/ledger engines (ADR-013). No hardcoded commission, cancellation, refund, withdrawal, or settlement business values.

**Notification Management** — SMS, Email, Push, In-app; templates, scheduling, event triggers  

**Analytics** — Users, Providers, Bookings, Revenue, Ratings, Performance, Follow-up metrics  

**Finance Admin RBAC** — Manage commissions, refunds, withdrawals, cancellation/settlement policies; Super Admin full access; MFA + audit + change history required for all money-rule changes.  

---

## 5. Marketplace Architecture

Implement a **unified Provider architecture** with **capabilities** (ADR-003, **ADR-028**).

Customer experience is **service-first**. Customers must **not** be required to choose Craftsman vs Store before searching. They search by need (e.g. AC repair, plumbing); the system returns suitable providers.

```text
Provider
  → Provider Type (CRAFTSMAN | STORE | future)
  → Provider Capabilities (CanCreateServices, CanAcceptBookings, CanReceivePayments,
      CanAdvertiseProducts, CanManageTeam, CanCreatePromotions, CanManageAvailability, …)
  → Listings / Services
  → Bookings
```

Marketplace search operates on **services/listings** (category, location, availability, rating, price, capabilities). Provider type is optional trust info / filter only (e.g. “Verified Professional” / “Verified Company”).

**Booking** depends on capability `CanAcceptBookings` (same engine for craftsman and store providers).  
**Payments / ledger / settlement / withdrawals** use unified Provider accounts — no separate money flows by type.

### Craftsman (type defaults — overridable by capabilities/subscription)

Individual technician / independent professional: services, availability, accept bookings, receive payments, profile.

### Store (type defaults — overridable)

Company / service center: multiple services, promotional product catalogs (ADR-027), promotions, bookings, payments, business profile, team.

Product catalog remains **promotional only** (no cart/checkout/product payment/orders/inventory/delivery).

---

## 6. Booking Architecture

Core workflow (**service-first**, ADR-028):

```text
Search Service → View Providers → Compare (rating, reviews, availability, distance, price, verification)
→ Select Provider → Select Service → Book request
→ Provider Availability Check → Provider Confirmation (requires CanAcceptBookings)
→ Payment → Service execution → Completion → Commission (admin rules) → Review
```

**Scheduling (ADR-019):** Provider Availability Calendar.  
**Identical booking engine** for craftsman and store providers.

Support: status history, audit, admin-configurable cancellation, lightweight disputes (ADR-021).

---

## 7. Follow-Up & Quality System

- **Before visit:** 24-hour reminder, scheduled notifications  
- **Arrival:** GPS + selfie verification  
- **During service:** status changes, timestamps, progress  
- **After service:** completion confirmation, satisfaction survey, rating  
- **Restriction rules:** warn → flag → restrict visibility if necessary → **require admin review**  

Do **not** automatically permanently block without review.

---

## 8. Financial Architecture

**Mandatory:** proper financial **ledger**. Do not rely only on wallet balances.

Support: Payments, Escrow readiness, Commissions, Provider earnings, Withdrawals, Subscriptions, Financial reporting.

Ledger entries are **immutable and auditable** (ID, User, Booking, Type, Debit, Credit, Currency, Status, Timestamp).

### Admin-configurable financial business rules (mandatory)

The following **must not be hardcoded** in backend logic; they are configured in the Administration Portal:

1. Commission rules  
2. Cancellation rules  
3. Refund rules  
4. Withdrawal rules  
5. Settlement rules  

Canonical money path:

```text
Customer Payment → Payment Gateway → Escrow / Holding → Ledger
→ Completion Approval → Commission Calculation (admin rules)
→ Provider Earnings → Withdrawal Request → Admin Approval → Settlement
```

See ADR-013 and `architecture/47-ADMIN-CONFIGURABLE-FINANCIAL-RULES.md`.

---

## 9. Payment Architecture

Areeba IXOPAY **Payment.js** only.

- No raw card data storage  
- No PCI-sensitive data in backend  
- Tokenized flow  

```text
App → Payment.js → Token → Backend Payment Service → IXOPAY
→ Webhook → Verification → Ledger Update
```

Required: Idempotency, webhook verification, payment state management, failure handling, reconciliation.

---

## 10. Subscription System

Craftsman subscriptions: plans, duration, payment, activation, expiry, renewal status, access restrictions — **configurable**.

---

## 10A. Chat (ADR-020)

Booking-scoped chat only (Customer ↔ Provider). No open marketplace messaging. Real-time + history + read status + push. Optional admin support access with audit.

## 10B. Disputes (ADR-021)

Lightweight: complaint → admin review → evidence → resolution notes → Open/Under Review/Resolved/Closed. May hold escrow/settlement; no complex arbitration in V1.

## 10C. Account Lifecycle (ADR-022)

Deletion request → verify → anonymize → retain financial/audit per configurable retention policies.

## 10D. Integrations (ADR-025)

All external systems behind ports/adapters (Payment, SMS, Email, Maps, OCR, Face).

## 10E. Design Gate (ADR-023)

UI coding blocked until branding/logo/colors/references/videos/screens and design direction are available; then UI/UX analysis + tokens + specs before UI build.

---

## 11–13. Database / Backend / Security

- Normalization, FKs, indexes, constraints, audit tables, soft-delete (non-financial), migrations, views where useful  
- Account deletion: validation, retention, anonymization, audit  
- Modular clean architecture, REST, versioning, authn/authz, validation, logging, monitoring  
- **Redis** caching, **background workers**, queue processing (notifications, schedules, payment reconciliation)  
- OTP protection, rate limiting, session management, RBAC  
- Admin: **web only**, **MFA required**, audit logs, session security, permission controls  

---

## 14–16. UI / UX / Design System / Localization

- Use uploaded design assets as **visual inspiration**  
- Do **NOT** clone pixel-by-pixel — analyze and improve  
- Before UI implementation produce: UI analysis, UX analysis, design tokens, component inventory, screen hierarchy, navigation map, accessibility review, responsive behavior, final design specification  
- Design system: modern, premium, minimal, professional, fast  
- Visual rules: rounded corners 12–20px, soft shadows, 8-point spacing, animations 150–250ms, large touch targets, accessible contrast  
- Localization from day one: **Arabic RTL** + **English LTR**  

---

## 17. Technology Rules

Follow approved KHADAMATI architecture (`docs/v1/`):

| Layer | Approved stack |
|-------|----------------|
| Backend | Spring Boot 3.x, Java 21, Maven |
| DB | PostgreSQL, Flyway |
| Admin + Store web | React, TypeScript, Material UI |
| Customer + Craftsman mobile | Flutter (approved mobile direction) |
| Payments | Areeba IXOPAY Payment.js + abstraction |

Do not introduce conflicting technologies.

---

## 18. Performance

Fast loading, lazy loading, optimized images, efficient lists, minimal rebuilds, smooth animations, offline-friendly where possible. Target **60 FPS** interactions.

---

## 19. Ambiguity Policy

Never assume decisions involving money, permissions, booking lifecycle, security, or data ownership.

Create an ADR or request clarification.

---

## 20. Implementation Gate

Do **NOT** start coding until completed and approved:

- [ ] Final architecture document  
- [ ] Database model  
- [ ] API specification  
- [ ] Feature traceability matrix  
- [ ] UI/UX specification (after design analysis)  
- [ ] Security review  
- [ ] Payment flow review  
- [ ] Deployment architecture  

---

## Final Objective

Build KHADAMATI as a **Lebanon-first** marketplace that is:

- Production-ready  
- Investor-ready  
- Scalable to GCC/international markets  
- Secure  
- PCI-compliant through Payment.js  
- Financially auditable  
- Maintainable  
- Designed with a premium user experience  

Do not reduce functionality from the approved proposal. Any deviation requires explicit approval.
