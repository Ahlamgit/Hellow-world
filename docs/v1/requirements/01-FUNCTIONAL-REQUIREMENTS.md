# 1. Functional Requirements Specification (FRS)

**Document ID:** KHAD-V1-FRS  
**Status:** Draft for Approval  
**Depends on:** Business Requirements, System Scope, Open Questions  

---

## 1.1 Purpose

Define what the KHADAMATI V1 platform **must do** for each client application and shared domain capability, independent of UI look-and-feel (UI assets arrive post-architecture approval).

## 1.2 Product Summary

KHADAMATI is a multi-sided marketplace connecting:

- **Customers** seeking home/maintenance services (and related store products/services)
- **Craftsmen** providing services under platform rules, verification, and subscriptions
- **Stores** offering products/services, bookings, and advertisements
- **Administrators** operating platform governance, monetization, quality, and support

## 1.3 Requirement Priority Legend

| Priority | Meaning |
|----------|---------|
| **P0** | Must ship for V1 go-live |
| **P1** | Should ship in V1 if capacity allows; otherwise early V1.x |
| **P2** | Explicitly deferred unless business re-prioritizes (see V2) |

---

## 1.4 Administration Portal — Functional Requirements

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-ADM-001 | Manage platform users (create/disable/role assignment within admin RBAC model) | P0 |
| FR-ADM-002 | Manage customers (view, search, status changes within approved rules) | P0 |
| FR-ADM-003 | Manage craftsmen (view, search, approval state, restriction state) | P0 |
| FR-ADM-004 | Manage stores (view, search, activation/suspension) | P0 |
| FR-ADM-005 | Review and decide craftsman onboarding applications | P0 |
| FR-ADM-006 | Manage notification templates (email, SMS, in-app) with locale keys | P0 |
| FR-ADM-007 | Trigger/monitor outbound notification deliveries (operational visibility) | P0 |
| FR-ADM-008 | Configure multi-dimensional commission rules (effective-dated; history; audit) — **not hardcoded** | P0 |
| FR-ADM-009 | Configure settlement rules + view settlement overview | P0 |
| FR-ADM-010 | Manage subscription plans for craftsmen (and optionally stores — Q-SUB-001) | P0 |
| FR-ADM-011 | Manage admin promotions / featured listings | P0 |
| FR-ADM-012 | Manage ratings moderation (hide/flag/restore) | P0 |
| FR-ADM-013 | Configure global settings (feature flags, operational parameters) | P0 |
| FR-ADM-014 | View audit logs (who/what/when/before-after for sensitive actions) | P0 |
| FR-ADM-015 | Analytics dashboards (core KPIs) | P1 |
| FR-ADM-016 | Exportable operational / financial reports | P1 |
| FR-ADM-017 | Quality restriction review (no permanent auto-ban) | P1 |
| FR-ADM-018 | Configure cancellation policies (actors, statuses, time, penalties, approvals) | P0 |
| FR-ADM-019 | Configure refund rules (full/partial/none/manual) | P0 |
| FR-ADM-020 | Configure withdrawal methods, minimums, approval workflows | P0 |
| FR-ADM-021 | Finance Admin RBAC + MFA for all money-policy changes | P0 |

## 1.5 Store Dashboard — Functional Requirements

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-STR-001 | Manage store profile (logo, business info, location, service areas, contact, status) | P0 |
| FR-STR-002 | CRUD **service listings** (bookable) | P0 |
| FR-STR-003 | Provider/staff affiliation management | P0 |
| FR-STR-004 | Manage bookings related to store services | P0 |
| FR-STR-005 | **Product catalog advertising** (promotional only — ADR-027) | P0 |
| FR-STR-006 | Catalog inquiries / leads inbox | P0 |
| FR-STR-007 | Promotions for services & catalog (subscription + admin rules) | P0 |
| FR-STR-008 | Store subscription status (Admin-managed plans) | P0 |
| FR-STR-009 | Analytics (views, impressions, inquiries, booking conversions) | P1 |
| FR-STR-010 | Store verification / onboarding status | P0 |

**Explicitly out:** cart, checkout, product payment, inventory, warehouse, product orders, fulfillment.

## 1.6 Customer Mobile App — Functional Requirements

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-CUS-001 | Register account | P0 |
| FR-CUS-002 | Login / logout / token refresh | P0 |
| FR-CUS-003 | Manage profile and addresses | P0 |
| FR-CUS-004 | Browse service categories | P0 |
| FR-CUS-005 | Search services | P0 |
| FR-CUS-006 | Browse stores | P0 |
| FR-CUS-007 | Create service booking | P0 |
| FR-CUS-008 | View booking history and status | P0 |
| FR-CUS-009 | Pay online via Areeba IXOPAY Payment.js (tokenized card flow) | P0 |
| FR-CUS-010 | Receive and view notifications | P0 |
| FR-CUS-011 | Submit reviews and ratings for completed jobs | P0 |
| FR-CUS-012 | Multi-language UI (Arabic RTL primary, English LTR) | P0 |
| FR-CUS-013 | OTP authentication | P0 |
| FR-CUS-014 | Provider proximity search | P0 |
| FR-CUS-015 | Chat with provider | P0 |
| FR-CUS-016 | Service completion confirmation | P0 |

