# KHADAMATI V1 — Implementation Readiness Execution Plan

**Document ID:** KHAD-V1-IMPL-READINESS-PLAN  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Lead Solution Architect & Delivery Governance Reviewer  

**Purpose:** Track every remaining blocker until the Implementation Gate can move from **B) NOT READY** to **A) READY FOR IMPLEMENTATION**.

**Production code / UI / application files:** **Not authorized** by this plan.  
**Architecture / V1 features:** **Do not change** — sources of truth below are binding.

---

## Sources of Truth (Only)

| Artifact | Role |
|----------|------|
| [`MASTER_IMPLEMENTATION_PROMPT_v1.0.md`](./MASTER_IMPLEMENTATION_PROMPT_v1.0.md) | Controlling brief |
| [`FINAL_ARCHITECTURE_READINESS_REPORT.md`](./FINAL_ARCHITECTURE_READINESS_REPORT.md) | Architecture readiness |
| [`FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md`](./FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md) | Closed decision set |
| [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) | Implementation gate (current **B**) |
| [`FINAL_SCOPE_BASELINE.md`](./FINAL_SCOPE_BASELINE.md) | Scope freeze (**A**) |
| [`FEATURE_TRACEABILITY_MATRIX.md`](./FEATURE_TRACEABILITY_MATRIX.md) | Feature ↔ build mapping |
| [`adr/`](./adr/README.md) ADR-001…028 | Architectural decisions |

Gate ID crosswalk (historical → this plan):

| Gate Report | This plan |
|-------------|-----------|
| G-01 | BLOCKER-001 |
| G-00 + G-02 | BLOCKER-002 |
| G-03 | BLOCKER-003 |
| G-04 | BLOCKER-004 |
| G-05 | BLOCKER-005 |
| G-06 | BLOCKER-006 |
| G-07 | BLOCKER-007 |

---

# 1. Readiness Gate Summary

| Dimension | Status |
|-----------|--------|
| **Implementation status** | **B) NOT READY — CODING BLOCKED** |
| **Architecture status** | **Approved** (ADR-001…028; Decisions Complete) |
| **Scope status** | **Frozen** ([`FINAL_SCOPE_BASELINE.md`](./FINAL_SCOPE_BASELINE.md) — **A) Scope Frozen**) |

### Explanation

Architecture and product scope are **complete**.

Remaining work is **execution preparation only**:

- Design assets → UI/UX specification  
- Formal stakeholder sign-off  
- Vendor selections (ports remain vendor-neutral)  
- Cloud hosting approval  
- Finance Lebanon **configuration content** (rules stay admin-configurable)  
- Compliance retention **numeric defaults**  
- Payment.js mobile **validation spike** (no production payment UI)

Closing **BLOCKER-001…007** is the path to amend the Implementation Gate Report to **A) READY FOR IMPLEMENTATION**.

---

# 2. Remaining Blocker Register

| ID | Blocker | Description | Required Action | Owner | Dependencies | Acceptance Criteria | Status |
|----|---------|-------------|-----------------|-------|--------------|---------------------|--------|
| BLOCKER-001 | Design assets & UI/UX specification | Approval package ready; **binaries missing**; colors Draft/Approved split (Approved empty) | Deposit assets; extract/approve colors; Product + Design signatures → COMPLETED | Design (+ Product) | Official logo · design docs | Assets + colors approved + dual signatures | **READY FOR APPROVAL** |
| BLOCKER-002 | Stakeholder sign-off | Sign-off package ready; signatures pending | Record approvals on STAKEHOLDER_SIGN_OFF_PACKAGE + Scope Baseline + Gate Report | Product / Arch / Eng / Business / Ops | Scope Baseline published | Formal approval recorded (names, dates, decisions) | **READY FOR APPROVAL** |
| BLOCKER-003 | External vendor decisions | Integration readiness framework ready; selections Pending | Select vendors; contracts; sandbox; technical validation; ports only | Business / Eng | ADR-025 ports | Vendors selected (or deferred); sandbox + validation; no business-logic coupling | **IN PREPARATION** |
| BLOCKER-004 | Cloud infrastructure approval | Blueprint + sizing/cost + prod ops ready; provider/budget/RPO/RTO Pending | Fill workload; select provider; approve budget/RPO/RTO; ops checklist | DevOps / Business | Portable deploy pattern (ADR-012/024) | Provider, budget, RPO/RTO, ops readiness approved | **IN PREPARATION** |
| BLOCKER-005 | Finance Lebanon configuration | Structure prepared; commercial values **Pending Business Decision** | Business/Finance fill values; approve; keep Admin-configurable | Finance | ADR-013 / 026 | Config values approved; remain Admin-editable; **not** hardcoded | **IN PREPARATION** |
| BLOCKER-006 | Compliance & retention defaults | Numeric retention/deletion defaults unset | Set defaults for account deletion, PII, financial, audit, documents | Compliance / Legal | ADR-022 | Defaults approved; financial & audit records protected | **Open** |
| BLOCKER-007 | Payment.js mobile validation spike | PASS WITH CONDITIONS; vendor checklist ready; live tests pending | Fill Areeba checklist; sandbox tests; Architect + Eng approval | Eng | Areeba sandbox (BLOCKER-003) | Checklist §8 complete → COMPLETED | **IN VALIDATION** |

