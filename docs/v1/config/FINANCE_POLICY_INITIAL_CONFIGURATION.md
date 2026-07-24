# KHADAMATI V1 — Finance Policy Initial Configuration (Lebanon)

**Document ID:** KHAD-V1-FINANCE-CONFIG  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Template — values pending Finance approval (BLOCKER-005)  
**Market:** Lebanon (default) — ADR-001  
**ADR:** ADR-013 · ADR-026  

**Related:** [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md)

```text
All values MUST remain Admin Portal configurable.
DO NOT hardcode commercial values in application code.
Architecture for policy engines is already approved.
This document captures INITIAL business configuration content only.
```

**Runtime source of truth:** Admin Finance / Super Admin configuration (MFA + audit + change history).  
**Currency context:** USD (Lebanon market default) unless Finance amends Market config.

---

## 1. Commission

| Parameter | Initial value | Effective from | Notes |
|-----------|---------------|----------------|-------|
| Default commission rule | **TBD** | TBD | % and/or fixed; priority |
| Applies to | Bookings / services | | Unified provider ledger |
| Exceptions | **TBD** | TBD | e.g. category, plan, promo |

| Exception ID | Criteria | Outcome | Priority | Active |
|--------------|----------|---------|----------|--------|
| EX-01 | TBD | TBD | | ☐ |

Finance confirmation: ☐ Name ______ Date ______

---

## 2. Cancellation

| Parameter | Initial value | Notes |
|-----------|---------------|-------|
| Allowed conditions | **TBD** | Who may cancel; booking states |
| Time rules / windows | **TBD** | Hours before start, etc. |
| Penalties | **TBD** | Customer / provider; fee or % |
| Interaction with refund policy | **TBD** | Cross-ref §3 |

Finance confirmation: ☐ Name ______ Date ______

---

## 3. Refund

| Scenario | Rule | Manual review? | Notes |
|----------|------|----------------|-------|
| Full refund | **TBD** | ☐ Yes / ☐ No | |
| Partial refund | **TBD** | ☐ Yes / ☐ No | |
| Manual review cases | **TBD** | Yes | Dispute / exception path |

| Parameter | Initial value |
|-----------|---------------|
| Default refund % by cancel window | TBD |
| Non-refundable components | TBD |
| Approval role | Finance Admin / Super Admin |

Finance confirmation: ☐ Name ______ Date ______

---

## 4. Withdrawal

| Parameter | Initial value | Notes |
|-----------|---------------|-------|
| Available methods | **TBD** | Config rows + adapters; not hardcoded rails |
| Minimum withdrawal | **TBD** | Minor units + currency |
| Maximum / daily limits | **TBD** | Optional |
| Approval process | **TBD** | Auto vs manual thresholds |
| Provider eligibility | `CanReceivePayments` | Capability-gated |

Finance confirmation: ☐ Name ______ Date ______

---

## 5. Settlement

| Parameter | Initial value | Notes |
|-----------|---------------|-------|
| Holding period | **TBD** | Days / hours after completion |
| Release conditions | **TBD** | No open dispute, etc. |
| Reserve % (if any) | **TBD** | |
| Cadence / batching | **TBD** | |

Finance confirmation: ☐ Name ______ Date ______

---

## 6. Operational Controls

| Control | Value |
|---------|-------|
| Money features enabled in Staging after config | ☐ |
| Money features enabled in Production after config | ☐ |
| Feature flag acknowledgement (`payments.enabled` etc.) | ☐ |
| Staging dry-run completed | ☐ |

---

## 7. Hardcoding Prohibition Checklist

| Check | Confirmed |
|-------|-----------|
| No commission % as domain constants | ☐ |
| No fixed cancel windows as sole truth in code | ☐ |
| No fixed refund % as sole truth in code | ☐ |
| No single permanent withdrawal method in domain | ☐ |
| No fixed settlement hold days as sole truth in code | ☐ |
| Seed data (if any) is non-production demo only | ☐ |

---

## 8. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Finance | | | ☐ Approve initial Lebanon values |
| Product Owner | | | ☐ Acknowledge commercial fit |
| Solution Architect | | | ☐ Acknowledge ADR-013/026 (config-only) |

**BLOCKER-005 closed when §§1–5 values are filled (not TBD) and Finance approves.**

---

**End of Finance Policy Initial Configuration**
