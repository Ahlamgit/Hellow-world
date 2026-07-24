# KHADAMATI V1 — Areeba IXOPAY Vendor Validation Checklist

**Document ID:** KHAD-V1-AREEBA-VENDOR-CHECKLIST  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Payment Integration Architect  

**Supports:**  
- **BLOCKER-007** — Payment.js Mobile Validation (**IN VALIDATION** — PASS WITH CONDITIONS)  
- **BLOCKER-003** — Vendor Evaluation (Payment category)

**Status:** Checklist ready — values **Pending Vendor Confirmation** until Areeba/IXOPAY responds  

```text
DO NOT write production code.
DO NOT implement payment.
DO NOT modify architecture.
DO NOT invent sandbox/merchant/runtime values.
Unknown items = Pending Vendor Confirmation.
```

**Related:**  
[`PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](./PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [`PAYMENT_JS_MOBILE_VALIDATION_PLAN.md`](./PAYMENT_JS_MOBILE_VALIDATION_PLAN.md) · [`../vendors/VENDOR_EVALUATION_MATRIX.md`](../vendors/VENDOR_EVALUATION_MATRIX.md) · ADR-001 · ADR-004 · ADR-025

---

## How to use

1. Send this checklist to Areeba / IXOPAY (or fill from official docs + account manager).  
2. Record each **Confirmed value** in the right-hand column.  
3. Leave unknowns as **Pending Vendor Confirmation**.  
4. When rows needed for live mobile spike are filled, execute PJS-01…10.  
5. Only then consider BLOCKER-007 → **COMPLETED** (see §8).

| Legend | Meaning |
|--------|---------|
| PVC | Pending Vendor Confirmation |
| Confirmed | Value recorded from vendor/docs |
| N/A | Not applicable (vendor stated) |

---

# 1. Sandbox Access

| # | Request / item | Confirmed value | Status |
|---|----------------|-----------------|--------|
| 1.1 | Sandbox URL (API / portal) | | **Pending Vendor Confirmation** |
| 1.2 | Merchant identifier (MID / merchant GUID) | | **Pending Vendor Confirmation** |
| 1.3 | API credentials process (how issued, rotated, who holds) | | **Pending Vendor Confirmation** |
| 1.4 | Sandbox API username / password or connector credentials (custody only — not in git) | | **Pending Vendor Confirmation** |
| 1.5 | Public integration key (Payment.js) — sandbox | | **Pending Vendor Confirmation** |
| 1.6 | Test accounts (buyer/merchant test users if any) | | **Pending Vendor Confirmation** |
| 1.7 | Test cards / payment methods (success, decline, 3DS) | | **Pending Vendor Confirmation** |
| 1.8 | Supported currencies in sandbox | | **Pending Vendor Confirmation** |
| 1.9 | Sandbox webhook endpoint registration process | | **Pending Vendor Confirmation** |
| 1.10 | Sandbox vs production config differences (documented) | | **Pending Vendor Confirmation** |

**KHADAMATI custody rule:** Secrets in secrets manager only; never commit credentials to the repository.

---

# 2. Payment.js Validation

| # | Confirm | Confirmed value / notes | Status |
|---|---------|-------------------------|--------|
| 2.1 | Hosted fields availability (card number + CVV iframes) | | **Pending Vendor Confirmation** |
| 2.2 | JavaScript initialization flow (`init` / public key / container IDs) | | **Pending Vendor Confirmation** |
| 2.3 | `tokenize()` → `transactionToken` contract | | **Pending Vendor Confirmation** |
| 2.4 | Mobile WebView compatibility (supported / caveats) | | **Pending Vendor Confirmation** |
| 2.5 | Required domains / origins / CSP allowlist for Payment.js script & iframes | | **Pending Vendor Confirmation** |
| 2.6 | Security restrictions (referrer, frame ancestors, cookie requirements) | | **Pending Vendor Confirmation** |
| 2.7 | Official Payment.js script URL (sandbox + production) | | **Pending Vendor Confirmation** |
| 2.8 | Error codes returned to client on tokenize failure | | **Pending Vendor Confirmation** |

Architecture expectation (already approved): hosted fields; no PAN/CVV to KHADAMATI — do not change.

---

# 3. Mobile Requirements

## 3.1 Android

| # | Confirm | Confirmed value / notes | Status |
|---|---------|-------------------------|--------|
| 3.1.1 | WebView requirements (min Android / WebView version) | | **Pending Vendor Confirmation** |
| 3.1.2 | JS bridge requirements / recommended pattern | | **Pending Vendor Confirmation** |
| 3.1.3 | Callback handling (tokenize success/error to host app) | | **Pending Vendor Confirmation** |
| 3.1.4 | Third-party cookie / iframe limitations | | **Pending Vendor Confirmation** |
| 3.1.5 | Known Android WebView issues with Payment.js | | **Pending Vendor Confirmation** |

## 3.2 iOS

| # | Confirm | Confirmed value / notes | Status |
|---|---------|-------------------------|--------|
| 3.2.1 | WKWebView requirements (min iOS) | | **Pending Vendor Confirmation** |
| 3.2.2 | JavaScript communication (message handlers) | | **Pending Vendor Confirmation** |
| 3.2.3 | Redirect handling (3DS / return URLs) | | **Pending Vendor Confirmation** |
| 3.2.4 | ATS / HTTPS-only constraints | | **Pending Vendor Confirmation** |
| 3.2.5 | Known iOS WKWebView issues with Payment.js | | **Pending Vendor Confirmation** |

KHADAMATI V1 host pattern (approved): Flutter WebView + first-party HTML loading Payment.js — awaiting vendor confirmation of compatibility.

---

# 4. 3DS Validation

| # | Confirm | Confirmed value / notes | Status |
|---|---------|-------------------------|--------|
| 4.1 | 3DS version support (e.g. 2.x) | | **Pending Vendor Confirmation** |
| 4.2 | Authentication flow (frictionless vs challenge) | | **Pending Vendor Confirmation** |
| 4.3 | Challenge handling in WebView | | **Pending Vendor Confirmation** |
| 4.4 | Redirect / callback behaviour (success, error, cancel URLs) | | **Pending Vendor Confirmation** |
| 4.5 | Whether external browser is ever required | | **Pending Vendor Confirmation** |
| 4.6 | How REQUIRES_ACTION / pending states are signaled to merchant API | | **Pending Vendor Confirmation** |

---

# 5. Webhook Validation

| # | Confirm | Confirmed value / notes | Status |
|---|---------|-------------------------|--------|
| 5.1 | Available webhook / callback events (success, fail, refund, etc.) | | **Pending Vendor Confirmation** |
| 5.2 | Signature verification method (header, algorithm, secret) | | **Pending Vendor Confirmation** |
| 5.3 | Retry policy (intervals, max attempts) | | **Pending Vendor Confirmation** |
| 5.4 | Idempotency requirements (event id / transaction uuid) | | **Pending Vendor Confirmation** |
| 5.5 | Event ordering guarantees (or lack thereof) | | **Pending Vendor Confirmation** |
| 5.6 | IP allowlisting (if any) | | **Pending Vendor Confirmation** |
| 5.7 | Sample sandbox payload (safe redacted example from vendor) | | **Pending Vendor Confirmation** |

KHADAMATI requirement (architecture): verify authenticity; finalize idempotently; ledger only after verified finality.

---

# 6. Refund and Transaction Support

| # | Confirm availability | Confirmed value / notes | Status |
|---|----------------------|-------------------------|--------|
| 6.1 | Refund API | | **Pending Vendor Confirmation** |
| 6.2 | Partial refund | | **Pending Vendor Confirmation** |
| 6.3 | Full refund | | **Pending Vendor Confirmation** |
| 6.4 | Transaction lookup | | **Pending Vendor Confirmation** |
| 6.5 | Payment status query | | **Pending Vendor Confirmation** |
| 6.6 | Void / cancel before capture (if distinct from refund) | | **Pending Vendor Confirmation** |
| 6.7 | Settlement / payout reporting to merchant | | **Pending Vendor Confirmation** |

Business refund **rules** remain Admin-configurable (ADR-013/026). This section only confirms **gateway capability**.

---

# 7. Currency & Market Validation

### KHADAMATI defaults (approved — ADR-001)

| Item | Value |
|------|-------|
| Default country / market | **Lebanon** |
| Default currency | **USD** |

### Validate with vendor

| # | Validate | Confirmed value / notes | Status |
|---|----------|-------------------------|--------|
| 7.1 | Supported USD transactions | | **Pending Vendor Confirmation** |
| 7.2 | Future multi-country / multi-currency capability | | **Pending Vendor Confirmation** |
| 7.3 | Currency handling rules (minor units, rounding, FX) | | **Pending Vendor Confirmation** |
| 7.4 | Lebanon merchant onboarding constraints | | **Pending Vendor Confirmation** |
| 7.5 | Any forced local currency (e.g. LBP) for V1 | | **Pending Vendor Confirmation** |

Do not hardcode commercial FX or currency rules in domain code; Market + gateway config.

---

# 8. Final Acceptance Checklist (BLOCKER-007 → COMPLETED)

BLOCKER-007 can become **COMPLETED** only after:

- [ ] Sandbox credentials received
- [ ] Android payment flow tested
- [ ] iOS payment flow tested
- [ ] Webhook verified
- [ ] 3DS behaviour confirmed
- [ ] Payment lifecycle validated
- [ ] Architect approval
- [ ] Engineering approval

| Gate | Current |
|------|---------|
| BLOCKER-007 | **IN VALIDATION** |
| Result (architecture) | PASS WITH CONDITIONS |
| Implementation | **B) NOT READY — CODING BLOCKED** |

Until §8 is fully checked: **keep IN VALIDATION**. Do not mark COMPLETED.

---

# 9. Cross-blocker linkage

| Blocker | Link |
|---------|------|
| BLOCKER-003 | Payment vendor rows use this checklist as evidence pack |
| BLOCKER-007 | Live conditions in Validation Report §9 depend on §§1–6 here |
| BLOCKER-005 | Settlement timing content may reference §6.7 / Finance config — values still Admin-configurable |

---

# 10. Vendor response log

| Date | Contact / channel | Summary | Attachments |
|------|-------------------|---------|-------------|
| | | | |

---

# 11. Sign-off (when §8 complete)

| Role | Name | Date | Decision |
|------|------|------|----------|
| Payment Integration Architect | | | ☐ Checklist complete |
| Solution Architect | | | ☐ Approve BLOCKER-007 COMPLETED |
| Engineering Lead | | | ☐ Approve BLOCKER-007 COMPLETED |

---

**End of Areeba IXOPAY Vendor Validation Checklist v1.0**
