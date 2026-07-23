# Areeba — Pre-Production Verification Checkpoint

**Status:** Documentation checkpoint — **awaiting approval before production cutover**  
**Date:** 2026-07-23  
**Branch:** `cursor/payment-gateway-migration-plan-4876`  
**Related:** [AREEBA_VERIFICATION_REPORT.md](./AREEBA_VERIFICATION_REPORT.md), [AREEBA_IMPLEMENTATION_PLAN_UPDATE.md](./AREEBA_IMPLEMENTATION_PLAN_UPDATE.md)

**Hard stop:** Do **not** enable production `Payment:Provider=Areeba`, remove Moyasar, start Phase 1C, or start React Native until this checkpoint is approved and Stage 1 sandbox evidence is signed.

---

## Executive summary

| Area | Verdict |
|------|---------|
| Attempt model in code | **Ready** — `BookingPayment` + `BookingPaymentAttempts` |
| EF migration authored | **Ready** — `20260723210419_AddBookingPaymentAttempts` (apply on SQL Server still required per environment) |
| Areeba adapter + webhook | **Ready for sandbox** |
| Mobile/Web client authority | **Ready** — no initiate→confirm; webhook/`VerifyAsync` finalize production payments |
| Unit tests | **82 passed** |
| Live Areeba sandbox E2E | **Pending** (merchant secrets / staging apply) |
| Production cutover | **Blocked** pending this approval + Stage 1 |

---

# 1. DATABASE REVIEW

## 1.1 Migration artifact

| Item | Evidence |
|------|----------|
| Migration name | `AddBookingPaymentAttempts` (`20260723210419_AddBookingPaymentAttempts.cs`) |
| Additive only | Yes — no drops of `BookingPayments` obligation rows |
| Backfill | Provider inference; attempt `#1` per existing payment; `CurrentAttemptId` set |
| Soft-delete filter column | Physical `[Deleted]` used in completed-attempt unique filter |

**Apply status:** Migration is in source control and builds with the model. **Physical apply to each SQL Server (Dev → Staging → Production) remains an ops checklist item** before that environment uses attempts at runtime.

### Checklist (ops)

- [ ] Migration applied on Dev SQL Server  
- [ ] Migration applied on Staging SQL Server  
- [ ] Row counts: `BookingPayments` unchanged vs pre-migration baseline  
- [ ] Every non-deleted `BookingPayments` row has ≥ 1 `BookingPaymentAttempts` row after backfill  
- [ ] Production apply only after Staging soak  

## 1.2 `BookingPaymentAttempts` structure

| Field | Present |
|-------|---------|
| `Id`, `BookingPaymentId`, `PaymentProvider`, `AttemptNumber` | Yes |
| `GatewaySessionId`, `GatewayTransactionId`, `Status` | Yes |
| `Amount`, `Currency`, `RequestDate` | Yes |
| `CompletedDate`, `FailedDate`, `FailureReason`, `WebhookEventId` | Yes |
| `CreatedAt` / `UpdatedAt` (as `CreatedDate` / `ModifiedDate`) | Yes |

**Relationship:**

```
ServiceRequest (Booking)
  └── BookingPayment          (1:0..1 obligation — unique ServiceRequestId)
        └── BookingPaymentAttempts  (1:N)
```

FK: `BookingPaymentAttempts.BookingPaymentId` → `BookingPayments.Id` (**Cascade** delete with payment).  
`BookingPayments.CurrentAttemptId` → attempts (**NoAction**).

## 1.3 Indexes (verified in migration)

