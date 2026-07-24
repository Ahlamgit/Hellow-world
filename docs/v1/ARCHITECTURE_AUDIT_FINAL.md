# KHADAMATI V1 — Final Architecture Audit

**Document ID:** KHAD-V1-AUDIT-FINAL  
**Status:** Architecture Review — Pre-Implementation  
**Date:** 2026-07-24  
**Review Stance:** Enterprise / SaaS / Cloud / Marketplace / Security / Performance / UX / DevOps  
**Scope:** Full review of `docs/v1/**` (all 45 deliverables + open questions). No implementation changes.  
**Target Horizon:** Lebanon → GCC → Middle East → International **without redesign**

---

## Executive Verdict

The V1 pack is a **strong modular foundation** with the right instincts: clean boundaries, Payment.js PCI reduction, outbox-driven side effects, money as minor units, and explicit open questions instead of invented policy.

It is **not yet commercial SaaS–hardened**.

The architecture still reads closer to a **well-structured marketplace MVP** than a **multi-market SaaS platform**. The largest gaps are:

1. Missing first-class **Market / Organization / Tenant** seams for multi-country expansion  
2. Missing a **Ledger / Balance** model for commissions, withdrawals, escrow, and wallet evolution  
3. **Module sprawl** (too many V1 bounded contexts for an MVP+ delivery)  
4. **Unresolved policy density** (70+ open questions) blocking schema freeze  
5. **Admin + Ads + Quality automation** scoped heavier than a disciplined V1 should carry  
6. Store/Craftsman **catalog duplication** without a shared Offer/Listing abstraction  
7. Booking **rescheduling / availability / concurrency** under-specified for marketplace peak load  

**Overall recommendation:** Approve architecture direction with **mandatory remediation before coding**. Do not start implementation until the Critical Remediation List in this audit is accepted (or consciously deferred with feature flags and schema seams).

---

## 1. Strengths

| # | Strength | Why it matters |
|---|----------|----------------|
| S1 | Modular monolith (not premature microservices) | Correct for V1 consistency of booking/payment/commission |
| S2 | Hexagonal payment port + IXOPAY-only adapter | Future gateways without rewriting business logic |
| S3 | Payment.js / no PAN on KHADAMATI servers | PCI scope minimization is commercially essential |
| S4 | Idempotency + webhook finalization pattern | Required for money safety |
| S5 | Transactional outbox for notifications/commissions | Avoids dual-write failures |
| S6 | Optimistic locking on booking/payment | Basic concurrency control present |
| S7 | Soft-delete + audit + timestamptz conventions | Ops/compliance baseline |
| S8 | Money as `amount_minor` + ISO currency | Multi-currency readiness start |
| S9 | Open Questions register instead of invented policy | Mature product discipline |
| S10 | UI gate until design assets | Prevents disposable UI debt |
| S11 | Shared web/Flutter packages planned | Brand/UI swappability without domain rewrite |
| S12 | Role + permission + resource scoping model | Needed for Admin/Store isolation |
| S13 | Expand/contract Flyway + CI quality gates | Production engineering hygiene |
| S14 | Explicit V2 extraction candidates | Avoids accidental big-bang rewrite |

---

## 2. Weaknesses (Challenged Decisions)

### 2.1 Business Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| **18 logical modules** for V1 | Too fine-grained for MVP+ staffing and cognitive load | Slow delivery, fuzzy ownership |
| `admin` as a domain module | Admin should be an **API composition/BFF surface**, not a bounded context owning business rules | Boundary leakage risk |
| `rating` and `quality` separated | Both measure post-job trust; duplicate survey/rating concepts | Double write paths, inconsistent scores |
| `catalog` as separate module on day 1 | Can start as projections owned by store/craftsman, extract later | Premature abstraction |
| Ads treated as P0 across Admin + Store | Monetization via ads is growth layer, not booking MVP core | Scope inflation |
| No **Market/Tenant** concept | “region_code on user” is not SaaS multi-market architecture | Redesign risk for GCC/international |
| No **Organization** for stores | Single operator assumption delays B2B store chains | Rework for store staff / franchises |
| Availability/calendar missing | Marketplace supply without availability is incomplete | Double booking / poor UX |
| Dispute capability only a booking state | Commercial marketplaces need evidence + finance holds | Weak trust ops |

