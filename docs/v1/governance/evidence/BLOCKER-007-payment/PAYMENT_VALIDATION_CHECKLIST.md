# BLOCKER-007 — Payment Validation Checklist

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-CHECKLIST-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-007 — Payment.js (Areeba IXOPAY) |
| **Business flow** | **APPROVED** |
| **Technical validation** | **PENDING** |
| **Blocker closure** | **NOT CLOSED** |

```text
NO PAYMENT IMPLEMENTATION in repository during Gate B.
This checklist governs sandbox validation evidence only.
```

---

## 1. Approved business flow

```text
Booking Created
        ↓
Payment Initiation
        ↓
Areeba IXOPAY Payment.js
        ↓
Payment Confirmation
        ↓
Booking Confirmation
        ↓
Ledger Recording
        ↓
Settlement Process
```

**Reference:** `PAYMENT_FLOW_APPROVAL_RECORD.md` v1.1

---

## 2. Pre-validation prerequisites

| # | Prerequisite | Status |
|---|--------------|--------|
| 1 | IXOPAY sandbox credentials obtained | ☐ |
| 2 | Sandbox merchant configuration complete | ☐ |
| 3 | Webhook endpoint design documented | ☐ |
| 4 | Callback URL strategy documented | ☐ |
| 5 | BLOCKER-003 vendor readiness (payment path) | ☐ |
| 6 | BLOCKER-005 finance model acknowledged | ☑ Business approved |

---

## 3. Sandbox validation checklist

| # | Test | Expected outcome | Evidence | IL | TA |
|---|------|------------------|----------|-----|-----|
| S-01 | Sandbox environment access | Successful authentication | Log/screenshot | ☐ | ☐ |
| S-02 | Payment.js load — web | Widget renders | Screenshot | ☐ | ☐ |
| S-03 | Payment.js load — mobile WebView | Widget renders Android | Screenshot | ☐ | ☐ |
| S-04 | Payment.js load — mobile WebView | Widget renders iOS | Screenshot | ☐ | ☐ |
| S-05 | Successful payment | Confirmation received | Transaction ID | ☐ | ☐ |
| S-06 | Declined payment | Graceful failure UX path documented | Transaction log | ☐ | ☐ |
| S-07 | Cancelled payment | Booking remains unpaid state | Transaction log | ☐ | ☐ |
| S-08 | Timeout / network failure | Idempotent recovery documented | Test notes | ☐ | ☐ |
| S-09 | 3DS / SCA (if applicable) | Challenge flow completes | Test notes | ☐ | ☐ |

---

## 4. Webhook validation checklist

| # | Test | Expected outcome | Evidence | IL | TA |
|---|------|------------------|----------|-----|-----|
| W-01 | Webhook endpoint reachable (sandbox) | HTTP 200 acknowledgment | Log | ☐ | ☐ |
| W-02 | Signature verification | Invalid signature rejected | Log | ☐ | ☐ |
| W-03 | Payment success webhook | Booking correlation updated | Log | ☐ | ☐ |
| W-04 | Payment failure webhook | Failure state recorded | Log | ☐ | ☐ |
| W-05 | Duplicate webhook (idempotency) | Single ledger effect | Log | ☐ | ☐ |
| W-06 | Out-of-order delivery | Correct final state | Log | ☐ | ☐ |

---

## 5. Callback validation checklist

| # | Test | Expected outcome | Evidence | IL | TA |
|---|------|------------------|----------|-----|-----|
| C-01 | Success callback redirect | User sees confirmation path | Screenshot | ☐ | ☐ |
| C-02 | Failure callback redirect | User sees retry/cancel path | Screenshot | ☐ | ☐ |
| C-03 | Callback tampering | Rejected / ignored | Log | ☐ | ☐ |

---

## 6. Failure scenario checklist

| # | Scenario | Required behavior | Validated |
|---|----------|-------------------|-----------|
| F-01 | Insufficient funds | Clear error; booking not confirmed | ☐ |
| F-02 | Card expired | Clear error; no duplicate charge | ☐ |
| F-03 | Webhook delay | Polling/reconciliation path documented | ☐ |
| F-04 | Double-submit | Idempotent payment initiation | ☐ |
| F-05 | Partial authorization | Not applicable / documented | ☐ |
| F-06 | Refund path (if sandbox supports) | Documented for future — V1 scope | ☐ |

---

## 7. Architecture compliance

| Rule | ADR | Confirmed |
|------|-----|-----------|
| Payment domain separation | ADR-030 | ☐ |
| Immutable ledger | ADR-004 | ☐ |
| Adapter/ports pattern | ADR-025 | ☐ |
| No wallet implementation V1 | Scope | ☐ |
| No financial bypass | Governance | ☐ |

---

## 8. Closure requirements

| Item | Status |
|------|--------|
| `AREEBA_IXOPAY_VALIDATION_REPORT.md` completed | ☐ |
| Integration Lead signature | ☐ |
| Technical Architect signature | ☐ |
| `PAYMENT_JS_VALIDATION_REPORT_v1.0` filed | ☐ |

**Related:** `PAYMENT_VALIDATION_RECORD.md` · `PAYMENT_CLOSURE_VALIDATION_REPORT.md`

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Payment validation checklist — Gate B evidence finalization |
