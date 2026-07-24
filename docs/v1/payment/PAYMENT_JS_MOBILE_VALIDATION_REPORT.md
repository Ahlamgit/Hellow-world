# KHADAMATI V1 — Payment.js Mobile Validation Report

**Document ID:** KHAD-V1-PAYMENTJS-VALIDATION-REPORT  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Solution Architect & Payment Integration Specialist  
**BLOCKER-007 status:** **IN VALIDATION**  
**Spike type:** Architecture & feasibility validation (no production code)

```text
DO NOT write production code.
DO NOT implement the payment module.
DO NOT modify backend architecture, database schema, UI, or V1 scope.
DO NOT replace Areeba IXOPAY / Payment.js.
```

**Sources:** Master Prompt v1.0 · Scope Baseline · Decisions Complete · ADR-004 · ADR-005 · ADR-025 · workflows/23-PAYMENT-FLOW · Flutter Architecture §12.8 · IXOPAY Payment.js public documentation (hosted fields / tokenization)

**Related:** [`PAYMENT_JS_MOBILE_VALIDATION_PLAN.md`](./PAYMENT_JS_MOBILE_VALIDATION_PLAN.md) · [`AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) · [`PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`](./PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md) · [`../vendors/VENDOR_EVALUATION_MATRIX.md`](../vendors/VENDOR_EVALUATION_MATRIX.md)

**Vendor evidence pack:** Fill [`AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) before live mobile tests. BLOCKER-007 remains **IN VALIDATION** until that checklist §8 is complete.

---

## Executive Summary

KHADAMATI’s approved payment architecture can safely support the intended mobile flow:

```text
Customer Mobile App
  → Payment.js (hosted fields)
  → Areeba IXOPAY Gateway
  → Payment Processing
  → Webhook Confirmation
  → Ledger Update
  → Booking Payment Completion
```

**Feasibility (architecture):** Confirmed against approved ADRs and IXOPAY Payment.js hosted-fields model (tokenize → `transactionToken` → server debit; PAN/CVV never touch KHADAMATI systems).

**Runtime sandbox proof:** Not executed — merchant/sandbox credentials and live WebView device tests are **Pending Vendor Confirmation** (BLOCKER-003 Payment).

### Payment.js Mobile Readiness Result

# PASS WITH CONDITIONS

See §9 for confirmed items, risks, and required actions before implementation authorization.

---

# 1. Payment Architecture Validation

## 1.1 Approved architecture (frozen for V1)

```text
Customer App (Flutter)
        ↓
KHADAMATI Backend Payment Service
        ↓
Payment Adapter Interface (PaymentGatewayPort)
        ↓
Areeba IXOPAY Adapter
        ↓
Payment.js Integration (client hosted fields → transactionToken)
        ↓
Webhook Handler (verified, idempotent)
        ↓
Ledger (+ booking payment completion)
```

Aligned with ADR-025, ADR-004, Master Prompt payment section, and `workflows/23-PAYMENT-FLOW.md`.

## 1.2 Confirmations

| Requirement | Validation | Evidence |
|-------------|------------|----------|
| Gateway-specific logic isolated | **Confirmed (architecture)** | Only `IxopayPaymentGatewayAdapter` speaks IXOPAY; domain uses port |
| Payment provider replaceable later | **Confirmed (architecture)** | Config-selected adapter; booking/ledger not vendor-coupled (ADR-025) |
| No raw card data in KHADAMATI systems | **Confirmed (architecture + vendor model)** | Payment.js hosted iframes; PAN/CVV vaulted by gateway; app/backend receive `transactionToken` only |
| PCI exposure minimized | **Confirmed (architecture intent)** | Hosted fields target SAQ-A class per IXOPAY Payment.js docs; public integration key client-side only; merchant secrets server-side only |

## 1.3 Explicit non-changes

- V1 gateway remains **Areeba IXOPAY Payment.js only**  
- No alternate mobile SDK mandated for V1 (may be evaluated later without changing scope)  
- No schema/API/UI implementation in this spike  

---

# 2. Payment.js Mobile Compatibility

## 2.1 Why WebView is required

Payment.js is a **browser JavaScript** library that injects hosted card number / CVV iframes. Native Flutter widgets cannot host those iframes directly.  

**Approved V1 approach** (Flutter Architecture §12.8):