### 2.2 Database Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| Soft-delete on financial tables implied by global standard | Payments/refunds/commission lines should be **immutable** (status transitions only) | Audit/finance integrity risk |
| Polymorphic `service_ref_type/id` on bookings | Weak referential integrity; hard reporting | Orphaned bookings, messy joins |
| No first-class **Offer/Listing** entity | `craftsman_services` vs `store_services` duplication | Divergent pricing/search logic |
| No ledger tables | Withdrawals computed ad hoc from commission lines will not scale to escrow/wallet | V2 redesign of money core |
| Translation approach undecided | Must freeze for catalog i18n (AR/EN critical for MENA) | Rework of all content tables |
| Search strategy incomplete | Indexes listed; no FTS/`pg_trgm`/geo index plan for “near me” | Search will degrade early |
| Partition candidates omit `bookings` / `payments` | Those become hottest tables | Painful later migrations |
| Missing payment status history | Only booking history emphasized | Hard payment dispute forensics |
| Timezone strategy incomplete | `timestamptz` store ≠ market local scheduling UX | Wrong reminder times across GCC |
| Unique keys underspecified | e.g. one active subscription per holder/plan, one rating per booking | Data anomalies |
| No `market_id` / `country_code` on monetary events | Tax/settlement per country later becomes retrofit | High cost |

### 2.3 Backend Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| 17 Maven modules suggested | Heavy for V1; package modules + ArchUnit may be better initially | Build complexity |
| Event catalog not normalized | Event names/versioning/schema evolution not specified | Breaking consumers later |
| Distributed scheduler locks absent | `@Scheduled` on multiple API replicas duplicates jobs | Duplicate SMS/charges/reminders |
| Redis optional | For SaaS rate-limits, flags, short-lived locks, Redis (or equivalent) is near-mandatory | Weak abuse protection |
| DTO/mapping strategy thin | Risk of entity leakage under delivery pressure | Coupling |
| Provider circuit-breakers named but no standard library choice | Resilience4j (or equivalent) should be decided | Inconsistent timeouts |
| Background job platform unspecified | Need failed-job visibility, retries, DLQ UI/metrics | Ops blindness |

### 2.4 Frontend Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| Theme/token contract not formalized | “Fill later” is fine, but need **token schema** now for UI replaceability | Brand swap friction |
| Accessibility only baseline | Admin tables need keyboard + contrast requirements explicit | Enterprise buyer friction |
| Lazy-loading/code-splitting not mandated | Large admin bundles hurt ops UX | Performance |
| Cookie vs Bearer still open | Security-sensitive; must decide pre-implementation | Auth rework |
| No BFF mention | Admin aggregation may over-fetch many module APIs | Chatty UI |

### 2.5 Flutter Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| State management still open | Must choose one before Sprint 1 mobile | Parallel patterns = debt |
| Offline strategy vague | “Careful queueing” is not an architecture | Partial writes / ghost bookings |
| Image pipeline unspecified | Compression, cache, progressive loading needed for docs/selfies | Perf + storage cost |
| Payment.js WebView as critical path | Highest technical risk in stack; needs hardened module + spike gate | Launch blocker risk |
| Push token lifecycle incomplete | Reinstall, multi-device, permission denial flows | Missed notifications |

### 2.6 Security

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| Admin MFA optional | For commercial SaaS admin, MFA should be **default required** | Account takeover risk |
| Rate limiting mostly auth-scoped | Need global API throttling + per-store/craftsman abuse limits | Scraping / SMS bombing |
| Step-up auth missing for withdrawals / payout approvals | Money movement needs re-auth or MFA | Fraud |
| Device/session inventory thin | “Logout everywhere” mentioned; session management UX/API incomplete | Support burden |
| Encryption-at-rest for object storage undecided | KYC docs require it in most regimes | Compliance gap |
| Bot protection / WAF only lightly referenced | Public search/register endpoints need protection | Cost & abuse |

