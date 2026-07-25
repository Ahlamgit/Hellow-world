# BLOCKER-007 — Areeba IXOPAY Validation Report

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-IXOPAY-001 |
| **Version** | 1.0 (template) |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Provider** | Areeba IXOPAY Payment.js |
| **Status** | **PENDING EXECUTION** — sandbox validation not yet performed |
| **Blocker closure** | **NOT CLOSED** |

```text
This report records IXOPAY sandbox validation RESULTS.
NO payment code in repository during Gate B evidence phase.
```

---

## 1. Validation scope

| Area | Covered by |
|------|------------|
| Sandbox access | §2 |
| Payment.js lifecycle | §3 |
| Webhooks | §4 |
| Callbacks | §5 |
| Failure scenarios | §6 |
| Ledger correlation | §7 |

**Checklist:** `PAYMENT_VALIDATION_CHECKLIST.md`

---

## 2. Sandbox environment

| Field | Value |
|-------|-------|
| Environment | IXOPAY Sandbox |
| Merchant ID | **Pending** — do not invent |
| API base URL | **Pending** — from credentials |
| Payment.js version | **Pending** |
| Validation date | |
| Executed by | Integration Lead |

| Result | Status |
|--------|--------|
| Sandbox access confirmed | ☐ Pass · ☐ Fail · ☐ Not executed |

**Evidence:** _(attach credential confirmation log — redact secrets)_

---

## 3. Payment.js lifecycle results

| Test ID | Description | Result | Evidence ref | Date |
|---------|-------------|--------|--------------|------|
| S-02 | Web widget load | ☐ | | |
| S-03 | Android WebView | ☐ | | |
| S-04 | iOS WebView | ☐ | | |
| S-05 | Successful payment | ☐ | | |
| S-06 | Declined payment | ☐ | | |
| S-07 | Cancelled payment | ☐ | | |
| S-08 | Timeout handling | ☐ | | |
| S-09 | 3DS / SCA | ☐ N/A · ☐ Pass · ☐ Fail | | |

**Summary:** ☐ All required tests passed · ☐ Failures documented with remediation

---

## 4. Webhook results

| Test ID | Description | Result | Evidence ref | Date |
|---------|-------------|--------|--------------|------|
| W-01 | Endpoint reachable | ☐ | | |
| W-02 | Signature verification | ☐ | | |
| W-03 | Success webhook | ☐ | | |
| W-04 | Failure webhook | ☐ | | |
| W-05 | Idempotency | ☐ | | |
| W-06 | Out-of-order handling | ☐ | | |

**Webhook URL (sandbox):** _(document at execution — not in Gate B repo)_

---

## 5. Callback results

| Test ID | Description | Result | Evidence ref | Date |
|---------|-------------|--------|--------------|------|
| C-01 | Success callback | ☐ | | |
| C-02 | Failure callback | ☐ | | |
| C-03 | Tamper rejection | ☐ | | |

---

## 6. Failure scenario results

| Scenario | Observed behavior | Acceptable? | Notes |
|----------|-------------------|-------------|-------|
| F-01 Insufficient funds | | ☐ | |
| F-02 Card expired | | ☐ | |
| F-03 Webhook delay | | ☐ | |
| F-04 Double-submit | | ☐ | |

---

## 7. Ledger correlation

| Check | Result | Notes |
|-------|--------|-------|
| Payment event maps to booking ID | ☐ | |
| Immutable ledger entry design validated | ☐ | ADR-004 |
| Settlement handoff documented | ☐ | BLOCKER-005 |

---

## 8. Architecture attestation

| Rule | ADR | IL | TA |
|------|-----|-----|-----|
| Payment domain separation | ADR-030 | ☐ | ☐ |
| No wallet V1 | Scope | ☐ | ☐ |
| Ports/adapters integration | ADR-025 | ☐ | ☐ |

---

## 9. Sign-off

| Approver | Role | Date | Decision |
|----------|------|------|----------|
| | Integration Lead | | ☐ Approved · ☐ Rejected |
| | Technical Architect | | ☐ Approved · ☐ Rejected |

**Upon dual approval:** This document becomes `PAYMENT_JS_VALIDATION_REPORT_v1.0`.

**Until executed:** BLOCKER-007 remains **NOT CLOSED**.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | IXOPAY validation report template — Gate B evidence finalization |
