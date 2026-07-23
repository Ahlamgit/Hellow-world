# KHADAMATI — Areeba Implementation Plan (MPGS Hosted Checkout)

**Status:** **Approved for implementation** (revised + attempt model locked)  
**Date:** 2026-07-23  

**Authoritative addendum:** [AREEBA_IMPLEMENTATION_PLAN_UPDATE.md](./AREEBA_IMPLEMENTATION_PLAN_UPDATE.md)  
**DB plan:** [AREEBA_DATABASE_MIGRATION_PLAN.md](./AREEBA_DATABASE_MIGRATION_PLAN.md)  
**Risks:** [AREEBA_RISK_ASSESSMENT.md](./AREEBA_RISK_ASSESSMENT.md)

`BookingPaymentAttempts` is **required**. Do not store multiple gateway attempts only inside `BookingPayment`.

## Objective (revised)

This is **not** a Moyasar → Areeba swap.  
The objective is to make KHADAMATI **payment architecture production-ready and gateway-independent**, with Areeba MPGS Hosted Checkout as the first production adapter.

| Goal | Meaning |
|------|---------|
| Gateway-independent | Booking business logic never depends on Areeba, Moyasar, or any PSP SDK |
| Production-ready | Secure webhooks, idempotency, amount/currency checks, payment history, mobile hosted checkout |
| Multi-gateway | Areeba (target), Moyasar (temporary), Development (dev/CI) coexist via config |
| Future-proof | New gateways plug in behind `IPaymentGateway` without changing booking workflows |

**Approved inputs:**

| Decision | Status |
|----------|--------|
| Production target = Areeba MPGS Hosted Checkout | Approved |
| Keep `IPaymentGateway` unchanged (as the seam) | Approved |
| Keep `DevelopmentPaymentGateway` | Approved |
| Keep Moyasar temporarily during migration | Approved |
| Mobile never marks payment completed | Approved |
| Webhook + server-side verification = only completion authority | Approved |
| Phase 1C still blocked | Approved |

**Parent docs:**

- [PAYMENT_GATEWAY_MIGRATION_PLAN.md](./PAYMENT_GATEWAY_MIGRATION_PLAN.md)
- [AREEBA_IMPLEMENTATION_DECISION.md](./AREEBA_IMPLEMENTATION_DECISION.md)

**Stop condition:** Do not implement code, migrations, UI, or Phase 1C until this revised plan is approved.

---

## Executive summary

| Workstream | Deliverable |
|------------|-------------|
| Architecture rule | Booking orchestration stays gateway-agnostic; adapters only in Infrastructure |
| Database | Correlation columns + **payment attempt history** for abandon/retry/switch/delayed webhooks |
| State machine | Explicit payment states including awaiting gateway confirmation, failed, cancelled, expired |
| Clients | Hosted checkout only; never finalize payment |
| Security | Signature, secrets, replay, idempotency, amount/currency/ownership checks |
| Migration | Six phases; Moyasar removed only after successful production period |
| Tests | Backend + mobile matrix covering success, failure, duplicates, mismatches |

---

# 1. PAYMENT ARCHITECTURE RULE

## 1.1 Keep (non-negotiable)

```
Customers / Web / Mobile
        ↓
Booking APIs (initiate payment, status read)
        ↓
BookingService / payment orchestration   ← gateway-agnostic
        ↓
IPaymentGateway
   ├── AreebaPaymentGateway      (production target)
   ├── MoyasarPaymentGateway     (temporary coexistence)
   └── DevelopmentPaymentGateway (development/testing only)
        ↓
Provider webhooks → signature verify → VerifyAsync → finalize booking
```

| Rule | Detail |
|------|--------|
| Keep `IPaymentGateway` | `CreateSessionAsync` + `VerifyAsync` (+ `ProviderName`) |
| Orchestration location | `BookingService` and payment/webhook application services only |
| No Areeba in booking workflows | No MPGS types, URLs, or headers inside booking state transitions |
| Multi-provider | Booking must work with Areeba, Moyasar, and Development via `Payment:Provider` |
| Future gateways | Add a new `IPaymentGateway` implementation + DI case + webhook adapter — **no** booking business-logic changes |

