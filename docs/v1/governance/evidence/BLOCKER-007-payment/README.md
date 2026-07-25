# BLOCKER-007 — Payment Evidence

| Field | Value |
|-------|-------|
| **Blocker ID** | BLOCKER-007 |
| **Folder** | `evidence/BLOCKER-007-payment/` |
| **Tracker** | `BLOCKER_CLOSURE_EXECUTION_TRACKER.md` (GOV-BLOCKER-TRACKER-001) |

## Purpose

Collect and archive Payment.js (Areeba IXOPAY) sandbox validation, mobile flow, webhook, and finance acceptance evidence required to close BLOCKER-007 before Gate A. **Excluded from Sprint 0** until closed.

## Owner

**Technical Lead** + **Finance Ops** (accountable)

## Required evidence

| Item | Location |
|------|----------|
| Sandbox test results | `sandbox/` |
| Payment.js integration validation | `payment-js/` |
| Webhook validation | `webhooks/` |
| 3DS flow evidence | `3ds/` |
| Payment lifecycle tests | `lifecycle/` |
| Ledger reconciliation samples | `ledger/` |
| Finance acceptance | `finance-signoff/` |
| Signed approval record | `APPROVAL_RECORD.md` |
| Completed checklist | `EVIDENCE_CHECKLIST.md` |

**Supporting references (outside this folder):** `payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`, `payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`, `payment/PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`, ADR-029, ADR-030, ADR-031

**Closure artifact:** `PAYMENT_JS_VALIDATION_REPORT_v1.0`

**Depends on:** BLOCKER-003 (payment vendor sandbox), BLOCKER-005 (ledger/settlement rules)

## Approval authority

| Role | Authority |
|------|-----------|
| Technical Lead | Technical validation, sandbox and webhook evidence |
| Finance Ops | Finance acceptance, reconciliation alignment |
| Integration Lead | Vendor sandbox readiness cross-check |

## Current status

| Field | Value |
|-------|-------|
| **Status** | **Open** |
| **Evidence package** | Structure prepared — validation pending dependencies |
| **Closed** | **No** |

## Closure criteria

- [ ] All items on `EVIDENCE_CHECKLIST.md` checked with evidence on file
- [ ] `APPROVAL_RECORD.md` — Decision: **Approved**
- [ ] BLOCKER-003 payment sandbox evidence referenced
- [ ] BLOCKER-005 finance rules referenced
- [ ] Closure artifact `PAYMENT_JS_VALIDATION_REPORT_v1.0` filed
- [ ] GOV-RBCS-001 + GOV-BLOCKER-TRACKER-001 updated within 1 business day

**No verbal approval.** Per `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` (GOV-BEMF-001).
