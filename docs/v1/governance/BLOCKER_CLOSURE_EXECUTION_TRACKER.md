# KHADAMATI — Blocker Closure Execution Tracker

| Field | Value |
|-------|-------|
| **Document ID** | GOV-BLOCKER-TRACKER-001 |
| **Version** | 1.4 |
| **Date** | 2026-07-25 |
| **Owner** | Program Execution Manager |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Blockers closed** | **0 / 7** |
| **Status** | **Active — operational tracker** |

```text
This document does NOT authorize coding.
It only tracks evidence collection, approval, and closure toward Gate A.
```

---

## 1. Purpose

This tracker is the **operational working document** for closing BLOCKER-001 through BLOCKER-007 before Gate A authorization.

| It does | It does not |
|---------|-------------|
| Track evidence, approvals, and closure status per blocker | Authorize production code, schema, APIs, or UI |
| Record owners, phases, and escalation | Select vendors or deploy infrastructure |
| Support Gate A transition preparation | Modify architecture, scope, or ADRs |

**Companion documents:**

| Document | Role |
|----------|------|
| **GOV-BLOCKER-TRACKER-001** (this tracker) | Day-to-day execution tracking |
| GOV-RBCS-001 | Official status register (update on closure) |
| GOV-BCEP-001 | Blocker requirements detail |
| GOV-BEMF-001 | Evidence rules & approval workflow |
| GOV-GATC-001 | Phase execution & Gate A ceremony |
| GOV-GA-AUDIT-001 | Independent readiness audit |

**Phase 1 readiness:** `PHASE_1_BLOCKER_APPROVAL_READINESS_REPORT.md` (GOV-P1-READINESS-001)  
**Phase 1 closure review:** `PHASE_1_CLOSURE_EXECUTION_REPORT.md` (GOV-P1-CLOSURE-001)  
**Phase 1 approval finalization:** `PHASE_1_APPROVAL_FINALIZATION_REPORT.md` (GOV-P1-FINAL-001) — **0 / 2 Phase 1 closed**

---

## 2. Master blocker dashboard

**Last updated:** 2026-07-25 · **Progress:** 0 / 7 **Closed**

| ID | Name | Owner | Current status | Required evidence | Gate impact |
|----|------|-------|----------------|-------------------|-------------|
| **BLOCKER-001** | Design | Design Lead + Product Owner | **Ready for Approval** | Approved logo assets · Design system approval · UI/UX approval signatures · `DESIGN_APPROVAL_SIGNOFF_v1.0` | Blocks **all UI** (ADR-023); Sprint 0 A7–A8 |
| **BLOCKER-002** | Stakeholder | Program Sponsor | **Ready for Approval** | Product approval · Business approval · Operations approval · `STAKEHOLDER_APPROVAL_REGISTER_v1.0` | Blocks **program authorization** |
| **BLOCKER-003** | Vendors | Integration Lead | **Open** | Vendor evaluation · Selected vendors · Contracts/agreements · Sandbox readiness · `VENDOR_READINESS_DOSSIER_v1.0` | Blocks **integrations** (payment, SMS, email, maps, OCR, storage) |
| **BLOCKER-004** | Cloud | Technical Architect + DevOps Lead | **Open** | Hosting decision · Budget approval · Backup strategy · RPO/RTO approval · `CLOUD_READINESS_DECISION_RECORD_v1.0` | Blocks **environment planning** (decisions only — no provisioning) |
| **BLOCKER-005** | Finance | Finance + Business Operations | **Ready for Approval** | Commission rules · Subscription rules · Refund policy · Settlement rules · Finance signatures · `FINANCE_RULE_MATRIX_v1.0` | Blocks **settlement/payment rules**; input to BLOCKER-007 |
| **BLOCKER-006** | Compliance | Legal / Compliance Officer | **Ready for Approval** | Retention approval · Data governance approval · Legal/compliance signatures · `COMPLIANCE_APPROVAL_PACK_v1.0` | Blocks **data lifecycle / domain schema** |
| **BLOCKER-007** | Payment.js validation | Technical Lead + Finance Ops | **Open** | Sandbox validation · Mobile validation · Webhook validation · Finance acceptance · `PAYMENT_JS_VALIDATION_REPORT_v1.0` | Blocks **booking payment**; excluded from Sprint 0 |

