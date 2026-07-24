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

**Important:** Stores are **service providers**.

Stores **DO NOT** sell products.

**Remove / out of scope:**

- Product inventory  
- Product catalog  
- Shopping cart  
- Product ordering  
- Product sales  

**Store features:**

- Store profile  
- Verification  
- Service listings  
- Provider/staff affiliation  
- Booking management  
- Promotions  
- Advertisements (admin-aligned; see Admin)  
- Analytics  

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

**Commission Management** — Commission rules, settlement views, financial reports  

**Notification Management** — SMS, Email, Push, In-app; templates, scheduling, event triggers  

**Analytics** — Users, Providers, Bookings, Revenue, Ratings, Performance, Follow-up metrics  

---

## 5. Marketplace Architecture

Implement a **unified provider architecture**.

Do not separate marketplace logic by provider type.

```text
Provider
  |-- Craftsman Profile
  |-- Store Profile
```

Marketplace search operates through:

**Listing** — service offering, provider availability, category, location, rating, subscription status.

---

## 6. Booking Architecture

Core workflow:

```text
Customer → Search service → Select provider → Choose date/time/location
→ Booking request → Provider confirmation → Payment → Service execution
→ Completion approval → Commission calculation → Provider payout → Review
```

Support: booking status history, timestamps, audit trail, cancellation rules.

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
