# BLOCKER-006 — Compliance Approval Pack

| Field | Value |
|-------|-------|
| **Document ID** | EVD-006-PACK-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-006 — Compliance |
| **Business direction** | **APPROVED** |
| **Blocker closure** | **NOT CLOSED** — Legal retention values pending |
| **Closure artifact** | `COMPLIANCE_APPROVAL_PACK_v1.0` (this pack upon signature) |

```text
DO NOT INVENT RETENTION DURATIONS.
Retention values require Legal authority only.
```

---

## 1. Pack contents

| Document | ID | Status |
|----------|-----|--------|
| `COMPLIANCE_GOVERNANCE_RECORD.md` | EVD-006-GOV-001 | ☑ v1.1 |
| `COMPLIANCE_POLICY.md` | — | ☑ On file |
| `DATA_RETENTION_POLICY.md` | — | ☑ Framework — durations pending Legal |
| `COMPLIANCE_CLOSURE_PREPARATION_RECORD.md` | — | ☑ On file |
| `COMPLIANCE_CLOSURE_VALIDATION_REPORT.md` | — | ☑ On file |

---

## 2. Privacy governance (approved direction)

| Principle | Status |
|-----------|--------|
| Data minimization | **Approved** (direction) |
| Purpose limitation | **Approved** (direction) |
| User rights workflow | Access · correction · deletion process — **Approved** (direction) |
| Administrator compliance workflows | **Approved** |
| Lebanon launch baseline | **Approved** |
| Multi-region readiness | **Approved** (architecture) |

---

## 3. Retention process (approved workflow)

```text
Data classification defined (COMPLIANCE_POLICY.md)
        ↓
Legal defines retention durations per data domain
        ↓
Legal/Compliance Officer approves DATA_RETENTION_POLICY.md values
        ↓
Technical Architect confirms implementability
        ↓
Signatures + archival
        ↓
BLOCKER-006 closure review
```

| Data domain | Retention duration | Authority |
|-------------|-------------------|-----------|
| Customer account data | **PENDING** | Legal |
| Provider / KYC data | **PENDING** | Legal |
| Financial / ledger records | **PENDING** | Legal |
| Chat / messaging | **PENDING** | Legal |
| Audit logs | **PENDING** | Legal |
| Marketing / consent records | **PENDING** | Legal |

**No durations are approved in this pack.**

---

## 4. Deletion process (approved direction)

| Step | Description |
|------|-------------|
| 1 | User or administrator initiates deletion request per policy |
| 2 | Identity verification per security rules |
| 3 | Legal hold check (if applicable — process TBD by Legal) |
| 4 | Soft-delete with audit trail |
| 5 | Hard-delete per approved retention schedule |
| 6 | Confirmation to requestor where required |

Implementation details deferred to post–Gate A under approved retention values.

---

## 5. Compliance workflow (Administrator)

| Workflow | Administrator role |
|----------|-------------------|
| Data subject requests | Route and track per approved process |
| Provider verification | KYC document handling per policy |
| Suspension / retention hold | Operational moderation aligned with Legal |
| Regional configuration | Lebanon baseline; multi-region ready |

---

## 6. Required signatures (pending)

| Approver | Role | Date | Status |
|----------|------|------|--------|
| | Legal / Compliance Officer | | **Pending** — retention values |
| | Technical Architect | | **Pending** — implementability |

---

## 7. Engineering constraints (unchanged)

- Must not assume legal basis without approved policy
- Must not hardcode retention periods in code
- Must support administrator-configured compliance workflows where approved

**BLOCKER-006 NOT CLOSED** until Legal provides retention values and signatures complete.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Compliance approval pack — Gate B evidence finalization |