| Index | Table | Columns | Unique | Notes |
|-------|-------|---------|--------|-------|
| `IX_BookingPaymentAttempts_BookingPaymentId_AttemptNumber` | Attempts | `(BookingPaymentId, AttemptNumber)` | Yes | Multiple attempts allowed; ordered numbering |
| `UX_BookingPaymentAttempts_OneCompletedPerPayment` | Attempts | `BookingPaymentId` | Yes | Filter: `[Status] = 3 AND [Deleted] = 0` → **only one Completed** |
| `IX_BookingPaymentAttempts_GatewayTransactionId` | Attempts | `GatewayTransactionId` | No | Filter: IS NOT NULL |
| `IX_BookingPaymentAttempts_WebhookEventId` | Attempts | `WebhookEventId` | Yes | Filter: IS NOT NULL → duplicate events rejected |
| `IX_BookingPayments_GatewayTransactionId` | Payments | `GatewayTransactionId` | No | Denormalized lookup |
| `IX_BookingPayments_TransactionReference` | Payments | `TransactionReference` | Yes | Filter: IS NOT NULL |
| `IX_BookingPayments_CurrentAttemptId` | Payments | `CurrentAttemptId` | No | FK support |

**Status indexing:** There is no standalone non-filtered `IX_*_Status` on all status values. **Completed** uniqueness is enforced via the filtered unique index on `Status = 3`. Application queries primarily key by payment id / gateway ids. Optional general `Status` index can be added later if reporting needs it — **not a cutover blocker**.

## 1.4 Rules confirmation

| Rule | Enforcement |
|------|-------------|
| Multiple attempts allowed | `(BookingPaymentId, AttemptNumber)` unique; initiate increments attempt number |
| Only one successful (Completed) attempt | Filtered unique index + app check before finalize |
| Completed payments cannot be modified / overwritten | `InitiatePaymentAsync` rejects `Completed`; finalize short-circuits if already `Completed`; client confirm cannot complete production providers |
| Existing `BookingPayment` records preserved | Additive migration + backfill; no delete of obligations |

---

# 2. PAYMENT STATE VALIDATION

## 2.1 Successful flow (authoritative)

```
Booking (AwaitingPayment)
  ↓
BookingPayment created (obligation)
  ↓
BookingPaymentAttempt created (AttemptNumber N)
  ↓
IPaymentGateway.CreateSessionAsync (Areeba MPGS)
  ↓
Client opens hosted checkout URL
  ↓
Customer pays on Areeba
  ↓
POST /api/v1/webhooks/areeba
  ↓
Signature validated (fail-closed Staging/Production)
  ↓
Idempotent WebhookEventId check
  ↓
ConfirmPaymentFromWebhookAsync
  ↓
IPaymentGateway.VerifyAsync (amount + currency + paid status)
  ↓
Attempt → Completed; Payment → Completed (PaidAt set)
  ↓
Booking → PaymentConfirmed → PendingCraftsmanConfirmation
  ↓
Slot reserved + craftsman notified
```

**Completion authority:** Webhook + server-side `VerifyAsync` only for Areeba/Moyasar. Clients do not finalize.

## 2.2 Failure / edge flows

| Scenario | Expected behavior | Code support |
|----------|-------------------|--------------|
| User closes checkout | Attempt/payment remain `Processing` / later `AwaitingGatewayConfirmation`; booking stays `AwaitingPayment`; retry allowed when prior attempt failed/expired/cancelled/pending | Supported (no auto-complete) |
| Gateway timeout | Return/cancel URL → client refreshes status; no Completed without verify | Supported |
| Failed card payment | Webhook ignored if non-success status **or** `VerifyAsync` fails → attempt/payment `Failed` + `FailedDate`/`FailedAt` | Supported |
| Duplicate webhook | Same `WebhookEventId` → no-op success path; unique index prevents dual insert | Supported |
| Delayed webhook (old attempt) | Correlate by gateway transaction/session; if payment already `Completed`, idempotent return; must not create second Completed | Supported |
| Incorrect amount | `VerifyAsync` fails → not Completed | Supported (`AreebaPaymentGateway`) |
| Incorrect currency | `VerifyAsync` fails → not Completed | Supported |
| Expired payment | Status `Expired` (= 8) defined; retry creates new attempt | Enum + retry path ready; **scheduled expiry job** is optional follow-up if not already covered by booking `ExpiresAt` |

