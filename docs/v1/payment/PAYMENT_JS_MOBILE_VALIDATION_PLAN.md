# KHADAMATI V1 — Payment.js Mobile Validation Plan

**Document ID:** KHAD-V1-PAYMENTJS-SPIKE  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Architecture validation executed — see [`PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](./PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) (BLOCKER-007 **IN VALIDATION**)  
**ADR:** ADR-004 · ADR-025 · Payment architecture  

**Related:** [`PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](./PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md) · [`../vendors/VENDOR_EVALUATION_MATRIX.md`](../vendors/VENDOR_EVALUATION_MATRIX.md)

```text
NO production implementation.
NO production UI.
Validation / spike only — discard throwaway code after report.
```

---

## 1. Objective

Prove that **Areeba IXOPAY Payment.js** can support the Customer Flutter payment step (post provider confirmation) with secure token handling, webhook lifecycle, failure handling, and duplicate-payment protection — before Implementation Gate → **A**.

---

## 2. Dependencies

| Dependency | Required |
|------------|----------|
| BLOCKER-003 Payment sandbox access | Yes |
| Payment.js documentation / credentials | Yes |
| Staging (or isolated) webhook endpoint for spike | Yes |
| Architect review of security model | Yes |

---

## 3. Scope of Validation

| Area | Validate | In spike | Out of scope |
|------|----------|----------|--------------|
| Mobile payment flow | Flutter host → Payment.js | Yes | Production screens |
| Payment.js compatibility | WebView / browser APIs / origins | Yes | Full app theming |
| Token handling | No PAN storage; token/session lifecycle | Yes | Card vault product |
| Security model | TLS, origin allowlist, secret custody | Yes | Pen-test |
| Webhook lifecycle | Signature/auth, idempotency, ledger post stub | Yes | Full ledger engine |
| Failure scenarios | Timeout, cancel, decline, network drop | Yes | All edge markets |
| Duplicate payment protection | Idempotency keys / gateway refs | Yes | Production reconcile jobs |

---

## 4. Test Scenarios

| ID | Scenario | Expected | Result | Notes |
|----|----------|----------|--------|-------|
| PJS-01 | Happy path authorize/capture (sandbox) | Success + webhook | ☐ | |
| PJS-02 | User cancels in Payment.js | Cancelled; no capture | ☐ | |
| PJS-03 | Declined card | Decline; safe UX | ☐ | |
| PJS-04 | Network drop mid-flow | No double charge; recoverable state | ☐ | |
| PJS-05 | Duplicate client submit | Single payment intent | ☐ | |
| PJS-06 | Duplicate webhook delivery | Idempotent processing | ☐ | |
| PJS-07 | Invalid webhook signature | Reject; alert | ☐ | |
| PJS-08 | Token / session expiry | Clear failure; retry path | ☐ | |
| PJS-09 | Android WebView host | Compatible | ☐ | |
| PJS-10 | iOS WebView host | Compatible | ☐ | |

---

## 5. Security Checklist

| Check | Pass |
|-------|------|
| No PAN / CVV stored in app or KHADAMATI DB | ☐ |
| Secrets only in secure config (not repo) | ☐ |
| Webhook authentication verified | ☐ |
| HTTPS only | ☐ |
| Payment state machine prevents double capture | ☐ |

---

## 6. Deliverables

1. This plan (prep)  
2. Spike execution notes (date, environment, builds)  
3. **Readiness result** below (Pass / Conditional Pass / Fail)  
4. Optional follow-up: `PAYMENT_JS_MOBILE_INTEGRATION_READINESS_REPORT.md` if Eng prefers separate artifact  

---

## 7. Readiness Result

| Field | Value |
|-------|-------|
| Executed by | Solution Architect / Payment Integration Specialist (architecture spike) |
| Date | 2026-07-24 |
| Environment | Architecture validation (sandbox device run **not** executed — credentials pending) |
| **Result** | ☐ Pass · ☑ **PASS WITH CONDITIONS** (Conditional Pass) · ☐ Fail |
| Blocking issues | Live Android/iOS WebView + webhook sandbox proof pending vendor credentials |
| Accepted risks (if Conditional) | Pending Eng Lead / Architect acceptance of report |
| Follow-ups before production pay UI | Complete PJS-01…10 in sandbox; confirm 3DS/webhook signature; see Validation Report §9 |

### Result definitions

| Result | Meaning |
|--------|---------|
| **Pass** | All critical scenarios pass; safe to proceed when gate flips to A |
| **Conditional Pass** | Non-blocking issues documented with mitigation dates; Architect + Product accept |
| **Fail** | Critical incompatibility or security failure — BLOCKER-007 remains Open |

---

## 8. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Engineering Lead | | | ☐ Accept result |
| Solution Architect | | | ☐ Accept result |
| Product Owner | | | ☐ Acknowledge (if Conditional) |

**BLOCKER-007 closed only on Pass or accepted Conditional Pass.**

---

**End of Payment.js Mobile Validation Plan**
