# BLOCKER-007 — Payment Flow Approval Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-FLOW-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Evidence status** | **Prepared** — awaiting technical validation |
| **Blocker closure** | **NOT CLOSED** |

---

## 1. Approved business payment flow

| Step | Stage | Responsible domain |
|------|-------|-------------------|
| 1 | Customer creates booking | Booking domain |
| 2 | Payment initiation | Payment domain (initiation only) |
| 3 | **Areeba IXOPAY Payment.js** processes payment | Payment adapter (vendor isolated) |
| 4 | Payment confirmation received | Payment domain |
| 5 | Booking confirmation | Booking domain |
| 6 | **Ledger record** created | Ledger domain (financial source of truth) |
| 7 | Settlement process (per finance rules) | Settlement domain (post-payment) |

```text
Customer Booking
        ↓
Payment Initiation
        ↓
Areeba IXOPAY Payment.js
        ↓
Payment Confirmation
        ↓
Booking Confirmation
        ↓
Ledger Record
        ↓
Settlement Process
```

**Business approver:** Project Owner / Business Owner — **Approved** 2026-07-25

---

## 2. Architecture preservation (confirmed)

| Principle | ADR / framework |
|-----------|-----------------|
| Payment domain separation | ADR-030 |
| Ledger separation — immutable financial truth | ADR-004 |
| Adapter / ports architecture — vendor isolation | ADR-025 |
| No direct card handling by KHADAMATI | Payment security framework |
| Booking/payment separation | ADR-030 |

---

## 3. Required technical evidence (NOT prepared = NOT closed)

| Evidence | Status | Owner |
|----------|--------|-------|
| Areeba IXOPAY sandbox validation | **Pending** | Integration Lead |
| Webhook validation | **Pending** | Integration Lead |
| Payment lifecycle test scenarios | **Pending** | Integration Lead |
| Ledger reconciliation samples | **Pending** | Finance Ops + Technical |
| Mobile (Android/iOS) validation | **Pending** | Integration Lead |
| Technical Architect approval | **Pending** | Technical Architect |

**Dependencies:** BLOCKER-003 (payment vendor sandbox) · BLOCKER-005 (finance rules matrix)

---

## 4. Explicit prohibition

| Activity | Status |
|----------|--------|
| Payment integration code | **FORBIDDEN** until Gate A |
| Production payment credentials | **FORBIDDEN** until Gate A + blocker closure |
| Sandbox testing for evidence | Allowed **only** under governance validation workflow |

**Closure artifact:** `PAYMENT_JS_VALIDATION_REPORT_v1.0` — not filed.

See also: `BUSINESS_APPROVAL_RECORD.md` · `APPROVAL_RECORD.md`
