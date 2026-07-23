# Areeba — Go-Live Decision

**Date:** 2026-07-23 (updated after remediation validation)  
**Based on:** [AREEBA_SANDBOX_VALIDATION_REPORT.md](./AREEBA_SANDBOX_VALIDATION_REPORT.md), [AREEBA_BLOCKER_RESOLUTION_PLAN.md](./AREEBA_BLOCKER_RESOLUTION_PLAN.md)  
**Branch:** `cursor/payment-gateway-migration-plan-4876`

---

## Final status

# NOT READY

**Production Areeba cutover is not approved.**

SQL Server migration compatibility was validated in remediation (local SQL Server 2022). Live Areeba sandbox credentials, Staging host apply confirmation, and browser/device QA remain open blockers.

---

## Recommendation summary

| Option | Selected |
|--------|----------|
| READY | |
| **NOT READY** | **Yes** |

---

## Passed checks

| ID | Check |
|----|-------|
| P1 | Gateway-independent `IPaymentGateway` architecture |
| P2 | `BookingPaymentAttempts` model + additive migrations |
| P3 | Areeba adapter + webhook implementation in repo |
| P4 | Unit: invalid webhook, duplicate path, amount/currency mismatch |
| P5 | Mobile/Web no longer use initiate→confirm as completion authority (code) |
| P6 | Production `Payment:Provider` remains **Moyasar** |
| P7 | No live secrets committed |
| P8 | **NEW:** SQL Server 2022 — `dotnet ef database update` **PASS** |
| P9 | **NEW:** Indexes + FKs for attempts verified on live SQL instance |
| P10 | **NEW:** Existing `BookingPayment` preserved; attempt backfill **PASS** |
| P11 | **NEW:** Migration rollback + re-apply **PASS** |
| P12 | Moyasar coexistence retained |

---

## Failed / open checks

| ID | Check | Severity |
|----|-------|----------|
| O1 | Areeba sandbox merchant credentials configured on Staging | **Blocker** |
| O2 | Official Staging SQL host migration (ops environment) | **Blocker** (compat proven; host apply pending) |
| O3 | Live successful sandbox payment E2E | **Blocker** |
| O4 | Live fail / abandon / delayed webhook E2E | **Blocker** |
| O5 | Live duplicate + invalid signature on Staging URL | **Blocker** |
| O6 | Chrome + mobile browser QA against Staging | **Blocker** |
| O7 | Android device checkout/return/refresh QA | **Blocker** |
| O8 | iOS device checkout/return/refresh QA | **Blocker** |

---

## Remaining risks

| Risk | Level | Notes |
|------|-------|-------|
| Webhook signature scheme mismatch vs boarding docs | High until live sandbox | Confirm headers with Areeba |
| Hosted checkout UX / deep links | Medium | Needs device QA |
| Staging host differs from local SQL container | Low–Med | Re-run same validation queries on Staging |
| Accidental production Provider switch | Critical if ignored | Keep Production on Moyasar |

---

## Explicit non-actions (still in force)

Until status flips to **READY** after open checks PASS:

- Do **not** enable Production `Payment:Provider=Areeba`
- Do **not** remove Moyasar
- Do **not** start Phase 1C
- Do **not** start React Native migration

---

## Criteria to flip to READY

1. Staging secrets: Areeba sandbox merchant + webhook secret injected (not committed).  
2. Staging SQL: same migrations applied; preservation/index queries green.  
3. Live §3 sandbox matrix all PASS with logs.  
4. Web + Android + iOS §4 QA all PASS.  
5. Product/Eng/Ops sign below as READY.

---

## Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| Engineering | | | **NOT READY** (ack) |
| QA | | | Pending Staging/device evidence |
| Ops | | | Pending Staging secrets + host migrate |
| Product | | | Hold cutover |

---

*End of go-live decision — STOP.*
