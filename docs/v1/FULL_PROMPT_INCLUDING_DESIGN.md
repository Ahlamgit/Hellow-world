# KHADAMATI V1 — Full Prompt (Architecture + Design)

> **Superseded as controlling brief by** [`MASTER_IMPLEMENTATION_PROMPT_v1.0.md`](./MASTER_IMPLEMENTATION_PROMPT_v1.0.md).  
> Keep this file for historical composite notes. Prefer Master Prompt + Feature Traceability Matrix + ADRs.

**Document ID:** KHAD-V1-FULL-PROMPT  
**Status:** Historical / secondary — use Master Prompt v1.0

---

## How to use this prompt

Paste this entire document (or attach it) as the controlling brief for implementation.

When design assets are uploaded, fill **Section D — Design Assets** with concrete paths/links, then proceed.

---

# A. Role & Mission

You are the Lead Solution Architect, Senior Product Manager, Senior Spring Boot Architect, Senior React Architect, Senior Flutter Architect, Senior Database Architect, Senior DevOps Engineer, Senior QA Lead, and Senior Business Analyst for the **KHADAMATI** platform.

Build **Version 1 (MVP+)** as a cleaner, modern, scalable, production-oriented commercial SaaS foundation — **not** a continuation of the previous ASP.NET / SQL Server / native mobile implementation.

Do **not** reuse prior business assumptions without validating them against `docs/v1/` and the open-questions register.

### Priorities

- Clean Architecture  
- Maintainability  
- Scalability  
- Security  
- Performance  
- Modular design  
- Production readiness  
- **Faithful UI reproduction of provided designs** (no generic templates)

---

# B. Fixed Technology Stack (Non-Negotiable)

### Backend
- Spring Boot 3.x  
- Java 21  
- Spring Security  
- Spring Data JPA  
- REST APIs  
- Maven  
- JWT Authentication  
- Clean layered / ports-and-adapters modular monolith  
- OpenAPI documentation  

### Database
- PostgreSQL  
- Flyway migrations  

### Administration Portal
- React + TypeScript + Material UI  
- **Sole admin login channel (web only)**  

### Store Dashboard
- React + TypeScript + Material UI  

### Customer Mobile App
- Flutter  

### Craftsman Mobile App
- Flutter  

### Payments (V1 only)
- **Areeba IXOPAY Payment.js only**  
- Must use a payment gateway **abstraction/port** so future gateways do not change business logic  
- KHADAMATI must **never** receive raw PAN/CVV  

---

# C. Product Surfaces & Modules

## Administration Portal (web only for admins)
Advertisements management · User management · Customer management · Craftsman management · Store management · Craftsman onboarding approvals · Notification templates · Email / SMS / In-app notifications · Commission management · Settlement overview · Subscription plans · Analytics dashboards · Reports · Ratings management · Global settings · Audit logs  

> Per architecture audit: prefer a **slim V1 admin** — deep analytics, self-serve ads ops complexity, and auto-quality restriction engines may be staged/deferred behind flags. Core governance must ship.

## Store Dashboard
Store profile · Products · Services · Orders · Advertisements · Booking management · Customers · Reports  

> Per architecture audit: **self-serve paid ads** are candidates to defer to V1.1/V2 unless business insists; keep catalog/orders/bookings/reports.

## Customer Mobile
Registration · Login · Profile · Search services · Browse categories · Browse stores · Service booking · Booking history · Online payment · Booking status · Notifications · Reviews · Ratings  

## Craftsman Mobile
Registration · Login · Profile · Service catalog · Guided onboarding · Document upload · Approval workflow · GPS proximity verification · Real-time selfie verification · QR / OTP job verification · Subscription management · Dashboard · Earnings · KPIs · Withdrawal requests · Notifications  

## Follow-up & Quality Layer
Scheduled reminders · 24-hour booking reminders · Arrival verification · Job progress monitoring · Customer satisfaction survey · Quality scoring · Restriction rules · Operational visibility dashboards  

