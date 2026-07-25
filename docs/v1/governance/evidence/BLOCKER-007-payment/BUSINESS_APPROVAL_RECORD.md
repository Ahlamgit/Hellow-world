# BLOCKER-007 — Business Approval Record (Payment.js)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-BUSINESS-001 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Decision** | **APPROVED** (business payment flow) |
| **Approver** | Project Owner / Business Owner |
| **Date** | 2026-07-25 |
| **Blocker closure** | **NOT CLOSED** — sandbox · webhook · technical validation pending |

---

## Approved payment flow

```text
Customer creates booking
    ↓
Payment initiated
    ↓
IXOPAY Payment.js processing
    ↓
Payment success
    ↓
Booking confirmation
    ↓
Ledger transaction creation
    ↓
Service execution
```

---

## Architecture preservation (confirmed)

| Principle | Reference |
|-----------|-----------|
| Payment adapter/ports architecture | ADR-025 |
| Vendor isolation | ADR-025 |
| Booking/payment separation | ADR-030 |
| Ledger as financial source of truth | ADR-004 |

---

## Closure note

Business payment flow approved. **BLOCKER-007 remains open** until sandbox validation, webhook evidence, BLOCKER-003/005 dependencies, and `PAYMENT_JS_VALIDATION_REPORT_v1.0` complete.
