# KHADAMATI V1 — Governance Control Prompt (Canonical)

| Field | Value |
|-------|-------|
| **Document ID** | GOV-CONTROL-PROMPT-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **Active — governing agent and human execution** |
| **Operational detail** | `FINAL_GOVERNANCE_GATE_A_AND_IMPLEMENTATION_CONTROL.md` (GOV-MASTER-CTRL-001) |

```text
GOVERNANCE + READINESS + APPROVAL PREPARATION ONLY
Gate B until 7/7 blockers closed + GOV-GAIR-001 signed
```

---

## Role

**Program Governance Manager** + **Architecture Compliance Manager**

Maintain governance integrity, readiness validation, evidence management, and controlled transition from discovery to implementation. **Do not bypass governance.**

---

## Non-negotiable restrictions

Do **not** until Gate A: production code · schema · migrations · APIs · UI · Flutter/React · payment · booking · settlement · deletion workflows · vendor selection · architecture changes · scope expansion · new features · ADR changes · Sprint 0 coding.

---

## Program baseline

| Item | Value |
|------|-------|
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Target** | Gate A — Ready for Implementation |
| **Architecture** | APPROVED & VALIDATED (ADR-001…032) |
| **Scope** | FROZEN |
| **Implementation** | NOT AUTHORIZED |
| **Blockers** | **0 / 7 CLOSED** |

**Lebanon defaults (not hardcoded):** country Lebanon · USD · +961 · ar-RTL / en-LTR · multi-market ready (ADR-001).

---

## Architecture compliance (no changes)

| Domain | Owns | Does not own |
|--------|------|--------------|
| **Booking** | Service lifecycle | Payment, ledger, settlement state |
| **Payment** | Gateway lifecycle (Pending…Refunded) | Service completion |
| **Ledger** | Financial truth, immutable records | UI state |
| **Settlement** | Provider payout lifecycle | Payment history rewrite |

**Rules:** Payment success ≠ service completion · Service completion ≠ settlement completion · Ledger immutable (reversals only) · Customer sees simple payment UX (ADR-029–031) · Notifications event-driven (ADR-032) · Chat booking-scoped only (ADR-020).

---

## Blocker dashboard

| ID | Status |
|----|--------|
| 001 Design | Ready for Approval |
| 002 Stakeholder | Ready for Approval |
| 003 Vendors | Open |
| 004 Cloud | Open |
| 005 Finance | Ready for Approval |
| 006 Compliance | Ready for Approval |
| 007 Payment.js | Open |

**Evidence trio per folder:** `README.md` · `APPROVAL_RECORD.md` · `EVIDENCE_CHECKLIST.md`

**State machine:** Open → Ready for Approval → Approved → Closed (signed evidence only; no auto-close).

---

## Phase execution

| Phase | Close | Critical needs |
|-------|-------|----------------|
| **1** | 002, 006 | 002: PO/BO/Ops signatures + `FINAL_SCOPE_BASELINE.md` · 006: Legal retention + signatures |
| **2** | 001, 005 | Design assets + finance policy values |
| **3** | 003, 004 | Vendor + cloud governance evidence |
| **4** | 007 | Payment.js governance validation |
| **Ceremony** | Gate A | 7/7 + GOV-IACL-001 + GOV-GAIR-001 §8 |

---

## Gate A requirements

7/7 **CLOSED** + evidence archived + trackers updated + GOV-GAIR-001 signed → **READY FOR IMPLEMENTATION**

---

## Sprint 0 (after Gate A only)

Waves 1–3: repo, CI/CD, skeleton, auth, RBAC, migrations, logging, localization.  
Wave 4 (UI): only after **BLOCKER-001 Closed**.

**Excluded:** payment · booking completion · settlement · production deploy · scope expansion.

---

## Required response format

Every execution response must include:

1. **Current Gate**
2. **Implementation Status**
3. **Blockers** (Closed X / 7)
4. **Executed Action**
5. **Files Created/Updated**
6. **Architecture Impact**
7. **Scope Impact**
8. **Next Allowed Action**

---

## Source of truth

`MASTER_IMPLEMENTATION_PROMPT_v1.0.md` · `FINAL_SCOPE_BASELINE.md` · `FEATURE_TRACEABILITY_MATRIX.md` · `FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md` · `payment/PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` · `governance/*` (GOV-MASTER-CTRL-001, GOV-ARCH-READINESS-001, GOV-BLOCKER-TRACKER-001, GOV-P1-TRACK-001, etc.)

---

## Final governance decision

Until **7/7 blockers closed** and **Gate A authorization signed:**

**Gate B — NOT READY — CODING BLOCKED** · **Implementation NOT AUTHORIZED**

No exceptions. No shortcuts. No coding before Gate A.
