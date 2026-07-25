# BLOCKER-006 — Retention Request (Pending Legal)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-006-RETENTION-REQ-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **NOT APPROVED** — Legal authority required |
| **Blocker closure** | **NOT CLOSED** |

```text
DO NOT USE THIS DOCUMENT AS APPROVED RETENTION POLICY.
Only Legal/Compliance Officer may define retention durations.
```

---

## 1. PO/BO discussion input (not legal approval)

| Field | Value |
|-------|-------|
| **Submitted by** | Ahlam (Project Owner / Business Owner) |
| **Date** | 2026-07-25 |
| **Context** | Testing / development discussion |
| **Proposed value** | **1 month** (for testing consideration only) |

---

## 2. Governance ruling

| Rule | Status |
|------|--------|
| PO/BO may propose values for Legal review | ☑ Recorded |
| PO/BO may **not** approve retention durations | ☑ Enforced |
| `1 month` is **NOT** in `DATA_RETENTION_POLICY.md` | ☑ |
| Production retention | **PENDING Legal** |

**Legal must review and approve** (or replace) any retention period before BLOCKER-006 closure.

---

## 3. Permitted interim approach (testing only)

| Data type | Testing / localhost | Production |
|-----------|---------------------|------------|
| Test user accounts | Ephemeral — delete after test cycles | N/A until Legal defines |
| Sandbox payment logs | Per IXOPAY sandbox policy | N/A |
| Production retention | **Not defined** | **Legal required** |

**No `1 month` value may be hardcoded** in application code or production configuration without Legal sign-off.

---

## 4. Next step

Legal / Compliance Officer to complete `LEGAL_COMPLIANCE_FINALIZATION_CHECKLIST.md` with authoritative retention values.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | PO proposed 1 month for testing discussion — pending Legal |