1. Backend creates payment intent/session (server-owned amount/currency/payable)  
2. Customer app opens a **secured WebView** hosting KHADAMATI-controlled HTML that loads Payment.js  
3. User enters card in hosted fields → `tokenize()` → `transactionToken`  
4. Token returned to Flutter via **JS bridge**  
5. Flutter calls backend debit with token + Idempotency-Key  
6. If 3DS / redirect required → continue in WebView (or external browser if vendor requires — see Open Questions)  
7. Finality via **webhook** (+ client poll / return URL)

| Approach | V1 stance |
|----------|-----------|
| WebView + Payment.js hosted fields | **Required primary path** |
| External browser only (no WebView) | Optional fallback if WebView blocked — **Pending Vendor Confirmation** for 3DS UX |
| Native IXOPAY/TokenEx mobile SDK | Out of V1 constraint; not required to pass this spike |

## 2.2 Android application flow

| Step | Behavior |
|------|----------|
| Entry | After provider confirmation; booking payable in Payable/Pending payment state |
| Load | WebView loads KHADAMATI payment page (HTTPS) with public integration key from backend |
| JS | Payment.js `init` on number/CVV containers; `tokenize` on submit |
| Bridge | `JavascriptChannel` / equivalent returns token or error to Dart |
| Debit | App → `POST /payments/{id}/debit` with token |
| 3DS | If `REQUIRES_ACTION`, continue navigation in WebView; do not trust client-alone finality |
| Return | Observe payment status via API; webhook is source of financial finality |

**Compatibility assessment:** Feasible. Risks: WebView JS bridge reliability, cookie/third-party iframe policies, Android WebView version variance — must be proven in sandbox device test (condition).

## 2.3 iOS application flow

Same logical sequence as Android using `WKWebView` + JS message handlers.

**Compatibility assessment:** Feasible. Risks: ATS HTTPS-only, iframe restrictions, 3DS popup/redirect handling — sandbox device test required (condition).

## 2.4 Token lifecycle

| Phase | Rule |
|-------|------|
| Create | Payment.js tokenize → single-use `transactionToken` |
| Transmit | HTTPS to KHADAMATI API only; never log full token |
| Consume | Server adapter debit once (idempotent merchant transaction id) |
| Expiry / reuse | Treat as single-use; retry = new tokenize or controlled recovery path |
| Storage | **Do not** persist PAN/CVV; optional last4/brand if gateway returns safe metadata |

## 2.5 Callback handling

| Channel | Role |
|---------|------|
| Client success/error callbacks from Payment.js | UX only (tokenize success/fail) |
| Debit API response | Immediate success / requires action / fail |
| Gateway webhook | **Authoritative** finalize for ledger + booking paid |
| Return / deep link URLs | Resume UX; reconcile with server status |

## 2.6 Secure communication requirements

- TLS everywhere (app ↔ API ↔ gateway)  
- Public integration key only in WebView page  
- Merchant/API credentials only on server (secrets manager)  
- WebView loads **first-party** KHADAMATI origin (allowlisted)  
- Certificate pinning: optional hardening — **Pending Security decision** (not invented here)

---

# 3. Payment Lifecycle Validation

## 3.1 Create Payment

| Step | Owner | Validation |
|------|-------|------------|
| Customer confirms booking (provider already confirmed — ADR-005) | Booking domain | In scope |
| System creates payment request/intent with **server amount** | Payment Service | Client must not dictate amount |
| Persist payment **Pending** + idempotency key support | Payment Service | Architecture confirmed |

## 3.2 Customer Payment

| Step | Owner | Validation |
|------|-------|------------|
| Payment.js hosted fields collect card | Gateway iframes | No PAN/CVV to KHADAMATI |
| Tokenize | Payment.js | Returns `transactionToken` |
| App submits token to backend | Customer App | Bridge + debit API |

## 3.3 Gateway Processing

| Step | Owner | Validation |
|------|-------|------------|
| Adapter maps debit command | IXOPAY Adapter | Isolated |
| IXOPAY processes | Gateway | Vendor runtime — Pending sandbox |

## 3.4 Result Handling

| Result | Expected system behavior | Validation |
|--------|--------------------------|------------|
| **Success** | Payment → Paid/Captured; emit domain event; ledger post; booking payment complete | Architecture confirmed |
| **Failure** (decline/error) | Payment → Failed; booking remains unpaid; user may retry under rules | Architecture confirmed |
| **Cancellation** | User abort → Cancelled / abandoned pending; no capture | Architecture confirmed |
| **Timeout** | Stay Pending; reconcile job + webhook decide; no double capture | Architecture confirmed |

## 3.5 Webhook Confirmation

