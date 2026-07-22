# KHADAMATI — Business Requirements (Phase 0 Baseline)

**Document version:** 1.0  
**Date:** 2026-07-22  
**Status:** Governance baseline — no implementation changes in this phase  
**Product:** Maintenance and home-services marketplace (customers, craftsmen, stores, administrators)

---

## 1. Purpose

This document captures **existing platform capabilities** as formal business requirements (`REQ-xxx`). It is the authoritative input for traceability, gap analysis, and phased delivery. Requirements reflect what is implemented today, including known gaps noted inline.

---

## 2. Stakeholders & Roles

| Role | Description |
|------|-------------|
| **Customer** | Books services, manages profile, addresses, subscriptions, support |
| **Craftsman** | Offers services, working hours, accepts/completes bookings |
| **Store Owner / Employee** | Manages store profile and product catalog |
| **Admin / SuperAdmin** | Full platform operations via admin portal |
| **SupportAgent / Moderator** | Scoped admin permissions for support and moderation |
| **Accountant** | Financial read/report permissions (RBAC-defined) |

---

## 3. Functional Requirements

### 3.1 Identity & Authentication

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-AUTH-001 | Users can register with email, phone, password, and role selection (Customer, Craftsman, StoreOwner) | Must | Confirm password enforced on web, Android, iOS register |
| REQ-AUTH-002 | Users can log in with email and password; receive JWT access + refresh tokens | Must | 15-minute access token; refresh 7–30 days |
| REQ-AUTH-003 | Users can refresh and revoke tokens (logout) | Must | Session tracked via `X-Session-Id` |
| REQ-AUTH-004 | Users can request password reset via email link | Must | Dev mode may expose reset link in API response |
| REQ-AUTH-005 | Users can reset password with token | Must | Confirm password on web |
| REQ-AUTH-006 | Authenticated users can change password | Must | Confirm password on web; revokes other sessions |
| REQ-AUTH-007 | Users can verify email address | Must | Resend verification supported |
| REQ-AUTH-008 | Users can verify phone via OTP | Should | SMS provider abstraction (Development default) |
| REQ-AUTH-009 | Account lockout after failed login attempts | Must | 5 attempts / 15-minute lockout |
| REQ-AUTH-010 | Password policy enforced (length, complexity, history) | Must | Min 8 chars; history count 5 |
| REQ-AUTH-011 | Admin can force-verify user email | Should | `POST /api/v1/auth/admin/verify-email` |

**Known gaps:** Admin user creation form lacks confirm-password field. Mobile change-password screens not implemented.

---

### 3.2 User Profile & Addresses

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-USER-001 | Users can view and update profile (name, language, avatar, etc.) | Must | `/api/v1/profile`, `/api/v1/users/me` |
| REQ-USER-002 | Users can manage multiple delivery/service addresses | Must | CRUD on `/api/v1/users/me/addresses` |
| REQ-USER-003 | Users can view login history | Should | Permission-gated |
| REQ-USER-004 | Users can view and revoke active sessions | Should | Logout other devices / all devices |

---

### 3.3 Service Catalog

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-CAT-001 | Public catalog of service categories (bilingual EN/AR) | Must | |
| REQ-CAT-002 | Public catalog of services with optional category filter | Must | |
| REQ-CAT-003 | Admin can CRUD categories | Must | Admin portal |
| REQ-CAT-004 | Admin can CRUD services | Must | Base price, active flag |

---

