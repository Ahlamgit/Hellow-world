# Questions Requiring Business Decision

**Document ID:** KHAD-V1-QUESTIONS  
**Status:** Open — Must be dispositioned before implementation freeze of affected domains  
**How to use:** For each question, set Status to `Answered` / `Deferred-to-V2` / `Assumed-with-owner-signoff`, and record Decision + Date + Owner.

---

## A. Identity, Auth, RBAC

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-AUTH-001 | Is login identifier email, phone, or either? | IAM schema, UX | Open |
| Q-AUTH-002 | Is MFA mandatory for admin users in V1? | Security | Open |
| Q-AUTH-003 | Must email/phone be verified before first session issuance? | Registration flow | Open |
| Q-AUTH-004 | Password reset via email link, SMS OTP, or both? | Auth + notifications | Open |
| Q-AUTH-005 | Refresh token lifetime and multi-device policy? | IAM | Open |
| Q-AUTH-006 | Embed permissions in JWT or resolve server-side from roles? | Authz performance | Open |
| Q-RBAC-001 | Final admin role taxonomy and permission matrix? | Admin portal | Open |
| Q-STR-001 | Can a store have multiple operator users? | Store IAM schema | Open |

## B. Localization, Currency, Region, Compliance

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-LOC-001 | Launch country/region(s) for V1? | Defaults, compliance | Open |
| Q-LOC-002 | Default locale and supported languages at launch (AR/EN/others)? | i18n | Open |
| Q-LOC-003 | Default currency and supported currencies? | Payments/pricing | Open |
| Q-LOC-004 | Any legal entity / tax invoice requirements at launch? | Invoicing depth | Open |
| Q-CUR-001 | Is FX conversion in-platform required in V1 or single-currency only with multi-currency readiness? | Money model | Open |
| Q-CMP-001 | Which privacy/regulatory regimes apply (GDPR-like, local PDPL, etc.)? | Retention, consents | Open |
| Q-CMP-002 | Marketing opt-in defaults and consent capture? | Customer profile | Open |
| Q-CMP-003 | May raw gateway webhook payloads be stored; if so, retention & encryption? | Payments storage | Open |

## C. Booking & Relationships

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-BOOK-001 | Who may cancel a booking and until when? | Lifecycle | Open |
| Q-BOOK-002 | Cancellation fees? | Payments/refunds | Open |
| Q-BOOK-003 | When are refunds full/partial/none? | Payment refunds | Open |
| Q-BOOK-004 | Refund SLA and manual vs automatic? | Ops + payment | Open |
| Q-BOOK-005 | No-show policy (customer or craftsman)? | Quality + fees | Open |
| Q-BOOK-006 | Dispute window after completion? | Booking states | Open |
| Q-BOOK-007 | Scheduling model: slots, windows, or ASAP? | Booking UX/API | Open |
| Q-BOOK-008 | Is prepayment mandatory before confirmation? | Booking↔Payment | Open |
| Q-BOOK-009 | Assignment model: customer picks craftsman, store assigns, or platform auto-assigns? | Booking domain | Open |
| Q-BOOK-010 | Is bidding / real-time matching in V1? (default recommendation: No) | Scope | Open |
| Q-REL-001 | Can craftsmen be independent, store-affiliated, or both? | Data model | Open |
| Q-REL-002 | Can stores offer services fulfilled by craftsmen on platform? | Booking ownership | Open |
| Q-REL-003 | Are products deliverable/shippable or pickup-only in V1? | Orders module depth | Open |

## D. Payments

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-PAY-001 | Are store product orders paid online in V1 via IXOPAY? | Payment payables | Open |
| Q-PAY-002 | Save cards / `withRegister` recurring profiles in V1? | Subscriptions renewal | Open |
| Q-PAY-003 | Which currencies are enabled on Areeba merchant account for launch? | Config | Open |

## E. Commissions & Settlements

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-COM-001 | Commission model structure (%, tiered, fixed+%, by category)? | Commission schema | Open |
| Q-COM-002 | Are store orders commissionable? | Commission triggers | Open |
| Q-COM-003 | How do ad fees interact with commission accounting? | Ledger | Open |
| Q-COM-004 | How do refunds reverse commissions? | Finance integrity | Open |
| Q-COM-005 | Who bears payment gateway fees? | Net earnings | Open |
| Q-SET-001 | Settlement cadence (weekly/monthly/on-demand)? | Settlement overview | Open |
| Q-SET-002 | Withdrawal rails (bank transfer, wallet, manual)? | Withdrawals | Open |
| Q-SET-003 | Module ownership of payouts (`commission` vs `payment` vs `payout`)? | Architecture packaging | Open |
| Q-SET-004 | Reserves/holds on craftsman balances? | Withdrawal available balance | Open |

## F. Subscriptions

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-SUB-001 | Do stores purchase subscriptions in V1, or craftsmen only? | Subscription module | Open |
| Q-SUB-002 | Which actions are gated by subscription entitlements? | Authz entitlements | Open |
| Q-SUB-003 | Auto-renew, grace period, and expiry behavior? | Subscription lifecycle | Open |
| Q-SUB-004 | Is there a free tier? | Plan catalog | Open |