---

## BLOCKER-001 — Design Assets & UI/UX Specification

**Gate mapping:** G-01 · **ADR:** ADR-023 · ADR-010 · ADR-018  
**Status:** **READY FOR APPROVAL** (not COMPLETED — binaries missing; colors not approved; signatures pending)

### Required inputs

| Input | State |
|-------|-------|
| Official logo binaries | ☐ `master/original` · `transparent` · `high-resolution` |
| App icons | ☐ `app-icon/android` · `app-icon/ios` |
| Variations | ☐ `variations/light` · `dark` · `monochrome` |
| Brand identity direction | ☑ Documented + final quality review |
| Colors | ☐ Draft placeholders only — Approved section empty |
| Typography / UX / RTL / responsive | ☑ Specified — approval table Pending |

### Required output

| Artifact | Path | State |
|----------|------|-------|
| Brand Identity Specification | [`design/BRAND_IDENTITY_SPECIFICATION.md`](./design/BRAND_IDENTITY_SPECIFICATION.md) | ☑ Ready for approval |
| UI/UX Design Specification | [`design/UI_UX_SPECIFICATION.md`](./design/UI_UX_SPECIFICATION.md) | ☑ Ready for approval |
| Design System Tokens | [`design/DESIGN_SYSTEM_TOKENS.md`](./design/DESIGN_SYSTEM_TOKENS.md) | ☑ Ready for approval |
| Color Reference | [`design/COLOR_REFERENCE.md`](./design/COLOR_REFERENCE.md) | ☑ Draft vs Approved; no guessed production HEX |
| Logo Asset Package | [`design/assets/LOGO_ASSET_PACKAGE.md`](./design/assets/LOGO_ASSET_PACKAGE.md) | ☑ Structure verified; binaries missing |
| Design Approval Record | [`design/DESIGN_APPROVAL_RECORD.md`](./design/DESIGN_APPROVAL_RECORD.md) | ☑ Approval table + design freeze rule |

### Acceptance criteria → COMPLETED

- [ ] Assets deposited  
- [ ] Colors approved  
- [ ] Product approval recorded  
- [ ] Design approval recorded  

No UI coding until COMPLETED **and** Implementation Gate → **A**. Logo concept preserved.

### Tracking

| Item | State |
|------|-------|
| Required folder structure | ☑ |
| Specs / approval package | ☑ |
| Binaries deposited | ☐ |
| Colors Approved section filled | ☐ |
| Approval table + signatures | ☐ |
| Blocker **COMPLETED** | ☐ |

---

## BLOCKER-002 — Stakeholder Sign-off

**Gate mapping:** G-00 (scope) + G-02 (gate)  
**Status:** **READY FOR APPROVAL** (not COMPLETED)  
**Package:** [`governance/STAKEHOLDER_SIGN_OFF_PACKAGE.md`](./governance/STAKEHOLDER_SIGN_OFF_PACKAGE.md)

### Approval checklist

Use the Sign-off Package for full detail. Summary:

#### Product

- [ ] Marketplace model (service-first; no mandatory provider-type chooser)  
- [ ] Provider model (unified + capabilities)  
- [ ] Store model (services + catalog ads; no e-commerce)  
- [ ] Customer + provider workflows  

#### Business

- [ ] Revenue model (subscriptions, promotions, commissions)  
- [ ] Admin-configurable finance policies (no hardcoded rules)  

#### Operations

- [ ] Verification · support · disputes · notifications  

#### Architecture & delivery gate

- [ ] Sign [`FINAL_SCOPE_BASELINE.md`](./FINAL_SCOPE_BASELINE.md)  
- [ ] Sign [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) when other blockers allow READY (or acknowledge current B)  
- [ ] Confirm no unauthorized V1 feature additions  
- [ ] Complete Approval Record in Sign-off Package §6  

### Acceptance criteria → COMPLETED

