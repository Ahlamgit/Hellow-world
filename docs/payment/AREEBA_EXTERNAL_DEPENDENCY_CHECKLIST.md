# Areeba — External Dependency Checklist

**Status:** Validation **blocked** — waiting on Areeba external dependencies  
**Date:** 2026-07-23  
**Scope:** Documentation only — no implementation changes in this checkpoint

---

## Blocker summary

Areeba sandbox validation cannot proceed until the items in Section 1 are supplied by Areeba (or the merchant onboarding team). Internal payment plumbing exists at the platform level; **Areeba-specific integration and live validation remain pending** credentials and API access.

**Explicitly out of scope for this checkpoint:** Phase 1C, React Native migration, Moyasar removal, and any new code changes.

---

# 1. Required external items

Checklist of items that must be obtained from **Areeba** or the merchant onboarding process before sandbox validation can begin.

| # | Item | Owner | Received | Notes |
|---|------|-------|----------|-------|
| 1 | **Areeba sandbox merchant account** | Areeba / merchant onboarding | ☐ | Merchant ID, account status, and sandbox environment access |
| 2 | **Sandbox API credentials** | Areeba | ☐ | API key, secret key, or equivalent authentication material for sandbox API |
| 3 | **Webhook credentials / configuration** | Areeba | ☐ | Webhook signing secret, signature algorithm documentation, and event payload schema |
| 4 | **Allowed callback URLs** | Areeba + Khadamati ops | ☐ | Whitelist for server webhook URL and browser return URLs (success / cancel) |
| 5 | **Test cards / payment methods** | Areeba | ☐ | Sandbox card numbers, Mada/local methods if applicable, and expected success/failure scenarios |
| 6 | **Staging environment URL** | Khadamati ops (with Areeba URL whitelist) | ☐ | Public HTTPS API host for staging webhooks (e.g. `https://staging-api.khadamati.com`) |

### Proposed Khadamati URLs (for Areeba whitelist — confirm with Areeba)

| Purpose | Proposed URL |
|---------|----------------|
| Server webhook | `https://<staging-api-host>/api/v1/webhooks/areeba` |
| Payment success redirect | `https://<staging-web-host>/pay/success` |
| Payment cancel redirect | `https://<staging-web-host>/pay/cancel` |

### Proposed configuration keys (to populate when credentials arrive)

| Key | Source |
|-----|--------|
| `Payment:Provider` | Set to `Areeba` in staging |
| `Payment:Areeba:MerchantId` | Areeba sandbox account |
| `Payment:Areeba:SecretKey` | Areeba sandbox API credentials |
| `Payment:Areeba:WebhookSecret` | Areeba webhook configuration |
| `Payment:Areeba:ApiBaseUrl` | Areeba sandbox API base URL |
| `Payment:Areeba:CallbackUrl` | Staging webhook URL (whitelisted) |
| `Payment:Areeba:SuccessUrl` | Staging success redirect (whitelisted) |
| `Payment:Areeba:CancelUrl` | Staging cancel redirect (whitelisted) |

---

# 2. Current technical readiness

## Implemented (internal platform — not yet validated against Areeba)

| Component | Status | Repository evidence |
|-----------|--------|---------------------|
| **Areeba gateway adapter** | ☐ **Not implemented** | `IPaymentGateway` abstraction exists; current providers are `Development` and `Moyasar` only. Areeba adapter is **blocked** pending API documentation and sandbox credentials. |
| **Payment attempts model** | ✅ **Implemented** (gateway-agnostic) | `BookingPayment` entity + `BookingPayments` table (`20260708065052_BookingModule`). Tracks amount, status, method, and `TransactionReference` per booking. |
| **Webhook processing** | ⚠️ **Partial** | Moyasar webhook path implemented (`PaymentWebhookService`, `POST /api/v1/webhooks/moyasar`, HMAC-SHA256). Areeba-specific webhook endpoint and signature logic **not yet implemented**. |
| **Idempotency handling** | ✅ **Implemented** (booking layer) | `ConfirmPaymentFromWebhookAsync` returns existing booking if payment already `Completed`; status guards prevent double-finalize. |
| **Mobile checkout flow** | ⚠️ **Partial** | Android/iOS call `POST /bookings/{id}/payment` and `/payment/confirm`. Hosted checkout (WebView / deep link) **not production-ready** for a real PSP — works with `Development` gateway only. |
| **Database migration** | ✅ **Implemented** | `BookingPayments` table and related booking payment fields applied via EF migrations. No Areeba-specific schema required. |

### Supporting infrastructure already in place

