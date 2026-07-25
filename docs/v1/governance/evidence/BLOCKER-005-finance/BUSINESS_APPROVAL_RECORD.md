# BLOCKER-005 — Business Approval Record (Finance)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-BUSINESS-001 |
| **Version** | 1.2 |
| **Blocker** | BLOCKER-005 — Finance |
| **Decision** | **APPROVED** (business model) |
| **Approver** | Project Owner / Business Owner |
| **Date** | 2026-07-25 |
| **Consolidation** | [BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md](../../BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md) |
| **Blocker closure** | **NOT CLOSED** — Finance matrix · attestation · full checklist pending |

---

## Commission model — APPROVED

| Decision | Status |
|----------|--------|
| Marketplace commission model accepted | **Approved** |
| Commission calculation remains **configurable** (administrator configuration) | **Approved** |
| Exact implementation rules follow approved architecture | **Confirmed** (ADR-004, ADR-013, ADR-030) |

```text
Customer Payment → Payment Domain → Ledger → KHADAMATI Commission → Provider Settlement
```

| Rule | Status |
|------|--------|
| Ledger immutable source of truth | Confirmed |
| No manual financial manipulation | Confirmed |
| Payment and settlement separated | Confirmed |

---

## Subscription pricing — APPROVED

### Provider subscriptions

| Tier | Status |
|------|--------|
| Basic | **Approved** |
| Pro | **Approved** |
| Premium | **Approved** |

### Advertising subscriptions

| Type | Status |
|------|--------|
| Service / store advertising packages | **Approved** |
| Featured placement | **Approved** |

**Administrator** manages configuration after implementation. Values **not hardcoded** (ADR-013).

---

## Scope constraint

| Item | Status |
|------|--------|
| V1 scope frozen | Confirmed |
| No wallets · instant withdrawals · manual financial adjustment screens | Excluded per scope baseline |

---

## Closure note

PO/BO business finance approval recorded. **BLOCKER-005 remains open** until `FINANCE_RULE_MATRIX_v1.0`, Finance Owner attestation, and full checklist complete.
