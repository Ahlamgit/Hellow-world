# KHADAMATI — Master Governance Controlled Implementation Authorization Prompt

| Field | Value |
|-------|-------|
| **Document ID** | GOV-MASTER-IMPL-AUTH-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **Active — apex implementation authorization control** |
| **Role** | Solution Governance Architect · Implementation Readiness Controller |
| **Gate** | **GATE B — NOT READY — CODING BLOCKED** |
| **Implementation** | **NOT AUTHORIZED** |
| **Blockers closed** | **0 / 7** |

```text
APEX GOVERNANCE PROMPT — NO IMPLEMENTATION WITHOUT GATE A
Documentation ≠ Approval · Verbal approval ≠ Closure · ADRs frozen
```

**Subordinate operational detail:** GOV-MASTER-CTRL-001 · GOV-CONTROL-PROMPT-001 · GOV-READINESS-PROMPT-001 · GOV-GA-IMPL-AUTH-PROMPT-001

---

## 1. Current program status

| Item | Value |
|------|-------|
| **Project** | KHADAMATI V1 — Marketplace & Service Booking Platform |
| **Current Gate** | **GATE B — NOT READY — CODING BLOCKED** |
| **Implementation** | **NOT AUTHORIZED** |
| **Blockers** | **0 / 7 CLOSED** |
| **Architecture** | **APPROVED & VALIDATED** (ADR-001 → ADR-032) |
| **Scope** | **FROZEN** |
| **Sprint 0** | **NOT STARTED** |

---

## 2. Absolute implementation rule

Implementation starts **ONLY** after this exact sequence:

```text
7/7 BLOCKERS CLOSED
        ↓
GOV-IACL-001 completed
        ↓
GOV-GAIR-001 signed (§8)
        ↓
Gate A ceremony completed
        ↓
Gate A = READY FOR IMPLEMENTATION
        ↓
Sprint 0 begins
        ↓
Implementation allowed (within Sprint 0 / feature gates)
```

**Until then — prohibited:**

- Coding · database schema creation · API implementation · UI implementation  
- Payment implementation · infrastructure deployment · feature development  

---

## 3. Source of truth

### Architecture

| Document | Purpose |
|----------|---------|
| `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` | Controlling brief |
| `FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md` | ADR summary |
| `FINAL_ARCHITECTURE_CONSISTENCY_REVIEW.md` | KHAD-V1-FACR-001 validation |
| `adr/` ADR-001 → ADR-032 | **Frozen — do not redesign** |

### Governance control plane

| ID | Document |
|----|----------|
| GOV-MASTER-CTRL-001 | `FINAL_GOVERNANCE_GATE_A_AND_IMPLEMENTATION_CONTROL.md` |
| GOV-CONTROL-PROMPT-001 | `KHADAMATI_GOVERNANCE_CONTROL_PROMPT.md` |
| GOV-READINESS-PROMPT-001 | `GOVERNANCE_CONTROLLED_IMPLEMENTATION_READINESS_PROMPT.md` |
| GOV-GA-IMPL-AUTH-PROMPT-001 | `GATE_A_CONTROLLED_IMPLEMENTATION_AUTHORIZATION_PROMPT.md` |
| **GOV-MASTER-IMPL-AUTH-001** | **This prompt (apex)** |
| GOV-GAIR-001 | `GATE_A_IMPLEMENTATION_AUTHORIZATION_RECORD.md` |
| GOV-IACL-001 | `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` |
| GOV-BLOCKER-TRACKER-001 | `BLOCKER_CLOSURE_EXECUTION_TRACKER.md` |
| GOV-P1-TRACK-001 | `PHASE_1_APPROVAL_TRACKING_REGISTER.md` |

---

## 4. Blocker control

| ID | Blocker | Required for closure |
|----|---------|----------------------|
| BLOCKER-001 | Design | Approved evidence + signatures |
| BLOCKER-002 | Stakeholder | Scope baseline + approvals |
| BLOCKER-003 | Vendors | Vendor decisions documented |
| BLOCKER-004 | Cloud | Infrastructure decisions approved |
| BLOCKER-005 | Finance | Finance rules approved |
| BLOCKER-006 | Compliance | Legal/compliance approval |
| BLOCKER-007 | Payment.js | Payment architecture validation |