### 2.7 Payment Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| No multi-MID / per-market merchant account model | Lebanon vs UAE may need separate Areeba/IXOPAY merchants | Expansion redesign |
| Reconciliation job described conceptually only | Commercial ops need daily reconcile reports | Unmatched funds |
| Escrow/hold states absent | Marketplace often captures then releases after completion | V2 money redesign risk |
| `transactionToken` handling TTL/logging not strict enough in docs | Token leakage/logging accidents | PCI/process risk |
| Store order / ad payable still open | Payable model can sprawl without a single `Payable` aggregate discipline | Inconsistent payment UX |

**What is correct and should be kept:** Payment.js only; no raw cards; port/adapter; server-side amount authority; webhook idempotency; commissions outside gateway adapter.

### 2.8 Booking Architecture

| Weakness | Challenge | Impact |
|----------|-----------|--------|
| State machine too rich for V1 | `EN_ROUTE`, `DISPUTED`, `REQUESTED` variants can wait | Implementation delay |
| **Rescheduling not designed** | Explicit gap vs marketplace reality | Support tickets / cancellations spike |
| Assignment + scheduling still open | Core product ambiguity | Wrong schema |
| Slot concurrency not designed | Optimistic row lock ≠ inventory lock | Overbooking |
| Payment sync races | Sync success vs delayed webhook ordering needs explicit precedence rules | Double side-effects |
| Cancellation/refund policy hole | Cannot implement safely | Finance risk |

### 2.9 Store vs Craftsman

Duplication is real:

- Profile + geo + media  
- Service catalog + pricing  
- Bookings attachment  
- Ratings/quality  
- Payouts (craftsman) vs settlements (store)

**Missing shared abstractions:**

1. `Party` / `AccountProfile` (customer/craftsman/store as roles on identity)  
2. `Listing` / `Offer` (bookable/sellable unit)  
3. `ProviderAffiliation` (craftsman ↔ store)  
4. `CapabilityEntitlement` (subscription gates)

Without these, GCC multi-brand store networks and independent craftsmen will fork the model.

### 2.10 Customer / Craftsman / Admin / Store UX Scope

| Surface | Issue | Recommendation |
|---------|-------|----------------|
| Customer | Registration channel undecided; MENA often phone-first | Decide phone OTP early |
| Customer | Search ranking undefined | Start with category + geo + rating sort; no ML |
| Customer | Payment WebView friction | Single payment module UX; minimize steps |
| Craftsman | Availability calendar missing | Add V1 “available windows” minimum |
| Craftsman | Withdrawal without ledger | Build balance projection from ledger |
| Admin | Too many P0 screens | Cut to governance MVP |
| Store Dashboard | Ads in V1 inflate scope | Defer paid self-serve ads |

### 2.11 Notifications / Quality / API / DevOps

- Notifications: good channel abstraction; missing quiet hours, priority lanes, OTP vs marketing isolation, budget caps for SMS.  
- Quality: auto-restrictions in V1 are dangerous; start **manual + recommended**, automate later.  
- API: role-prefixed routes are pragmatic but inconsistent with resource-oriented REST; cursor pagination missing for mobile feeds.  
- DevOps: cloud, Redis, worker split, RPO/RTO, observability stack still open — too many foundational undecideds for “production-grade SaaS” claim.

---

## 3. Risks

| ID | Risk | Severity | Likelihood | Notes |
|----|------|----------|------------|-------|
| AR-01 | Open questions block schema; team codes assumptions anyway | Critical | High | Policy drift |
| AR-02 | Payment.js Flutter WebView/3DS fails under real devices | Critical | Medium | Spike-gate required |
| AR-03 | Soft-deleted / mutable money records corrupt finance | Critical | Medium | Make financial events immutable |
| AR-04 | Duplicate reminder/commission jobs on scaled API nodes | High | High | Need leader election/locks |
| AR-05 | No ledger → withdrawals/escrow force rewrite | High | High | Add ledger seam now |
| AR-06 | Multi-country launch requires tenant retrofit | High | High | Add Market now |
| AR-07 | Admin MFA optional → privileged compromise | High | Medium | Require MFA |
| AR-08 | Ads + analytics + auto-quality over-scope V1 | High | High | Cut scope |
| AR-09 | Store/craftsman catalog fork | High | High | Introduce Listing |
| AR-10 | Search/geo performance collapses under growth | Medium | Medium | Plan FTS + geo indexes |
| AR-11 | SMS cost explosion from retries/reminders | Medium | High | Caps + channel policy |
| AR-12 | KYC media retention undecided → legal exposure | High | Medium | Decide before storing biometrics |
| AR-13 | UI built against unstable API shapes | Medium | Medium | Freeze OpenAPI for P0 flows |
| AR-14 | Legacy .NET mental models leak into Spring design | Medium | Medium | Keep isolation discipline |

