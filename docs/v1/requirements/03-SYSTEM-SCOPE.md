# 3. System Scope

**Document ID:** KHAD-V1-SCOPE  
**Status:** Draft for Approval  

---

## 3.1 In Scope (V1)

### 3.1.1 Client Applications

| Application | In Scope |
|-------------|----------|
| Administration Portal (React/TS/MUI) | Admin governance; **web-only**; MFA required |
| Store Dashboard (React/TS/MUI) | Services, promotional catalog ads, bookings, subscriptions, promotions, analytics — **no e-commerce** |
| Customer Mobile (Flutter) | Discovery, booking, pay, chat, reviews, notifications |
| Craftsman Mobile (Flutter) | Onboarding, jobs, IDV field checks, subscriptions, earnings |

### 3.1.2 Backend & Platform

- Spring Boot 3.x modular REST API (Java 21, Maven)
- Spring Security JWT (+ admin MFA)
- PostgreSQL + Flyway
- OpenAPI documentation
- **Market** config (Lebanon default; multi-market ready)
- **Provider + Listing** unified marketplace model
- Payment abstraction + Areeba IXOPAY Payment.js adapter
- **Financial ledger** (immutable)
- Notification orchestration (email, SMS, in-app, push)
- Booking engine (**confirm → pay**)
- Commission engine + settlements views
- Subscription entitlement engine (craftsman)
- Identity verification (OCR/face/GPS/QR-OTP integration points)
- **Chat** (customer ↔ provider)
- Admin-managed promotions / featured listings
- Audit logging
- Reporting / analytics
- **Redis** + **background workers** / queues
- Dockerized deployment + CI/CD readiness

### 3.1.3 Quality / Follow-up Layer

- 24-hour booking reminders
- Arrival GPS + selfie verification
- Job progress monitoring
- Customer satisfaction survey
- Quality scoring
- Restriction rules: warn → flag → restrict visibility → **admin review** (no permanent auto-block)

## 3.2 Out of Scope (V1)

| Item | Rationale |
|------|-----------|
| Store **e-commerce** (cart, checkout, product payment, inventory, orders, fulfillment) | ADR-002/027 — promotional catalog only |
| Self-serve advertising marketplace | ADR-007 — admin rules + subscription-gated store promotions |
| Self-serve advertising marketplace | ADR-007 — admin-managed promotions only |
| Non-Areeba payment gateways | Explicit V1 constraint |
| Admin login / admin APIs on mobile | ADR-006 |
| Permanent automatic provider bans without review | ADR-008 |
| Reusing prior ASP.NET / SQL Server runtime | New version / new stack |
| UI coding before UI/UX specification from design analysis | ADR-010 + Master Prompt §14/20 |
| Native Kotlin/Swift rewrite as primary apps | ADR-011 — Flutter approved |
| Full multi-tenant white-label SaaS | Market readiness only |
| Advanced ML recommendations / dynamic pricing | V2 candidate |

## 3.3 Scope Boundaries by Bounded Context

| Bounded Context | Owns | Does Not Own |
|-----------------|------|--------------|
| Identity & Access | Users, credentials, roles, sessions | Business profiles beyond auth identity |
| Customer Domain | Customer profile, addresses | Payments execution |
| Craftsman Domain | Craftsman profile, catalog, onboarding state | Final approval policy UI (Admin uses it) |
| Store Domain | Store profile, products, services, orders | Platform-wide ads policy |
| Booking | Booking aggregate & transitions | Payment capture internals |
| Payments | Payment intents, gateway adapters, webhooks | Commission business rates |
| Commissions | Commission lines from events | Bank settlement execution details |
| Subscriptions | Plans, entitlements, renewals | Marketing content |
| Notifications | Templates, deliveries, provider adapters | Business decision to notify (domain emits events) |
| Trust & Safety | Verification cases, scores, restrictions | Booking price calculation |
| Catalog/Search | Categories, search indexes/projections | Ranking ML (V2) |
| Ads | Campaigns, placements, scheduling | Creative design tooling |
| Reporting | Aggregations, exports | Real-time BI warehouse (optional V1.x/V2) |
| Admin Ops | Settings, audit query UI | End-user UX |

## 3.4 Integration Scope (V1)

| Integration | Direction | Notes |
|-------------|-----------|-------|
| Areeba IXOPAY (Payment.js + server API + callbacks) | Bi-directional | Required |
| Email provider | Outbound | Provider TBD Q-NTF-001 |
| SMS provider | Outbound | Provider TBD Q-NTF-002 |
| Push notifications (FCM/APNs via Flutter) | Outbound | Required for in-app/push |
| OCR provider | Outbound | Provider TBD Q-OCR-001 |
| Face recognition provider | Outbound | Provider TBD Q-IDV-002 |
| Object storage (documents/media) | Bi-directional | Provider TBD Q-STO-001 |
| Maps / Geolocation | Client + optional server validate | Provider TBD Q-MAP-001 |

## 3.5 Data Scope

**In scope data classes:** identity, profiles, catalogs, bookings, orders, payments, commissions, subscriptions, notifications, ads, ratings, verification artifacts metadata, audit events, settings.

**Sensitive data handling:** card PANs/CVV never stored by KHADAMATI; biometric samples retention policy TBD (Q-IDV-006).

## 3.6 Deployment Scope

- Local Docker Compose for development
- Staging environment
- Production environment
- CI/CD pipelines for backend, web apps, and Flutter build artifacts

Exact cloud vendor is a decision (Q-DEP-001) but architecture remains cloud-portable.

## 3.7 Questions Requiring Business Decision

`Q-COMMS-001`, `Q-NTF-001/002`, `Q-OCR-001`, `Q-IDV-002/006`, `Q-STO-001`, `Q-MAP-001`, `Q-DEP-001`, plus monetization/settlement questions that affect inclusion of payout execution depth in V1.