### 3.4 Booking & Scheduling

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-BOOK-001 | Customers can search craftsmen by service | Must | |
| REQ-BOOK-002 | Customers can find nearby craftsmen by coordinates and radius | Must | Geo ordering; integration test currently flaky |
| REQ-BOOK-003 | Customers can check craftsman availability for a date | Must | Slot reservations |
| REQ-BOOK-004 | Customers can create a service booking (service request) | Must | Status workflow |
| REQ-BOOK-005 | Customers can view booking list and detail | Must | |
| REQ-BOOK-006 | Craftsman can accept or reject booking | Must | |
| REQ-BOOK-007 | Customer or craftsman can cancel booking | Must | |
| REQ-BOOK-008 | Craftsman can mark booking complete or no-show | Must | |
| REQ-BOOK-009 | Parties can reschedule booking | Must | |
| REQ-BOOK-010 | Customer can confirm booking | Should | |
| REQ-BOOK-011 | Customer can pay for booking | Should | Development payment provider default |
| REQ-BOOK-012 | Customer can confirm payment | Should | Moyasar webhook supported |
| REQ-BOOK-013 | Customer can submit review after booking | Should | |
| REQ-BOOK-014 | Admin can monitor all bookings and stats | Must | Admin portal |

**Known gaps:** No optimistic concurrency (`rowversion`) on slots/bookings. `BookingService` is monolithic (~650 LOC) with limited unit test coverage.

---

### 3.5 Craftsman Portal

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-CRAFT-001 | Craftsman can manage profile (bio, specialization, service area) | Must | |
| REQ-CRAFT-002 | Craftsman can link/unlink offered services | Must | |
| REQ-CRAFT-003 | Craftsman can set working hours | Must | |

---

### 3.6 Store Portal

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-STORE-001 | Store owner can manage store profile | Must | |
| REQ-STORE-002 | Store owner can CRUD products | Must | |

---

### 3.7 Subscriptions

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-SUB-001 | Public listing of subscription plans | Must | Role-targeted plans |
| REQ-SUB-002 | User can subscribe to a plan | Must | |
| REQ-SUB-003 | User can view current subscription and history | Must | |
| REQ-SUB-004 | User can cancel subscription | Must | |
| REQ-SUB-005 | User can toggle auto-renew | Should | |
| REQ-SUB-006 | Admin can CRUD plans (clone, activate, suspend, archive) | Must | |
| REQ-SUB-007 | Admin can assign/cancel user subscriptions | Must | |

---

### 3.8 Payments & Coupons

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-PAY-001 | Booking payment initiation and confirmation | Should | Moyasar integration scaffolded |
| REQ-PAY-002 | Payment webhook processing (Moyasar) | Should | `POST /api/v1/webhooks/moyasar` |
| REQ-PAY-003 | Customers can validate coupon codes | Should | |
| REQ-PAY-004 | Admin can manage coupons | Must | |
| REQ-PAY-005 | Admin can view payments | Must | |

---

### 3.9 Messaging & Notifications

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-MSG-001 | Users can register device push tokens | Should | FCM/APNs providers scaffolded |
| REQ-MSG-002 | In-app notifications list with mark-read | Must | |
| REQ-MSG-003 | Chat per booking (conversations and messages) | Should | Web + mobile |
| REQ-MSG-004 | Admin can send/manage platform notifications | Should | Generic admin module |

---

### 3.10 Support & Complaints

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-SUP-001 | Users can file complaints | Must | |
| REQ-SUP-002 | Users can open support tickets | Must | |
| REQ-SUP-003 | Users can view their complaints and tickets | Must | |
| REQ-SUP-004 | Admin can resolve complaints | Must | |
| REQ-SUP-005 | Admin can close support tickets | Must | |

---

### 3.11 Verification & Trust

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-VER-001 | Users can upload verification documents | Must | |
| REQ-VER-002 | Admin can approve/reject verification documents | Must | |

---

### 3.12 Locations (Regions & Cities)

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-LOC-001 | Public read of active regions and cities | Must | |
| REQ-LOC-002 | Admin can CRUD regions and cities | Must | |
| REQ-LOC-003 | Seed/import location data | Should | Admin bulk endpoint |

**Known gaps:** No `Countries` table; defaults lean SAR/SA/Riyadh. Currency not fully globalized.

---

### 3.13 Marketing

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-MKT-001 | Public active advertisements by placement | Should | Home page placements |
| REQ-MKT-002 | Admin can CRUD advertisements | Must | |

