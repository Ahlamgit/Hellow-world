# Areeba — Go-Live Decision

**Date:** 2026-07-23  
**Based on:** [AREEBA_SANDBOX_VALIDATION_REPORT.md](./AREEBA_SANDBOX_VALIDATION_REPORT.md)  
**Branch:** `cursor/payment-gateway-migration-plan-4876`

---

## Recommendation

# Needs fixes

**Not ready for production Areeba cutover.**

Architecture and implementation remain approved, but **required sandbox validation was not completed** in this environment (no Staging SQL Server apply; no Areeba sandbox credentials; no device/browser E2E). Go-live must wait until those checks PASS.

---

## Passed checks

| ID | Check |
|----|-------|
| P1 | Gateway-independent architecture with `IPaymentGateway` |
| P2 | `BookingPaymentAttempts` model + migration authored (additive, backfill, indexes) |
| P3 | Areeba MPGS adapter + DI + readiness wiring |
| P4 | Webhook-driven completion; HMAC/invalid signature unit coverage |
| P5 | Amount/currency mismatch rejected in `VerifyAsync` unit tests |
| P6 | Duplicate webhook idempotency unit coverage |
| P7 | Mobile initiate→confirm removed (Android + iOS code) |
| P8 | Web hosted redirect + Development-only confirm |
| P9 | Production `Payment:Provider` still **Moyasar** (cutover not enabled) |
| P10 | No live payment secrets committed in tracked config |
| P11 | Payment-related unit tests: **18/18 passed** (full suite **82/82** previously green) |
| P12 | Moyasar coexistence retained in code/config |

---

## Failed / blocked checks

| ID | Check | Severity | Notes |
|----|-------|----------|-------|
| B1 | Areeba sandbox credentials configured & used | **Blocker** | Secrets absent in agent env |
| B2 | SQL Server Staging migration applied & verified | **Blocker** | Docker SQL pull blocked (`overlayfs` permission) |
| B3 | Live successful sandbox payment E2E | **Blocker** | Depends on B1–B2 |
| B4 | Live failed / abandoned payment E2E | **Blocker** | Depends on B1–B2 |
| B5 | Live duplicate / invalid webhook on Staging | **Blocker** | Depends on B1–B2 |
| B6 | Live wrong amount/currency against sandbox | **Blocker** | Unit only so far |
| B7 | Web Staging UI checkout QA | **Blocker** | No Staging stack here |
| B8 | Android device sandbox QA | **Blocker** | No device lab |
| B9 | iOS device sandbox QA | **Blocker** | No device lab |

No production code defect was proven in this phase; blockers are **validation environment / evidence gaps**.

---

## Remaining risks

| Risk | Level | Mitigation |
|------|-------|------------|
| MPGS webhook signature header differs from assumed HMAC/shared-secret | High until sandbox | Confirm with boarding; fail-closed already coded |
| Hosted checkout URL / `checkout.js` bridge not production-shaped | Medium | Set `Payment:Areeba:HostedCheckoutUrl` after sandbox UX check |
| Filtered indexes / backfill issues on real Staging data | Medium | Run §2.3 SQL verification after migrate |
| Client deep-link return UX polish | Medium | Device QA on Android/iOS |
| Premature production Provider switch | Critical if ignored | Keep Production on Moyasar until decision flips to Ready |

---

## Explicit non-actions (still in force)

Until a future decision flips to **Ready for production** after sandbox PASS:

- Do **not** set Production `Payment:Provider=Areeba`
- Do **not** remove Moyasar
- Do **not** start Phase 1C
- Do **not** start React Native migration

---

## Exit criteria to flip recommendation to Ready

1. Staging SQL migration applied; data/index checks green.  
2. Staging Areeba sandbox E2E matrix (§3) all PASS with logs attached.  
3. Web + Android + iOS sandbox client matrix (§4) all PASS.  
4. Security review of Staging webhook signatures signed by Ops.  
5. Product/Eng/Ops re-approve this decision document as **Ready for production**.

---

## Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| Engineering | | | Acknowledge **Needs fixes** (validation incomplete) |
| QA | | | Pending Staging/device evidence |
| Ops | | | Pending Staging migrate + secrets |
| Product | | | Hold production cutover |

---

*End of go-live decision — STOP.*