## G. Onboarding & Identity Verification

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-ONB-001 | Mandatory document checklist for craftsman approval? | Onboarding UX | Open |
| Q-ONB-002 | Resubmission rules after rejection? | Onboarding states | Open |
| Q-ONB-003 | Final onboarding status names? | Schema enums | Open |
| Q-ONB-004 | Any auto-approve rules or always human approval? | Workflow | Open |
| Q-IDV-001 | GPS proximity distance & accuracy thresholds? | Job verification | Open |
| Q-IDV-002 | Face recognition provider selection? | Integration | Open |
| Q-IDV-003 | Face match pass threshold? | Verification | Open |
| Q-IDV-004 | Is liveness detection required in V1? | Mobile capture | Open |
| Q-IDV-005 | Anti-spoof / mock-location policy depth? | Trust & safety | Open |
| Q-IDV-006 | Retention period for docs/selfies? | Compliance + storage | Open |
| Q-IDV-007 | Which job steps require QR vs OTP vs both? | Booking gates | Open |
| Q-IDV-008 | Admin override allowed for failed verifications? | Ops | Open |
| Q-OCR-001 | OCR provider selection? | Integration | Open |

## H. Notifications & Communications

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-NTF-001 | Email provider? | Notifications | Open |
| Q-NTF-002 | SMS provider? | Notifications | Open |
| Q-NTF-003 | Who receives 24h reminders (customer, craftsman, both)? | Reminder job | Open |
| Q-NTF-004 | Exact reminder window definition? | Scheduler | Open |
| Q-NTF-005 | Channel matrix per event? | Dispatcher config | Open |
| Q-NTF-006 | User preference center for non-transactional messages? | Notification UX | Open |
| Q-COMMS-001 | Is in-app chat in V1? (architecture default: No / V2) | Scope | Open |

## I. Ads, Catalog, Ratings, Quality

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-ADS-001 | Are ads paid in V1? How billed? | Ads + payments | Open |
| Q-ADS-002 | Which placement slots exist? (needs design input) | Ads serving | Open |
| Q-ADS-003 | Subscription includes ad credits? | Entitlements | Open |
| Q-ADS-004 | Track impressions/clicks in V1? | Metrics schema | Open |
| Q-ADS-005 | Creative compliance policy? | Moderation | Open |
| Q-PRD-001 | Product inventory model (simple qty, variants, unlimited)? | Store products | Open |
| Q-RATE-001 | Can ratings be edited after submit? | Ratings API | Open |
| Q-QUA-001 | Satisfaction survey question set? | Quality | Open |
| Q-QUA-002 | Quality score formula & weights? | Scoring job | Open |
| Q-QUA-003 | Restriction rules thresholds & durations? | Enforcement | Open |
| Q-QUA-004 | Job progress SLA definitions? | Monitoring | Open |

## J. Platform Engineering / Ops Decisions

| ID | Question | Blocks | Status |
|----|----------|--------|--------|
| Q-DEP-001 | Cloud provider / hosting target? | Deploy | Open |
| Q-DEP-002 | API+worker same process or separate? | Deploy topology | Open |
| Q-DEP-003 | Redis (or equivalent) required in V1? | Infra | Open |
| Q-DEP-004 | RPO/RTO targets? | HA/backup | Open |
| Q-DEP-005 | Flyway via startup vs dedicated migrate job in prod? | Release process | Open |
| Q-BE-001 | Use Java virtual threads in V1? | Backend | Open |
| Q-FE-001 | Web monorepo tooling (pnpm/nx/turbo)? | Frontend | Open |
| Q-FE-002 | Web token storage: HttpOnly cookie vs memory Bearer? | Web security | Open |
| Q-FL-001 | Flutter state management: Riverpod vs Bloc? | Mobile | Open |
| Q-FL-002 | Minimum iOS/Android versions? | Mobile | Open |
| Q-DB-001 | Translations via tables vs JSONB? | DB | Open |
| Q-DB-002 | UUIDv4 vs UUIDv7? | DB | Open |
| Q-DB-003 | Data retention periods by class? | Compliance jobs | Open |
| Q-STO-001 | Object storage provider? | Media | Open |
| Q-MAP-001 | Maps provider for address/geocode? | Location UX | Open |
| Q-API-001 | Swagger exposed in production (protected or hidden)? | API ops | Open |
| Q-OBS-001 | Log/metrics backend choice? | Observability | Open |
| Q-OBS-002 | Mobile crash analytics tool? | Mobile ops | Open |
| Q-OBS-003 | Application log retention? | Ops | Open |
| Q-SEC-001 | Argon2id vs BCrypt for passwords? | Security | Open |
| Q-SEC-002 | Malware scanning for uploads in V1? | Media security | Open |
| Q-SEC-003 | Penetration test scope before launch? | Launch gate | Open |
| Q-RPT-001 | Data warehouse in V1 or SQL read models only? | Reporting | Open |
| Q-CICD-001 | CVE severity gate policy? | CI | Open |
| Q-CICD-002 | Mobile store publishing automated or manual? | Release | Open |
| Q-KPI-001 | Target median time-to-book? | Success metrics | Open |
| Q-KPI-002 | Target payment capture success rate? | Success metrics | Open |

---

## Blocking Set for Schema Freeze (Minimum)

Before coding money + booking persistence, at least decide or explicitly defer:

1. Q-BOOK-007, Q-BOOK-008, Q-BOOK-009  
2. Q-COM-001, Q-COM-004  
3. Q-SET-001, Q-SET-002  
4. Q-SUB-001, Q-SUB-002  
5. Q-AUTH-001, Q-RBAC-001  
6. Q-LOC-001..003  
7. Q-PAY-001, Q-PAY-002  

Deferred items must be feature-flagged off and must not invent production policy silently.

---

## Decision Log Template

| ID | Decision | Owner | Date | Links |
|----|----------|-------|------|-------|
| | | | | |
