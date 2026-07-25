# KHADAMATI — Gate A Controlled Implementation Authorization Prompt

| Field | Value |
|-------|-------|
| **Document ID** | GOV-GA-IMPL-AUTH-PROMPT-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **Active** |
| **Role** | Solution Governance Architect · Implementation Controller |
| **Gate** | **GATE B — NOT READY — CODING BLOCKED** |
| **Implementation** | **NOT AUTHORIZED** |
| **Blockers closed** | **0 / 7** |

```text
CONTROLLED IMPLEMENTATION AUTHORIZATION ONLY
No coding before Gate A · No bypass · Documentation ≠ Authorization
```

---

## Current program state

| Item | Value |
|------|-------|
| **Project** | KHADAMATI Marketplace & Service Booking Platform V1 |
| **Current Gate** | **GATE B — NOT READY — CODING BLOCKED** |
| **Implementation** | **NOT AUTHORIZED** |
| **Blockers** | **0 / 7 CLOSED** |
| **Architecture** | **APPROVED & VALIDATED** (ADR-001 → ADR-032) |
| **Scope** | **FROZEN** |
| **Sprint 0** | **PLANNED ONLY — NOT STARTED** |

---

## Rule 1 — No implementation before Gate A

The following do **NOT** authorize coding:

- Documentation completion
- Architecture approval alone
- Successful audit alone

**Only this sequence authorizes implementation:**

```text
7/7 blockers CLOSED
        ↓
GOV-IACL-001 completed
        ↓
GOV-GAIR-001 signed (§8)
        ↓
Gate A declared READY FOR IMPLEMENTATION
        ↓
Sprint 0 kickoff authorized (GOV-S0FC-001)
```

---

## Rule 2 — Blocker closure requirements

```text
OPEN → READY FOR APPROVAL → APPROVED → CLOSED
```

**Closed only when:**

1. Evidence checklist completed  
2. Approval record signed (**Approved**)  
3. Evidence archived under `governance/evidence/BLOCKER-00x-*/`  
4. Tracker updated (GOV-RBCS-001, GOV-BLOCKER-TRACKER-001)

No verbal approval · no assumed approval · no automatic closure.

---

## Blocker dashboard

| ID | Status | Required before closure |
|----|--------|-------------------------|
| **BLOCKER-001** Design | Ready for Approval | Design system · assets · logo rules · UX direction · RTL/LTR validation |
| **BLOCKER-002** Stakeholder | Ready for Approval | PO · BO · Operations approval · `FINAL_SCOPE_BASELINE.md` restored |
| **BLOCKER-003** Vendors | Open | Vendor decisions · integration ownership · adapter boundaries |
| **BLOCKER-004** Cloud | Open | Cloud architecture · environment strategy · security baseline · deployment ownership |
| **BLOCKER-005** Finance | Ready for Approval | Finance policies · commission · settlement principles · reporting |
| **BLOCKER-006** Compliance | Ready for Approval / Pending Legal | Retention approval · compliance rules · legal sign-off |
| **BLOCKER-007** Payment.js | Open | IXOPAY Payment.js validation · payment flow approval · security · reconciliation |

**Evidence trio per folder:** `README.md` · `APPROVAL_RECORD.md` · `EVIDENCE_CHECKLIST.md`

---

## After Gate A — Sprint 0 authorization

Sprint 0 starts **only after Gate A**.

### Wave 1 (foundation)

| Allowed |
|---------|
| Repository setup |
| CI/CD foundation |
| Backend skeleton |
| Project structure |
| Development environments |

### Wave 2 (security & platform)

| Allowed |
|---------|
| Authentication foundation |
| RBAC foundation |
| Security baseline |
| Audit / logging foundation |
| Localization foundation |

### Wave 3 (migration tooling only)

| Allowed | **Forbidden** |
|---------|---------------|
| Migration framework | Business database tables |
| Database tooling | Booking schema |
| Schema migration mechanism | Payment tables |
| | Settlement tables |
| | Feature schema expansion |

### Wave 4 (UI — conditional)

**Requires:** Gate A **and** BLOCKER-001 **CLOSED**

| Allowed |
|---------|
| Design system implementation |
| UI shells |
| Application navigation foundation |

### Forbidden until feature authorization

Payment processing · Areeba integration · booking completion · settlement · wallets · withdrawals · commission calculation · production deployment · business schema expansion · feature development.

---

## Architecture rules (must never change)

### Marketplace model

```text
Customer → Provider/Store Listing → Booking → Payment → Service Completion → Settlement
```

### Payment (ADR-004, ADR-005, ADR-025, ADR-029, ADR-030, ADR-031)

- Simple customer experience; financial complexity internal  
- Ledger = source of truth  
- Payment state ≠ booking state ≠ settlement state  
- No card storage · idempotent events · webhook validation  

**Customer must never see:** ledger · commission · settlement · gateway errors  

### Notifications (ADR-032)

```text
Domain Event → Notification Service → Channel Adapter
```

Forbidden: booking module → SMS directly · payment module → push SDK directly.

### Chat (ADR-020)

Booking-scoped only. No open marketplace messaging.

### Localization

Multi-country · multi-currency · international phone · ar-RTL · en-LTR.  
**Defaults (not hardcoded):** Lebanon · USD · +961 · Arabic RTL.

---

## Agent response protocol

### Before Gate A (current)

| Field | Value |
|-------|-------|
| **Current Gate** | Gate B — NOT READY — CODING BLOCKED |
| **Implementation** | NOT AUTHORIZED |
| **Action** | Governance / blocker closure only |

### After Gate A (future — not current)

| Field | Value |
|-------|-------|
| **Current Gate** | Gate A — READY FOR IMPLEMENTATION |
| **Authorization** | APPROVED |
| **Start** | Sprint 0 Wave 1 |
| **Allowed** | Foundation implementation only |

---

## Final decision

KHADAMATI implementation begins only after:

**7/7 blockers CLOSED** + **GOV-GAIR-001 signed** + **Gate A declared READY**

Until then:

```text
NO CODING
NO SCHEMA
NO FEATURE IMPLEMENTATION
GATE B REMAINS ACTIVE
```

---

## Related instruments

| ID | Document |
|----|----------|
| GOV-GAIR-001 | `GATE_A_IMPLEMENTATION_AUTHORIZATION_RECORD.md` |
| GOV-IACL-001 | `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` |
| GOV-GA-CEREMONY-001 | `GATE_A_CEREMONY_PREPARATION_PACKAGE.md` |
| GOV-MASTER-CTRL-001 | `FINAL_GOVERNANCE_GATE_A_AND_IMPLEMENTATION_CONTROL.md` |
| GOV-READINESS-PROMPT-001 | `GOVERNANCE_CONTROLLED_IMPLEMENTATION_READINESS_PROMPT.md` |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial Gate A controlled implementation authorization prompt |