## 2.3 Payment status values (append-only)

| Int | Name |
|-----|------|
| 1 | Pending |
| 2 | Processing |
| 3 | Completed |
| 4 | Failed |
| 5 | Refunded (future) |
| 6 | Cancelled |
| 7 | AwaitingGatewayConfirmation |
| 8 | Expired |

---

# 3. SECURITY REVIEW

| Control | Verification |
|---------|--------------|
| Areeba secrets as environment variables only | Config keys `Payment:Areeba:ApiPassword`, `WebhookSecret`, etc.; `.env.production.example` placeholders; `appsettings*.json` empty / `${…}` tokens |
| No secrets in repository | No live merchant passwords committed; examples only |
| Webhook signature validation | `PaymentWebhookService.ValidateHmacSignature` — HMAC-SHA256 hex + shared-secret fallback; Staging/Production fail-closed if secret missing |
| Replay protection | Signature required when secret configured; unique `WebhookEventId`; server re-verify via Retrieve Order |
| Duplicate event protection | Unique filtered index + early lookup by `WebhookEventId` |
| Amount validation | `AreebaPaymentGateway.VerifyAsync` compares gateway amount to obligation amount |
| Currency validation | Same — currency must match |
| Booking ownership validation | `InitiatePaymentAsync` / customer booking APIs require authenticated customer ownership; webhooks authorized by signature only (no user JWT) |

**Logging:** Do not log API passwords, webhook secrets, PAN/CVV (adapter/logging rules in implementation plan).

**Boarding follow-up:** Confirm exact Areeba notification header name/algorithm in sandbox before production (code accepts `X-Areeba-Signature` / `X-Notification-Secret` / `X-Webhook-Secret`).

---

# 4. MOBILE PAYMENT REVIEW

## 4.1 Android

| Check | Result |
|-------|--------|
| ~~initiate → immediate confirm~~ | **Removed** from `BookingRepository.pay` |
| Initiate session | Yes — `initiatePayment` |
| Open checkout URL | Yes — ViewModel/Screen launches `ACTION_VIEW` when `checkoutUrl` present |
| Wait / no client complete | Yes — does not call `confirmPayment` |
| Refresh payment status | Yes — “Refresh payment status” reloads booking |

## 4.2 iOS

| Check | Result |
|-------|--------|
| ~~initiate → immediate confirm~~ | **Removed** from `BookingViewModel.pay` |
| Initiate session | Yes |
| Open checkout URL | Yes — `UIApplication.shared.open` |
| Wait / no client complete | Yes — no `confirmPayment` call |
| Refresh payment status | Yes — detail refresh button + `loadBooking` after pay |

**Rule confirmed:** Mobile apps must never mark payment completed; UI reflects API booking/payment status only.

---

# 5. WEB PAYMENT REVIEW

| Check | Result |
|-------|--------|
| Hosted checkout redirect | `BookingPaymentPage` assigns `window.location` to `checkoutUrl` after initiate |
| Open checkout fallback | Outlined button still links `checkoutUrl` |
| Return / pending handling | “Refresh payment status” loads booking; navigates away when no longer `AwaitingPayment` |
| Successful payment state | Shown after API status leaves `AwaitingPayment` (webhook-driven) |
| Failed / waiting messaging | Info alerts for webhook-only completion on non-Development providers |
| Development confirm | Manual confirm button **only** when `provider === 'Development'` (local/CI) |

Return URLs are configured via `Payment:Areeba:SuccessUrl` / `CancelUrl` (and Moyasar equivalents during coexistence).

---

# 6. TEST REPORT

## 6.1 Backend unit matrix

