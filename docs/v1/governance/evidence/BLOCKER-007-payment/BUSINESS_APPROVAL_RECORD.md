# BLOCKER-007 — Business Approval Record (Payment)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-BUSINESS-001 |
| **Version** | 1.2 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Decision** | **APPROVED** (business payment flow) |
| **Approver** | Project Owner / Business Owner |
| **Date** | 2026-07-25 |
| **Consolidation** | [BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md](../../BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md) |
| **Blocker closure** | **NOT CLOSED** — sandbox · webhook · TA validation pending |

---

## Approved customer payment flow

| Step | Action |
|------|--------|
| 1 | Customer creates booking |
| 2 | Customer confirms request |
| 3 | Payment starts |
| 4 | **Areeba IXOPAY Payment.js** processes payment |
| 5 | Payment confirmation received |
| 6 | **Ledger records transaction** |
| 7 | Booking continues |

---

## Provider (approved)

- Receives booking confirmation after successful payment
- Settlement follows approved financial architecture (ADR-004, ADR-030)

---

## Architecture rules (confirmed)

| Rule | Status |
|------|--------|
| Payment domain separated | **Confirmed** |
| Adapter / ports pattern preserved | **Confirmed** (ADR-025) |
| No direct card handling | **Confirmed** |
| Webhook validation required | **Required** — evidence pending |
| Sandbox testing required | **Required** — evidence pending |
| Ledger as financial source of truth | **Confirmed** (ADR-004) |

---

## Closure note

PO/BO business payment flow **APPROVED**. **BLOCKER-007 remains open** until Areeba IXOPAY sandbox validation, webhook validation, Technical Architect approval, and `PAYMENT_JS_VALIDATION_REPORT_v1.0`.
