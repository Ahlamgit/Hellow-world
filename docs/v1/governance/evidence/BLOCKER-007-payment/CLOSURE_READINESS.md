# BLOCKER-007 — Payment.js Closure Readiness

| Field | Value |
|-------|-------|
| **Blocker** | BLOCKER-007 — Payment.js |
| **Date** | 2026-07-25 |
| **Gate** | Gate A TRANSITION IN PROGRESS |
| **Status** | **Open** — not Closed |
| **Depends on** | BLOCKER-003 (sandbox) · BLOCKER-005 (finance rules) |

## Validation checklist (governance)

| Area | Requirement | Evidence folder | Ready | Validated |
|------|-------------|-----------------|-------|-----------|
| Payment.js readiness | Client tokenization; no card storage | `payment-js/` | Framework defined | ☐ |
| Sandbox readiness | Areeba IXOPAY sandbox access | `sandbox/` | Pending BLOCKER-003 | ☐ |
| Webhook strategy | Signature verification, idempotency | `webhooks/` | Per ADR-031 | ☐ |
| 3DS flow | Mobile + web validation plan | `3ds/` | Pending | ☐ |
| Payment lifecycle | Pending → Processing → Paid/Failed | `lifecycle/` | Per ADR-030 | ☐ |
| Retry scenarios | Event-driven recovery | `lifecycle/` | Per ADR-031 | ☐ |
| Reconciliation | Delayed confirmation handling | `ledger/` | Per payment framework | ☐ |
| Finance acceptance | Reconciliation alignment | `finance-signoff/` | Pending BLOCKER-005 | ☐ |

## Architecture preservation

```text
Customer → Payment Service → Payment Port → IXOPAY Adapter → Areeba Gateway
```

- Booking/payment state separation (ADR-030) — unchanged  
- Ledger immutability (ADR-004) — unchanged  
- No payment logic in booking module — unchanged  

## Supporting references

- `payment/PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`
- ADR-029 · ADR-030 · ADR-031

## Gaps

| Gap | Owner | Action |
|-----|-------|--------|
| Sandbox test execution | Integration Lead | After BLOCKER-003 sandbox |
| Mobile Payment.js validation | Technical Lead | File evidence |
| `PAYMENT_JS_VALIDATION_REPORT_v1.0` | Program Governance | File on closure |
| `APPROVAL_RECORD.md` signed | TL + Finance Ops | Pending |

**No payment implementation** during evidence collection.
