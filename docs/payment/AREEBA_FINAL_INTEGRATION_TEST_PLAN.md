# Areeba — Final Integration Test Plan

**Remediation validation reviewed:** 2026-07-23  
**Status:** Preparation only — **NOT READY FOR PRODUCTION CUTOVER**  
**Branch:** `cursor/payment-gateway-migration-plan-4876`  
**Go-live decision:** [AREEBA_GO_LIVE_DECISION.md](./AREEBA_GO_LIVE_DECISION.md) → **NOT READY** until this plan’s live matrix is signed PASS  

**Accepted completed items (do not re-litigate):**

- ✅ SQL Server 2022 migration validation  
- ✅ BookingPayment preservation  
- ✅ PaymentAttempt backfill validation  
- ✅ Index validation  
- ✅ Rollback/re-apply validation  
- ✅ `FixPaymentProviderDefault` migration correction  

**Remaining blockers (this plan targets):**

1. Areeba sandbox credentials  
2. Real sandbox payment lifecycle tests  
3. Browser/device checkout QA  

**Hard rules during execution:**

- Do **not** set Production `Payment:Provider=Areeba`  
- Do **not** remove Moyasar  
- Do **not** start Phase 1C or React Native  
- Use **Staging** (or isolated sandbox stack) only  
- **STOP** after documentation and validation preparation until Ops injects credentials and QA executes this plan

---

## Prerequisites checklist (before any live test)

| # | Prerequisite | Owner | Done? |
|---|--------------|-------|-------|
| 1 | Areeba MPGS sandbox merchant boarded | Ops / Merchant | ☐ |
| 2 | Staging secrets injected (env/vault) — not committed | Ops | ☐ |
| 3 | Staging SQL has `AddBookingPaymentAttempts` + `FixPaymentProviderDefault` | Ops | ☐ |
| 4 | Staging API publicly reachable for webhooks (or tunnel) | Ops | ☐ |
| 5 | Web Staging build pointed at Staging API | Eng | ☐ |
| 6 | Android/iOS builds pointed at Staging API | Eng | ☐ |
| 7 | Test customer + craftsman accounts on Staging | QA | ☐ |

---

# 1. AREEBA SANDBOX CONFIGURATION

## 1.1 Sandbox merchant account

| Item | Requirement |
|------|-------------|
| Product | Areeba **MPGS / ePayment** (`epayment.areeba.com`) |
| Portal | Merchant admin → Integration Settings → API password |
| Mode | **Sandbox / test** merchant only |
| Test cards | Use Areeba-provided test PAN set from boarding docs |

## 1.2 API credentials (Staging secret store only)

| Config key | Env var | Notes |
|------------|---------|-------|
| `Payment:Provider` | `Payment__Provider` | `Areeba` on **Staging only** |
| `Payment:Areeba:MerchantId` | `Payment__Areeba__MerchantId` | From boarding |
| `Payment:Areeba:ApiUsername` | `Payment__Areeba__ApiUsername` | Typically `merchant.{MerchantId}` |
| `Payment:Areeba:ApiPassword` | `Payment__Areeba__ApiPassword` | Portal-generated API password |
| `Payment:Areeba:ApiBaseUrl` | `Payment__Areeba__ApiBaseUrl` | Default `https://epayment.areeba.com` |
| `Payment:Areeba:ApiVersion` | `Payment__Areeba__ApiVersion` | Default `100` |
| `Payment:Areeba:MerchantName` | `Payment__Areeba__MerchantName` | Display name on checkout |

## 1.3 Webhook configuration

| Item | Value |
|------|-------|
| Endpoint | `POST https://{staging-api-host}/api/v1/webhooks/areeba` |
| Secret | `Payment__Areeba__WebhookSecret` |
| Headers accepted by API | `X-Areeba-Signature`, `X-Notification-Secret`, or `X-Webhook-Secret` |
| Validation | HMAC-SHA256 hex over raw body **or** shared-secret equality; fail-closed on Staging |
| Confirm with Areeba | Exact header name + algorithm during first sandbox notify |

Register the callback in Areeba merchant notifications so paid orders hit Staging.

## 1.4 Callback / return URLs

| Config | Env var | Example (Staging) |
|--------|---------|-------------------|
| Callback (server webhook) | `Payment__Areeba__CallbackUrl` | `https://staging-api.khadamati.example/api/v1/webhooks/areeba` |
| Success (client return) | `Payment__Areeba__SuccessUrl` | `https://staging.khadamati.example/pay/success` |
| Cancel | `Payment__Areeba__CancelUrl` | `https://staging.khadamati.example/pay/cancel` |
| Optional hosted template | `Payment__Areeba__HostedCheckoutUrl` | May include `{sessionId}` / `{orderId}` |

Deep links (mobile): align Success/Cancel with app links / custom scheme used by Android/iOS builds under test.

## 1.5 Environment variable template (do not commit values)

