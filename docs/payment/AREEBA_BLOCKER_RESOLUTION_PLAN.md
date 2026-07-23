# Areeba — Blocker Resolution Plan & Remediation Validation

**Status:** Remediation validation phase  
**Date:** 2026-07-23  
**Branch:** `cursor/payment-gateway-migration-plan-4876`  
**Related:** [AREEBA_SANDBOX_VALIDATION_REPORT.md](./AREEBA_SANDBOX_VALIDATION_REPORT.md), [AREEBA_GO_LIVE_DECISION.md](./AREEBA_GO_LIVE_DECISION.md)

**Constraint:** Production `Payment:Provider` must remain **Moyasar**. Do not remove Moyasar. Do not start Phase 1C or React Native.

---

## Blocker map

| Blocker | Resolution approach | Status after this phase |
|---------|---------------------|-------------------------|
| B1 Areeba sandbox credentials | Document required env/setup; await Ops secrets | **OPEN** — credentials still unavailable |
| B2 Staging SQL migration | Run SQL Server 2022 locally (Docker vfs) + EF update + validation queries + rollback | **RESOLVED in agent** (local SQL Server); **Staging host still pending Ops** |
| B3 Live sandbox payment lifecycle | Document matrix; execute via unit tests where possible; live MPGS pending B1 | **PARTIAL** — unit PASS; live E2E OPEN |
| B4 Browser/device QA | Document QA scripts; cannot run devices here | **OPEN** |

---

# 1. SANDBOX ENVIRONMENT SETUP

## 1.1 Required Areeba sandbox configuration

| Setting | Env var | Purpose |
|---------|---------|---------|
| Provider (Staging only) | `Payment__Provider=Areeba` | Activate adapter (**never set on Production until go-live**) |
| Merchant id | `Payment__Areeba__MerchantId` | MPGS merchant |
| API username | `Payment__Areeba__ApiUsername` | Usually `merchant.{MerchantId}` |
| API password | `Payment__Areeba__ApiPassword` | ePayment portal API password |
| API base URL | `Payment__Areeba__ApiBaseUrl` | Default `https://epayment.areeba.com` |
| API version | `Payment__Areeba__ApiVersion` | Default `100` |
| Webhook secret | `Payment__Areeba__WebhookSecret` | Signature validation |
| Callback URL | `Payment__Areeba__CallbackUrl` | `https://{staging-api}/api/v1/webhooks/areeba` |
| Success URL | `Payment__Areeba__SuccessUrl` | Web/app return |
| Cancel URL | `Payment__Areeba__CancelUrl` | Cancel/timeout return |
| Optional hosted URL template | `Payment__Areeba__HostedCheckoutUrl` | `{sessionId}` / `{orderId}` placeholders |

Also keep Moyasar env vars available for fallback regression (`Payment__Moyasar__*`).

## 1.2 Rules

| Rule | Status |
|------|--------|
| Never commit secrets | **PASS** — repo uses empty / `${…}` placeholders only |
| Use local env vars or secret manager | **Documented** — Staging should inject via host secrets / CI vault |
| Keep production credentials isolated | **PASS** — Production remains `Provider=Moyasar`; Areeba prod secrets must not be used on Staging |

## 1.3 Staging `.env` template (do not commit filled values)

```bash
Payment__Provider=Areeba
Payment__Areeba__MerchantId=
Payment__Areeba__ApiUsername=
Payment__Areeba__ApiPassword=
Payment__Areeba__WebhookSecret=
Payment__Areeba__ApiBaseUrl=https://epayment.areeba.com
Payment__Areeba__ApiVersion=100
Payment__Areeba__CallbackUrl=https://staging-api.example/api/v1/webhooks/areeba
Payment__Areeba__SuccessUrl=https://staging-web.example/pay/success
Payment__Areeba__CancelUrl=https://staging-web.example/pay/cancel
```

**B1 remains OPEN** until Ops supplies sandbox merchant credentials.

---

# 2. STAGING DATABASE VALIDATION

