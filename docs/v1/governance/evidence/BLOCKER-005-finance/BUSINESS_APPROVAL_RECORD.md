# BLOCKER-005 — Business Approval Record (Finance)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-BUSINESS-001 |
| **Version** | 1.1 |
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

| Revenue mechanism | Status |
|-------------------|--------|
| Provider subscriptions | **Approved** |
| Store/service advertising subscriptions | **Approved** |
| Advertisement packages | **Approved** |

**Pricing management** belongs to **Administrator configuration** after implementation (not hardcoded). **No new pricing features** added beyond frozen V1 scope.

---

## Scope constraint

| Item | Status |
|------|--------|
| V1 scope frozen | Confirmed |
| No wallets · instant withdrawals · manual financial adjustment screens | Excluded per scope baseline |

---

## Closure note

PO/BO business finance approval recorded. **BLOCKER-005 remains open** until `FINANCE_RULE_MATRIX_v1.0`, Finance Owner attestation, and full checklist complete.
