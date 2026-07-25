# Feature Traceability Matrix — KHADAMATI V1

**Document ID:** KHAD-V1-FTM  
**Status:** Aligned to **Final Scope Baseline v1.0** (Scope Frozen)  
**Source:** Master Implementation Prompt v1.0 · ADR-001…030 · FINAL_SCOPE_BASELINE.md  
**Rule:** No proposal feature may disappear silently. Status values: `Specified` · `Deferred (ADR)` · `Blocked (Q-*)` · `Implemented` (post-coding) · **`Out of V1`** (see Scope Baseline §8)

> **Scope freeze:** Features marked Included in [`FINAL_SCOPE_BASELINE.md`](./FINAL_SCOPE_BASELINE.md) are the only authorized V1 build targets. Additions require Change Control ([`IMPLEMENTATION_READINESS_EXECUTION_PLAN.md`](./IMPLEMENTATION_READINESS_EXECUTION_PLAN.md) §4).  
> **Implementation:** Coding blocked until gate **A** — see [`READINESS_BLOCKER_CLOSURE_STATUS.md`](./READINESS_BLOCKER_CLOSURE_STATUS.md) (**BLOCKER-001…007**) and [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) (currently **B**).

---

## Legend

| Column | Meaning |
|--------|---------|
| BR-ID | Business requirement / feature id |
| Module | Bounded context |
| Entity | Primary DB entities |
| API | Representative endpoints |
| UI | Screen / surface |
| Status | Traceability state |

**Apps:** `CUS` Customer Flutter · `CRF` Craftsman Flutter · `STR` Store Dashboard · `ADM` Admin Portal (web only)

---

