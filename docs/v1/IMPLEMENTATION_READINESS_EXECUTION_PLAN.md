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
| BLOCKER-001 | Design assets & UI/UX specification | Official logo received; brand + UI/UX + tokens drafted — pending approval | Complete review/approval of design specs; commit master logo binaries to `design/assets/` | Design (+ Product) | Official logo reference | UI specification **approved** before any UI implementation | **ASSET RECEIVED — DESIGN SPECIFICATION IN PROGRESS** |
| BLOCKER-002 | Stakeholder sign-off | Scope / business / ops / architecture gate approvals not recorded | Complete approval checklist; record signatures on Scope Baseline + Gate Report | Product / Arch / Eng / Business / Ops | Scope Baseline published | Formal approval recorded (names, dates, decisions) | **Open** |
| BLOCKER-003 | External vendor decisions | SMS, Email, Storage, Maps, OCR/Face not selected; Payment sandbox/process to validate | Complete vendor checklist; confirm adapter-only integration | Business / Eng | ADR-025 ports | Vendors selected (or deferred with risk ack); Payment sandbox path confirmed; no business-logic coupling | **Open** |
| BLOCKER-004 | Cloud infrastructure approval | Target cloud/region/hosting not approved | Approve infra decision sheet (no architecture change) | DevOps / Business | Portable deploy pattern (ADR-012/024) | Cloud provider, region, and hosting choices approved for staging/prod | **Open** |
| BLOCKER-005 | Finance Lebanon configuration | Policy **content** not prepared; architecture already approved | Provide initial admin-configurable values for commission/cancel/refund/withdrawal/settlement | Finance | ADR-013 / 026 | Config values approved for staging; remain Admin-editable; **not** hardcoded | **Open** |
| BLOCKER-006 | Compliance & retention defaults | Numeric retention/deletion defaults unset | Set defaults for account deletion, PII, financial, audit, documents | Compliance / Legal | ADR-022 | Defaults approved; financial & audit records protected | **Open** |
| BLOCKER-007 | Payment.js mobile validation spike | Mobile Payment.js feasibility not evidenced | Run non-production spike; publish readiness report | Eng | Areeba sandbox access (BLOCKER-003 Payment) | Readiness report accepted; critical failures resolved or risk accepted | **Open** |

---

## BLOCKER-001 — Design Assets & UI/UX Specification

**Gate mapping:** G-01 · **ADR:** ADR-023 · ADR-010 · ADR-018  
**Status:** **ASSET RECEIVED — DESIGN SPECIFICATION IN PROGRESS**

### Required inputs

| Input | State |
|-------|-------|
| Official logo (خدماتي / KHADAMATI · home + tools · orange + navy) | ☑ Received (brand reference) |
| Brand identity direction | ☑ Documented |
| Colors / typography from logo | ☑ Tokenized (hex confirm pending master file sample) |
| Optional: prior UI screens / video / reference apps | ☐ Optional inspiration |

### Required output (drafted)

| Artifact | Path | State |
|----------|------|-------|
| Brand Identity Specification | [`design/BRAND_IDENTITY_SPECIFICATION.md`](./design/BRAND_IDENTITY_SPECIFICATION.md) | ☑ Draft |
| UI/UX Design Specification | [`design/UI_UX_SPECIFICATION.md`](./design/UI_UX_SPECIFICATION.md) | ☑ Draft — in progress / ready for review |
| Design System Tokens | [`design/DESIGN_SYSTEM_TOKENS.md`](./design/DESIGN_SYSTEM_TOKENS.md) | ☑ Draft |

Covers: visual + UX analysis · color/type systems · components · app icon · RTL/LTR · responsive · accessibility · Customer / Craftsman / Store / Admin surfaces.

### Acceptance criteria

- UI/UX Design Specification **approved** before UI implementation  
- No UI coding until this blocker is **Closed** (approval) and Implementation Gate → **A**  
- Logo concept preserved (refinement only)  

### Tracking

| Item | State |
|------|-------|
| Official logo reference received | ☑ |
| Specs drafted | ☑ |
| Master logo binaries in `design/assets/` | ☐ |
| Hex sampled from master file | ☐ |
| Spec approved | ☐ |
| Blocker closed | ☐ |

---

## BLOCKER-002 — Stakeholder Sign-off

**Gate mapping:** G-00 (scope) + G-02 (gate)  
**Status:** Open  

### Approval checklist

#### Product

- [ ] V1 scope (Scope Baseline)  
- [ ] Included features  
- [ ] Excluded features  

#### Business

- [ ] Revenue model  
- [ ] Subscription model  
- [ ] Promotion model  
- [ ] Commission approach (admin-configurable; architecture approved)  

#### Operations

- [ ] Booking workflow  
- [ ] Provider onboarding  
- [ ] Support workflow  

#### Architecture & delivery gate

- [ ] Sign [`FINAL_SCOPE_BASELINE.md`](./FINAL_SCOPE_BASELINE.md)  
- [ ] Sign [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) when other blockers allow READY (or acknowledge current B)  
- [ ] Confirm no unauthorized V1 feature additions  

### Acceptance criteria

Formal approval recorded (role, name, date, decision) on Scope Baseline and Gate Report sign-off tables.