| Control | Validation |
|---------|------------|
| Signature / shared-secret verification | **Required** (architecture); exact algo — **Pending Vendor Confirmation** |
| Idempotency | Process once per gateway event / payment transition |
| Duplicate event handling | Safe no-op if already terminal success |
| Unknown payment id | Log + alert; do not create money |

## 3.6 Ledger Update

Webhook-confirmed (or verified debit final) success → ledger entries → booking payment completion.  
**No money movement outside the ledger** (ADR-004).

---

# 4. Security Validation

## 4.1 Card data

| Control | Status |
|---------|--------|
| No card numbers stored in KHADAMATI DB | **Confirmed (design)** |
| No CVV stored | **Confirmed (design)** |
| No sensitive payment data in logs | **Required** — tokens/PAN masked; enforce in coding standards later |
| Only gateway-safe metadata (e.g. last4/brand) if provided | Allowed |

## 4.2 Communication

| Control | Status |
|---------|--------|
| HTTPS requirements | **Mandatory** |
| Token security (single-use, TLS, no logs) | **Mandatory** |
| API credential protection (server secrets) | **Mandatory** |

## 4.3 Webhooks

| Control | Status |
|---------|--------|
| Authentication / signature | **Mandatory**; mechanism **Pending Vendor Confirmation** |
| Replay protection | Timestamp/nonce or idempotent event keys — implement per vendor docs |
| Duplicate prevention | Idempotent finalize |

## 4.4 Mobile security

| Control | Status |
|---------|--------|
| App ↔ API JWT session | Existing auth model |
| Payment state protection | Server-side state machine; client cannot force Paid |
| WebView hardening | JS bridge allowlist; no arbitrary URL injection; disable file access as applicable |
| Session handling during 3DS | Preserve payment id; re-auth if session expires mid-flow |

---

# 5. Failure Scenario Analysis

## 5.1 Customer cases

| Case | Handling |
|------|----------|
| User closes payment screen | Payment remains Pending/Cancelled per timeout policy; no capture without gateway success |
| Network interruption after tokenize, before debit | No debit; user retries; new tokenize if needed |
| Network interruption after debit request | Payment Pending; webhook/reconcile settles; Idempotency-Key prevents double debit |
| Payment timeout | Reconcile job; user sees pending/failed honestly |
| User retries payment | New attempt under payment state rules; prior Pending resolved first |

## 5.2 Gateway cases

| Case | Handling |
|------|----------|
| Declined payment | Failed; booking unpaid; message from safe gateway codes |
| Gateway unavailable | Fail soft; retry policy at adapter; circuit breaker |
| Duplicate callback | Idempotent finalize |
| Delayed webhook | Client polling + reconcile; ledger only on verified finality |

## 5.3 Booking cases

| Case | Handling |
|------|----------|
| Payment successful but booking update fails | Outbox/retry; payment/ledger truth retained; booking repair job — **must not** reverse money silently |
| Booking cancelled after payment | Refund path via admin-configurable refund policy (ADR-013/026); reversing ledger entries |
| Partial workflow failure | Compensate via explicit refund/cancel policies; never manual DB money edits |

---

# 6. Ledger Impact Validation

## 6.1 Mapping

| Event | Records |
|-------|---------|
| Payment created | Payment transaction Pending |
| Capture / paid confirmed | Payment Paid + **ledger entry(ies)** + booking payment status Paid |
| Failure | Payment Failed; no capture ledger |
| Refund | Refund record + reversing / refund ledger entries |

**Rule:** No money movement outside the ledger (ADR-004). Escrow-ready holds remain architectural capability.

## 6.2 Payment states (validated for V1 model)

| State | Meaning |
|-------|---------|
| Pending | Intent created / awaiting gateway finality |
| Authorized | Pre-auth if used (gateway-dependent — Pending Vendor Confirmation) |
| Paid | Captured / settled success for booking payable |
| Failed | Decline or hard error |
| Cancelled | Abandoned / voided before success |
| Refunded | Full/partial per policy |

Exact gateway mapping for Authorized vs Paid: **Pending Vendor Confirmation**.

---

# 7. Admin Impact Validation

Admin Portal (web only) must be able to view — without manual DB edits:

| Capability | Required |
|------------|----------|
| Payment status | Yes |
| Transaction reference (KHADAMATI + gateway) | Yes |
| Gateway response status / safe codes | Yes |
| Audit history (who/what/when; webhook receipts) | Yes |
| Trigger/monitor refunds per policy + RBAC | Yes (Finance roles) |

Finance configuration remains Admin-configurable (commission/cancel/refund) — not part of this spike to invent values.

---

# 8. Payment Decisions Required