| Layer | What exists |
|-------|-------------|
| **API** | `POST /api/v1/bookings/{id}/payment`, `POST /api/v1/bookings/{id}/payment/confirm` |
| **Orchestration** | `BookingService.InitiatePaymentAsync`, `ConfirmPaymentAsync`, `ConfirmPaymentFromWebhookAsync` |
| **Abstraction** | `IPaymentGateway` — `CreateSessionAsync`, `VerifyAsync` |
| **Web** | `BookingPaymentPage`, `/pay` checkout page, initiate + confirm API calls |
| **Admin** | Read-only payment list/detail (`AdminPaymentsController`) |
| **Tests** | `PaymentWebhookServiceTests`, `BookingServiceTests` (payment confirm paths) |

## Not validated

The following cannot be confirmed until Section 1 external items are received and Areeba integration is wired to staging:

| Validation | Blocked by |
|------------|------------|
| **Real gateway communication** | No Areeba sandbox credentials; no `AreebaPaymentGateway` in repository |
| **Real webhook delivery** | No Areeba webhook URL whitelisted; no Areeba webhook handler deployed |
| **Real checkout completion** | No end-to-end test against Areeba hosted checkout; mobile clients do not complete hosted checkout flow |

---

# 3. Next execution steps

**Do not execute until Section 1 items are received and checked.**

## Step 1 — Configure sandbox environment

- [ ] Receive and securely store Areeba sandbox credentials (Key Vault / env vars — not committed to git)
- [ ] Populate staging `Payment:Areeba:*` configuration keys
- [ ] Set `Payment:Provider=Areeba` in staging only (keep `Development` for local dev; **do not remove Moyasar**)
- [ ] Confirm Areeba has whitelisted staging webhook and redirect URLs
- [ ] Implement `AreebaPaymentGateway` + Areeba webhook handler (future implementation wave — **not in this checkpoint**)

## Step 2 — Deploy staging

- [ ] Apply any pending EF migrations to staging database
- [ ] Deploy API build with Areeba provider configuration to staging host
- [ ] Verify `GET /api/v1/health/ready` and `GET /api/v1/health/integrations` report payment provider ready
- [ ] Confirm webhook endpoint is reachable from Areeba sandbox (firewall / TLS)

## Step 3 — Run payment test matrix

| # | Scenario | Expected result |
|---|----------|-----------------|
| 1 | Initiate payment for `AwaitingPayment` booking | `checkoutUrl` returned; `BookingPayment` → `Processing` |
| 2 | Complete payment via Areeba hosted checkout (web) | Redirect to success URL |
| 3 | Areeba webhook delivered (paid/success) | HMAC/signature valid; booking → `PendingCraftsmanConfirmation` |
| 4 | Duplicate webhook delivery | Idempotent — no double charge; booking unchanged after first completion |
| 5 | Failed / declined test card | Payment remains incomplete; booking stays `AwaitingPayment` |
| 6 | Expired payment window | Booking payment window enforced (`PaymentDueAt`) |
| 7 | Mobile initiate → hosted checkout → return | Checkout opens; confirm or webhook completes booking |
| 8 | Client confirm after webhook | Idempotent — no error; booking already completed |

## Step 4 — Collect evidence

- [ ] Screenshots or recordings of successful Areeba checkout (web + mobile)
- [ ] Webhook request/response logs (redact secrets)
- [ ] Database rows: `BookingPayments` status `Completed`, correct `TransactionReference`
- [ ] Booking status transition: `AwaitingPayment` → `PendingCraftsmanConfirmation`
- [ ] Slot reservation confirmed (no duplicate booking on same slot)
- [ ] Failed-payment and duplicate-webhook test results
- [ ] Store evidence in `docs/payment/AREEBA_SANDBOX_VALIDATION_REPORT.md` (create when Step 3 completes)

## Step 5 — Approve or reject production cutover

| Outcome | Criteria | Action |
|---------|----------|--------|
| **Approve** | All Step 3 scenarios pass; evidence collected; no P1/P2 defects | Schedule production `Payment:Provider=Areeba`; keep Moyasar webhook active for in-flight transactions during overlap window |
| **Reject** | Any critical failure (webhook, verify, mobile checkout, idempotency) | Remain on current provider; document defects; do **not** remove Moyasar |
| **Defer** | External dependency still missing | Remain blocked; update this checklist |

---

## Checkpoint

| Rule | Status |
|------|--------|
| No code changes in this checkpoint | ✅ |
| No Phase 1C | ✅ |
| No React Native migration | ✅ |
| No Moyasar removal | ✅ |
| Areeba validation | **BLOCKED** — awaiting Section 1 external items |

**STOP.** Await Areeba sandbox credentials and merchant onboarding before any further payment implementation or validation work.