## 1.2 What must not happen

| Anti-pattern | Why forbidden |
|--------------|---------------|
| `if (provider == "Areeba")` inside booking domain transitions | Couples marketplace logic to one PSP |
| Clients setting `PaymentStatus.Completed` | Clients are not completion authority |
| Silent Development fallback when Areeba/Moyasar misconfigured in Staging/Production | Hides outages; fake “paid” risk |
| Removing Moyasar before soak | Blocks rollback and in-flight drain |

## 1.3 Implementation scope (exact changes)

### Backend

| Change | Detail |
|--------|--------|
| `AreebaPaymentGateway` | New Infrastructure adapter for MPGS hosted checkout |
| Configuration | `Payment:Areeba:*` secrets/URLs; `Payment:Provider` accepts `Areeba` |
| DI registration | `case "areeba"` beside existing Moyasar + Development |
| Webhook handling | `POST /api/v1/webhooks/areeba` → verify → gateway-agnostic finalize |
| Logging / errors | Structured Serilog; no secrets/PAN; map failures to existing exception types |
| Orchestration updates | Persist provider/attempt/correlation fields only through agnostic services |

### Database

| Change | Detail |
|--------|--------|
| Required migrations | Additive only (see §2) |
| Backward compatibility | Existing APIs and rows keep working |
| Rollback | Config rollback + additive columns left in place preferred |

### Web

| Change | Detail |
|--------|--------|
| Checkout redirect | Open/redirect to `checkoutUrl` |
| Status display | Pending / awaiting confirmation / completed / failed from **API only** |

### Mobile

| Change | Detail |
|--------|--------|
| Flow correction | Remove initiate→immediate confirm |
| States | Pending / completed / failed from API |
| Deep link / return | Refresh status; never mark paid locally |

---

# 2. DATABASE DESIGN REVIEW

## 2.1 Current `BookingPayments` schema (verified)

From EF + `20260708065052_BookingModule`:

| Column | SQL | Nullable | Notes |
|--------|-----|----------|-------|
| `Id` | `uniqueidentifier` | NO | PK |
| `ServiceRequestId` | `uniqueidentifier` | NO | **Unique** → enforces 1:0..1 payment per booking today |
| `PayerUserId` / `PayeeUserId` | `uniqueidentifier` | NO | |
| `Amount` / `Currency` | `decimal(18,2)` / `nvarchar(3)` | NO | |
| `Status` | `int` | NO | `PaymentStatus` enum |
| `PaymentMethod` | `nvarchar(50)` | NO | |
| `TransactionReference` | `nvarchar(200)` | YES | Overloaded session/invoice id |
| `PaidAt` | `datetime2` | YES | Acts as completed timestamp today |
| `FailureReason` | `nvarchar(max)` | YES | **Already exists** |
| `CreatedAt` | via `BaseEntity` | NO | **Already exists** (`CreatedAt`) |

**Not present today:** `PaymentProvider`, `GatewaySessionId`, `GatewayTransactionId`, `WebhookEventId`, attempt history, `GatewayStatus`, `FailedAt`, `CompletedAt` (distinct from `PaidAt`).

### Production gap

A customer may:

1. Start payment  
2. Abandon checkout  
3. Retry payment  
4. Switch gateway (during migration / rollback)  
5. Receive delayed webhook events  

The current **1:0..1** `ServiceRequestId` unique constraint **cannot** store attempt history. Overwriting one row loses troubleshooting data and complicates delayed webhooks for abandoned sessions.

## 2.2 Design decision — payment header + attempts

**Recommended model (gateway-independent):**

```
BookingPayments              ← one current/active payment header per booking (keep 1:0..1)
   └── BookingPaymentAttempts  ← N attempts (history)  [NEW TABLE]
```

| Entity | Role |
|--------|------|
| `BookingPayment` | Current payment aggregate for the booking (amount, currency, current status, active attempt pointer) |
| `BookingPaymentAttempt` | Each initiate/checkout try: provider, session/order ids, gateway status, timestamps, webhook event ids |

