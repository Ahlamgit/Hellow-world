# BLOCKER-007 — Business Approval Record (Payment)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-BUSINESS-001 |
| **Version** | 1.1 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Decision** | **APPROVED** (business payment flow) |
| **Approver** | Project Owner / Business Owner |
| **Date** | 2026-07-25 |
| **Consolidation** | [BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md](../../BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md) |
| **Blocker closure** | **NOT CLOSED** — sandbox · webhook · technical validation pending |

---

## Approved customer payment flow

| Step | Action |
|------|--------|
| 1 | Customer creates booking |
| 2 | Customer confirms service request |
| 3 | Payment initiated |
| 4 | Payment processed through **Areeba IXOPAY Payment.js** flow |
| 5 | Payment confirmation received |
| 6 | Booking continues |

```text
Customer creates booking → Payment initiated → IXOPAY Payment.js processing
    → Payment success → Booking confirmation → Ledger transaction creation → Service execution
```

---

## Provider (approved)

| Rule | Status |
|------|--------|
| Receives booking confirmation after successful payment | **Approved** |
| Settlement follows approved financial architecture | **Approved** (ADR-004, ADR-030) |

---

## Approved payment principles

| Principle | Status |
|-----------|--------|
| Payment domain separation | **Confirmed** |
| No direct card handling by KHADAMATI | **Confirmed** |
| Payment adapter / ports pattern | **Confirmed** (ADR-025) |
| Webhook validation required before production | **Required** — evidence pending |
| Sandbox validation required before production | **Required** — evidence pending |
| Ledger as financial source of truth | **Confirmed** (ADR-004) |
| Booking/payment separation | **Confirmed** (ADR-030) |

---

## Closure note

PO/BO business payment flow approved. **BLOCKER-007 remains open** until sandbox validation, webhook evidence, BLOCKER-003/005 dependencies, Technical Architect attestation, and `PAYMENT_JS_VALIDATION_REPORT_v1.0` complete.