## Cross-cutting
Payments · Commissions · Advertisements · Ratings · Reviews · OCR · Face Recognition · Notifications · Services · Products · Stores · Booking Engine · Identity Verification · Subscription Management  

---

# D. Design Assets (REQUIRED before UI implementation)

## D.1 Upload checklist (attach before coding UI)

| Asset | Status | Location / Link |
|-------|--------|-----------------|
| UI design video | ☐ Pending upload | `_TBD_` |
| Screenshots (all key screens) | ☐ Pending upload | `_TBD_` |
| Branding (logo, wordmark, variants) | ☐ Pending upload | `_TBD_` |
| Color palette (primary/secondary/neutral/semantic) | ☐ Pending upload | `_TBD_` |
| Typography (display/body fonts + scales) | ☐ Pending upload | `_TBD_` |
| UX requirements / interaction notes | ☐ Pending upload | `_TBD_` |
| Iconography / illustration set | ☐ Pending upload | `_TBD_` |
| Motion / animation guidance | ☐ Pending upload | `_TBD_` |
| Responsive / mobile breakpoints notes | ☐ Pending upload | `_TBD_` |
| RTL (Arabic) design variants if separate | ☐ Pending upload | `_TBD_` |

Until these are provided and reviewed: **do not invent a marketing or product visual system**. Architecture and API work may proceed only where UI is not required; UI screens must wait.

## D.2 Design fidelity rules (mandatory)

1. **Reproduce the provided designs faithfully** — layouts, spacing, hierarchy, components, and states.  
2. **Do not** use generic AI/template looks (no default purple-on-white SaaS theme, no cream+terracotta cliché, no fake dashboard chrome unless the design shows it).  
3. Map designs into:
   - Web: `packages/ui` design tokens + MUI theme overrides  
   - Flutter: `khad_ui` tokens + themed widgets  
4. Business logic stays in backend / domain layers — **UI replacement must not require business rewrites**.  
5. Support **RTL** readiness for Arabic when designs include it.  
6. Implement empty, loading, error, and success states as shown (or as UX notes specify).  
7. If a design contradicts an approved business rule, **stop and escalate** — do not silently invent behavior.  
8. Administrators never appear as a login persona inside mobile designs; if a design shows admin on mobile, treat as out of scope / error and use web Admin Portal only.

## D.3 Frontend design hard rules (landing / promotional surfaces)

When implementing branded/marketing surfaces from design:

- One composition in the first viewport (not a dashboard unless designed as one)  
- Brand first — brand/product name is hero-level  
- Expressive fonts per brand (not Inter/Roboto/Arial/system defaults unless design specifies)  
- Atmospheric backgrounds per design (not flat single-color defaults)  
- Full-bleed hero only when the design shows it  
- Hero budget discipline: brand, one headline, one supporting line, CTA group, dominant visual  
- No detached hero overlays/badges unless in the design  
- Cards only when required for interaction or explicitly designed  
- One job per section  
- Ship intentional motion only when specified (2–3 purposeful motions for visually led pages)  

**Exception:** Inside Admin Portal / Store Dashboard operational screens, follow the uploaded product UI designs and established app chrome — not marketing-hero rules.

## D.4 Design → engineering mapping

| Design deliverable | Engineering artifact |
|--------------------|----------------------|
| Color palette | CSS/JS theme tokens + Flutter `ColorScheme` tokens |
| Typography | Web font loading + Flutter text themes |
| Components | Shared UI kit wrappers (do not scatter one-off styles) |
| Screens | Feature pages/screens matching frame names |
| Flows | Navigation graphs matching UX prototype/video |
| Motion | Explicit animation specs; prefer subtle, purposeful |

---

# E. Confirmed Business Rules (Do Not Reopen Without Product Owner)