## 2.1 Environment used for remediation

| Item | Result |
|------|--------|
| Engine | Microsoft SQL Server **2022** (RTM-CU26) Developer Edition on Linux |
| Hosting | Docker `mcr.microsoft.com/mssql/server:2022-latest` with **vfs** storage driver (overlayfs failed earlier) |
| Database | `KhadamatiDb` |
| EF tools | `dotnet ef database update` against local container |

> Note: This validates migration **compatibility** with SQL Server 2022. Applying the same migrations on the real Staging host remains an Ops checklist item (same scripts).

## 2.2 Migrations executed

| Migration | Result |
|-----------|--------|
| Full chain through `AddBookingPaymentAttempts` | **PASS** |
| `FixPaymentProviderDefault` (default `Development` on `PaymentProvider`) | **PASS** — added this phase to clear pending model changes |

### Migration execution result

```
dotnet ef database update ... --connection "Server=127.0.0.1,1433;Database=KhadamatiDb;..."
→ Done. (success)
```

`__EFMigrationsHistory` contains:

- `20260723210419_AddBookingPaymentAttempts`
- `20260723212353_FixPaymentProviderDefault`

## 2.3 Validation queries (executed)

| Check | Result |
|-------|--------|
| `BookingPaymentAttempts` table exists | **PASS** |
| Header columns on `BookingPayments` | **PASS** (`PaymentProvider`, `CurrentAttemptId`, gateway ids, `FailedAt`) |
| Indexes: `(BookingPaymentId, AttemptNumber)`, `GatewayTransactionId`, `WebhookEventId`, one-Completed | **PASS** |
| FK payment ↔ attempts | **PASS** |
| Existing payment preserved + backfill | **PASS** — seeded `inv_moyasar_preserve_1` survived; `PaymentProvider=Moyasar`; attempt `#1` created; `CurrentAttemptId` set |

### Preservation evidence

| Field | Value after migrate |
|-------|---------------------|
| `TransactionReference` | `inv_moyasar_preserve_1` (unchanged) |
| `Amount` | `100.00` (unchanged) |
| `PaymentProvider` | `Moyasar` (backfilled) |
| Attempt count | `1` |
| Attempt `GatewayTransactionId` | `inv_moyasar_preserve_1` |

## 2.4 Rollback confirmation

| Step | Result |
|------|--------|
| `dotnet ef database update 20260709142854_FixRescheduledFromCascade` | **PASS** |
| `BookingPaymentAttempts` absent after rollback | **PASS** |
| `PaymentProvider` column absent after rollback | **PASS** |
| Re-apply to latest | **PASS** — table/indexes restored |

**B2 status:** **RESOLVED for SQL Server 2022 compatibility in agent.** Staging cloud host apply still required before declaring Staging fully green.

---

# 3. AREEBA SANDBOX TEST MATRIX

Legend: **PASS** / **FAIL** / **BLOCKED** (cannot run without credentials or live gateway).

## 3.1 Payment success

| Field | Value |
|-------|-------|
| Input | Create booking → initiate Areeba session → complete MPGS checkout → webhook paid |
| Expected | Attempt+payment `Completed`; booking advances; slot reserved |
| Actual | **BLOCKED** live. Unit: session create + HMAC webhook confirm + verify captured **PASS** |
| Database state | N/A live. Design: one Completed attempt via filtered unique index |

## 3.2 Payment failure

| Field | Value |
|-------|-------|
| Input | Declined card / verify non-success status |
| Expected | Payment/attempt `Failed`; booking stays `AwaitingPayment` |
| Actual | **BLOCKED** live. Code path sets Failed on verify failure |
| Database state | N/A live |

## 3.3 Payment abandonment

| Field | Value |
|-------|-------|
| Input | User opens checkout then leaves without paying |
| Expected | Remains `Processing` / `AwaitingGatewayConfirmation`; not Completed |
| Actual | **BLOCKED** live. Clients do not auto-confirm (**code PASS**) |
| Database state | N/A live |