## 1.7 Craftsman Mobile App — Functional Requirements

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-CRF-001 | Register account | P0 |
| FR-CRF-002 | Login / logout / token refresh | P0 |
| FR-CRF-003 | Manage profile | P0 |
| FR-CRF-004 | Maintain personal service catalog | P0 |
| FR-CRF-005 | Complete guided onboarding (documents, identity steps) | P0 |
| FR-CRF-006 | Upload required documents | P0 |
| FR-CRF-007 | Track approval workflow status | P0 |
| FR-CRF-008 | GPS proximity verification for jobs (thresholds TBD Q-IDV-001) | P0 |
| FR-CRF-009 | Real-time selfie verification for jobs/onboarding (provider TBD Q-IDV-002) | P0 |
| FR-CRF-010 | QR and/or OTP job verification | P0 |
| FR-CRF-011 | Subscription plan browse/subscribe/renew | P0 |
| FR-CRF-012 | Dashboard with earnings and KPIs | P0 |
| FR-CRF-013 | Submit withdrawal requests (payout rails TBD Q-SET-002) | P0 |
| FR-CRF-014 | Receive and view notifications | P0 |

## 1.8 Cross-Cutting Domain Requirements

| ID | Domain | Requirement | Priority |
|----|--------|-------------|----------|
| FR-X-001 | Payments | Abstract payment gateway interface; V1 adapter = Areeba IXOPAY only | P0 |
| FR-X-002 | Commissions | Calculate and persist commission events on monetizable transactions | P0 |
| FR-X-003 | Booking Engine | Stateful booking lifecycle with auditable transitions | P0 |
| FR-X-004 | Notifications | Template-driven email, SMS, in-app channels | P0 |
| FR-X-005 | Identity Verification | Document + face + GPS + job verification orchestration | P0 |
| FR-X-006 | OCR | Extract structured fields from uploaded identity/business documents | P1 |
| FR-X-007 | Face Recognition | Match selfie to reference identity artifact | P0 |
| FR-X-008 | Subscriptions | Plan entitlement gating for craftsman (and optionally store) capabilities | P0 |
| FR-X-009 | Ads | Inventory placement and campaign lifecycle | P0 |
| FR-X-010 | Ratings/Reviews | Post-job feedback with moderation | P0 |
| FR-X-011 | Quality Follow-up | Reminders, surveys, scoring, restrictions | P0 |
| FR-X-012 | Audit | Immutable (append-only) audit trail for privileged actions | P0 |
| FR-X-013 | i18n | Message keys + locale-ready content model | P0 |
| FR-X-014 | Multi-currency readiness | Money stored with currency code; conversion policy TBD Q-CUR-001 | P0 |
| FR-X-015 | Multi-region readiness | Region/tenant-ready configuration seams (single-region deploy OK for V1) | P0 |

## 1.9 Non-Functional Requirements (Mapped to Functions)

| ID | NFR | Functional Implication |
|----|-----|------------------------|
| NFR-001 | High performance | Pagination, indexed queries, async notification/payment callbacks |
| NFR-002 | Horizontal scalability | Stateless API nodes behind load balancer |
| NFR-003 | Secure authentication | JWT access + refresh rotation; secure secret storage |
| NFR-004 | RBAC | Role + permission checks on every privileged endpoint |
| NFR-005 | Audit trails | Audit events for admin and money-moving actions |
| NFR-006 | OWASP Top 10 | Input validation, authz, injection defense, secure headers, rate limits |
| NFR-007 | Multi-language readiness | No hardcoded user-facing strings in backend responses where templates apply |
| NFR-008 | Cloud + Docker | Containerized services; 12-factor config |
| NFR-009 | CI/CD readiness | Automated build/test/scan/deploy pipelines |

## 1.10 Explicit Non-Functions for V1 (Out of FRS Scope)

- Additional payment gateways beyond Areeba IXOPAY
- Marketplace chat/messaging (unless decided under Q-COMMS-001)
- Native Android/iOS (non-Flutter) apps
- Real-time websocket marketplace bidding (unless decided under Q-BOOK-010)

See also: [03-SYSTEM-SCOPE.md](./03-SYSTEM-SCOPE.md)

## 1.11 Questions Requiring Business Decision

See master list entries referenced above: `Q-COM-*`, `Q-SET-*`, `Q-SUB-001`, `Q-ADS-*`, `Q-PRD-001`, `Q-IDV-*`, `Q-CUR-001`, `Q-COMMS-001`, `Q-BOOK-010` in [QUESTIONS-REQUIRING-BUSINESS-DECISION.md](../QUESTIONS-REQUIRING-BUSINESS-DECISION.md).
