# KHADAMATI V1 — Retention Policy Decisions

**Document ID:** KHAD-V1-RETENTION  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Decisions pending Compliance / Legal (BLOCKER-006 **IN PREPARATION**)  
**ADR:** ADR-022 · ADR-014  

**Related:** [`RETENTION_AND_DATA_GOVERNANCE_FRAMEWORK.md`](./RETENTION_AND_DATA_GOVERNANCE_FRAMEWORK.md) · [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md)

```text
Financial records and audit records MUST remain protected
when legally required — they cannot be removed with casual
account deletion.
Defaults are Admin-configurable; do not hardcode as sole truth.
```

---

## 1. Account Deletion

| Decision | Value | Notes |
|----------|-------|-------|
| Request channel | TBD | In-app request + Admin handling |
| Cool-off / grace period | **TBD** (days) | |
| Soft-delete vs anonymize | Per ADR-022 | Unlink display PII; retain money/audit |
| Customer-initiated delete | ☐ Allowed with review | |
| Provider-initiated delete | ☐ Allowed with review | |
| Admin-forced delete / ban | ☐ Defined | |

Compliance confirmation: ☐ Name ______ Date ______

---

## 2. Personal Data Retention

| Data class | Retention default | Action after period | State |
|------------|-------------------|---------------------|-------|
| Customer profile PII | **TBD** | Anonymize / purge per policy | ☐ |
| Provider profile PII | **TBD** | Anonymize / purge per policy | ☐ |
| Contact channels (email/phone) | **TBD** | | ☐ |
| Device / push tokens | **TBD** | | ☐ |
| Chat message content | **TBD** | Anonymize labels; purge/retain per config | ☐ |

---

## 3. Provider Documents Retention

| Document class | Retention default | Purge allowed? | State |
|----------------|-------------------|----------------|-------|
| KYC / ID documents | **TBD** | After retention elapsed | ☐ |
| Verification selfies / face artifacts | **TBD** | After retention elapsed | ☐ |
| Business licenses (stores) | **TBD** | | ☐ |
| Service media (images) | **TBD** | | ☐ |

---

## 4. Audit Logs Retention

| Parameter | Value | Protected |
|-----------|-------|-----------|
| Audit log retention | **TBD** | **Yes — protected** |
| Immutable / append-only | Yes (architecture) | Yes |
| Purge before legal minimum | **Forbidden** if legally required | |

Compliance confirmation: ☐ Name ______ Date ______

---

## 5. Financial Records Retention

| Record class | Retention default | Protected |
|--------------|-------------------|-----------|
| Ledger entries | **TBD** (legal minimum+) | **Yes — protected** |
| Payments / refunds / commissions | **TBD** | **Yes** |
| Withdrawal requests / settlements | **TBD** | **Yes** |
| Finance policy change history | **TBD** | **Yes** |

| Rule | Confirmed |
|------|-----------|
| Account deletion does **not** erase financial records | ☐ |
| PII on financial records anonymized where feasible; money facts retained | ☐ |
| Legal hold overrides purge jobs | ☐ |

---

## 6. Cross-Cutting Rules

| Rule | Status |
|------|--------|
| Retention defaults configured via Admin (Market / Compliance settings) | Required |
| No hardcoded retention days as sole production truth | Required |
| Financial + audit protection enforced in deletion workflows | Required |
| Documented legal basis / jurisdiction notes (Lebanon launch) | TBD by Legal |

---

## 7. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Compliance / Legal | | | ☐ Approve defaults |
| Security | | | ☐ Acknowledge |
| Finance | | | ☐ Acknowledge financial retention |
| Product Owner | | | ☐ Acknowledge UX messaging |
| Solution Architect | | | ☐ Acknowledge ADR-022 |

**BLOCKER-006 closed when §§1–5 defaults are set (not TBD) and Compliance/Legal approves.**

---

**End of Retention Policy Decisions**