## 3.4 Duplicate webhook

| Field | Value |
|-------|-------|
| Input | Same `WebhookEventId` delivered twice |
| Expected | Second ignored / idempotent; no double completion |
| Actual | Unit duplicate path **PASS**; unique index on `WebhookEventId` **PASS** (SQL). Live **BLOCKED** |
| Database state | Index `IX_BookingPaymentAttempts_WebhookEventId` present |

## 3.5 Invalid signature webhook

| Field | Value |
|-------|-------|
| Input | Bad `X-Areeba-Signature` |
| Expected | `401` / Unauthorized; no DB mutation |
| Actual | Unit **PASS**. Live Staging **BLOCKED** |
| Database state | Unchanged on reject |

## 3.6 Wrong amount

| Field | Value |
|-------|-------|
| Input | Order amount ≠ `BookingPayment.Amount` |
| Expected | `VerifyAsync` fails; not Completed |
| Actual | Unit `VerifyAsync_WrongAmount_Fails` **PASS**. Live **BLOCKED** |
| Database state | N/A live |

## 3.7 Wrong currency

| Field | Value |
|-------|-------|
| Input | Order currency ≠ payment currency |
| Expected | Reject |
| Actual | Unit `VerifyAsync_WrongCurrency_Fails` **PASS**. Live **BLOCKED** |
| Database state | N/A live |

## 3.8 Delayed webhook

| Field | Value |
|-------|-------|
| Input | Success webhook arrives after customer returned / refreshed |
| Expected | Idempotent complete if not yet Completed; no overwrite if already Completed |
| Actual | **BLOCKED** live. Code short-circuits on Completed + event id |
| Database state | N/A live |

---

# 4. CLIENT QA

## 4.1 Web

| Browser | Checkout redirect | Return / refresh | Pending | Completed | Status |
|---------|-------------------|------------------|---------|-----------|--------|
| Chrome (desktop) | Code supports `location.assign(checkoutUrl)` | Refresh status API | Code PASS | Navigate when status changes | **BLOCKED** live |
| Mobile browser | Same page responsive | Same | Code PASS | Code PASS | **BLOCKED** live |

## 4.2 Android

| Check | Status |
|-------|--------|
| Checkout opening (`ACTION_VIEW`) | **PASS (code)** — live device **BLOCKED** |
| Return / deep link | Partial (browser return + manual refresh); deep link polish TBD — **BLOCKED** live |
| Payment refresh | **PASS (code)** |

## 4.3 iOS

| Check | Status |
|-------|--------|
| Checkout opening (`UIApplication.open`) | **PASS (code)** — live device **BLOCKED** |
| Return / deep link | Partial — **BLOCKED** live |
| Payment refresh | **PASS (code)** |

### QA script (for Staging when ready)

1. Point apps/web to Staging API with `Payment:Provider=Areeba`.  
2. Customer booking → Pay → confirm external checkout opens.  
3. Complete/cancel/fail in Areeba UI.  
4. Return to app/web → Refresh until status updates.  
5. Confirm UI never shows Completed before API does.

---

# 5. FINAL GO-LIVE CHECKLIST

See updated [AREEBA_GO_LIVE_DECISION.md](./AREEBA_GO_LIVE_DECISION.md).

| Gate | Required for READY |
|------|--------------------|
| Sandbox credentials on Staging | Pending |
| Staging host SQL migrate (same as validated here) | Pending Ops |
| Live matrix §3 all PASS | Pending |
| Web Chrome + mobile browser QA | Pending |
| Android + iOS device QA | Pending |
| Production Provider still Moyasar until approval | **PASS** |

---

## Remediation phase outcomes

| Outcome | Detail |
|---------|--------|
| Unblocked | SQL Server 2022 migration apply, indexes, preservation backfill, rollback/re-apply |
| New migration | `FixPaymentProviderDefault` |
| Still blocked | Live Areeba sandbox + real Staging host + browser/device QA |
| Production | **Not enabled** |

---

*End of blocker resolution plan.*