---

## 4. Recommended Improvements (Prioritized)

### P0 — Must fix before implementation starts

1. **Introduce `Market` (or `OperatingCountry`) aggregate**  
   - Fields: country_code, default_currency, supported_locales, timezone, payment_merchant_ref, tax_profile_ref  
   - Every booking/payment/listing carries `market_id`  
   - Enables Lebanon → GCC without redesign  

2. **Introduce a minimal double-entry `Ledger`**  
   - Accounts: platform, craftsman payable, store payable, pending/escrow, gateway clearing  
   - Postings from payment captured, refund, commission, withdrawal  
   - Gateway adapter never posts commissions directly  

3. **Create shared `Listing` (Offer) model**  
   - Replaces divergent `craftsman_services` / `store_services` as the bookable unit  
   - Provider = craftsman and/or store via affiliation  

4. **Collapse V1 modules**  

| Keep as modules | Merge / demote |
|-----------------|----------------|
| iam, customer, provider(craftsman+store core), listing/catalog, booking, payment, ledger/commission, subscription, notification, media, identity_verification, platform | `rating`+`quality` → `trust` |
| | `admin` → API facades only |
| | `ads` → V1.1/V2 (feature-flagged stub OK) |
| | `reporting` → read models inside domains + thin export API initially |
| | `audit` can live in platform |

5. **Slim booking state machine for V1**  
   Recommended V1 states:  
   `PENDING_PAYMENT → CONFIRMED → IN_PROGRESS → COMPLETED | CANCELLED | EXPIRED`  
   Keep arrival/QR as **verification checkpoints**, not necessarily separate primary statuses. Add `RESCHEDULED` transition support.

6. **Financial immutability rules**  
   - No soft-delete on payments, refunds, ledger_entries, commission_lines  
   - Corrections via reversing entries  

7. **Require Admin MFA in V1**  
8. **Mandate Redis (or equivalent) for rate-limit, distributed locks, flag cache**  
9. **Separate worker process profile** for schedulers/outbox (even if same image)  
10. **Freeze blocking business decisions** listed in Section 12 (minimum set)  
11. **Payment reconciliation + webhook precedence ADR** before coding payments  
12. **Cursor pagination** for mobile list endpoints  

### P1 — Should fix during early sprints (before public launch)

1. Availability windows for craftsmen  
2. Reschedule booking API + policy  
3. Geo index + basic full-text search  
4. Session inventory / logout all devices  
5. Step-up auth for withdrawals  
6. Quiet hours + SMS budget guards  
7. Payment status history table  
8. Formal design-token schema in `packages/ui` / `khad_ui`  
9. Resilience4j (or chosen) standard for all providers  
10. Manual quality restrictions first; auto-restrictions behind flag default OFF  

### P2 — Explicitly defer to V2 (prepare seams only)

- Self-serve paid ads auctioning  
- Wallet top-ups / consumer wallet  
- Full escrow productization  
- Multi-gateway routing beyond config switch  
- AI recommendations  
- Chat  
- Advanced BI warehouse  
- Multi-entity white-label tenancy beyond Market  

---

## 5. Technical Debt (If Built As Currently Written)

| Debt Item | Cost if ignored |
|-----------|-----------------|
| 18-module sprawl | Slow features, circular deps |
| Polymorphic booking service refs | Reporting nightmares |
| Soft-delete money rows | Irreversible finance bugs |
| No Market/Ledger | Country expansion rewrite |
| Ads in P0 Store Dashboard | Delayed core booking quality |
| Auto quality restrictions | False positives / supply collapse |
| Undecided Flutter state mgmt | Two patterns forever |
| `@Scheduled` without locks | Duplicate customer communications |
| OpenAPI role-prefix inconsistency | Client generator pain |
| Missing cursor pagination | Mobile perf issues |