This supports abandon/retry/switch/delayed webhooks without deleting history.

**Alternative (not preferred):** drop unique `ServiceRequestId` and store many `BookingPayments` rows per booking — conflicts with existing DTO/API assumptions of a single `payment` on booking.

## 2.3 Columns on `BookingPayments` (header) — add

| Column | Type | Max | Null | Purpose |
|--------|------|-----|------|---------|
| `PaymentProvider` | `nvarchar(50)` | 50 | NOT NULL, default `'Development'` | Active/last provider |
| `GatewaySessionId` | `nvarchar(200)` | 200 | NULL | Active attempt session id (denormalized for fast API) |
| `GatewayTransactionId` | `nvarchar(200)` | 200 | NULL | Active attempt transaction/order id |
| `WebhookEventId` | `nvarchar(200)` | 200 | NULL | Last processed event on active attempt (optional denorm) |
| `CurrentAttemptId` | `uniqueidentifier` | — | NULL | FK to active `BookingPaymentAttempt` |
| `CompletedAt` | `datetime2` | — | NULL | When moved to Completed (can mirror/replace use of `PaidAt`; keep `PaidAt` for compat or set both) |
| `FailedAt` | `datetime2` | — | NULL | When moved to Failed/Expired/Cancelled terminal failure |

**Already present — reuse:**

| Column | Action |
|--------|--------|
| `CreatedAt` | Keep (`BaseEntity`) — do **not** add duplicate |
| `FailureReason` | Keep — update on failure paths |
| `PaidAt` | Keep for API compat; set together with `CompletedAt` on success |

## 2.4 Evaluate — attempt fields

| Proposed field | Decision | Where | Rationale |
|----------------|----------|-------|-----------|
| `PaymentAttemptId` | **Add** | Attempt table PK (`Id`) | Stable id for troubleshooting and webhook correlation |
| `PaymentAttemptNumber` | **Add** | Attempt table `int NOT NULL` | Monotonic per booking payment (1, 2, 3…) |
| `GatewayStatus` | **Add** | Attempt table `nvarchar(50) NULL` | Raw PSP status string for ops (e.g. `CAPTURED`, `FAILED`) without polluting domain enum |
| `CreatedAt` | **Reuse/add** | Attempt: own `CreatedAt`; header already has it | Attempt timeline |
| `CompletedAt` | **Add** | Header + attempt | Success timestamp clarity |
| `FailedAt` | **Add** | Header + attempt | Failure/expiry/cancel timestamp |
| `FailureReason` | **Reuse/add** | Header exists; add on attempt | Per-attempt reason |

### Proposed `BookingPaymentAttempts` table

| Column | SQL | Null | Notes |
|--------|-----|------|-------|
| `Id` (`PaymentAttemptId`) | `uniqueidentifier` | NO | PK |
| `BookingPaymentId` | `uniqueidentifier` | NO | FK → `BookingPayments` |
| `AttemptNumber` | `int` | NO | Unique per `(BookingPaymentId, AttemptNumber)` |
| `PaymentProvider` | `nvarchar(50)` | NO | Provider used for this attempt |
| `Status` | `int` | NO | Same `PaymentStatus` (or attempt-specific subset) |
| `GatewaySessionId` | `nvarchar(200)` | YES | |
| `GatewayTransactionId` | `nvarchar(200)` | YES | |
| `GatewayStatus` | `nvarchar(50)` | YES | Raw PSP status |
| `WebhookEventId` | `nvarchar(200)` | YES | |
| `CheckoutUrl` | `nvarchar(1000)` | YES | Optional; may omit if sensitive/long-lived URLs undesirable |
| `FailureReason` | `nvarchar(1000)` | YES | Prefer bounded length vs `max` |
| `CreatedAt` | `datetime2` | NO | |
| `CompletedAt` | `datetime2` | YES | |
| `FailedAt` | `datetime2` | YES | |

### Index / uniqueness rules