```bash
# STAGING ONLY
Payment__Provider=Areeba
Payment__Areeba__MerchantId=
Payment__Areeba__ApiUsername=
Payment__Areeba__ApiPassword=
Payment__Areeba__WebhookSecret=
Payment__Areeba__ApiBaseUrl=https://epayment.areeba.com
Payment__Areeba__ApiVersion=100
Payment__Areeba__CallbackUrl=https://<staging-api>/api/v1/webhooks/areeba
Payment__Areeba__SuccessUrl=https://<staging-web>/pay/success
Payment__Areeba__CancelUrl=https://<staging-web>/pay/cancel

# Keep for regression / fallback drills (unused while Provider=Areeba)
Payment__Moyasar__SecretKey=
Payment__Moyasar__PublishableKey=
Payment__Moyasar__WebhookSecret=
```

## 1.6 Rules (mandatory)

| Rule | Enforcement |
|------|-------------|
| No secrets in repository | Empty placeholders in `appsettings*.json`; filled values only in env/vault |
| No production credentials in development/staging sandbox | Separate sandbox merchant; Production stays `Provider=Moyasar` |
| Readiness | `GET /api/v1/health/integrations` → Payment Ready when Areeba keys present |

### Pre-flight commands

```bash
curl -s https://<staging-api>/api/v1/health/integrations | jq '.providers[] | select(.category=="Payment")'
# Expect: provider Areeba, status Ready
```

---

# 2. REAL PAYMENT TEST MATRIX

Execute on Staging with Areeba sandbox. Record **Input / Expected / Actual / DB state** for each case.  
Attach evidence: API logs, webhook raw (redacted), SQL screenshots, screen recordings.

### Shared SQL helpers

```sql
-- After each scenario, replace @BookingId
SELECT b.Id, b.BookingReference, b.Status AS BookingStatus
FROM ServiceRequests b WHERE b.Id = @BookingId;

SELECT p.Id, p.Status, p.PaymentProvider, p.Amount, p.Currency,
       p.TransactionReference, p.GatewayTransactionId, p.CurrentAttemptId, p.PaidAt, p.FailedAt
FROM BookingPayments p WHERE p.ServiceRequestId = @BookingId;

SELECT a.AttemptNumber, a.Status, a.GatewaySessionId, a.GatewayTransactionId,
       a.WebhookEventId, a.CompletedDate, a.FailedDate, a.FailureReason
FROM BookingPaymentAttempts a
JOIN BookingPayments p ON p.Id = a.BookingPaymentId
WHERE p.ServiceRequestId = @BookingId
ORDER BY a.AttemptNumber;
```

---

## 2.1 Successful payment

| Field | Detail |
|-------|--------|
| **Steps** | Create booking → Customer confirm (`AwaitingPayment`) → Initiate payment → Open checkout → Complete with test card → Webhook received → Server `VerifyAsync` → Booking advances |
| **Input** | Valid sandbox card; amount/currency matching booking |
| **Expected** | Attempt `Completed`; payment `Completed` + `PaidAt`; booking `PendingCraftsmanConfirmation` (via `PaymentConfirmed`); craftsman notified; **exactly one** Completed attempt |
| **Actual** | ☐ PASS / ☐ FAIL — _fill during execution_ |
| **Database state** | ☐ `PaymentProvider=Areeba`; ☐ `CurrentAttemptId` set; ☐ `WebhookEventId` set; ☐ `UX_…_OneCompletedPerPayment` satisfied |
| **Evidence** | Webhook 200 log; SQL result set; booking GET JSON |

---

## 2.2 Failed payment

| Field | Detail |
|-------|--------|
| **Steps** | Initiate → checkout → use decline test card (or force decline) |
| **Input** | Gateway decline / non-success status |
| **Expected** | Webhook ignored **or** verify fails; payment/attempt `Failed` (+ `FailedAt`/`FailedDate`); booking remains `AwaitingPayment`; retry allowed (new attempt) |
| **Actual** | ☐ PASS / ☐ FAIL |
| **Database state** | ☐ No `Completed` attempt; ☐ `FailureReason` populated when verify fails |
| **Evidence** | Gateway response / webhook payload status; SQL |

---

## 2.3 Abandoned checkout

| Field | Detail |
|-------|--------|
| **Steps** | Initiate → open checkout → close/cancel without paying → refresh booking |
| **Input** | User exit / cancel URL |
| **Expected** | Status stays `Processing` or `AwaitingGatewayConfirmation`; **not** `Completed`; booking still `AwaitingPayment` |
| **Actual** | ☐ PASS / ☐ FAIL |
| **Database state** | ☐ Attempt not Completed; ☐ no `PaidAt` |
| **Evidence** | Client screenshot + SQL |

---

## 2.4 Duplicate webhook

| Field | Detail |
|-------|--------|
| **Steps** | Complete one successful payment; re-POST identical webhook body + same event id + valid signature |
| **Input** | Same `WebhookEventId` / payload twice |
| **Expected** | First processed; second idempotent (no error storm); **no** second Completed attempt; no double slot reservation |
| **Actual** | ☐ PASS / ☐ FAIL |
| **Database state** | ☐ Single Completed attempt; ☐ unique `WebhookEventId` row |
| **Evidence** | Two webhook responses; SQL attempt count = 1 Completed |

---

## 2.5 Invalid webhook

