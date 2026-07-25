# BLOCKER-007 — Payment Validation Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-VALID-001 |
| **Version** | 1.0 (preparation) |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Business flow** | **APPROVED** — see `PAYMENT_FLOW_APPROVAL_RECORD.md` |
| **Technical validation** | **PENDING** |
| **Blocker closure** | **NOT CLOSED** |

---

## 1. Approved business flow (reference)

```text
Customer Booking
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

---

## 2. Architecture rules (must hold in validation)

| Rule | Status |
|------|--------|
| Payment domain separation | Required |
| Immutable ledger | Required (ADR-004) |
| No wallet implementation | **V1 excluded** |
| No instant withdrawal | **V1 excluded** |
| No financial bypass | Required |
| Adapter/ports pattern (ADR-025) | Required |

---

## 3. Technical validation matrix (execution pending)

| # | Validation | Method | Result | Owner | Date |
|---|------------|--------|--------|-------|------|
| 1 | IXOPAY sandbox access | Credentials + environment | **Pending** | Integration Lead | |
| 2 | Payment.js lifecycle — success | Sandbox test | **Pending** | Integration Lead | |
| 3 | Payment.js lifecycle — failure | Sandbox test | **Pending** | Integration Lead | |
| 4 | Webhook delivery | Sandbox endpoint | **Pending** | Integration Lead | |
| 5 | Payment callback handling | Sandbox | **Pending** | Integration Lead | |
| 6 | 3DS / SCA flow (if applicable) | Sandbox | **Pending** | Integration Lead | |
| 7 | Ledger record correlation | Test evidence | **Pending** | Integration Lead + Finance | |
| 8 | Mobile WebView / SDK path (Android) | Device test | **Pending** | Integration Lead | |
| 9 | Mobile WebView / SDK path (iOS) | Device test | **Pending** | Integration Lead | |

**NO PAYMENT CODE** in repository as part of this evidence preparation.

---

## 4. Dependencies

| Blocker | Dependency | Met? |
|---------|------------|------|
| BLOCKER-003 | Payment vendor sandbox | ☐ |
| BLOCKER-005 | Admin finance configuration model (TA) | ☐ |

---

## 5. Required approvers

| Approver | Role | Status |
|----------|------|--------|
| | Integration Lead | **Pending** |
| | Technical Architect | **Pending** |

**Closure artifact target:** `PAYMENT_JS_VALIDATION_REPORT_v1.0` — not filed.