---

## 6. Performance Concerns

| Area | Concern | Mitigation |
|------|---------|------------|
| Booking peaks | Hot row updates + status history inserts | Keep transactions short; index `(status, scheduled_start)`; consider partition later |
| Concurrent payments | Idempotency + gateway latency | Pending state machine; async finalize; pool tuning |
| Notification storms | 24h reminders + status events | Batch scheduler, rate-limited dispatcher, priority queues |
| Image/KYC uploads | Large selfies/docs | Client compression, size caps, async virus scan, CDN |
| Search/browse | Unbounded `ILIKE` scans | `pg_trgm`, category filters, geo (`ll_to_earth`/PostGIS later), pagination |
| Admin analytics | Heavy aggregates on OLTP | Materialized views / replica; no sync full scans |
| Outbox growth | Publisher lag | Indexed unpublished poll; partitioning; metrics alerts |
| Flutter lists | Jank on images | cache + thumbnail variants |

**Estimate:** Architecture can scale to early commercial load **if** Redis locks, worker split, and search indexes are adopted. It will **not** scale to multi-country high volume without Market + Ledger + read-path discipline.

---

## 7. Security Concerns

| Concern | Severity | Required action |
|---------|----------|-----------------|
| Admin MFA optional | High | Make mandatory |
| Withdrawal without step-up auth | High | MFA/re-auth |
| KYC media encryption/retention undecided | High | Decide before onboarding launch |
| WebView payment token handling | High | Never log tokens; short-lived; bridge hardening |
| Rate limits incomplete | High | Global + endpoint classes |
| CSRF if cookie auth chosen | Medium | Explicit CSRF strategy ADR |
| Public registration bot abuse | Medium | Captcha/WAF/rate-limit |
| Soft secrets in mobile flavors | Medium | Only public keys in apps |
| Audit gaps on settings/commission edits | Medium | Enforce audit interceptor |

OWASP mapping in docs is directionally correct but **controls are not yet operationalized** into concrete defaults.

---

## 8. Scalability Concerns

| Dimension | Current readiness | Gap |
|-----------|-------------------|-----|
| Horizontal API scale | Good (stateless) | Needs sticky-less design confirmed; distributed jobs |
| Database | Good start | Market partitioning, booking/payment growth plan |
| Multi-region active-active | Not ready | No data residency / regional routing model |
| Multi-currency | Partial | FX policy + per-market MID missing |
| Multi-gateway | Good abstraction | Routing/failover rules absent (OK for V1) |
| Notification fanout | Partial | Needs worker autonomy + backlog metrics |
| Search | Weak | No dedicated search path |
| Tenant isolation (future white-label) | Weak | Market ≠ full tenant; document boundary |

---

## 9. Version 2 Preparation Assessment

| V2 Capability | Ready without major redesign? | Verdict |
|---------------|-------------------------------|---------|
| Multiple countries | **No** (as written) | Add `Market` now |
| Multiple currencies | Partially | OK storage; need market defaults + FX policy seam |
| Multiple payment gateways | **Yes** | Port/adapter is correct |
| Subscriptions | Mostly | Exists; renew/grace still open |
| Advertisements | Partially | Module exists; better deferred, keep placement interface |
| Wallet | **No** | Needs Ledger now |
| Escrow | **No** | Needs Ledger + payable hold states now |
| AI recommendations | Partially | Outbox events help; need feature store later |

**Conclusion:** Payment gateway abstraction is future-ready. **Money core and market core are not.** Fix those before coding.

---

## 10. Design / UI Replaceability Review

### What already supports UI swap

- Business logic in backend modules  
- OpenAPI as contract  
- `packages/ui` + `khad_ui` tokenized wrappers planned  
- Explicit ban on inventing marketing UI before assets  

### What to add so UI can be replaced without touching business logic