| Case | Coverage | Status |
|------|----------|--------|
| Create Areeba payment session | `AreebaPaymentGatewayTests` | Pass |
| Gateway failure / invalid response | `AreebaPaymentGatewayTests` | Pass |
| Successful webhook (HMAC) | `PaymentWebhookServiceTests` | Pass |
| Invalid webhook signature | Moyasar + Areeba tests | Pass |
| Duplicate webhook path | Areeba duplicate event test (idempotent booking service calls) | Pass |
| Wrong amount / currency | `VerifyAsync_*_Fails` | Pass |
| Captured matching verify | `VerifyAsync_CapturedMatching_Succeeds` | Pass |
| Areeba readiness ready/misconfigured | `IntegrationReadinessServiceTests` | Pass |
| Moyasar coexistence webhook | Existing Moyasar tests | Pass |
| Failed payment handling | Finalize sets Failed on verify failure (service logic) | Code present; extend dedicated service test if desired |
| Expired payment | Enum + retry initiate path | Code present; automated expiry job optional |
| Retry payment (new attempt) | `GetNextPaymentAttemptNumberAsync` + initiate | Code present |

**Aggregate:** `dotnet test Khadamati.Tests` → **82 passed**, 0 failed (2026-07-23).

## 6.2 Frontend / mobile matrix

| Case | Status |
|------|--------|
| Web checkout redirect + refresh | Implemented — **manual sandbox QA pending** |
| Android checkout open + refresh | Implemented — **manual sandbox QA pending** |
| iOS checkout open + refresh | Implemented — **manual sandbox QA pending** |

## 6.3 Integration / sandbox (required before Stage 2)

| Case | Status |
|------|--------|
| Apply migration on Staging SQL Server | Pending ops |
| Areeba sandbox: create session → pay → webhook → booking advances | Pending |
| Invalid signature rejected on Staging | Pending |
| Duplicate webhook no double slot | Pending |
| Moyasar still works when `Payment:Provider=Moyasar` | Pending regression |

---

# 7. CUTOVER PLAN

Do **not** set production `Payment:Provider=Areeba` until Stage 1 is signed.

## Stage 1 — Areeba sandbox

| Action | Owner |
|--------|-------|
| Apply `AddBookingPaymentAttempts` on Staging | Ops |
| Configure Staging `Payment:Provider=Areeba` + sandbox secrets | Ops |
| Register webhook URL `…/api/v1/webhooks/areeba` | Ops / Areeba boarding |
| Run §6.3 sandbox matrix (Web + Android + iOS) | QA |
| Keep Moyasar code + webhook deployed | Engineering |

**Exit:** Signed sandbox evidence pack.

## Stage 2 — Production with limited traffic

| Action | Owner |
|--------|-------|
| Apply migration on Production (maintenance window) | Ops |
| Set production secrets; switch `Payment:Provider=Areeba` | Ops |
| Monitor success rate, webhook 401s, stuck `AwaitingPayment` | Ops |
| Prefer limited cohort / short observation window if product allows | Product |

**Exit:** Stable metrics for agreed observation period.

## Stage 3 — Moyasar fallback period

| Action | Owner |
|--------|-------|
| Keep Moyasar gateway + `/webhooks/moyasar` live | Engineering |
| Drain in-flight Moyasar `Processing` payments | Ops |
| Rollback path: revert `Payment:Provider=Moyasar` if Areeba outage | Ops |
| Recommended duration | ≥ 30 days successful Areeba operation |

## Stage 4 — Moyasar deprecation decision

| Gate | Requirement |
|------|-------------|
| No in-flight Moyasar payments past TTL | Met |
| Soak window complete | Met |
| Product/Eng/Ops sign-off | Required |
| Then | Remove Moyasar adapter, webhook, config keys, docs; rotate secrets |

**Explicit non-goals until later approval:** Phase 1C, React Native, refunds/escrow.

---

## Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| Engineering | | | Approve sandbox → Stage 1 / Changes requested |
| QA | | | Approve mobile/web sandbox matrix / Changes requested |
| Ops / Security | | | Approve secrets + cutover stages / Changes requested |
| Product | | | Approve limited production traffic / Changes requested |

---

*End of pre-production verification checkpoint — documentation only. STOP.*
