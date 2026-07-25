# BLOCKER-006 — Compliance Evidence

| Field | Value |
|-------|-------|
| **Blocker ID** | BLOCKER-006 |
| **Folder** | `evidence/BLOCKER-006-compliance/` |
| **Tracker** | `BLOCKER_CLOSURE_EXECUTION_TRACKER.md` (GOV-BLOCKER-TRACKER-001) |

## Purpose

Collect and archive retention, data governance, and legal compliance approval evidence required to close BLOCKER-006 before Gate A. Enables data lifecycle and domain schema planning after closure.

## Owner

**Legal / Compliance Officer** (accountable)

## Required evidence

| Item | Location |
|------|----------|
| Primary approval package | `COMPLIANCE_APPROVAL_PACKAGE.md` (EVD-006-PKG-001) |
| Compliance policy | `COMPLIANCE_POLICY.md` (EVD-006-POLICY-001) |
| Data retention policy | `DATA_RETENTION_POLICY.md` (EVD-006-RETENTION-001) |
| KYC evidence | `kyc/` |
| Financial records retention | `financial-records/` |
| Chat / messaging retention | `chat/` |
| Deletion procedures | `deletion/` |
| Signed approval record | `APPROVAL_RECORD.md` |
| Completed checklist | `EVIDENCE_CHECKLIST.md` |

**Closure artifact:** `COMPLIANCE_APPROVAL_PACK_v1.0`

No legal assumptions by engineering. No vendor selection in this blocker package.

## Approval authority

| Role | Authority |
|------|-----------|
| Legal / Compliance Officer | Retention durations, legal compliance |
| Business Owner | Business risk acceptance |
| Technical Architect | Technical feasibility of retention/deletion |

## Current status

| Field | Value |
|-------|-------|
| **Status** | **Ready for Approval** |
| **Evidence package** | Prepared — signatures and legal values pending |
| **Closed** | **No** |

## Closure criteria

- [ ] All items on `EVIDENCE_CHECKLIST.md` checked with evidence on file
- [ ] `APPROVAL_RECORD.md` — Decision: **Approved**
- [ ] `COMPLIANCE_APPROVAL_PACKAGE.md` §10 signatures complete
- [ ] Approved retention values documented (not placeholders)
- [ ] Closure artifact `COMPLIANCE_APPROVAL_PACK_v1.0` filed
- [ ] GOV-RBCS-001 + GOV-BLOCKER-TRACKER-001 updated within 1 business day

**No verbal approval.** Per `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` (GOV-BEMF-001).