1. **Strict presentation isolation:** no business rules in React/Flutter widgets  
2. **Design token schema** (`color.brand.primary`, `space.4`, `font.display`, radii, motion) committed empty/filled later  
3. **View-model / controller layer** in Flutter & web features  
4. **Stable semantic API error codes** mapped to UX, not hardcoded English  
5. **Component gallery** driven by tokens (post-design)  
6. Avoid MUI/Flutter widgets leaking into domain mappers  

**Verdict:** Directionally good; formalize token + view-model contracts as ADRs at kickoff.

---

## 11. Area-by-Area Audit Summary

### 1) Business Architecture
**Score: 68/100**  
Solid capability map; over-modularized; missing Market/Ledger/Listing; Admin/Ads/Quality overweight for V1.

### 2) Database Architecture
**Score: 70/100**  
Good conventions; weak financial immutability; polymorphic refs; incomplete search/timezone/market keys.

### 3) Backend Architecture
**Score: 78/100**  
Best part of the pack (hexagonal, outbox, idempotency). Needs worker locks, Redis, thinner module packaging.

### 4) Frontend Architecture
**Score: 74/100**  
Sensible React feature architecture; unresolved auth storage; a11y/perf mandates incomplete.

### 5) Flutter Architecture
**Score: 72/100**  
Clean feature structure; Payment.js WebView + offline + state choice are unresolved risks.

### 6) Security
**Score: 69/100**  
Correct PCI stance; admin MFA/rate-limit/step-up/KYC retention gaps for commercial SaaS.

### 7) Payment Architecture
**Score: 82/100**  
Strongest workflow design; add multi-MID, reconcile, ledger integration, escrow seams.

### 8) Booking Architecture
**Score: 64/100**  
Lifecycle present but policy holes, no reschedule, state overgrowth, concurrency for slots weak.

### 9) Store Module
**Score: 60/100**  
Duplication with craftsman; affiliation undecided; ads scope questionable.

### 10–11) Customer & Craftsman Experience
**Score: 67/100**  
Flows covered; MENA phone-first auth, availability, and payment friction need product decisions.

### 12) Administration
**Score: 58/100**  
Too broad for V1. Keep approvals, users, settings, audit, basic commissions/subscriptions, basic templates. Move deep analytics, ads ops, auto-quality config depth to V2.

### 13) Store Dashboard
**Score: 62/100**  
Core catalog/orders/bookings belong. **Self-serve advertisements should move out of V1** (admin-operated featured placements optional).

### 14) Notifications
**Score: 76/100**  
Good architecture; add priority, quiet hours, OTP isolation, cost controls.

### 15) Follow-up Layer
**Score: 63/100**  
Valuable, but auto-restrictions + complex scoring should be staged; reminders + survey first.

### 16) API Design
**Score: 75/100**  
Solid conventions; add cursor pagination, tighten resource model, freeze P0 OpenAPI.

### 17) Performance
**Score: 66/100**  
Aware but not operationalized (search, locks, worker isolation).

### 18) DevOps
**Score: 68/100**  
Container/CI direction good; cloud/secrets/DR/observability still TBD.

### 19) Future Readiness
**Score: 61/100**  
Gateway abstraction excellent; Market/Ledger/Wallet/Escrow readiness insufficient.

### 20) UI Replaceability
**Score: 77/100**  
Good intent; formalize tokens/view-models.

---

## 12. Minimum Decisions Required Before Coding

These are **architecture blockers**, not nice-to-haves:

| Decision | Why |
|----------|-----|
| Q-LOC-001..003 + adopt `Market` | Multi-country seam |
| Q-AUTH-001 (phone vs email) | IAM + UX for MENA |
| Q-AUTH-002 → **MFA required for admin** | Security baseline |
| Q-BOOK-007/008/009 + reschedule policy | Booking schema |
| Q-REL-001/002 + Listing model | Store/craftsman unity |
| Q-COM-001/004 + Ledger approach | Money core |
| Q-SET-002/004 | Withdrawals |
| Q-PAY-001 + payable list for V1 | Payment scope |
| Q-SUB-001/002 | Entitlements |
| Q-DEP-001/002/003 → Redis **yes**, workers **separate** | Runtime reliability |
| Ads in V1? → recommend **No self-serve ads** | Scope control |
| Flutter state library choice | Mobile consistency |