## 1. Identity & Access

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-IAM-01 | Customer registration | iam, customer | users, customers | POST /auth/register/customer | CUS: Register | Specified |
| BR-IAM-02 | Customer login | iam | users, refresh_tokens | POST /auth/login (audience customer-app) | CUS: Login | Specified |
| BR-IAM-03 | Customer OTP authentication | iam, notification | otp_challenges | POST /auth/otp/* | CUS: OTP | Specified |
| BR-IAM-04 | Craftsman registration/login | iam, provider | users, providers, craftsman_profiles | POST /auth/register/craftsman, /auth/login | CRF: Auth | Specified |
| BR-IAM-05 | Store operator login | iam, provider | users, store_profiles, store_users | POST /auth/login (store-web) | STR: Login | Specified |
| BR-IAM-06 | Admin login (web only) | iam | users, roles | POST /auth/login (admin-web only) | ADM: Login | Specified |
| BR-IAM-07 | Admin MFA | iam | mfa_credentials | POST /auth/mfa/* | ADM: MFA | Specified |
| BR-IAM-08 | Reject admin on mobile APIs | iam | — | AUTH_ADMIN_WEB_ONLY | CUS/CRF: none | Specified |
| BR-IAM-09 | Session / refresh / logout | iam | refresh_tokens | /auth/refresh, /logout | All | Specified |
| BR-IAM-10 | RBAC permissions | iam | roles, permissions | enforced on all /admin/** | ADM | Specified |
| BR-IAM-11 | Rate limiting / lockout | iam, platform | login_attempts | filters | All | Specified |
| BR-IAM-12 | Account deletion (validation, retention, anonymization, audit) | iam, audit | users + related | DELETE /me or admin delete | CUS/CRF/ADM | Specified |

---

## 2. Market & Localization

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-MKT-01 | Default Market Lebanon | platform | markets | GET /markets/current | All (config) | Specified |
| BR-MKT-02 | Currency USD default | platform, ledger | markets, money cols | money DTO | All | Specified |
| BR-MKT-03 | Phone +961 formatting | iam | users.phone_e164 | validation | Auth screens | Specified |
| BR-MKT-04 | Arabic RTL primary | i18n clients | translations | Accept-Language | All | Specified |
| BR-MKT-05 | English LTR secondary | i18n clients | translations | Accept-Language | All | Specified |
| BR-MKT-06 | Timezone Asia/Beirut default | platform | markets.timezone | scheduling | Booking | Specified |
| BR-MKT-07 | Multi-market readiness (no Lebanon hardcode) | platform | markets | market_id FKs | — | Specified |
| BR-MKT-08 | Unified Provider + capability model (ADR-028) | provider | providers, provider_capabilities | capability checks | All | Specified |
| BR-MKT-09 | Service-first discovery (no mandatory Craftsman/Store chooser) | catalog | listings | search APIs | CUS | Specified |
| BR-MKT-10 | Booking gated by CanAcceptBookings (type-agnostic) | booking | bookings, capabilities | accept/create | CRF/STR | Specified |
| BR-MKT-11 | Unified provider ledger/payments (no type-split money flows) | ledger, payment | ledger_accounts | finance APIs | CRF/STR/ADM | Specified |

---

## 3. Customer — Discovery & Booking

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-CUS-01 | Profile management | customer | customers, addresses | /customers/me | CUS: Profile | Specified |
| BR-CUS-02 | Categories browse (service-first entry) | catalog | categories, translations | GET /categories | CUS: Categories | Specified |
| BR-CUS-03 | Search services by need (no mandatory type chooser) | catalog | listings, providers, capabilities | GET /listings?q= | CUS: Search | Specified |
| BR-CUS-04 | Filters (category, location, price, rating, optional type) | catalog | listings | query params | CUS: Filters | Specified |
| BR-CUS-05 | Provider proximity / distance | catalog | listings, geo | GET /listings?near= | CUS: Near me | Specified |
| BR-CUS-05a | Compare providers (rating, reviews, availability, distance, price, verification) | catalog | listings, ratings | list/detail | CUS: Results | Specified |
| BR-CUS-06 | Select provider then service/listing | catalog, provider | listings, providers | GET detail | CUS: Detail | Specified |
| BR-CUS-06a | Trust label from type (Professional / Company) — not entry gate | provider | providers.type | display only | CUS: badges | Specified |
| BR-CUS-07 | Create booking request | booking | bookings | POST /bookings | CUS: Booking form | Specified |
| BR-CUS-08 | Date/time selection from **provider availability** | booking | availability, booking_schedule | availability + booking APIs | CUS: Available times | Specified |
| BR-CUS-08a | View provider available slots/windows | booking | working_hours, exceptions | GET listings/{id}/availability | CUS: Calendar | Specified |
| BR-CUS-09 | Location/address | booking, customer | addresses, booking snapshot | payload | CUS: Address | Specified |
| BR-CUS-10 | Notes | booking | bookings.notes | payload | CUS: Notes | Specified |
| BR-CUS-11 | Booking tracking / history | booking | bookings, status_history | GET /bookings | CUS: Bookings | Specified |
| BR-CUS-12 | Payment after provider confirm | payment, ledger | payments, ledger_entries | /payments/* | CUS: Pay | Specified |
| BR-CUS-13 | Completion confirmation | booking | bookings | POST /bookings/{id}/confirm-completion | CUS: Complete | Specified |
| BR-CUS-14 | Ratings | trust | ratings | POST /bookings/{id}/ratings | CUS: Rate | Specified |
| BR-CUS-15 | Reviews | trust | reviews | same / separate | CUS: Review | Specified |
| BR-CUS-16 | Notifications | notification | deliveries, in_app | GET /notifications | CUS: Inbox | Specified |
| BR-CUS-17 | Chat (booking-scoped only) | chat | conversations, messages, read_status | /chat/** | CUS: Chat | Specified |
| BR-CUS-18 | Open dispute/complaint | trust | disputes, evidence | POST /bookings/{id}/disputes | CUS: Dispute | Specified |
| BR-CUS-19 | Account deletion request | iam | deletion workflow | DELETE flow / request | CUS: Settings | Specified |

---

## 4. Craftsman

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-CRF-01 | Profile / skills / services | provider, listing | craftsman_profiles, listings | /craftsmen/me/** | CRF: Profile/Catalog | Specified |
| BR-CRF-02 | Service areas | provider | service_areas | CRUD | CRF: Areas | Specified |
| BR-CRF-03 | Pricing | listing | listings.price | CRUD | CRF: Pricing | Specified |
| BR-CRF-04 | Availability calendar (hours, exceptions, holidays) | provider, booking | working_hours, calendar_exceptions | /providers/me/availability | CRF: Availability | Specified |
| BR-CRF-05 | Document submission | idv, media | verification_documents | upload APIs | CRF: Documents | Specified |
| BR-CRF-06 | Identity verification workflow | idv | verification_cases | /verifications/** | CRF: Verify | Specified |
| BR-CRF-07 | Approval status | provider | onboarding_status | GET onboarding | CRF: Status | Specified |
| BR-CRF-08 | OCR integration point | idv | ocr_results | async jobs | CRF/ADM | Specified |
| BR-CRF-09 | Face verification integration | idv | face_checks | async jobs | CRF | Specified |
| BR-CRF-10 | Receive bookings | booking | bookings | GET jobs | CRF: Jobs | Specified |
| BR-CRF-11 | Accept/reject requests | booking | bookings | POST accept/reject | CRF: Respond | Specified |
| BR-CRF-12 | Daily schedule | booking | bookings | GET schedule | CRF: Schedule | Specified |
| BR-CRF-13 | Job lifecycle | booking | bookings, history | progress APIs | CRF: Job detail | Specified |
| BR-CRF-14 | GPS proximity validation | idv | gps_checks | POST gps-checks | CRF: Arrive | Specified |
| BR-CRF-15 | Selfie verification | idv | face_checks | POST face-checks | CRF: Selfie | Specified |
| BR-CRF-16 | QR/OTP handshake | idv | job_challenges | POST challenges/verify | CRF: Confirm | Specified |
| BR-CRF-17 | Start / complete job | booking | bookings | progress transitions | CRF: Job | Specified |
| BR-CRF-18 | Earnings dashboard | ledger, reporting | ledger_entries | GET earnings | CRF: Earnings | Specified |
| BR-CRF-19 | KPIs | reporting | projections | GET kpis | CRF: Dashboard | Specified |
| BR-CRF-20 | Subscriptions | subscription, payment | plans, subscriptions | /subscriptions/** | CRF: Plans | Specified |
| BR-CRF-21 | Withdrawal requests | ledger | withdrawal_requests | POST withdrawals | CRF: Withdraw | Specified |
| BR-CRF-22 | Notifications | notification | in_app | inbox APIs | CRF: Inbox | Specified |
| BR-CRF-23 | Booking-scoped chat | chat | conversations, messages | /chat/** | CRF: Chat | Specified |
| BR-CRF-24 | Provider dispute/complaint | trust | disputes | dispute APIs | CRF: Dispute | Specified |

---

## 5. Store Dashboard (Services + Catalog Advertising — No E-Commerce)

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-STR-01 | Store profile (logo, business info, location, areas, contact, status) | provider | store_profiles, media | /store/me | STR: Profile | Specified |
| BR-STR-02 | Store verification documents / status | idv, provider | verification_cases | onboarding APIs | STR: Verify | Specified |
| BR-STR-03 | Service listings management | listing | listings | /store/listings | STR: Services | Specified |
| BR-STR-04 | Provider/staff affiliation | provider | affiliations | /store/staff | STR: Staff | Specified |
| BR-STR-05 | Booking management | booking | bookings | /store/bookings | STR: Bookings | Specified |
| BR-STR-06 | **Product catalog advertising** (non-transactional) | catalog_ads | catalog_items, media | /store/catalog-items | STR: Catalog | Specified |
| BR-STR-07 | Customer catalog inquiries (leads) | catalog_ads | catalog_inquiries | /store/inquiries | STR: Inquiries | Specified |
| BR-STR-08 | Promotions — services & catalog (subscription-gated) | ads | placements | /store/promotions | STR: Promos | Specified |
| BR-STR-09 | Store subscription status (Admin-defined plans) | subscription | subscriptions | /store/subscriptions | STR: Subscription | Specified |
| BR-STR-10 | Analytics (views, impressions, inquiries, booking conversions) | reporting | projections | /store/analytics | STR: Analytics | Specified |
| BR-STR-11 | Service areas & availability linkage | provider, booking | service_areas, availability | store APIs | STR | Specified |
| BR-STR-X1 | Product inventory / warehouse | — | — | — | — | **Out ADR-027** |
| BR-STR-X2 | Shopping cart | — | — | — | — | **Out ADR-027** |
| BR-STR-X3 | Product checkout / payment / orders | — | — | — | — | **Out ADR-027** |
| BR-STR-X4 | Product delivery / fulfillment | — | — | — | — | **Out ADR-027** |

**Admin store subscriptions:** BR-ADM-10 extended — Admin manages store subscription plans (visibility, featured, ad limits, promoted service/catalog caps).

---

## 6. Administration Portal (Web Only)

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-ADM-01 | Manage customers | customer, admin | customers | /admin/customers | ADM: Customers | Specified |
| BR-ADM-02 | Manage craftsmen | provider | craftsman_profiles | /admin/craftsmen | ADM: Craftsmen | Specified |
| BR-ADM-03 | Manage stores | provider | store_profiles | /admin/stores | ADM: Stores | Specified |
| BR-ADM-04 | Craftsman onboarding approval | provider, idv | verification_cases | approve/reject | ADM: Onboarding | Specified |
| BR-ADM-05 | Store onboarding approval | provider, idv | verification_cases | approve/reject | ADM: Store onboard | Specified |
| BR-ADM-06 | Verification workflow ops | idv | cases, docs | /admin/verifications | ADM: IDV | Specified |
| BR-ADM-07 | Status management | provider | status fields | PATCH status | ADM | Specified |
| BR-ADM-08 | Ad packages / featured / slots | ads | packages, placements | /admin/ads/** | ADM: Ads | Specified |
| BR-ADM-09 | Provider visibility promotions | ads | placements | /admin/promotions | ADM: Promos | Specified |
| BR-ADM-10 | Subscription plans CRUD | subscription | plans | /admin/subscription-plans | ADM: Plans | Specified |
| BR-ADM-11 | Subscription status/expiry/payments | subscription | subscriptions | /admin/subscriptions | ADM | Specified |
| BR-ADM-12 | Commission rules configuration (multi-dimensional, effective-dated; not hardcoded) | commission | commission_rules, history | /admin/commission-rules | ADM: Commission Config | Specified |
| BR-ADM-13 | Settlement rule configuration + settlement views | ledger, reporting | settlement_rules, views | /admin/settlement-rules, /admin/settlements | ADM: Settlements | Specified |
| BR-ADM-14 | Financial reports | reporting, ledger | exports | /admin/reports/finance | ADM: Finance reports | Specified |
| BR-ADM-15 | Notification templates | notification | templates | /admin/notification-templates | ADM: Templates | Specified |
| BR-ADM-16 | Channel config SMS/Email/Push/In-app | notification | settings | /admin/notifications | ADM | Specified |
| BR-ADM-17 | Scheduling / event triggers | notification | rules | /admin/notification-rules | ADM | Specified |
| BR-ADM-18 | Analytics dashboards | reporting | aggregates | /admin/analytics/** | ADM: Analytics | Specified |
| BR-ADM-19 | Follow-up metrics | trust, quality | scores, surveys | /admin/quality/** | ADM: Quality | Specified |
| BR-ADM-20 | Restriction review (no silent permanent ban) | trust | actor_restrictions | approve restriction | ADM: Restrictions | Specified |
| BR-ADM-21 | Audit logs | audit | audit_events | /admin/audit-events | ADM: Audit | Specified |
| BR-ADM-22 | Global settings | platform | global_settings | /admin/settings | ADM: Settings | Specified |
| BR-ADM-23 | Ratings moderation | trust | ratings, reviews | /admin/ratings | ADM: Ratings | Specified |
| BR-ADM-24 | Cancellation policy management | booking, policy | cancellation_policies, history | /admin/cancellation-policies | ADM: Cancellation Policies | Specified |
| BR-ADM-25 | Refund rules management | payment, ledger | refund_rules, history | /admin/refund-rules | ADM: Refund Rules | Specified |
| BR-ADM-26 | Withdrawal configuration (methods, mins, approval) | ledger | withdrawal_methods, configs, history | /admin/withdrawal-configs | ADM: Withdrawal Config | Specified |
| BR-ADM-27 | Finance RBAC (Finance Admin vs Super Admin) | iam | roles, permissions | admin authz | ADM: Roles | Specified |
| BR-ADM-28 | Policy change history + MFA-gated money rule edits | audit, iam | *_history, audit_events | all policy APIs | ADM | Specified |
| BR-ADM-29 | Dispute review queue & resolution | trust | disputes | /admin/disputes | ADM: Disputes | Specified |
| BR-ADM-30 | Chat support access (audited) | chat | conversations | /admin/chat/** | ADM: Support chat | Specified |
| BR-ADM-31 | Retention policy configuration | platform | retention_settings | /admin/settings/retention | ADM: Retention | Specified |

---

## 7. Payments, Ledger, Subscriptions, Commissions

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-PAY-01 | Payment.js tokenization | payment | — (client) | public key only | CUS/CRF WebView | Specified |
| BR-PAY-02 | Debit via token | payment | payments | POST /payments/{id}/debit | CUS/CRF | Specified |
| BR-PAY-03 | Webhook verification + idempotency | payment | payment_webhooks | POST /webhooks/areeba-ixopay | — | Specified |
| BR-PAY-04 | Payment state machine | payment | payments | GET /payments/{id} | CUS | Specified |
| BR-PAY-05 | Failure handling + reconcile job | payment, worker | payments | worker | ADM ops | Specified |
| BR-PAY-06 | Ledger update on money events | ledger | ledger_entries | internal | — | Specified |
| BR-PAY-07 | Escrow / holding state then release | ledger | ledger_accounts, entries | internal | — | Specified |
| BR-PAY-08 | Commission calculation from **active admin rules** | commission, ledger | commission_lines, commission_rules | events | ADM config + runtime | Specified |
| BR-PAY-09 | Provider withdrawal request validated by **admin withdrawal config** | ledger | withdrawal_requests, methods | POST withdrawals | CRF/ADM | Specified |
| BR-PAY-10 | Subscription charge via Payment.js | subscription, payment | subscriptions, payments | subscribe APIs | CRF | Specified |
| BR-PAY-11 | Gateway abstraction port | payment | — | adapters | — | Specified |
| BR-PAY-12 | Dynamic cancellation evaluation via admin policies | booking | cancellation_policies | cancel + preview APIs | CUS/CRF/STR/ADM | Specified |
| BR-PAY-13 | Dynamic refund evaluation via admin refund rules | payment, ledger | refund_rules, refunds | refund APIs | System/ADM | Specified |
| BR-PAY-14 | Settlement execution per admin settlement rules | ledger | settlement_rules, batches | workers + ADM | ADM | Specified |
| BR-PAY-15 | No hardcoded commission/cancel/refund/withdrawal/settlement values | policy | — | code review gate | — | Specified |
| BR-PAY-16 | **Financial complexity internal; simple customer payment UX** (ADR-029) | payment, ledger | payments, ledger_entries | customer: status APIs only; ledger internal | CUS: status only; CRF: earnings summary; ADM: full finance | Specified |
| BR-PAY-17 | **Separate state ownership: booking / payment / ledger / settlement** (ADR-030) | booking, payment, ledger | bookings, payments, ledger_entries, settlement_batches | per-domain status APIs — no unified mega-status | CUS: booking + payment; CRF: booking + earnings; ADM: all domains | Specified |

---

## 8. Quality / Follow-up

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-QUA-01 | 24h reminder | notification, worker | booking_reminders | scheduler | Push/In-app | Specified |
| BR-QUA-02 | Arrival GPS + selfie | idv | gps_checks, face_checks | job APIs | CRF | Specified |
| BR-QUA-03 | During-service progress tracking | booking | status_history | progress | CRF/CUS | Specified |
| BR-QUA-04 | Completion confirmation | booking | bookings | confirm APIs | CUS/CRF | Specified |
| BR-QUA-05 | Satisfaction survey | trust | surveys, responses | survey APIs | CUS | Specified |
| BR-QUA-06 | Warn / flag / restrict visibility | trust | actor_restrictions | system + admin | ADM | Specified |
| BR-QUA-07 | No permanent auto-block w/o review | trust | actor_restrictions | admin gate | ADM | Specified |

---

## 9. Cross-cutting Platform

| BR-ID | Feature | Module | Entity | API | UI | Status |
|-------|---------|--------|--------|-----|-----|--------|
| BR-PLT-01 | Redis cache / locks / rate limit | platform | — | infra | — | Specified |
| BR-PLT-02 | Background workers / queues | platform | outbox, jobs | workers | — | Specified |
| BR-PLT-03 | Flyway migrations | db | — | — | — | Specified |
| BR-PLT-04 | OpenAPI versioned REST | api | — | /api/v1 | — | Specified |
| BR-PLT-05 | Structured logging / monitoring | platform | — | — | — | Specified |
| BR-PLT-06 | Soft delete (non-financial) | all masters | deleted_at | — | — | Specified |
| BR-PLT-07 | Immutable financial records | ledger, payment | ledger_entries, payments | — | — | Specified |
| BR-PLT-08 | Design system / tokens (post analysis) | ui packages | — | — | All clients | Specified (pre-UI gate) |
| BR-PLT-09 | No hardcoded commission/cancel/refund/withdrawal/settlement business values | policy | config tables | code review gate | ADM config UIs | Specified |

---

## Coverage Statement

| Category | Count Specified | Explicitly Out / Deferred |
|----------|----------------:|---------------------------|
| IAM / Auth | 12 | — |
| Market / i18n | 7 | — |
| Customer | 17 | — |
| Craftsman | 22 | — |
| Store | 11 in / **4 e-commerce out** | ADR-002 amended + ADR-027 catalog advertising |
| Admin | 28 | Self-serve ads marketplace deferred ADR-007; money policies admin-configurable ADR-013 |
| Money | 15 | Policies admin-configurable ADR-013; numeric values by Finance Admin |
| Quality | 7 | Permanent auto-ban forbidden |
| Platform | 8 | — |

**Silent drop check:** E-commerce product commerce remains **explicitly out** (ADR-027). Promotional product catalog is **in**. Chat retained. Admin mobile login forbidden.

---

## Remaining Open Items (do not invent)

Still require ADR or Q-* before coding those behaviors:

- Exact cancellation/refund fee matrix (Q-BOOK-001..004)  
- Commission percentage model (Q-COM-001)  
- Withdrawal rail details (Q-SET-002)  
- Chat transport (WebSocket vs poll) — engineering spike ADR  
- OCR/Face provider vendors (Q-OCR-001, Q-IDV-002)  
- Email/SMS providers (Q-NTF-001/002)  

---

## Gate Sign-off

| Reviewer | Date | Ack |
|----------|------|-----|
| Product Owner | | ☐ |
| Solution Architect | | ☐ |
| Engineering Lead | | ☐ |
