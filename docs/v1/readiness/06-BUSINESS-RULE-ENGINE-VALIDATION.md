# Business Rule Engine Validation

**Document ID:** KHAD-V1-RULE-ENGINE  
**Authority:** ADR-013 · `architecture/47-ADMIN-CONFIGURABLE-FINANCIAL-RULES.md`

---

## Validation Result: PASS (Architecture)

| Rule Domain | Admin Configurable | Runtime Engine | History/Audit | Hardcoded Values Forbidden |
|-------------|--------------------|----------------|---------------|----------------------------|
| Commission | ✓ multi-dimensional, effective-dated | ✓ resolve + calculate | ✓ | ✓ |
| Cancellation | ✓ role, status, time, penalties, approval | ✓ booking evaluates | ✓ | ✓ |
| Refund | ✓ full/partial/none/manual | ✓ refund service | ✓ | ✓ |
| Withdrawal | ✓ methods, mins, approval | ✓ validate request | ✓ | ✓ |
| Settlement | ✓ holds, reserves, release schedule | ✓ settlement job | ✓ | ✓ |

---

## Commission Rules — Confirmed Capabilities

- Default % and/or fixed amount  
- Category, provider-type, market, subscription level, booking value dimensions  
- Effective dates, activate/deactivate, priority  
- Historical versions; `commission_lines` store rule_id + version  

## Cancellation Rules — Confirmed Capabilities

- Who: Customer, Craftsman, Store, Admin  
- Allowed statuses  
- Time-based conditions  
- Penalties  
- Refund linkage  
- Approval requirements  

## Refund Rules — Confirmed Capabilities

- Full / Partial / None / Manual approval  
- Depend on status, reason, time, party, payment status  
- Always create financial + ledger + audit records  

## Withdrawal Rules — Confirmed Capabilities

- Methods enabled per market (Lebanon examples as config rows)  
- Minimum amounts  
- Approval workflow  
- Adapter key indirection (no single hardwired rail)  

## Settlement Rules — Confirmed Capabilities

- Holds / reserves  
- Release schedule / cadence  
- Batch processing  

---

## Coding Gate Statement

> No financial business rule constants may ship as the sole source of truth in domain services. Starter seed data for non-prod is allowed; production values are Finance Admin configuration.