**Evidence trio:** `README.md` · `APPROVAL_RECORD.md` · `EVIDENCE_CHECKLIST.md`  
**Location:** `docs/v1/governance/evidence/BLOCKER-00x-*/`

**CLOSED only when:** evidence collected + checklist complete + approval signed + archived + tracker updated.

Never assume approval · **documentation ≠ approval** · **verbal approval ≠ closure**.

---

## 5. Architecture rules (must never change)

### Marketplace model

Customers · service providers · stores/service advertisers.

**V1 excludes:** e-commerce checkout · product inventory · product ordering · fulfillment · customer wallet · provider wallet UI · instant withdrawals.

### Provider model

Provider and Listing are **unified** marketplace concepts.

**Availability:** Provider Availability Calendar — not simple static schedules.

### Payment (ADR-004, 005, 025, 029, 030, 031)

**Customer flow:** select service → confirm booking → pay → confirmation → track service.

**Customer never sees:** ledger · commission · settlement · escrow · financial operations.

### Domain separation — never one shared status

| Domain | Owns |
|--------|------|
| **Booking** | Requested, Confirmed, Awaiting Payment, Paid, In Progress, Completed, Cancelled |
| **Payment** | Pending, Processing, Paid, Failed, Cancelled, Refunded |
| **Ledger** | Financial truth · immutable records · create/reverse/audit only |
| **Settlement** | Provider payout lifecycle |

Payment success ≠ service completion · Service completion ≠ settlement completion.

### Areeba IXOPAY Payment.js

```text
Payment Port → IXOPAY Adapter → Gateway
```

Never: store cards · direct gateway SDK in business modules.

### Payment failure handling (post–Gate A design)

Idempotent payments · duplicate webhook protection · reconciliation · recovery · audit trails.

### Notifications (ADR-032)

Business Event → Notification Event → Orchestrator → In-App / Push / SMS·Email adapters.  
Never call SMS/email/push vendors from business modules.

### Chat (ADR-020)

Booking-scoped only. No open marketplace messaging.

### Globalization

Multi-country · multi-currency · international phone formats.  
**Defaults (not hardcoded):** Lebanon · +961 · USD · ar-RTL · en-LTR.

---

## 6. Sprint 0 (after Gate A only)

**Allowed foundation:** repository · CI/CD · environments · security baseline · logging · observability · migration **tooling framework**.

**Forbidden before Gate A:** coding · schema · APIs · UI · business logic.

**After Gate A, before feature authorization:** do not create business tables/workflows unless approved (Wave 3 = tooling only; domain schema requires BLOCKER-006 + authorization).

**UI (Wave 4):** Gate A + BLOCKER-001 **Closed**.

---

## 7. Implementation start decision

### "Can we start coding?" — evaluate

| Condition | Answer |
|-----------|--------|
| Blockers < 7 **OR** GOV-GAIR-001 unsigned | **NO** — Gate B · NOT AUTHORIZED |
| 7/7 closed **AND** GOV-IACL-001 complete **AND** GOV-GAIR-001 signed **AND** ceremony complete | **YES** — Gate A · Sprint 0 authorized |

**Current answer: NO**

---

## 8. Required AI response format

Every execution response must include:

1. **Current Gate**
2. **Implementation Status**
3. **Blockers** (x / 7 Closed)
4. **Architecture Impact**
5. **Scope Impact**
6. **Executed Action**
7. **Files Created/Updated**
8. **Next Allowed Action**

---

## Final governance decision

Until Gate A:

```text
NO IMPLEMENTATION · NO EXCEPTIONS · NO SHORTCUTS · NO BYPASSING GOVERNANCE
```

Implementation begins only after:

**7/7 blockers closed** + **Gate A approval completed** + **GOV-GAIR-001 signed**

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial master governance controlled implementation authorization prompt |
