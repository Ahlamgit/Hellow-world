# Areeba IXOPAY Payment.js — Verification Report

**Date:** 2026-07-28  
**Branch:** `cursor/areeba-paymentjs-7b80`  
**Base:** `cursor/areeba-adapter-7b80`  
**Directive:** Payment.js migration (Phases A–J)  
**Production cutover:** Not approved — blocked pending sandbox validation (Phase K)

---

## Executive summary

The Areeba integration has been redesigned from a hosted-checkout placeholder to the official IXOPAY Payment.js architecture. Existing payment orchestration (`BookingPayment`, `BookingPaymentAttempt`, webhook idempotency, `BookingService`) is preserved. Moyasar and Development gateways remain unchanged.

Automated tests pass (103 unit, 7 integration). Sandbox validation with live IXOPAY credentials is **not** performed in this phase.

---

## Phase completion

| Phase | Deliverable | Status |
|-------|-------------|--------|
| A | Implementation plan | ✅ Complete |
| B | Additive EF migration (`AreebaPaymentJsFields`) | ✅ Complete |
| C | `IPaymentGateway` contract redesign | ✅ Complete |
| D | `AreebaPaymentGateway` IXOPAY Debit/Status | ✅ Complete |
| E | Web Payment.js integration | ✅ Complete |
| F | IXOPAY webhook (HMAC-SHA512, `OK` response) | ✅ Complete |
| G | `POST /bookings/{id}/payment/authorize` | ✅ Complete |
| H | Android/iOS gateway-aware payment flow | ✅ Complete (API only; no Payment.js WebView) |
| I | Unit tests | ✅ Complete — 103 passed |
| J | Integration tests | ✅ Complete — 7 passed |
| K | Sandbox validation | ⏸ Blocked — credentials required |
| L | This verification report | ✅ Complete |

---

## Architecture compliance

### Gateway abstraction

- `BookingService` contains no IXOPAY, Payment.js, Debit API, or merchant credential logic.
- Provider-specific code is confined to `AreebaPaymentGateway`, `AreebaWebhookService`, and related infrastructure.
- `IPaymentGateway` supports token-based flows via `InitialisePaymentAsync`, `AuthorizeAsync`, and `VerifyAsync`.

### Payment flow (Areeba)

1. Client calls initiate payment → backend creates `BookingPaymentAttempt` and returns Payment.js config (public key, script URL, amount, currency, merchant transaction id).
2. Web loads Payment.js from IXOPAY CDN; card entry occurs in PCI iframe.
3. Client calls `payment.tokenize()` → receives `transactionToken`.
4. Client submits `transactionToken` to `POST /bookings/{id}/payment/authorize`.
5. Backend calls `AuthorizeAsync` (Debit or Preauthorize per config).
6. Attempt transitions to `AwaitingGatewayConfirmation`; booking payment follows.
7. IXOPAY webhook validates signature, idempotency, amount/currency, and finalizes via `BookingService`.

### Preserved behaviour

- Moyasar and Development gateways retain redirect checkout + client-side `ConfirmPayment`.
- `SupportsClientSideConfirmation => false` for Areeba — clients cannot complete booking from token submission alone.
- `RequiresClientAuthorizationHandoff => true` for Areeba — separate token handoff step.
- Webhook + server verification remain the only source of payment truth.

---

## Key changes

### Backend

| Area | Change |
|------|--------|
| `IPaymentGateway` | `CreateSessionAsync` replaced by `InitialisePaymentAsync` + `AuthorizeAsync` |
| `AreebaPaymentGateway` | Payment.js init config; Debit/Preauthorize; Status API verify |
| `AreebaOptions` | `PublicIntegrationKey`, `ApiKey`, `ApiUser`, `ApiPassword`, `SharedSecret`, `PaymentJsScriptUrl`, `TransactionMode` |
| `BookingPaymentAttempt` | `ProviderUuid`, `MerchantTransactionId`, `TransactionToken`, `GatewayStatus`, `ReturnType`, `RedirectUrl`, `ThreeDSReference`, `FailedAt`, `WebhookEventId`, `RetryReason`, `RetryCount` |
| State machine | `AwaitingGatewayConfirmation`, `Expired`, `Abandoned` |
| Webhook | IXOPAY postback schema; HMAC-SHA512 Base64 signature; `200 OK` body |
| API | `POST /bookings/{id}/payment/authorize` |

### Web

- `PaymentJsCheckout.tsx` — loads Payment.js, iframe card fields, tokenize, authorize.
- `BookingPages.tsx` — gateway-aware: Payment.js path vs Moyasar/Dev checkout.

### Mobile

- Android/iOS no longer auto-confirm when `requiresClientAuthorizationHandoff` is true.
- Authorize API endpoint added; Payment.js WebView **not** implemented (users directed to web for card entry).

### Database

- Migration: `20260728063807_AreebaPaymentJsFields` (additive only).

---

## Test results

```
Unit tests:        103 passed, 0 failed
Integration tests:   7 passed, 0 failed
```

Coverage includes:

- Areeba gateway initialise/authorize/verify
- IXOPAY webhook signature validation and idempotency
- Integration readiness for new Areeba config keys
- Existing booking/payment integration flows

---

## Known gaps and follow-ups

| Item | Severity | Notes |
|------|----------|-------|
| Sandbox validation (Phase K) | Required before production | Needs IXOPAY sandbox credentials |
| Mobile Payment.js WebView | Medium | API aligned; card entry web-only for now |
| Live Payment.js CDN test | Medium | Requires sandbox merchant key |
| 3DS REDIRECT handling (web) | Medium | Backend stores `RedirectUrl`; full redirect UX not sandbox-tested |
| Preauth + capture split | Low | Configurable via `TransactionMode`; capture flow not separately tested |

---

## Security review (implementation-level)

| Control | Status |
|---------|--------|
| API credentials server-side only | ✅ |
| Frontend receives public integration key only | ✅ |
| Webhook HMAC-SHA512 validation | ✅ Implemented |
| Amount/currency/merchant transaction verification | ✅ Implemented |
| Webhook idempotency | ✅ Preserved |
| Completed-attempt protection | ✅ Preserved |
| Client cannot finalize Areeba payment | ✅ `SupportsClientSideConfirmation = false` |

Full security review for production cutover remains pending.

---

## Production cutover checklist

- [ ] Phase K sandbox validation complete (`AREEBA_SANDBOX_VALIDATION_REPORT.md`)
- [ ] Webhook endpoint reachable from IXOPAY production
- [ ] Security review approved
- [ ] Performance review approved
- [ ] Rollback tested
- [ ] Executive approval granted
- [ ] `Payment:Provider` changed in production (currently Moyasar)

**Do not enable Areeba in production until all items are checked.**

---

## Recommendation

Approve this branch for merge into the payment hardening line after review. Proceed to Phase K (sandbox validation) only when IXOPAY sandbox credentials are available. Keep Moyasar as the active production provider until sandbox passes and executive approval is granted.