### Evidence package paths

Each blocker folder includes **`README.md`**, **`APPROVAL_RECORD.md`**, and **`EVIDENCE_CHECKLIST.md`** (standard evidence trio). See `evidence/README.md`.

| ID | Folder | Package (if prepared) | Standard trio |
|----|--------|----------------------|---------------|
| BLOCKER-001 | `evidence/BLOCKER-001-design/` | `DESIGN_APPROVAL_PACKAGE.md` | ✓ |
| BLOCKER-002 | `evidence/BLOCKER-002-stakeholder/` | `STAKEHOLDER_APPROVAL_PACKAGE.md` | ✓ |
| BLOCKER-003 | `evidence/BLOCKER-003-vendors/` | — | ✓ |
| BLOCKER-004 | `evidence/BLOCKER-004-cloud/` | — | ✓ |
| BLOCKER-005 | `evidence/BLOCKER-005-finance/` | `FINANCE_POLICY_APPROVAL_PACKAGE.md` | ✓ |
| BLOCKER-006 | `evidence/BLOCKER-006-compliance/` | `COMPLIANCE_APPROVAL_PACKAGE.md` | ✓ |
| BLOCKER-007 | `evidence/BLOCKER-007-payment/` | — | ✓ |

---

## 2.1 Per-blocker execution detail

### BLOCKER-001 — Design

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Approved logo assets | Package — assets pending | ☐ |
| Design system approval | Package prepared | ☐ |
| UI/UX approval signatures | §9 pending | ☐ |
| Closure artifact `DESIGN_APPROVAL_SIGNOFF_v1.0` | — | ☐ |

**Tracker status:** Ready for Approval · **Closed:** ☐

---

### BLOCKER-002 — Stakeholder

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Product Owner approval | Package §6 | ☐ |
| Business Owner approval | Package §6 | ☐ |
| Operations Owner approval | Package §6 | ☐ |
| Closure artifact `STAKEHOLDER_APPROVAL_REGISTER_v1.0` | — | ☐ |

**Tracker status:** Ready for Approval · **Closed:** ☐

---

### BLOCKER-003 — Vendors

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Vendor evaluation (per integration) | ☐ | ☐ |
| Selected vendors documented | ☐ | ☐ |
| Contracts/agreements referenced | ☐ | ☐ |
| Sandbox readiness (payment, SMS, email, maps, OCR, storage) | ☐ | ☐ |
| Closure artifact `VENDOR_READINESS_DOSSIER_v1.0` | — | ☐ |

**Tracker status:** Open · **Closed:** ☐

---

### BLOCKER-004 — Cloud

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Hosting decision | ☐ | ☐ |
| Budget approval | ☐ | ☐ |
| Backup strategy | ☐ | ☐ |
| RPO/RTO approval | ☐ | ☐ |
| Closure artifact `CLOUD_READINESS_DECISION_RECORD_v1.0` | — | ☐ |

**Tracker status:** Open · **Closed:** ☐

---

### BLOCKER-005 — Finance

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Commission rules (values approved) | Package — pending | ☐ |
| Subscription rules | Package — pending | ☐ |
| Refund policy | Package — pending | ☐ |
| Settlement rules | Package — pending | ☐ |
| Finance / Business signatures | Pending | ☐ |
| Closure artifact `FINANCE_RULE_MATRIX_v1.0` | — | ☐ |

**Tracker status:** Ready for Approval · **Closed:** ☐

---

### BLOCKER-006 — Compliance

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Retention approval (legal values) | Package — pending | ☐ |
| Data governance approval | Package prepared | ☐ |
| Legal/compliance signatures | §10 pending | ☐ |
| Closure artifact `COMPLIANCE_APPROVAL_PACK_v1.0` | — | ☐ |

**Tracker status:** Ready for Approval · **Closed:** ☐

---

### BLOCKER-007 — Payment.js