Formal approval recorded (role, name, date, decision) on Sign-off Package, Scope Baseline, and Gate Report.  
Keep **READY FOR APPROVAL** until signatures exist.

### Tracking

| Item | State |
|------|-------|
| Sign-off package published | ☑ |
| Product checklist complete | ☐ |
| Business checklist complete | ☐ |
| Operations checklist complete | ☐ |
| Scope Baseline signed | ☐ |
| Gate acknowledgements recorded | ☐ |
| Blocker closed | ☐ |

---

## BLOCKER-003 — External Vendor Decisions

**Gate mapping:** G-03 · **ADR:** ADR-025  
**Status:** **IN PREPARATION** (not COMPLETED)  
**Artifacts:** [`vendors/VENDOR_INTEGRATION_READINESS_MATRIX.md`](./vendors/VENDOR_INTEGRATION_READINESS_MATRIX.md) · [`vendors/VENDOR_EVALUATION_MATRIX.md`](./vendors/VENDOR_EVALUATION_MATRIX.md)

**Rule:** Maintain vendor-neutral **ports/adapters**. Do **not** couple business logic to vendors. No vendor approved without Technical → Security → Business → Contract → Integration authorization.

### Vendor decision checklist

#### Payment — Areeba IXOPAY (architecture candidate; approval pending validation)

| Validate | Owner | State |
|----------|-------|-------|
| Sandbox access | Eng / Business | ☐ PVC |
| Payment.js requirements | Eng | ☐ PVC |
| Webhook requirements | Eng | ☐ PVC |
| Credentials process (secrets, rotation) | Eng / DevOps | ☐ PVC |

#### SMS

Requirements: OTP · Booking notifications · Reminders  

| Decision | State |
|----------|-------|
| Vendor selected | ☐ |
| Adapter contract confirmed | ☐ |
| Credential/process defined | ☐ |

#### Email

Requirements: Verification emails · Notifications · Reports  

| Decision | State |
|----------|-------|
| Vendor selected | ☐ |
| Adapter contract confirmed | ☐ |
| Credential/process defined | ☐ |

#### Storage (object)

Requirements: Images · Documents · Attachments  

| Decision | State |
|----------|-------|
| Vendor selected | ☐ |
| Adapter contract confirmed | ☐ |
| Bucket/lifecycle policy outline | ☐ |

#### Maps

Requirements: Location · Distance · Service area  

| Decision | State |
|----------|-------|
| Vendor selected | ☐ |
| Adapter contract confirmed | ☐ |
| Licensing/quota noted | ☐ |

#### OCR / Face verification

Requirements: Provider onboarding · Identity verification  

| Decision | State |
|----------|-------|
| OCR vendor selected (or phased deferral with Product ack) | ☐ |
| Face verification vendor selected (or phased deferral with Product ack) | ☐ |
| Adapter contracts confirmed | ☐ |

### Acceptance criteria → COMPLETED

- Required vendors selected **or** explicitly deferred with Product risk acknowledgement  
- Contracts approved (as applicable)  
- Sandbox credentials available  
- Technical validation completed (Payment checklist + BLOCKER-007 conditions)  
- Architecture remains port/adapter based (no vendor leakage into domain services)  

Keep **IN PREPARATION** until above are done.

---

## BLOCKER-004 — Cloud Infrastructure Approval

**Gate mapping:** G-04 · **ADR:** ADR-012 / ADR-024  
**Status:** **IN PREPARATION** (not COMPLETED)  
**Artifacts:** [`infra/CLOUD_INFRASTRUCTURE_DECISION.md`](./infra/CLOUD_INFRASTRUCTURE_DECISION.md) · [`infra/CLOUD_SIZING_AND_COST_FRAMEWORK.md`](./infra/CLOUD_SIZING_AND_COST_FRAMEWORK.md) · [`infra/PRODUCTION_OPERATIONS_READINESS.md`](./infra/PRODUCTION_OPERATIONS_READINESS.md)

**Rule:** Decide hosting **targets**. Do **not** modify approved architecture. Remain vendor-neutral until approved. Use sizing/cost framework for provider comparison (no invented prices). Production ops requirements must be met before launch.

### Production infrastructure decision sheet

| Decision | Options / notes | Approved value | State |
|----------|-----------------|----------------|-------|
| Cloud provider | AWS / Azure / GCP / other — not locked | Pending Infrastructure Approval | ☐ |
| Region | Prefer Lebanon-proximate / compliance-approved | Pending Infrastructure Approval | ☐ |
| Database hosting | Managed PostgreSQL preferred | Pending Infrastructure Approval | ☐ |
| Object storage | Align with BLOCKER-003 Storage | Pending Infrastructure Approval | ☐ |
| Redis hosting | Required (cache/queues) | Pending Infrastructure Approval | ☐ |
| Worker hosting | Async jobs / outbox / settlement | Pending Infrastructure Approval | ☐ |
| Monitoring / logging | Metrics + alerting + central logs | Pending Infrastructure Approval | ☐ |
| Backup strategy | DB PITR + blob versioning | Pending Infrastructure Approval | ☐ |
| Disaster recovery | RPO/RTO | **Pending Infrastructure Decision** | ☐ |