---

### 3.14 Administration & RBAC

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-ADM-001 | Admin dashboard with KPIs | Must | Users, bookings, revenue, tickets |
| REQ-ADM-002 | Admin generic module list/export/bulk actions | Must | `GET/POST /api/v1/admin/{module}` |
| REQ-ADM-003 | Admin user management (CRUD, suspend, roles, permissions) | Must | |
| REQ-ADM-004 | Role-permission matrix management | Must | |
| REQ-ADM-005 | System settings key-value management | Must | |
| REQ-ADM-006 | Analytics and report generation | Should | PDF via QuestPDF |
| REQ-ADM-007 | System health monitoring | Must | DB latency, integration readiness |
| REQ-ADM-008 | Database backup and restore jobs | Should | |
| REQ-ADM-009 | Audit logs and activity logs | Should | |
| REQ-ADM-010 | Permission-gated admin navigation | Must | Web `permissions.ts` + API `HasPermission` |

---

### 3.15 Platform & Operations

| ID | Requirement | Priority | Notes |
|----|-------------|----------|-------|
| REQ-OPS-001 | Health and readiness endpoints | Must | `/api/v1/health`, `/ready` |
| REQ-OPS-002 | Integration readiness check | Should | Email, SMS, payment, push |
| REQ-OPS-003 | API rate limiting | Must | AspNetCoreRateLimit |
| REQ-OPS-004 | CORS for web origins | Must | Dev ports 5173–5177 |
| REQ-OPS-005 | Bilingual UI (Arabic / English) | Must | Web i18n; mobile L10n |
| REQ-OPS-006 | Database migrate and seed on startup | Must | EF migrations; demo seeder in Development |
| REQ-OPS-007 | Soft-delete on core entities | Must | Global query filters |

---

## 4. Non-Functional Requirements

| ID | Requirement | Target |
|----|-------------|--------|
| REQ-NFR-001 | API version prefix | `api/v1` |
| REQ-NFR-002 | Backend framework | .NET 8, Clean Architecture |
| REQ-NFR-003 | Database | SQL Server; EF Core 8 canonical schema |
| REQ-NFR-004 | Web stack | React 19, Vite 8, MUI 9, TypeScript |
| REQ-NFR-005 | Mobile | Android (Kotlin/Compose), iOS (SwiftUI) |
| REQ-NFR-006 | Auth transport | JWT Bearer + refresh tokens |
| REQ-NFR-007 | Logging | Serilog (console + file) |
| REQ-NFR-008 | Unit test baseline | ~73 xUnit tests in `Khadamati.Tests` |
| REQ-NFR-009 | Integration tests | `Khadamati.IntegrationTests` (6 scenarios) |

---

## 5. Out of Scope (Current Baseline)

- Multi-country tax/VAT engine
- Real-time GPS craftsman tracking
- Native mobile admin app
- Automated credential rotation (planned Phase 0.2 migration)
- SQL script deployment as production path (EF migrations are canonical)

---

## 6. Related Documents

| Document | Purpose |
|----------|---------|
| [TRACEABILITY_MATRIX.md](./TRACEABILITY_MATRIX.md) | Requirement → implementation mapping |
| [DATABASE_BASELINE.md](./DATABASE_BASELINE.md) | Schema authority and drift |
| [API_INVENTORY.md](./API_INVENTORY.md) | REST endpoint catalog |
| [MODULE_INVENTORY.md](./MODULE_INVENTORY.md) | Web, admin, mobile screens |
| [DEPLOYMENT_RUNBOOK.md](./DEPLOYMENT_RUNBOOK.md) | Environments, secrets, operations |

---

## 7. Approval

| Role | Name | Date | Status |
|------|------|------|--------|
| Product Owner | _Pending_ | | |
| Tech Lead | _Pending_ | | |
| Security | _Pending_ | | |
