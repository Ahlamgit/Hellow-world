# BLOCKER-007 — Areeba IXOPAY Final Validation Checklist

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-CLOSURE-CHK-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-007 — Payment.js (Areeba IXOPAY) |
| **Governance status** | **READY FOR CLOSURE VALIDATION** |
| **Blocker closure** | **NOT CLOSED** |

```text
NO PAYMENT IMPLEMENTATION in repository during Gate B.
Sandbox validation evidence only.
```

---

## 1. Approved business flow

```text
Booking
        ↓
Payment initiation
        ↓
Areeba IXOPAY Payment.js
        ↓
Payment confirmation
        ↓
Booking confirmation
        ↓
Ledger recording
        ↓
Settlement process
```

**Reference:** `PAYMENT_FLOW_APPROVAL_RECORD.md` v1.1

---

## 2. Required validation

### 2.1 Sandbox connectivity

| # | Test | Executed | Pass | Evidence ref |
|---|------|----------|------|--------------|
| 1 | Sandbox credentials / environment access | ☐ | ☐ | |
| 2 | Payment.js widget load (web) | ☐ | ☐ | |
| 3 | Payment.js load (mobile WebView) | ☐ | ☐ | |
| 4 | Successful payment transaction | ☐ | ☐ | |

### 2.2 Webhook handling

| # | Test | Executed | Pass | Evidence ref |
|---|------|----------|------|--------------|
| 5 | Webhook endpoint reachable | ☐ | ☐ | |
| 6 | Signature verification | ☐ | ☐ | |
| 7 | Success webhook processing | ☐ | ☐ | |
| 8 | Failure webhook processing | ☐ | ☐ | |
| 9 | Idempotent duplicate webhook | ☐ | ☐ | |

### 2.3 Callback handling

| # | Test | Executed | Pass | Evidence ref |
|---|------|----------|------|--------------|
| 10 | Success callback path | ☐ | ☐ | |
| 11 | Failure callback path | ☐ | ☐ | |
| 12 | Callback tamper rejection | ☐ | ☐ | |

### 2.4 Payment failure scenarios

| # | Scenario | Executed | Pass | Evidence ref |
|---|----------|----------|------|--------------|
| 13 | Declined card | ☐ | ☐ | |
| 14 | Cancelled payment | ☐ | ☐ | |
| 15 | Timeout / network failure | ☐ | ☐ | |
| 16 | Double-submit idempotency | ☐ | ☐ | |

**Detailed matrix:** `PAYMENT_VALIDATION_CHECKLIST.md`  
**Results record:** `AREEBA_IXOPAY_VALIDATION_REPORT.md` (pending execution)

---

## 3. Architecture compliance

| Rule | ADR | Validated |
|------|-----|-----------|
| Payment domain separation | ADR-030 | ☐ |
| Immutable ledger | ADR-004 | ☐ |
| Ports/adapters pattern | ADR-025 | ☐ |
| No wallet V1 | Scope | ☐ |

---

## 4. Evidence on file

| Document | On file | Executed |
|----------|---------|----------|
| `PAYMENT_FLOW_APPROVAL_RECORD.md` v1.1 | ☑ | N/A |
| `PAYMENT_VALIDATION_CHECKLIST.md` | ☑ | ☐ |
| `AREEBA_IXOPAY_VALIDATION_REPORT.md` | ☑ Template | ☐ |
| `PAYMENT_CLOSURE_VALIDATION_REPORT.md` | ☑ | ☐ |

---

## 5. Closure validation matrix

| Criterion | Evidence | Validation complete | Signature | Closure decision |
|-----------|----------|---------------------|-----------|------------------|
| Business flow approved | `BUSINESS_APPROVAL_RECORD.md` | ☑ | PO/BO ☑ | — |
| Sandbox connectivity | IXOPAY report | ☐ | — | **BLOCKED** |
| Webhook handling | IXOPAY report | ☐ | — | **BLOCKED** |
| Callback handling | IXOPAY report | ☐ | — | **BLOCKED** |
| Failure scenarios | IXOPAY report | ☐ | — | **BLOCKED** |
| Integration Lead approval | Sign-off | ☐ | ☐ | **BLOCKED** |
| Technical Architect approval | Sign-off | ☐ | ☐ | **BLOCKED** |
| `PAYMENT_JS_VALIDATION_REPORT_v1.0` filed | Closure artifact | ☐ | — | **BLOCKED** |

**Closure decision:** ☐ **CLOSED** · ☑ **NOT CLOSED**

---

## 6. Required signatures

| Approver | Role | Date | Status |
|----------|------|------|--------|
| | **Integration Lead** | | **Pending** |
| | **Technical Architect** | | **Pending** |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Areeba IXOPAY final validation checklist |