### Environments

| Env | Purpose | Approved |
|-----|---------|----------|
| Development | Engineering | ☐ |
| Testing / Staging | Pre-prod / finance dry-run | ☐ |
| Production | Live | ☐ |

### Acceptance criteria → COMPLETED

Cloud provider, region, and hosting choices for DB / Redis / workers / storage / observability / backup / DR **approved** in writing. Architecture unchanged.  
Keep **IN PREPARATION** until checklist complete.

---

## BLOCKER-005 — Finance Lebanon Configuration

**Gate mapping:** G-05 · **ADR:** ADR-013 / ADR-026  
**Status:** **IN PREPARATION** (not COMPLETED)  
**Artifacts:** [`config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md`](./config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md) · [`governance/FINANCE_POLICY_APPROVAL_MATRIX.md`](./governance/FINANCE_POLICY_APPROVAL_MATRIX.md) · [`governance/FINANCE_POLICY_LIFECYCLE.md`](./governance/FINANCE_POLICY_LIFECYCLE.md)

**Architecture:** Approved.  
**Need:** Initial **business configuration values** only.

**Important:** Values must remain **Admin-configurable**. Do **not** hardcode. Mark unknowns **Pending Business Decision**.

### Required inputs

| Area | Structure documented | Values |
|------|---------------------|--------|
| Currency (Lebanon / USD) | ☑ | Defaults set; display nuances PBD |
| Commission | ☑ | **Pending Business Decision** |
| Subscriptions (provider + store) | ☑ | **Pending Business Decision** |
| Cancellation | ☑ | **Pending Business Decision** |
| Refund | ☑ | **Pending Business Decision** |
| Withdrawal | ☑ | **Pending Business Decision** |
| Settlement | ☑ | **Pending Business Decision** |
| Admin permissions / audit | ☑ | Confirmed required |

### Acceptance criteria → COMPLETED

- Finance signs off Lebanon initial values (no remaining PBD for launch set)  
- Documented as configuration (seed/admin), **not** constants in application code  
- Change history / audit expectations acknowledged  
- Keep **IN PREPARATION** until values approved

---

## BLOCKER-006 — Compliance & Retention Defaults

**Gate mapping:** G-06 · **ADR:** ADR-022  
**Status:** Open  

### Required decisions

| Topic | Default / rule | Protect? | State |
|-------|----------------|----------|-------|
| Account deletion handling | | Soft-delete / request workflow per ADR | ☐ |
| Personal data retention | | Per policy | ☐ |
| Financial record retention | | **Protected** — do not purge with account alone | ☐ |
| Audit log retention | | **Protected** | ☐ |
| Document retention (KYC/media) | | Per policy | ☐ |

### Acceptance criteria

- Numeric or explicit policy defaults approved by Compliance/Legal  
- Financial records and audit records remain protected from casual deletion  
- Admin retention configuration model remains the runtime control (ADR-022)  

---

## BLOCKER-007 — Payment.js Mobile Validation Spike

**Gate mapping:** G-07 · **ADR:** ADR-004 / ADR-025 / Payment architecture  
**Status:** **IN VALIDATION** (not COMPLETED)  

**Constraint:** Validation only. **No production implementation.**

### Validation plan

| Area | Validate | State |
|------|----------|-------|
| Mobile payment flow | Customer app WebView / Payment.js path | ☑ Architecture validated |
| Payment.js compatibility | Flutter host + required browser APIs | ☑ Feasible; device proof pending |
| Token handling | No PAN storage; token lifecycle | ☑ Architecture validated |
| Secure communication | TLS, CSP/origin rules as required | ☑ Requirements confirmed |
| Webhook lifecycle | Auth, idempotency, ledger post | ☑ Architecture validated; signature algo pending vendor |
| Failure scenarios | Timeout, cancel, decline, network drop | ☑ Documented |
| Duplicate payment prevention | Idempotency keys / gateway refs | ☑ Architecture validated |
| Live sandbox PJS-01…10 | Android/iOS WebView | ☐ Blocked on credentials |

### Required output

| Artifact | State |
|----------|-------|
| [`payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](./payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) | ☑ PASS WITH CONDITIONS |
| [`payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) | ☑ Published — values PVC |

