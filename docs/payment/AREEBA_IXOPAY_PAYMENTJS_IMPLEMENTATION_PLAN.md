# Areeba IXOPAY Payment.js — Implementation Plan

**Status:** Complete (Phases A–J); Phase K blocked  
**Date:** 2026-07-28  
**Branch:** `cursor/areeba-paymentjs-7b80`  
**Supersedes:** Hosted-checkout Areeba adapter (`AREEBA_IXOPAY_PAYMENTJS_ARCHITECTURE_AUDIT.md`)

---

## Objective

Migrate Areeba integration to official IXOPAY Payment.js architecture while preserving `BookingPayment`, `BookingPaymentAttempt`, `PaymentWebhookEvent`, gateway abstraction, and Moyasar/Development providers.

---

## Phases

| Phase | Deliverable | Status |
|-------|-------------|--------|
| A | This implementation plan | ✅ |
| B | Additive EF migration for Payment.js attempt fields + state enums | ✅ |
| C | `IPaymentGateway` redesign (`InitialisePayment`, `Authorize`, `Verify`) | ✅ |
| D | `AreebaPaymentGateway` IXOPAY Debit/Status API | ✅ |
| E | Web Payment.js checkout page | ✅ |
| F | IXOPAY webhook (HMAC-SHA512, postback schema, `OK` response) | ✅ |
| G | `POST /bookings/{id}/payment/authorize` + BookingService integration | ✅ |
| H | Android/iOS: remove auto-confirm; gateway-aware initiate/authorize | ✅ (API; no Payment.js WebView) |
| I | Unit tests (gateway, webhook, authorize, state machine) | ✅ 103 passed |
| J | Integration tests | ✅ 7 passed |
| K | Sandbox validation (credentials required) | ⏸ Blocked |
| L | Verification report | ✅ `AREEBA_IXOPAY_PAYMENTJS_VERIFICATION_REPORT.md` |

---

## Preserved components

- `BookingPayment`, `BookingPaymentAttempt`, `PaymentWebhookEvent`
- Webhook idempotency and completed-attempt protection
- `BookingService` orchestration (no IXOPAY types)
- Moyasar + Development gateways (redirect + client confirm)
- `SupportsClientSideConfirmation => false` for Areeba booking finalization

---

## New concepts

| Concept | Purpose |
|---------|---------|
| `RequiresClientAuthorizationHandoff` | Client must submit `transactionToken` via `/payment/authorize` |
| `InitialisePaymentAsync` | Returns Payment.js config (Areeba) or checkout URL (Moyasar/Dev) |
| `AuthorizeAsync` | Server-side Debit/Preauthorize with `transactionToken` |
| `AwaitingGatewayConfirmation` | Payment/attempt state between authorize and webhook |

---

## Out of scope

- Production `Payment:Provider=Areeba`
- Moyasar removal
- Phase 1C, React Native migration
- Sandbox validation without credentials

---

## Stop condition

Complete through Phase J + verification report. Await approval before Phase K sandbox and production cutover.