| Evidence item | On file | Approved |
|---------------|---------|----------|
| Sandbox validation | ☐ | ☐ |
| Mobile validation (Android/iOS) | ☐ | ☐ |
| Webhook validation | ☐ | ☐ |
| Finance acceptance | ☐ | ☐ |
| Closure artifact `PAYMENT_JS_VALIDATION_REPORT_v1.0` | — | ☐ |

**Dependencies:** BLOCKER-003 (payment sandbox) · BLOCKER-005 (ledger/finance expectations)

**Tracker status:** Open · **Closed:** ☐

---

## 3. Definition of done

A blocker may be marked **CLOSED** only when **all** criteria are met:

| # | Criterion |
|---|-----------|
| 1 | **Required evidence exists** per §2 and GOV-BCEP-001 §2 |
| 2 | **Evidence reviewed** by designated approver(s) |
| 3 | **Required approvals signed** in `evidence/BLOCKER-00x-*/APPROVAL_RECORD.md` (Decision: **Approved**) |
| 4 | **`EVIDENCE_CHECKLIST.md`** complete for the blocker |
| 5 | **Approval record archived** under `governance/evidence/BLOCKER-00x-*/` |
| 6 | **Closure artifact** filed with version ID |
| 7 | **This tracker updated** + GOV-RBCS-001 within **1 business day** (GOV-BEMF-001 E-005) |

**No verbal approval accepted** (GOV-BEMF-001 E-002).

---

## 4. Closure sequence

Execute per GOV-GATC-001. Update this tracker as each blocker closes.

### Phase 1 — Governance alignment

**Readiness validation:** GOV-P1-READINESS-001 · **Closure review:** GOV-P1-CLOSURE-001 (2026-07-25) — BLOCKER-002 **READY**, BLOCKER-006 **BLOCKED**; **0 / 2 closed**

| Close | Owner | Distribution | Signatures | Status |
|-------|-------|--------------|------------|--------|
| **BLOCKER-002** Stakeholder | Program Sponsor | Package ready | §6 pending (PO, BO, Ops) | ☐ Not closed |
| **BLOCKER-006** Compliance | Legal / Compliance | Package ready | §10 pending; §2 retention values pending | ☐ Not closed |

**Phase 1 complete:** ☐

---

### Phase 2 — Design & finance

| Close | Owner | Status |
|-------|-------|--------|
| **BLOCKER-001** Design | Design Lead + PO | ☐ Not closed |
| **BLOCKER-005** Finance | Finance + Business | ☐ Not closed |

**Phase 2 complete:** ☐

---

### Phase 3 — Platform decisions

| Complete | Owner | Status |
|----------|-------|--------|
| **BLOCKER-003** Vendors | Integration Lead | ☐ Not closed |
| **BLOCKER-004** Cloud | Architect + DevOps | ☐ Not closed |

**Phase 3 complete:** ☐

---

### Phase 4 — Payment validation

| Complete | Owner | Status |
|----------|-------|--------|
| **BLOCKER-007** Payment.js | Tech Lead + Finance Ops | ☐ Not closed |

**Prerequisites:** Phase 2 BLOCKER-005 + Phase 3 BLOCKER-003

**Phase 4 complete:** ☐

---

### Then — Gate A review

When **all phases complete** (7 / 7 Closed):

1. Complete `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` (GOV-IACL-001)
2. Gate A steering review (GOV-GATC-001 §9)
3. Sign `GATE_A_IMPLEMENTATION_AUTHORIZATION_RECORD.md` §8 (GOV-GAIR-001)
4. Update `FINAL_IMPLEMENTATION_GATE_REPORT.md` to Gate A

---

## 5. Ownership matrix

| Role | Responsibility |
|------|----------------|
| **Program Execution Manager** | Maintain this tracker; weekly status; escalation coordination |
| **Program Sponsor** | BLOCKER-002 accountable; Gate A decision |
| **Product Owner** | Scope confirmation; BLOCKER-001 co-sign; stakeholder content |
| **Business Owner** | BLOCKER-002 / BLOCKER-005 business decisions |
| **Design Owner (Design Lead)** | BLOCKER-001 evidence and signatures |
| **Finance Owner** | BLOCKER-005 values; BLOCKER-007 finance co-sign |
| **Compliance Owner (Legal)** | BLOCKER-006 retention and governance signatures |
| **Technical Architect** | Architecture gate; BLOCKER-004 accountable |
| **Integration Lead** | BLOCKER-003 dossier; BLOCKER-007 technical lead |
| **DevOps Owner** | BLOCKER-004 cloud record support |
| **Program Governance Manager** | GOV-RBCS-001 integrity; evidence framework compliance |