| Index | Unique? | Filter / keys |
|-------|---------|----------------|
| `UX_BookingPaymentAttempts_Payment_AttemptNumber` | Yes | `(BookingPaymentId, AttemptNumber)` |
| `UX_BookingPaymentAttempts_WebhookEventId` | Yes | `WebhookEventId` WHERE NOT NULL |
| `IX_BookingPaymentAttempts_GatewayTransactionId` | No* | WHERE NOT NULL (*unique filtered if product guarantees global order id uniqueness) |
| `UX_BookingPayments_TransactionReference` | Yes | WHERE NOT NULL (header; after duplicate cleanup) |
| Keep | Yes | `UX` / unique on `BookingPayments.ServiceRequestId` |

### Existing records compatibility

1. Add header columns with defaults/nulls.  
2. Backfill `PaymentProvider` (`KHD-%` → Development; else Moyasar when historical gateway ref present).  
3. Create attempts table.  
4. For each existing `BookingPayments` row, insert **AttemptNumber = 1** copying `TransactionReference` into session/transaction fields as best-effort.  
5. Set `CurrentAttemptId` to that attempt.  
6. Add indexes after duplicate checks.

### Backward compatibility / rollback

| Concern | Approach |
|---------|----------|
| API | Keep single `payment` on booking DTO; expose current attempt fields; admin can list attempts later |
| Moyasar / Development | Write attempts the same way as Areeba |
| Rollback | Prefer leave new tables/columns; app rollback via `Payment:Provider`; down migration only if required and unused |

---

# 3. PAYMENT STATE MACHINE REVIEW

## 3.1 Final payment states (domain)

Extend / clarify `PaymentStatus` for production-ready flows:

| State | Meaning |
|-------|---------|
| `Pending` | Payment record created; session not yet opened / not sent to gateway |
| `Processing` | Gateway session created; customer directed to checkout |
| `AwaitingGatewayConfirmation` | Customer returned from checkout **or** timeout window; waiting for webhook / verify (**new**) |
| `Completed` | Server verified paid; booking may advance |
| `Failed` | Gateway declined / verify failed |
| `Cancelled` | Customer cancelled checkout / explicit cancel |
| `Expired` | Payment window elapsed without successful confirmation (**new**; today only booking expiry exists) |
| `Refunded` | Reserved for future refund wave (keep; do not implement now) |

> **Enum change note:** Adding `AwaitingGatewayConfirmation` and `Expired` requires an approved enum/migration change. Until coded, document them as the target machine; map gateway raw values into `GatewayStatus` on attempts immediately.

## 3.2 Happy path

```
Pending
  ↓  CreateSessionAsync success
Processing
  ↓  Customer leaves checkout / return deep link / poll
AwaitingGatewayConfirmation
  ↓  Webhook + VerifyAsync success (amount + currency)
Completed
  ↓  Booking orchestration (gateway-agnostic)
PaymentConfirmed → PendingCraftsmanConfirmation → …
```

## 3.3 Failure paths

```
Processing → Failed
Processing → Cancelled
Processing → Expired

AwaitingGatewayConfirmation → Failed
AwaitingGatewayConfirmation → Expired
AwaitingGatewayConfirmation → Cancelled
```

Retry (new attempt):

```
Failed | Cancelled | Expired | Processing (abandoned)
  ↓  Initiate payment again (new AttemptNumber)
Pending/Processing (new attempt)
```

Delayed webhook for an old attempt:

- Match by `GatewayTransactionId` / `WebhookEventId` / attempt session  
- If newer attempt is active and old attempt not Completed: update **that attempt** only  
- Only promote booking when the **accepted** paid attempt is valid for the current booking amount/currency and booking still `AwaitingPayment`

## 3.4 Completion authority (hard rule)

| Actor | May finalize `Completed`? |
|-------|---------------------------|
| Mobile client | **No** |
| Web client | **No** |
| Query-string / deep-link params | **No** |
| Development fake UI alone | **No** (even Dev must go through server verify path) |
| Areeba/Moyasar **webhook** + **server-side `VerifyAsync`** | **Yes** (required authority) |

### Client endpoints

