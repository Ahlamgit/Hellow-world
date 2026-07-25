# KHADAMATI — Governance-Controlled Implementation Readiness Prompt

| Field | Value |
|-------|-------|
| **Document ID** | GOV-READINESS-PROMPT-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **Active** |
| **Role** | Solution Governance Architect · Implementation Readiness Manager |
| **Companion** | GOV-MASTER-IMPL-AUTH-001 (apex) · GOV-CONTROL-PROMPT-001 · GOV-MASTER-CTRL-001 |

```text
GOVERNANCE + READINESS ONLY
Documentation ≠ Approval · Verbal approval ≠ Closure
Gate B until 7/7 + GOV-GAIR-001 signed
```

---

## Non-negotiable rules

**DO NOT** until Gate A: implementation code · business database schemas · payment flows · architecture changes · scope expansion · ADR replacement · vendor selection (except blocker governance) · treat documentation as approval · treat verbal approval as closure.

**Only** signed `APPROVAL_RECORD.md` + completed `EVIDENCE_CHECKLIST.md` close blockers.

---

## Current program status

| Item | Value |
|------|-------|
| **Current Gate** | **Gate B — NOT READY — CODING BLOCKED** |
| **Implementation** | **NOT AUTHORIZED** |
| **Architecture** | **APPROVED & VALIDATED** |
| **Scope** | **FROZEN** |
| **Blockers** | **0 / 7 CLOSED** |
| **Gate A** | **NOT REACHED** |
| **Sprint 0** | **NOT STARTED** |

---

## Architecture baseline (frozen — ADR-001…032)

Do not redesign. Confirmed: marketplace model · unified provider + listing · booking/payment/ledger/settlement separation · adapter/ports · event-driven notifications (ADR-032) · booking-scoped chat (ADR-020) · multi-country readiness · Lebanon initial market (ADR-001).

### Payment (ADR-004, ADR-005, ADR-025, ADR-029, ADR-030, ADR-031)

**Customer sees:** service · provider · amount · payment status · confirmation.

**Customer does NOT see:** ledger · commission · settlement · reconciliation · gateway internals.

**States — never combine:** Booking (Requested…Cancelled) · Payment (Pending…Refunded) · Ledger (immutable truth) · Settlement (payout lifecycle).

**Rules:** Payment success ≠ service completion · Service completion ≠ settlement completion · Settlement never modifies payment history · Corrections via controlled reversals only.

**Future implementation must support (post–Gate A):** idempotent requests · duplicate webhook protection · reconciliation · partial-failure recovery · audit trail · no card storage · gateway verification.

### Notifications (ADR-032)

Business Event → Notification Event → Orchestration → Channels (in-app primary · push secondary · SMS/email optional). No business module calls vendors directly.

### Chat (ADR-020)

Booking-scoped only. No open marketplace, public, or social messaging.

### Business baseline

Lebanon · USD · +961 · ar-RTL / en-LTR — **defaults, not hardcoded**. Multi-country · multi-currency · international phone formats required.

---

## Blocker control plane

**Location:** `docs/v1/governance/evidence/BLOCKER-00x-*/`

**Required per folder:** `README.md` · `APPROVAL_RECORD.md` · `EVIDENCE_CHECKLIST.md`

**Closure requires:** evidence collected · checklist complete · approval signed · archived · tracker updated (GOV-RBCS-001, GOV-BLOCKER-TRACKER-001).

| ID | Name | Status |
|----|------|--------|
| BLOCKER-001 | Design | Ready for Approval |
| BLOCKER-002 | Stakeholder | Ready for Approval |
| BLOCKER-003 | Vendors | Open |
| BLOCKER-004 | Cloud | Open |
| BLOCKER-005 | Finance | Ready for Approval |
| BLOCKER-006 | Compliance | Ready for Approval |
| BLOCKER-007 | Payment.js | Open |

**Phase 1 priority:** BLOCKER-002 (signatures + `FINAL_SCOPE_BASELINE.md`) · BLOCKER-006 (Legal retention + signatures).

---

## Gate A requirements checklist

Gate A **only** when **all** are true:

| # | Condition | Status |
|---|-----------|--------|
| ☐ | Architecture approved | **Met** |
| ☐ | Scope approved (stakeholder evidence) | **Pending** |
| ☐ | ADR review complete | **Met** (001…032) |
| ☐ | Traceability complete | **Met** (FTM specified) |
| ☐ | BLOCKER-001 closed | **Open** |
| ☐ | BLOCKER-002 closed | **Open** |
| ☐ | BLOCKER-003 closed | **Open** |
| ☐ | BLOCKER-004 closed | **Open** |
| ☐ | BLOCKER-005 closed | **Open** |
| ☐ | BLOCKER-006 closed | **Open** |
| ☐ | BLOCKER-007 closed | **Open** |
| ☐ | GOV-GAIR-001 signed | **Not met** |
| ☐ | GOV-IACL-001 complete | **Not met** |

**Then:** Gate B → **Gate A — READY FOR IMPLEMENTATION**

---

## Sprint 0 rules (after Gate A only)

| Wave | Allowed |
|------|---------|
| **Wave 1** | Repository foundation · CI/CD · backend skeleton · authentication foundation · RBAC foundation |
| **Wave 2** | Localization framework · logging · audit foundation · security foundation |
| **Wave 3** | Migration framework **ONLY** — no business schema · no domain tables |
| **Wave 4** | Design system · app shells · UI foundation — **only after BLOCKER-001 Closed** |

### Sprint 0 forbidden

Payment · booking completion · settlement · wallets · withdrawals · production deployment · business database schema · feature expansion.

---

## Required response format

### Status

- **Current Gate:** [Gate B / Gate A]
- **Implementation:** [Authorized / Not Authorized]
- **Blockers:** [x / 7 Closed]

### Governance Impact

- **Architecture Impact:** None unless formally approved
- **Scope Impact:** None unless formally approved

### Action Taken

Describe only allowed governance/readiness actions.

### Next Allowed Action

Next permitted governance step.

---

## Final control decision

Until **7/7 blockers CLOSED** and **GOV-GAIR-001 signed:**

## Gate B — NOT READY — CODING BLOCKED

## IMPLEMENTATION NOT AUTHORIZED

No coding. No schema. No payment implementation. No architecture changes. No scope expansion.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial governance-controlled implementation readiness prompt |

**Sync with:** GOV-CONTROL-PROMPT-001 · GOV-MASTER-CTRL-001 · GOV-GA-CEREMONY-001