### Acceptance criteria → COMPLETED

- [ ] Sandbox credentials received  
- [ ] Android payment flow tested  
- [ ] iOS payment flow tested  
- [ ] Webhook verified  
- [ ] 3DS behaviour confirmed  
- [ ] Payment lifecycle validated  
- [ ] Architect approval  
- [ ] Engineering approval  

Keep status **IN VALIDATION** until all boxes above are checked.

### Dependencies

- BLOCKER-003 Payment sandbox / credentials path (use Areeba checklist as evidence)  

---

# 3. Implementation Prerequisites Checklist

Before coding authorization, **all** must be complete:

| # | Prerequisite | Blocker | State |
|---|--------------|---------|-------|
| 1 | Scope approved | BLOCKER-002 | [ ] |
| 2 | Architecture approved | *(already approved — ADR-001…028)* | [x] |
| 3 | Design specification approved | BLOCKER-001 | [ ] |
| 4 | Vendors selected | BLOCKER-003 | [ ] |
| 5 | Cloud approved | BLOCKER-004 | [ ] |
| 6 | Finance configuration approved | BLOCKER-005 | [ ] |
| 7 | Compliance rules approved | BLOCKER-006 | [ ] |
| 8 | Payment.js mobile validation completed | BLOCKER-007 | [ ] |

**Gate flip rule:** Only when all rows above are satisfied may [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) be amended to **A) READY FOR IMPLEMENTATION**.

Until then:

```text
DO NOT write production code.
DO NOT implement UI.
DO NOT generate application files.
DO NOT change approved architecture.
DO NOT introduce new V1 features.
```

---

# 4. Change Control Reminder

After scope freeze ([`FINAL_SCOPE_BASELINE.md`](./FINAL_SCOPE_BASELINE.md) §10):

**No new feature enters V1** without:

1. Business justification  
2. Architecture impact review  
3. Database impact review  
4. API impact review  
5. Security review  
6. Approval decision: **Add to V1** · **Move to future roadmap** · **Reject**

Unauthorized scope additions during readiness or implementation are **out of process** and must be refused or escalated.

---

# 5. Progress Log

| Date | Event | Blockers closed | Gate |
|------|-------|-----------------|------|
| 2026-07-24 | Execution plan published | None | **B) NOT READY** |
| 2026-07-24 | Blocker closure pack published — [`READINESS_BLOCKER_CLOSURE_STATUS.md`](./READINESS_BLOCKER_CLOSURE_STATUS.md) | 0/7 (prep artifacts ready) | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-001 → **ASSET RECEIVED — DESIGN SPECIFICATION IN PROGRESS** (brand + UI/UX + tokens drafted) | 0/7 (none closed; 001 in progress) | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-001 → **READY FOR APPROVAL** (approval package; not Completed) | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-001 final approval readiness; asset completeness verified fail — remains **READY FOR APPROVAL** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-007 → **IN VALIDATION** (Payment.js report PASS WITH CONDITIONS) | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-007 Areeba IXOPAY vendor validation checklist published — still **IN VALIDATION** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-002 → **READY FOR APPROVAL** (Stakeholder Sign-off Package; not COMPLETED) | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-005 → **IN PREPARATION** (Finance Lebanon structure; values PBD) | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-005 Finance Policy Approval Matrix published — still **IN PREPARATION** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-005 Finance Policy Lifecycle published — still **IN PREPARATION** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-004 → **IN PREPARATION** (Cloud Infrastructure Decision v1.1) | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-004 Cloud Sizing and Cost Framework published — still **IN PREPARATION** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-004 Production Operations Readiness published — still **IN PREPARATION** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-004 Prod Ops Readiness v1.1 KHADAMATI-specific — still **IN PREPARATION** | 0/7 | **B) NOT READY — CODING BLOCKED** |
| 2026-07-24 | BLOCKER-003 → **IN PREPARATION** (Vendor Integration Readiness Matrix) | 0/7 | **B) NOT READY — CODING BLOCKED** |

Active closure tracker: [`READINESS_BLOCKER_CLOSURE_STATUS.md`](./READINESS_BLOCKER_CLOSURE_STATUS.md).  
Update that dashboard (and this table) when a blocker moves to Closed. Do **not** change gate decision from B until §3 checklist is complete.

---

# 6. Document Control

| Action | Document |
|--------|----------|
| Created | This plan |
| Tracks | BLOCKER-001…007 → Gate A |
| Related | Gate Report · Master Prompt · FTM · Scope Baseline |

**End of Implementation Readiness Execution Plan v1.0**
