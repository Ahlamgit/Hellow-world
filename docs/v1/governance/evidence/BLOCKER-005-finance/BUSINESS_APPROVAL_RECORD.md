# BLOCKER-005 — Business Approval Record (Finance)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-BUSINESS-001 |
| **Blocker** | BLOCKER-005 — Finance |
| **Decision** | **APPROVED** (business model) |
| **Approver** | Project Owner / Business Owner |
| **Date** | 2026-07-25 |
| **Blocker closure** | **NOT CLOSED** — Finance values · matrix · full signatures pending |

---

## Commission model (approved)

```text
Customer Payment → Payment Domain → Ledger → KHADAMATI Commission → Provider Settlement
```

| Rule | Status |
|------|--------|
| Ledger immutable source of truth | Confirmed |
| No manual financial manipulation | Confirmed |
| Payment and settlement separated | Confirmed (ADR-030) |
| Admin-configurable rules — not hardcoded | Confirmed (ADR-013) |

---

## Subscription pricing (approved)

| Revenue mechanism | Status |
|-------------------|--------|
| Provider subscriptions | **Approved** |
| Store/service advertiser subscriptions | **Approved** |
| Advertisement packages | **Approved** |

Pricing remains **configurable** through approved business configuration (not hardcoded in application code).

---

## Closure note

Business approval recorded. **BLOCKER-005 remains open** until `FINANCE_RULE_MATRIX_v1.0`, Finance attestation, and full checklist complete.
