# ADR-022: Account Retention & Deletion

**Status:** Accepted — 2026-07-24  
**Supersedes / completes:** ADR-014  

---

## Decision

KHADAMATI supports a full account lifecycle:

1. Account deletion **request**  
2. **Verification** (OTP / re-auth)  
3. **Anonymization** of personal identifiers  
4. **Audit preservation**  
5. Financial and legally required records retained per **configurable retention policies** (Market / Admin settings — not hardcoded durations in domain constants)

---

## Lifecycle

```text
Active → Deletion Requested → Verified → Soft-disabled
    → Cooling-off (configurable)
    → Anonymize PII
    → Retain finance/KYC per policy
    → Optional media purge when retention elapsed
```

---

## What Must Remain

| Data | Retention |
|------|-----------|
| Ledger entries, payments, refunds, commissions | Retain; unlink display PII → anonymized refs |
| Bookings financial history | Retain |
| Audit events | Retain per audit retention policy |
| KYC media | Configurable retention then purge |
| Chat messages | Anonymize sender labels; retain or purge per chat retention config |

---

## Configurable Policies

Admin/Compliance settings (per Market):

- Cooling-off days  
- PII anonymization delay  
- KYC media retention days  
- Chat retention days  
- Audit retention days  

---

## Security / Audit

Every step emits `audit_events`. Finance rows never hard-deleted for convenience.

---

## Consequences

Deletion is a worker-driven workflow. Exact numeric defaults seeded for Lebanon staging; production values set by Compliance/Admin.