| Field | Detail |
|-------|--------|
| **Steps** | POST webhook with wrong/missing signature while secret configured |
| **Input** | Invalid `X-Areeba-Signature` |
| **Expected** | Rejected (`401` / Unauthorized); **no** payment status change |
| **Actual** | ☐ PASS / ☐ FAIL |
| **Database state** | ☐ Unchanged vs pre-request snapshot |
| **Evidence** | HTTP status + SQL before/after |

---

## 2.6 Amount mismatch

| Field | Detail |
|-------|--------|
| **Steps** | Prefer sandbox order whose paid amount ≠ `BookingPayment.Amount` (or controlled verify harness if boarding allows); ensure finalize calls `VerifyAsync` |
| **Input** | Gateway amount ≠ obligation amount |
| **Expected** | Verification fails; payment **not** Completed |
| **Actual** | ☐ PASS / ☐ FAIL _(unit already PASS; live must confirm)_ |
| **Database state** | ☐ Failed or non-Completed; ☐ no `PaidAt` |
| **Evidence** | Verify failure reason; SQL |

---

## 2.7 Currency mismatch

| Field | Detail |
|-------|--------|
| **Steps** | Same as amount, with currency differing from `BookingPayment.Currency` |
| **Input** | Gateway currency ≠ obligation currency |
| **Expected** | Rejected; not Completed |
| **Actual** | ☐ PASS / ☐ FAIL _(unit already PASS; live must confirm)_ |
| **Database state** | ☐ Non-Completed |
| **Evidence** | Verify failure reason; SQL |

---

## 2.8 Matrix sign-off

| Case | Result | Tester | Date |
|------|--------|--------|------|
| Successful payment | | | |
| Failed payment | | | |
| Abandoned checkout | | | |
| Duplicate webhook | | | |
| Invalid webhook | | | |
| Amount mismatch | | | |
| Currency mismatch | | | |

**Section 2 exit:** All rows PASS before go-live consideration.

---

# 3. CLIENT VALIDATION

## 3.1 Web

| Check | Procedure | Expected | Result |
|-------|-----------|----------|--------|
| Checkout redirect | Initiate payment on Staging web | Browser navigates to Areeba hosted checkout | ☐ |
| Return URL | Complete or cancel payment | Lands on Success/Cancel URL; app usable | ☐ |
| Pending state | After initiate / before webhook | UI shows pending / awaiting; **not** paid | ☐ |
| Completed state | After successful webhook | Refresh/navigate shows advanced booking status | ☐ |
| Chrome desktop | Full success path | PASS | ☐ |
| Mobile browser | Full success path | PASS | ☐ |

**Note:** Confirm button must **not** finalize when provider is Areeba (Development-only confirm).

## 3.2 Android

| Check | Procedure | Expected | Result |
|-------|-----------|----------|--------|
| Open hosted checkout | Tap Pay | External browser / Custom Tab opens `checkoutUrl` | ☐ |
| Return handling | After pay/cancel, return to app | App resumes; no crash | ☐ |
| Refresh payment status | Tap refresh / reload booking | Status matches API; Completed only after server | ☐ |
| No auto-confirm | Inspect network | No immediate `payment/confirm` after initiate | ☐ |

## 3.3 iOS

| Check | Procedure | Expected | Result |
|-------|-----------|----------|--------|
| Open hosted checkout | Tap Pay | Safari / system browser opens checkout | ☐ |
| Return handling | After pay/cancel | Returns to app reliably | ☐ |
| Refresh payment status | Refresh control / reload | Matches API; no local “paid” flag | ☐ |
| No auto-confirm | Inspect network | No immediate confirm call | ☐ |

## 3.4 Client sign-off

| Surface | Tester | Date | PASS/FAIL |
|---------|--------|------|-----------|
| Web Chrome | | | |
| Web mobile browser | | | |
| Android | | | |
| iOS | | | |

---

# 4. FINAL GO-LIVE DECISION

Authoritative file: **[AREEBA_GO_LIVE_DECISION.md](./AREEBA_GO_LIVE_DECISION.md)**

| Outcome | When allowed |
|---------|----------------|
| **READY FOR PRODUCTION** | §§1–3 all PASS on Staging; risks accepted; Product/Eng/Ops signed |
| **NOT READY** | Any open blocker (credentials, live matrix, client QA) |

**Current outcome (preparation phase): NOT READY** — see decision doc.

### Evidence pack to attach before flipping to READY

1. Staging secrets confirmation (names only — not secret values)  
2. Completed §2 matrix table with logs/SQL  
3. Completed §3 client QA table with recordings  
4. `GET /health/integrations` Payment Ready screenshot  
5. Confirmation Production still `Provider=Moyasar` until cutover change ticket  

---

## Execution order (recommended)

1. Inject Staging sandbox secrets (§1)  
2. Confirm integrations readiness  
3. Run §2.1 success path end-to-end  
4. Run remaining §2 failure/edge cases  
5. Run §3 Web → Android → iOS  
6. Update go-live decision to READY or keep NOT READY with gaps listed  

---

*End of final integration test plan — preparation only. STOP until sandbox execution + approval.*