Do **not** invent values. Track and fill exclusively via:

**[`AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md)**

| Decision | Status |
|----------|--------|
| IXOPAY / Areeba sandbox credentials | **Pending Vendor Confirmation** |
| Merchant configuration (MID, connector, callback URL allowlist) | **Pending Vendor Confirmation** |
| Supported payment methods (card brands, Apple/Google Pay, etc.) | **Pending Vendor Confirmation** |
| Supported currencies (Lebanon default USD per ADR-001) | **Pending Vendor Confirmation** (Market config vs gateway enablement) |
| 3DS requirements / challenge flow | **Pending Vendor Confirmation** |
| Mobile redirect behavior (WebView vs external browser) | **Pending Vendor Confirmation** |
| Refund API availability & partial refund support | **Pending Vendor Confirmation** |
| Settlement information / timing toward ledger release | **Pending Vendor Confirmation** (+ Finance BLOCKER-005) |
| Webhook signature algorithm & headers | **Pending Vendor Confirmation** |
| Public integration key + Payment.js script URL (sandbox/prod) | **Pending Vendor Confirmation** |

Also tracked under BLOCKER-003 Payment checklist.

---

# 9. Final Validation Result

## Payment.js Mobile Readiness Result

# PASS WITH CONDITIONS

### Confirmed items

- End-to-end architecture path is coherent and approved  
- Port/adapter isolation and future gateway replaceability  
- PCI minimization via Payment.js hosted fields (no PAN/CVV in KHADAMATI)  
- WebView + JS bridge is the correct V1 mobile hosting model for Payment.js  
- Lifecycle covering create → tokenize → debit → webhook → ledger → booking paid  
- Failure, idempotency, and admin visibility requirements are specified  
- Ledger is mandatory for money movement  

### Risks

| Risk | Severity | Mitigation |
|------|----------|------------|
| Sandbox/credentials unavailable | High | BLOCKER-003; block production pay UI |
| Android/iOS WebView iframe / 3DS quirks | High | Mandatory device sandbox spike before Gate A pay UI |
| Delayed/duplicate webhooks | Medium | Idempotent handler + reconcile job |
| Booking update after pay fails | Medium | Outbox/retry; never silent money reverse |
| Logging token leakage | Medium | Logging standards + review gates |

### Required actions before implementation authorization (conditions)

1. Complete [`AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) §§1–7 with vendor (no invented values)  
2. Obtain Areeba IXOPAY sandbox + merchant config (BLOCKER-003)  
3. Execute live plan scenarios PJS-01…PJS-10 on Android and iOS WebViews ([`PAYMENT_JS_MOBILE_VALIDATION_PLAN.md`](./PAYMENT_JS_MOBILE_VALIDATION_PLAN.md))  
4. Confirm webhook signature verification + 3DS / redirect behavior (checklist §§4–5)  
5. Complete Final Acceptance Checklist (vendor checklist §8): sandbox, Android, iOS, webhook, 3DS, lifecycle, Architect + Eng approval  
6. Eng Lead + Solution Architect accept this report (upgrade to PASS if live tests succeed, or keep PASS WITH CONDITIONS with dated mitigations)  
7. Do **not** implement production payment UI until Implementation Gate → **A**  
8. Keep BLOCKER-007 **IN VALIDATION** until checklist §8 is fully satisfied — then COMPLETED

### Result definitions applied

| Result | Applied? |
|--------|----------|
| PASS | ☐ No — live sandbox not executed |
| **PASS WITH CONDITIONS** | ☑ Architecture feasibility validated; runtime proof pending |
| FAIL | ☐ No — no architectural blocker found against approved design |

---

# 10. Approval of This Report

| Role | Name | Date | Decision |
|------|------|------|----------|
| Solution Architect | | | ☐ Accept PASS WITH CONDITIONS |
| Engineering Lead | | | ☐ Accept · ☐ Request live re-test first |
| Product Owner | | | ☐ Acknowledge conditions |
| Security | | | ☐ Acknowledge PCI approach |

**BLOCKER-007 → COMPLETED** only when result is **PASS** or **accepted PASS WITH CONDITIONS** and required live checks are done or explicitly waived with dates.

Until then: status remains **IN VALIDATION**.

---

# 11. Explicit Non-Authorization

```text
This report does NOT authorize:
- Production payment module coding
- Payment screens
- Payment APIs / DB migrations
- Scope or architecture changes
```

Implementation Gate remains **B) NOT READY — CODING BLOCKED**.

---

**End of Payment.js Mobile Validation Report v1.0**