---

## 13. Recommended V1 Scope Rebalance (Commercial Discipline)

### Keep in V1 (must)
- IAM + profiles  
- Listings/catalog browse + search (basic)  
- Booking + pay (IXOPAY Payment.js)  
- Craftsman onboarding approval + essential IDV  
- Subscriptions (craftsman)  
- Commissions + settlement overview + withdrawal requests  
- Notifications (push/in-app + email; SMS for OTP/critical only)  
- Ratings  
- Admin governance (users, approvals, settings, audit, plans, commission rules)  
- Store catalog/orders/bookings/reports (basic)

### Move to V1.1 / V2
- Self-serve store advertisement manager  
- Deep analytics dashboards  
- Automatic quality restrictions engine  
- Advanced OCR automation beyond assistive admin review  
- Chat, wallet, escrow product, multi-gateway  
- Complex dispute center  

---

## 14. Scores

### Scoring Method
- **Final Readiness Score:** quality of architecture as a commercial SaaS foundation after this review’s lens (design maturity).  
- **Implementation Readiness Score:** readiness to begin coding **now** without high rework probability.  
- **Production Readiness Score:** readiness to run a real multi-market production business (includes build/ops maturity). Since no implementation exists, this remains low by definition.

| Score Type | Score | Interpretation |
|------------|------:|----------------|
| **Final Readiness Score** | **73 / 100** | Strong modular direction; missing SaaS money/market primitives and V1 scope discipline |
| **Implementation Readiness Score** | **58 / 100** | Not ready to code until P0 remediations + blocking questions are closed |
| **Production Readiness Score** | **22 / 100** | Architecture-only; zero implemented runtime hardening |

### Score Bridge (If P0 remediations accepted)

| Score Type | Potential after P0 doc updates (still pre-code) |
|------------|------:|
| Final Readiness | ~84 |
| Implementation Readiness | ~78 |
| Production Readiness | still ~25 (needs build + hardening) |

---

## 15. Overall Recommendation

**DO NOT BEGIN IMPLEMENTATION YET.**

**Conditional Approve** the V1 architecture pack as the baseline, subject to a **Remediation Revision** that updates the docs (not code) to include:

1. `Market` multi-country seam  
2. `Ledger` money core (commissions/withdrawals/escrow-ready)  
3. Shared `Listing` + provider affiliation model  
4. Module consolidation + Admin/Ads/Quality scope cut  
5. Slim booking lifecycle + reschedule + payment race rules  
6. Financial immutability + Redis/worker/MFA mandates  
7. Closed blocking question set  

### Recommended next sequence

1. Business workshop to close Section 12 decisions (especially Market, Booking, Commission, Auth)  
2. Publish `ARCHITECTURE_REMEDIATION_V1.1` doc updates (amend existing v1 docs)  
3. Upload UI design assets  
4. Only then open implementation Sprint 0/1  

### One-line recommendation

Treat the current pack as an **excellent MVP architecture draft**; upgrade it into a **commercial SaaS architecture** by adding Market + Ledger + Listing and by cutting V1 scope before any code is written.

---

## Appendix A — Challenge Log (Selected “Sacred Cows”)

| Current statement | Challenge result |
|-------------------|------------------|
| “18 modules, extract later” | Too many for V1; merge now on paper |
| “Soft delete standard columns” | Not for money tables |
| “region_code on user = multi-region ready” | Insufficient; need Market aggregate |
| “Ads in Store Dashboard V1” | Defer self-serve ads |
| “Quality auto-restrictions in V1” | Default OFF; manual first |
| “Redis optional” | For SaaS, treat as required |
| “Admin MFA optional” | Required |
| “Polymorphic service refs OK” | Prefer Listing FK |
| “Modular monolith solves scalability” | Solves complexity; not search/notify/money scale alone |

## Appendix B — Documents Reviewed

All files under `docs/v1/` including index, approval checklist, questions register, requirements 01–07, architecture 08–16, standards 17–20, workflows 21–31, devops 32–39, planning 40–45.

---

**Audit complete. No implementation changes made.**