---

## 6. Risk escalation rules

If a blocker is **delayed** beyond agreed target:

| Step | Action | Owner |
|------|--------|-------|
| 1 | **Identify owner** and root cause (missing evidence, approver unavailable, dependency) | Blocker owner |
| 2 | **Record impact** on Gate A date and downstream blockers (e.g. 007 blocked by 003) | Program Execution Manager |
| 3 | **Escalate** to Program Sponsor at steering if >5 business days slip or on critical path | Program Execution Manager |
| 4 | **Update plan** — revised target date in tracker; no gate bypass | Program Governance |

**Critical path:** 002/006 → 001/005 → 003/004 → 007 → Gate A

**No gate bypass:** Architecture validation does not substitute for blocker closure (GOV-GA-AUDIT-001).

---

## 7. Gate A transition checklist

| Requirement | Status | Evidence |
|-------------|--------|----------|
| BLOCKER-001 Closed | **PENDING** | — |
| BLOCKER-002 Closed | **PENDING** | — |
| BLOCKER-003 Closed | **PENDING** | — |
| BLOCKER-004 Closed | **PENDING** | — |
| BLOCKER-005 Closed | **PENDING** | — |
| BLOCKER-006 Closed | **PENDING** | — |
| BLOCKER-007 Closed | **PENDING** | — |
| **7 / 7 blockers closed** | **PENDING** (0 / 7) | This tracker + GOV-RBCS-001 |
| **Evidence archived** | **PENDING** | `governance/evidence/` per GOV-BEMF-001 |
| **GOV-IACL-001 complete** | **PENDING** | Authorization checklist |
| **Gate authorization signed** | **PENDING** | GOV-GAIR-001 §8 |
| **GOV-FIGR-001 → Gate A** | **PENDING** | Gate report |
| **Sprint 0 approved** | **PENDING** | GOV-S0FC-001 kickoff after Gate A |

---

## 8. Current recommendation

| Field | Value |
|-------|-------|
| **Current gate** | **REMAIN GATE B — NOT READY — CODING BLOCKED** |
| **Reason** | **0 / 7 blockers closed** — four Ready for Approval awaiting signatures/values; three In Preparation |
| **Architecture** | Approved & validated — does not override blocker requirement |
| **Implementation** | **NOT AUTHORIZED** |

### Next milestone

```text
7 / 7 blockers CLOSED
  → Gate A ceremony (GOV-GATC-001 §9)
  → GOV-GAIR-001 §8 signed
  → Sprint 0 authorization (GOV-S0FC-001)
```

### Immediate execution focus (Phase 1 + 2)

1. Sign and archive BLOCKER-002 stakeholder package  
2. Obtain compliance retention values + sign BLOCKER-006  
3. Sign and archive BLOCKER-001 design package + assets  
4. Approve finance values + sign BLOCKER-005  

---

## 9. Tracker change log

| Date | Blocker | Change | Updated by |
|------|---------|--------|------------|
| 2026-07-25 | — | Tracker initialized — 0 / 7 closed | Program Execution Manager |

*Add a row within 1 business day of any blocker status change.*

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial operational blocker closure execution tracker |
| 1.1 | 2026-07-25 | Standard evidence trio (`README.md`, `APPROVAL_RECORD.md`, `EVIDENCE_CHECKLIST.md`) in all seven blocker folders |
| 1.2 | 2026-07-25 | Phase 1 approval readiness report (GOV-P1-READINESS-001); Phase 1 not complete |
| 1.3 | 2026-07-25 | Phase 1 closure execution report (GOV-P1-CLOSURE-001); 0/7 closed |
| 1.4 | 2026-07-25 | Phase 1 approval finalization report (GOV-P1-FINAL-001); Phase 2 preview |

**Sync with:** GOV-RBCS-001 on every closure event.
