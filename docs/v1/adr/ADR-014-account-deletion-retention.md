# ADR-014: Account Deletion and Data Retention

## Status
Accepted direction — details require Compliance sign-off before coding deletion jobs.

## Context
Master Prompt requires account deletion with validation, retention, anonymization, and audit. Exact retention periods and Lebanese/future GCC legal obligations are not fully specified.

## Decision
1. Soft-deactivate account immediately (`users.status = DELETED_PENDING` / `DISABLED`).  
2. Anonymize direct PII after cooling-off (duration = **Market retention policy config**, not hardcoded magic number in domain services — default seed TBD by Compliance).  
3. **Never delete** ledger entries, payments, commission lines, or booking financial history; replace display names with anonymized tokens.  
4. KYC media purge per IDV retention policy (Q-IDV-006 still needs value).  
5. Full audit event on request, anonymization, and purge steps.

## Consequences
Deletion is a **workflow**, not a single SQL delete. Blocked on Compliance filling retention durations (see readiness Blocked list).
