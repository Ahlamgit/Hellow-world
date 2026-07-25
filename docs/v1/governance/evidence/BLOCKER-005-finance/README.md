# BLOCKER-005 — Finance Evidence

| Field | Value |
|-------|-------|
| **Blocker ID** | BLOCKER-005 |
| **Folder** | `evidence/BLOCKER-005-finance/` |
| **Tracker** | `BLOCKER_CLOSURE_EXECUTION_TRACKER.md` (GOV-BLOCKER-TRACKER-001) |

## Purpose

Collect and archive finance policy approval evidence (commission, subscription, refund, settlement, withdrawal, cancellation) required to close BLOCKER-005 before Gate A. Financial architecture is approved; **policy values and signatures** are pending.

## Owner

**Finance** + **Business Operations** (accountable)

## Required evidence

| Item | Location |
|------|----------|
| Primary approval package | `FINANCE_POLICY_APPROVAL_PACKAGE.md` (EVD-005-PKG-001) |
| Commission rules | `commission/` |
| Subscription rules | `subscription/` |
| Settlement rules | `settlement/` |
| Withdrawal rules | `withdrawal/` |
| Cancellation rules | `cancellation/` |
| Refund policy | `refund/` |
| Signed approval record | `APPROVAL_RECORD.md` |
| Completed checklist | `EVIDENCE_CHECKLIST.md` |

**Closure artifact:** `FINANCE_RULE_MATRIX_v1.0`

Rules remain Admin configurable — no hardcoding.

## Approval authority

| Role | Authority |
|------|-----------|
| Finance | Policy values, settlement, refund, commission |
| Business Owner | Business model acceptance |
| Business Operations | Operational applicability |

## Current status

| Field | Value |
|-------|-------|
| **Status** | **Ready for Approval** |
| **Evidence package** | Prepared — signatures and values pending |
| **Closed** | **No** |

## Closure criteria

- [ ] All items on `EVIDENCE_CHECKLIST.md` checked with evidence on file
- [ ] `APPROVAL_RECORD.md` — Decision: **Approved**
- [ ] `FINANCE_POLICY_APPROVAL_PACKAGE.md` §10 signatures complete
- [ ] Closure artifact `FINANCE_RULE_MATRIX_v1.0` filed
- [ ] GOV-RBCS-001 + GOV-BLOCKER-TRACKER-001 updated within 1 business day

**No verbal approval.** Per `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` (GOV-BEMF-001).