| ID | Rule |
|----|------|
| BR-001 | V1 payment gateway implementation = Areeba IXOPAY Payment.js only |
| BR-002 | Payment abstraction/port required for future gateways |
| BR-003 | Four client surfaces: Admin Portal, Store Dashboard, Customer App, Craftsman App |
| BR-004 | Craftsman onboarding requires approval workflow |
| BR-005 | IDV includes GPS, selfie, QR/OTP |
| BR-006 | Quality follow-up layer in scope (stage automation carefully) |
| BR-007 | Multi-language, multi-currency, multi-region **readiness** required |
| **BR-008** | **Administrators must NOT log in from apps. Admins use Administration Portal (web) only.** |

### BR-008 enforcement

- Flutter apps: no admin login UI  
- Auth login requires `audience` / `clientId`: `admin-web` \| `store-web` \| `customer-app` \| `craftsman-app`  
- Admin roles + non-`admin-web` → reject `AUTH_ADMIN_WEB_ONLY`  
- `/api/v1/admin/**` requires admin permissions **and** `admin-web` audience  

---

# F. Architecture Source of Truth

Read and obey:

| Doc | Path |
|-----|------|
| Index | `docs/v1/00-INDEX.md` |
| Open questions | `docs/v1/QUESTIONS-REQUIRING-BUSINESS-DECISION.md` |
| Final audit | `docs/v1/ARCHITECTURE_AUDIT_FINAL.md` |
| Approval checklist | `docs/v1/APPROVAL-CHECKLIST.md` |
| Requirements 01–07 | `docs/v1/requirements/` |
| Architecture 08–16 | `docs/v1/architecture/` |
| Standards 17–20 | `docs/v1/standards/` |
| Workflows 21–31 | `docs/v1/workflows/` |
| DevOps 32–39 | `docs/v1/devops/` |
| Planning 40–45 | `docs/v1/planning/` |
| **This full prompt** | `docs/v1/FULL_PROMPT_INCLUDING_DESIGN.md` |

Legacy `/src` ASP.NET / SQL Server / native apps are **not** authoritative for V1.

---

# G. Mandatory Pre-Implementation Remediations (from Final Audit)

Before coding money/booking cores, architecture must incorporate (docs first if not already amended):

1. **`Market` (OperatingCountry)** seam — country, currency, locales, timezone, payment merchant ref  
2. **`Ledger`** minimal double-entry — platform / payable / pending-escrow / gateway clearing  
3. **Shared `Listing` (Offer)** — unify store/craftsman bookable catalog units + affiliation  
4. Module consolidation (slim V1 bounded contexts; admin as facade; merge rating+quality into trust where practical)  
5. Slim booking states + **reschedule** support + payment webhook precedence rules  
6. **No soft-delete** on financial records (immutable + reversing entries)  
7. Admin **MFA required** (treat as default unless Product explicitly overrides in writing)  
8. **Redis** (or equivalent) required for rate-limits / distributed locks / flag cache  
9. **Separate worker** process/profile for schedulers & outbox  
10. Close blocking `Q-*` decisions (see Section H)  

**Scores to beat (current audit):** Final 73 · Implementation 58 · Production 22 — do not pretend production readiness until built and hardened.

---

# H. Ambiguity Policy

Whenever a business rule is ambiguous:

1. Do **not** invent product policy  
2. Use / extend `docs/v1/QUESTIONS-REQUIRING-BUSINESS-DECISION.md` with `Q-AREA-###`  
3. Feature-flag deferred behavior  

### Minimum blockers before schema freeze

- Q-LOC-001..003 (+ Market)  
- Q-AUTH-001 (identifier)  
- Q-AUTH-002 (admin MFA — recommend required)  
- Q-BOOK-007/008/009 (+ cancel/refund + reschedule)  
- Q-REL-001/002 (+ Listing)  
- Q-COM-001/004 (+ Ledger)  
- Q-SET-002/004  
- Q-PAY-001  
- Q-SUB-001/002  
- Q-DEP-001/002/003  

**Already decided:** Q-AUTH-007 — admins web-only.

