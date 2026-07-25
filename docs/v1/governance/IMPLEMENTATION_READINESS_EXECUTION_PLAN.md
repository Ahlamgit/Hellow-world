# KHADAMATI — Implementation Readiness Execution Plan

| Field | Value |
|-------|-------|
| **Document ID** | GOV-IREP-001 |
| **Version** | 1.2 |
| **Status** | Active — Gate B phase; Sprint 0 planned |
| **Last updated** | 2026-07-25 |
| **Prepared by** | Program Readiness Manager |

---

## Purpose

This plan defines **what the program may execute now** (readiness phase) versus **what remains blocked** until Gate A. It supersedes any informal implementation start.

**Companion document:** `BLOCKER_CLOSURE_EXECUTION_PLAN.md` (GOV-BCEP-001 v1.0)

---

## Current Program Position

| Dimension | Status |
|-----------|--------|
| Gate | **B — NOT READY — CODING BLOCKED** |
| Scope | **FROZEN** |
| Architecture | **APPROVED** |
| Engineering standards | **DEFINED** |
| Implementation | **BLOCKED** |

---

## Phase Model

### Phase 0 — Readiness (CURRENT)

**Objective:** Close all implementation blockers.

**Duration:** Until Gate A declared.

**Activities:**

| Activity | Owner | Output |
|----------|-------|--------|
| Design approval workflow | Design Lead | `DESIGN_APPROVAL_SIGNOFF_v1.0` |
| Stakeholder sign-off | Program Sponsor | `STAKEHOLDER_APPROVAL_REGISTER_v1.0` |
| Vendor readiness dossier | Integration Lead | `VENDOR_READINESS_DOSSIER_v1.0` |
| Cloud decision record | Technical Architect | `CLOUD_READINESS_DECISION_RECORD_v1.0` |
| Finance rule matrix | Finance | `FINANCE_RULE_MATRIX_v1.0` |
| Compliance pack | Legal / Compliance | `COMPLIANCE_APPROVAL_PACK_v1.0` |
| Payment.js validation | Technical Lead | `PAYMENT_JS_VALIDATION_REPORT_v1.0` |

**Prohibited in Phase 0:**

- Sprint execution for product delivery
- Code, schema, UI, deployment
- Vendor selection as an engineering task
- Architecture or scope changes

**Status tracking:** `READINESS_BLOCKER_CLOSURE_STATUS.md` (weekly minimum)

---

### Phase 1 — Gate A Authorization (GATED)

**Entry criteria:** All seven blockers **Closed**; Gate A checklist complete in `FINAL_IMPLEMENTATION_GATE_REPORT.md`.

**Objective:** Formal authorization to begin implementation.

**Activities:**

- Gate A steering review
- Implementation authorization record
- Sprint 0 planning (environment setup per approved cloud record — still no ad-hoc provisioning)
- Backlog activation against `FEATURE_TRACEABILITY_MATRIX.md`

**Exit criteria:** Signed Gate A; `FINAL_IMPLEMENTATION_GATE_REPORT.md` updated to **READY FOR IMPLEMENTATION**.

---

### Phase 2 — Sprint 0 Foundation (POST Gate A)

**Entry criteria:** Gate A declared; `SPRINT_0_FOUNDATION_CHARTER.md` (GOV-S0FC-001) active.

**Objective:** Platform foundations per Sprint 0 allowed list — **not** product features.

**Authorized workstreams:** See `SPRINT_0_FOUNDATION_CHARTER.md` § Allowed.

**Gate dependencies within Sprint 0:**

- A1–A6, A9–A10: After Sprint 0 kickoff (Gate A)
- A7–A8 (design system, app shells): **BLOCKER-001 Closed** (ADR-023)

**Prohibited in Sprint 0:** Payment, booking completion, settlement, production deployment, feature expansion.

**Exit criteria:** Sprint 0 exit checklist in charter.

---

### Phase 3 — Feature Implementation (POST Sprint 0)

**Not authorized until Sprint 0 exit criteria met.**

Planned sequence (subject to Gate A approval):

| Stream | Prerequisite blockers | Notes |
|--------|---------------------|-------|
| Core platform & auth | Gate A + compliance (retention baseline) | Per ADRs |
| Admin configuration (finance rules) | BLOCKER-005 closed | No hardcoded rules |
| Integrations (adapters) | BLOCKER-003 closed | Ports/adapters only |
| Customer / Craftsman / Store UI | BLOCKER-001 closed | No UI before design |
| Payment & booking payment | BLOCKER-007 closed | After sandbox validation |
| Settlement & ledger | BLOCKER-005 closed | Admin-configurable rules |

---

## Execution Dependency Order (Readiness)

Aligned with `BLOCKER_CLOSURE_EXECUTION_PLAN.md` §3:

```
Phase 1 (parallel):  BLOCKER-002, BLOCKER-006
Phase 2 (parallel):  BLOCKER-001, BLOCKER-005
Phase 3 (parallel):  BLOCKER-004, BLOCKER-003
Phase 4 (sequential): BLOCKER-007
```

---

## Roles and Cadence

| Role | Readiness responsibility |
|------|--------------------------|
| Program Readiness Manager | Gate status, document integrity, steering packs |
| Program Sponsor | BLOCKER-002 escalation |
| Product Owner | Scope confirmation, design approval co-sign |
| Technical Architect | Architecture compliance, BLOCKER-004 |
| Design Lead | BLOCKER-001 evidence |
| Finance | BLOCKER-005, payment validation co-sign |
| Legal / Compliance | BLOCKER-006 |
| Technical / Integration Lead | BLOCKER-003, BLOCKER-007 |

| Meeting | Cadence |
|---------|---------|
| Blocker stand-up | Weekly |
| Steering / gate review | Bi-weekly |
| Gate A decision | Ad hoc when 7/7 closed |

---

## Implementation Authorization Rules (Reminder)

### After Gate A — Allowed

- Sprint planning
- Database implementation
- API implementation
- UI implementation (if BLOCKER-001 closed)
- Testing per engineering standards

### Always prohibited without change control

- Unapproved scope additions
- Hardcoded financial rules
- Vendor coupling outside adapters
- Architecture changes without ADR

---

## Final Recommendation

| Item | Determination |
|------|---------------|
| **Current phase** | Phase 0 — Readiness only |
| **Current gate** | **B — NOT READY — CODING BLOCKED** |
| **Required action** | Execute blocker closure per `BLOCKER_CLOSURE_EXECUTION_PLAN.md` |
| **Implementation start** | **Not authorized** |

---

## Document History

| Version | Date | Change |
|---------|------|--------|
| 1.0 | — | Initial readiness execution plan |
| 1.1 | 2026-07-25 | Refocused on Phase 0 blocker closure; Phase 2 explicitly gated |
| 1.2 | 2026-07-25 | Added Sprint 0 Foundation Charter (GOV-S0FC-001); Phase 2/3 split |