| Endpoint | Role after this plan |
|----------|----------------------|
| `POST /bookings/{id}/payment` | Create attempt + session; return `checkoutUrl` |
| `GET /bookings/{id}` | Status refresh for UI |
| `POST /bookings/{id}/payment/confirm` | **Must not be a client completion authority.** Prefer deprecate for production providers, or reduce to “request status sync” that **only** finalizes if webhook already applied **or** server `VerifyAsync` proves paid. Clients never set Completed themselves. |

**Mobile/Web must never mark payment completed.** UI shows Completed only when API returns `Completed`.

---

# 4. MOBILE PAYMENT RULE

## 4.1 Current (forbidden)

```
initiate payment
  ↓
immediately confirm payment   ← REMOVE (Android + iOS)
```

This only “works” with `DevelopmentPaymentGateway` and is unsafe for Areeba/Moyasar.

## 4.2 Required mobile flow

```
Mobile:
  1. Request payment session          POST /bookings/{id}/payment
  2. Receive checkout URL (+ session)
  3. Open hosted checkout             Custom Tabs / SFSafariViewController
  4. Wait for return / deep link / status refresh
  5. Show Pending / AwaitingGatewayConfirmation / Completed / Failed from API

Server:
  Webhook receives payment event
    ↓
  Verify signature
    ↓
  Verify transaction (VerifyAsync: amount + currency + status)
    ↓
  Update BookingPayment (+ attempt)
    ↓
  Advance booking state
```

## 4.3 Mobile UX states

| API payment/booking signal | Mobile UI |
|----------------------------|-----------|
| Pending / Processing | “Continue to payment” / open checkout |
| AwaitingGatewayConfirmation | “Confirming payment…” (poll GET booking) |
| Completed | Success; show next booking status |
| Failed / Cancelled / Expired | Error + retry if booking still payable |

No React Native in this wave.

---

# 5. SECURITY REQUIREMENTS

Document before coding; implement in Infrastructure/API only.

| Requirement | Definition |
|-------------|------------|
| **Areeba webhook signature mechanism** | Confirm with boarding (HMAC-SHA256 over raw body or Areeba-documented header). Read **raw body** before deserialize. Fixed-time compare. Fail-closed in Staging/Production. |
| **Secret storage method** | `Payment__Areeba__ApiPassword`, `Payment__Areeba__WebhookSecret` via env/secret store only. Empty placeholders in git. Never ship secrets to mobile/web clients. |
| **Replay attack prevention** | Reject invalid/missing signatures; unique `WebhookEventId`; optional timestamp skew window if Areeba provides event time; always re-verify with Retrieve Order. |
| **Idempotency handling** | Same event id → no-op success; already `Completed` → no-op; slot reservation protected by Phase 1A uniqueness. |
| **Duplicate webhook handling** | Unique filtered index on attempt `WebhookEventId`; concurrent deliveries must be safe. |
| **Amount validation** | `VerifyAsync` compares gateway amount to `BookingPayment.Amount` (minor-unit rules per MPGS). Mismatch → do not complete. |
| **Currency validation** | Gateway currency must equal `BookingPayment.Currency`. Mismatch → do not complete. |
| **Booking ownership validation** | Initiate/status endpoints require authenticated customer owns booking. Webhooks do **not** use user JWT; they authorize via signature and correlate to payment/attempt ids only. Admin payment views remain permission-gated. |

### Sensitive logging

| Allow | Deny |
|-------|------|
| Payment id, attempt id/number, booking id, provider | API passwords, webhook secrets, Basic auth headers |
| Gateway session/order ids, HTTP status | PAN, CVV, full card payloads |
| High-level failure reasons | Unredacted webhook bodies with card metadata |

---

# 6. MIGRATION STRATEGY

Do **not** remove Moyasar immediately.