### Tracking

| Item | State |
|------|-------|
| Product checklist complete | ☐ |
| Business checklist complete | ☐ |
| Operations checklist complete | ☐ |
| Scope Baseline signed | ☐ |
| Gate acknowledgements recorded | ☐ |
| Blocker closed | ☐ |

---

## BLOCKER-003 — External Vendor Decisions

**Gate mapping:** G-03 · **ADR:** ADR-025  
**Status:** Open  

**Rule:** Maintain vendor-neutral **ports/adapters**. Do **not** couple business logic to vendors.

### Vendor decision checklist

#### Payment — Areeba IXOPAY (selected gateway)

| Validate | Owner | State |
|----------|-------|-------|
| Sandbox access | Eng / Business | ☐ |
| Payment.js requirements | Eng | ☐ |
| Webhook requirements | Eng | ☐ |
| Credentials process (secrets, rotation) | Eng / DevOps | ☐ |

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

### Acceptance criteria

- Required vendors selected **or** explicitly deferred with Product risk acknowledgement  
- Payment sandbox + webhook + credentials path confirmed  
- Architecture remains port/adapter based (no vendor leakage into domain services)  

---

## BLOCKER-004 — Cloud Infrastructure Approval

**Gate mapping:** G-04 · **ADR:** ADR-012 / ADR-024  
**Status:** Open  

**Rule:** Decide hosting **targets**. Do **not** modify approved architecture.

### Production infrastructure decision sheet

| Decision | Options / notes | Approved value | State |
|----------|-----------------|----------------|-------|
| Cloud provider | Per org preference; portable pattern | | ☐ |
| Region | Prefer Lebanon-proximate / approved locality | | ☐ |
| Database hosting | Managed PostgreSQL | | ☐ |
| Object storage | Align with BLOCKER-003 Storage | | ☐ |
| Redis hosting | Required (cache/queues) | | ☐ |
| Worker hosting | Async jobs / outbox consumers | | ☐ |
| Monitoring | Metrics / alerting | | ☐ |
| Logging | Centralized, retention-aware | | ☐ |
| Backup strategy | DB + critical blobs | | ☐ |
| Disaster recovery | RPO/RTO targets per ADR-024 | | ☐ |

### Environments

| Env | Purpose | Approved |
|-----|---------|----------|
| Dev | Engineering | ☐ |
| Staging | Pre-prod / finance dry-run | ☐ |
| Production | Live | ☐ |

### Acceptance criteria

Cloud provider, region, and hosting choices for DB / Redis / workers / storage / observability / backup / DR **approved** in writing. Architecture unchanged.

---

## BLOCKER-005 — Finance Lebanon Configuration

**Gate mapping:** G-05 · **ADR:** ADR-013 / ADR-026  
**Status:** Open  

**Architecture:** Approved.  
**Need:** Initial **business configuration values** only.

**Important:** Values must remain **Admin-configurable**. Do **not** hardcode.

### Required inputs

#### Commission

| Input | Value / notes | State |
|-------|---------------|-------|
| Default commission rules | | ☐ |
| Exceptions | | ☐ |

#### Cancellation

| Input | Value / notes | State |
|-------|---------------|-------|
| Time rules | | ☐ |
| Penalties | | ☐ |

#### Refund

| Input | Value / notes | State |
|-------|---------------|-------|
| Full refund rules | | ☐ |
| Partial refund rules | | ☐ |
| Manual approval cases | | ☐ |

#### Withdrawal

| Input | Value / notes | State |
|-------|---------------|-------|
| Available methods | | ☐ |
| Minimum withdrawal | | ☐ |
| Approval process | | ☐ |

#### Settlement

| Input | Value / notes | State |
|-------|---------------|-------|
| Holding period | | ☐ |
| Release rules | | ☐ |

### Acceptance criteria

- Finance signs off initial Lebanon policy content for staging  
- Documented as configuration (seed/admin), **not** constants in application code  
- Change history / audit expectations acknowledged  

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

**Gate mapping:** G-07 · **ADR:** ADR-004 / Payment architecture  
**Status:** Open  

**Constraint:** Validation only. **No production implementation.**

### Validation plan

| Area | Validate | State |
|------|----------|-------|
| Mobile payment flow | Customer app WebView / Payment.js path | ☐ |
| Payment.js compatibility | Flutter host + required browser APIs | ☐ |
| Token handling | No PAN storage; token lifecycle | ☐ |
| Secure communication | TLS, CSP/origin rules as required | ☐ |
| Webhook lifecycle | Auth, idempotency, ledger post | ☐ |
| Failure scenarios | Timeout, cancel, decline, network drop | ☐ |
| Duplicate payment prevention | Idempotency keys / gateway refs | ☐ |

### Required output

`docs/v1/PAYMENTJS_MOBILE_INTEGRATION_READINESS_REPORT.md` (or equivalent named report)

Must state: Pass / Conditional Pass / Fail, with risks and follow-ups.

### Acceptance criteria

- Readiness report published and accepted by Eng Lead + Architect  
- Blocking technical failures resolved **or** Product accepts documented risk with mitigation date  

### Dependencies

- BLOCKER-003 Payment sandbox / credentials path  

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