---

# I. Non-Functional Requirements

High performance · Horizontal scalability · Secure authentication · RBAC · Audit trails · OWASP Top 10 · Multi-language readiness · Multi-currency readiness · Multi-region readiness · Configurable settings · Cloud deployment · Docker · CI/CD readiness  

---

# J. Implementation Constraints

1. **No coding** until architecture remediations accepted **and** design assets uploaded/reviewed for UI work.  
2. New V1 code lives in the approved folder structure (`apps/`, `backend/`, `db/`, `packages/`) — do not extend legacy .NET runtime as V1.  
3. API-first: OpenAPI updated with endpoints.  
4. Money APIs: idempotency keys + webhook idempotency.  
5. Domain events via transactional outbox.  
6. Problem Details (RFC 7807) errors with stable `code`.  
7. Tests for authz negatives and money paths.  
8. No secrets in git.  
9. UI must match designs; tokens in shared UI packages.  
10. Prefer expand/contract Flyway migrations.  

---

# K. Payment Flow Summary (Areeba IXOPAY Payment.js)

1. Client creates payment intent (server amount authoritative)  
2. Client initializes Payment.js with **public** integration key + hosted fields  
3. `tokenize()` → single-use `transactionToken`  
4. Client calls API debit with token + Idempotency-Key  
5. Adapter calls IXOPAY debit; handle success / 3DS `REQUIRES_ACTION` / failure  
6. Authenticated webhook finalizes idempotently  
7. Emit outbox events (`PaymentCaptured` / `PaymentFailed`)  
8. Commissions/ledger postings consume events — **never** inside gateway adapter  

Flutter: Payment.js via secured WebView + JS bridge (spike-gated).

---

# L. Suggested Delivery Order

1. Resolve blocking questions + apply audit remediations to docs  
2. Ingest design assets → extract tokens → screen inventory  
3. Platform kernel + IAM (including admin-web audience enforcement)  
4. Profiles + media + listings/catalog  
5. Booking engine  
6. Payments (IXOPAY)  
7. Onboarding + IDV + admin approvals  
8. Subscriptions + ledger/commissions + withdrawals  
9. Notifications completeness  
10. Ratings / quality (manual restrictions first)  
11. Store dashboard + slim admin  
12. Hardening, reports, launch  

Follow milestones in `docs/v1/planning/41-MILESTONES.md`.

---

# M. Definition of Done (per feature)

- Matches accepted architecture + **design frames**  
- OpenAPI + Flyway (if schema)  
- Unit/API tests including authz deny paths  
- Admin-web audience respected where relevant  
- No PAN/PII leakage in logs  
- Feature flags for risky automation  
- Staging demo evidence  

---

# N. Explicit Non-Goals for V1 Coding Kickoff

- Implementing UI before design upload  
- Adding non-Areeba gateways  
- Admin login inside Flutter apps  
- Rebuilding legacy ASP.NET stack  
- Inventing commission/refund policy values  
- Generic template UI unrelated to brand designs  

---

# O. Kickoff Message (copy when designs are ready)

```text
Implement KHADAMATI V1 per docs/v1/FULL_PROMPT_INCLUDING_DESIGN.md.

Design assets are available at:
- Video: <path>
- Screenshots: <path>
- Branding/colors/typography: <path>
- UX notes: <path>

Obey BR-008 (admins web-only), Payment.js-only with abstraction,
Market/Ledger/Listing remediations, and faithful design reproduction.

Do not invent unresolved Q-* policies; escalate instead.
Start with [IAM | Catalog | Booking | …] vertical slice.
```

---

# P. Approval Gate

| Gate | Required |
|------|----------|
| Architecture pack reviewed | ☐ |
| Final audit P0 remediations accepted | ☐ |
| Blocking questions dispositioned | ☐ |
| Design assets uploaded & reviewed | ☐ |
| This full prompt acknowledged by eng lead | ☐ |

**Only when all are checked → implementation begins.**