| Phase | Name | Actions | Exit |
|-------|------|---------|------|
| **1** | Implement Areeba beside Moyasar | Adapter + DI + webhook + DB attempts/columns; default Provider remains Development/Moyasar per env | CI green; both adapters registered |
| **2** | Sandbox testing | Staging `Payment:Provider=Areeba`; sandbox pay; mobile/web checkout; security tests | Signed sandbox checklist |
| **3** | Production validation | Limited production or full prod with monitoring; Moyasar webhook still live | Metrics acceptable |
| **4** | Switch default provider | `Payment:Provider=Areeba` for new sessions | All new payments on Areeba |
| **5** | Moyasar fallback period | Keep Moyasar code + webhook for rollback and in-flight drain (recommend ≥ 30 days) | No critical Areeba regressions |
| **6** | Deprecate Moyasar | After successful operation + drain: remove Moyasar gateway/webhook/config | Areeba + Development only |

**Rollback at any phase 1–5:** set `Payment:Provider=Moyasar` (or Development in non-prod). Booking logic unchanged.

---

# 7. TEST REQUIREMENTS

## 7.1 Backend

| Test | Expectation |
|------|-------------|
| Create Areeba payment session | Session + checkout URL; attempt #1 (or N); provider Areeba |
| Invalid webhook rejected | Bad/missing signature → unauthorized; no state change |
| Valid webhook completes payment | Signature OK + VerifyAsync OK → Completed + booking advances |
| Duplicate webhook ignored | Second delivery idempotent; no double slot lock |
| Wrong amount rejected | Verify fails; not Completed |
| Wrong currency rejected | Verify fails; not Completed |
| Failed payment handled | Attempt/header → Failed; booking remains payable or follows expiry rules |
| Expired payment handled | Attempt/header → Expired; retry creates new attempt |

Additional: ownership checks on initiate; Development + Moyasar regression; migration apply on SQL Server.

## 7.2 Mobile

| Test | Expectation |
|------|-------------|
| Checkout opened correctly | Uses `checkoutUrl`; no immediate confirm |
| Payment pending state handled | UI pending/processing/awaiting confirmation from API |
| Completed state handled | Only after API shows Completed |
| Failed state handled | Shows failure; retry path if allowed |

## 7.3 Regression

| Area | Expectation |
|------|-------------|
| Existing booking flow | Create → confirm booking → await payment → complete path |
| Moyasar during migration | Still creates/verifies when Provider=Moyasar |
| Phase 1A / 1B | Concurrency + webhook HMAC fail-closed remain green |

---

# 8. DEPLOYMENT MAPPING

Maps to §6 phases for ops clarity:

| Stage | Maps to | Environment |
|-------|---------|-------------|
| Development environment | Phase 1 | Local/CI, Provider=Development |
| Sandbox validation | Phase 2 | Staging + Areeba sandbox |
| Production with provider configuration | Phases 3–4 | Production secrets + Provider switch |
| Moyasar deprecation | Phases 5–6 | Fallback window then removal |

---

## Suggested work order (after approval only)

1. Approve this revised plan (architecture + DB attempt model + state machine)  
2. Additive DB migration (header columns + `BookingPaymentAttempts`)  
3. Gateway-agnostic orchestration updates (attempts, states) — still no Areeba types in booking  
4. `AreebaPaymentGateway` + webhook + DI + readiness  
5. Restrict client completion authority  
6. Web + mobile hosted checkout  
7. Full test matrix  
8. Execute migration phases 1→6  

---

## Approval checklist

- [ ] Approve gateway-independent architecture rule (§1)  
- [ ] Approve header + `BookingPaymentAttempts` history model (§2)  
- [ ] Approve payment state machine including `AwaitingGatewayConfirmation` / `Expired` (§3)  
- [ ] Approve webhook + server verify as sole completion authority (§3–4)  
- [ ] Approve mobile flow correction (§4)  
- [ ] Approve security requirements (§5)  
- [ ] Approve six-phase Moyasar coexistence/deprecation (§6)  
- [ ] Approve expanded test matrix (§7)  

**Still blocked until approval:**

- Database migration  
- Areeba coding  
- Payment UI changes  
- Phase 1C  

**Sign-off:**

| Role | Name | Date | Decision |
|------|------|------|----------|
| Product | | | Approve / Changes requested |
| Engineering | | | Approve / Changes requested |
| Ops / Security | | | Approve / Changes requested |

---

*End of revised document — documentation only; STOP and wait for approval.*
