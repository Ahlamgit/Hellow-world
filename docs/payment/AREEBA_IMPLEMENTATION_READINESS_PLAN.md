# Areeba — Implementation Readiness Plan

**Status:** Implementation complete (adapter layer) — **awaiting sandbox validation / production cutover approval**  
**Date:** 2026-07-23  
**Prerequisite:** `AREEBA_EXTERNAL_DEPENDENCY_CHECKLIST.md` (validation items tracked separately)

---

## Purpose

Define how Khadamati can **implement** the Areeba payment adapter and webhook pipeline **without** sandbox credentials, while keeping **live validation** blocked until Areeba external dependencies are received.

**Correction (approved):** External credentials are required for **sandbox validation**, not for **adapter implementation**.

---

# 1. Separate development from validation

## A) Code implementation possible without credentials

These items can be built, reviewed, and unit-tested using **mocked HTTP responses** and **fixture payloads**. No Areeba sandbox account required.

| # | Work item | Deliverable | Notes |
|---|-----------|-------------|-------|
| A1 | **AreebaPaymentGateway adapter structure** | `AreebaPaymentGateway : IPaymentGateway` | Implement `CreateSessionAsync` / `VerifyAsync` with `HttpClient` + mocked responses; use placeholder `ApiBaseUrl` from config |
| A2 | **Configuration model** | `Payment:Areeba:*` section in `appsettings.json` + `AreebaOptions` strongly-typed class | Keys optional in Development; validated at startup only when `Payment:Provider=Areeba` |
| A3 | **Dependency injection registration** | `RegisterPaymentProvider()` — `case "areeba"` | Register `HttpClient`, gateway, webhook services; default remains `Development` |
| A4 | **Payment request/response DTOs** | `AreebaSessionRequest`, `AreebaSessionResponse`, `AreebaVerifyResponse`, `AreebaWebhookDto` | Map to/from existing `PaymentSessionRequest`, `PaymentSessionDto`, `PaymentVerificationResult` |
| A5 | **Webhook controller structure** | `AreebaWebhookController` — `POST /api/v1/webhooks/areeba` | Read raw body; delegate to webhook service; no booking logic in controller |
| A6 | **Signature validation service abstraction** | `IAreebaWebhookSignatureValidator` (or `IPaymentWebhookSignatureValidator`) | HMAC/signature per Areeba spec (document algorithm placeholder until official docs; unit-test with known vectors) |
| A7 | **Payment attempt model** | `BookingPaymentAttempt` entity + EF migration | One booking payment may have multiple attempts (retries, re-initiate); links to `BookingPayment` |
| A8 | **Webhook event idempotency store** | `PaymentWebhookEvent` (or `ProcessedWebhookEvent`) | Unique index on `(Provider, ProviderEventId)` to reject duplicate deliveries |
| A9 | **Idempotency handling** | Webhook service + `BookingService.ConfirmPaymentFromWebhookAsync` (existing) | Attempt-level + booking-level guards; no double completion |
| A10 | **Logging and audit events** | Structured logs + `IAuditService.LogSecurityEventAsync` / payment audit entries | Log initiate, verify, webhook received/processed/skipped (no secrets in logs) |
| A11 | **Unit tests (mocked)** | `AreebaPaymentGatewayTests`, `AreebaWebhookServiceTests` | See Section 4 |

### Implementation boundary

All Areeba-specific logic lives under:

```
src/backend/Khadamati.Infrastructure/Services/Payments/Areeba/
src/backend/Khadamati.Application/DTOs/Payments/Areeba/
src/backend/Khadamati.Application/Interfaces/ (abstractions only)
src/backend/Khadamati.API/Controllers/ (webhook controller only)
```

`BookingService` continues to call **`IPaymentGateway`** and **`ConfirmPaymentFromWebhookAsync`** only — no Areeba types.

---

## B) Requires Areeba sandbox (validation only)

These activities **cannot** be completed without Section 1 items from `AREEBA_EXTERNAL_DEPENDENCY_CHECKLIST.md`.

| # | Activity | Blocked by |
|---|----------|------------|
| B1 | **Real checkout** | Sandbox credentials + whitelisted redirect URLs + test cards |
| B2 | **Real transaction verification** | Live API call to Areeba verify/status endpoint |
| B3 | **Real webhook delivery** | Public staging URL whitelisted by Areeba |
| B4 | **Browser/device testing** | Hosted checkout on web WebView + mobile deep links |

**Validation evidence** is captured in a future `AREEBA_SANDBOX_VALIDATION_REPORT.md` after B1–B4 pass.

---

# 2. Areeba adapter requirements

## 2.1 Must implement `IPaymentGateway`

```csharp
public interface IPaymentGateway
{
    string ProviderName { get; }
    Task<PaymentSessionDto> CreateSessionAsync(PaymentSessionRequest request, CancellationToken cancellationToken = default);
    Task<PaymentVerificationResult> VerifyAsync(string sessionId, decimal expectedAmount, string currency, CancellationToken cancellationToken = default);
}
```

| Method | Areeba responsibility |
|--------|----------------------|
| `CreateSessionAsync` | Call Areeba session/checkout API; return `SessionId` + `CheckoutUrl` |
| `VerifyAsync` | Call Areeba status API; validate amount/currency; return success/failure |
| `ProviderName` | `"Areeba"` |

Reference implementation pattern: `MoyasarPaymentGateway.cs` (do not modify Moyasar for Areeba work).

## 2.2 Must NOT contain Areeba-specific code

| Layer | Rule |
|-------|------|
| **`BookingService`** | Uses `IPaymentGateway` only; no `Areeba*` imports |
| **Booking workflow / state machine** | Gateway-agnostic status transitions |
| **Mobile clients (Android/iOS)** | Call existing `POST /bookings/{id}/payment` and `/payment/confirm` only |
| **Web booking pages** | Use generic `checkoutUrl` from API response — no Areeba SDK in client |

## 2.3 Provider registration

Extend `DependencyInjection.RegisterPaymentProvider()`:

```csharp
case "areeba":
    services.AddHttpClient(nameof(AreebaPaymentGateway));
    services.AddScoped<IPaymentGateway, AreebaPaymentGateway>();
    break;
```

| Environment | `Payment:Provider` | Notes |
|-------------|-------------------|-------|
| Local dev | `Development` | Unchanged |
| CI / unit tests | `Development` or mocked `Areeba` | No live calls |
| Staging (pre-credentials) | `Development` | Adapter deployed but not selected |
| Staging (post-credentials) | `Areeba` | Validation wave |
| Production | `Moyasar` until cutover approved | **Do not switch without approval** |

## 2.4 Configuration model (proposed)

| Key | Required when `Provider=Areeba` | Purpose |
|-----|--------------------------------|---------|
| `Payment:Areeba:MerchantId` | Yes | Merchant identifier |
| `Payment:Areeba:SecretKey` | Yes | API authentication |
| `Payment:Areeba:ApiBaseUrl` | Yes | Sandbox/production API base |
| `Payment:Areeba:WebhookSecret` | Staging/Production | Signature validation |
| `Payment:Areeba:CallbackUrl` | Yes | Server webhook URL |
| `Payment:Areeba:SuccessUrl` | Yes | Browser return URL |
| `Payment:Areeba:CancelUrl` | Yes | Browser cancel URL |

Update `IntegrationReadinessService` to recognize `Areeba` provider (mirrors Moyasar checks).

---

# 3. Webhook architecture

## 3.1 Endpoint

```
POST /api/v1/webhooks/areeba
[AllowAnonymous]
```

- Read **raw request body** before deserialization (required for signature validation).
- Accept signature from Areeba header (exact header name TBD from Areeba docs; support config override).

## 3.2 Processing flow

```
Receive webhook (raw body + headers)
        ↓
Validate signature (IAreebaWebhookSignatureValidator)
        ↓
Parse AreebaWebhookDto
        ↓
Validate event id / idempotency
   → If ProviderEventId already in PaymentWebhookEvent: return 200 OK (already processed)
        ↓
Resolve transaction reference → BookingPaymentAttempt
   → Lookup by SessionId / ProviderTransactionId / metadata payment_id
        ↓
Load parent BookingPayment + ServiceRequest
        ↓
Verify transaction (IPaymentGateway.VerifyAsync OR Areeba status API)
   → Enforce amount + currency match expected values on attempt
        ↓
Update BookingPaymentAttempt
   → Status: Completed | Failed
   → ProviderTransactionId, ProviderEventId, ProcessedAt
        ↓
Record PaymentWebhookEvent (idempotency)
        ↓
Update BookingPayment
   → Status, TransactionReference, PaidAt / FailureReason
        ↓
Continue booking workflow
   → BookingService.ConfirmPaymentFromWebhookAsync(transactionReference)
   → Slot reservation, craftsman notification (existing logic)
```

## 3.3 Data model (new)

### `BookingPaymentAttempt`

Tracks each gateway session/initiation. Parent: `BookingPayment` (1:N).

| Field | Purpose |
|-------|---------|
| `Id` | PK |
| `BookingPaymentId` | FK to `BookingPayments` |
| `Provider` | `"Areeba"` |
| `SessionId` | Gateway session / checkout reference |
| `ProviderTransactionId` | Set after payment |
| `Amount`, `Currency` | Expected values for mismatch checks |
| `Status` | Pending, Processing, Completed, Failed, Cancelled |
| `CheckoutUrl` | Hosted checkout link |
| `FailureReason` | Decline/error message |
| `CreatedAt`, `CompletedAt` | Timestamps |

### `PaymentWebhookEvent` (idempotency)

| Field | Purpose |
|-------|---------|
| `Provider` | `"Areeba"` |
| `ProviderEventId` | Unique event ID from Areeba payload |
| `ProcessedAt` | When handled |
| `BookingPaymentAttemptId` | Optional FK |

**Unique index:** `(Provider, ProviderEventId)`

### Relationship to existing `BookingPayment`

| Entity | Role |
|--------|------|
| `BookingPayment` | Booking-level payment record (1:1 with `ServiceRequest`) — **keep** |
| `BookingPaymentAttempt` | Per-gateway-session detail — **new** |
| `PaymentWebhookEvent` | Webhook deduplication — **new** |

`BookingService.InitiatePaymentAsync` creates/updates `BookingPayment` and records a new `BookingPaymentAttempt` when session is created (via a dedicated `IPaymentAttemptService` to keep Areeba logic out of `BookingService` internals — optional thin orchestration hook).

## 3.4 Service layering

| Service | Responsibility |
|---------|----------------|
| `AreebaWebhookController` | HTTP ingress only |
| `AreebaWebhookService` (or `PaymentWebhookOrchestrator`) | Signature, idempotency, attempt lookup, verify, delegate to booking |
| `IAreebaWebhookSignatureValidator` | Signature algorithm |
| `IPaymentAttemptRepository` | CRUD for attempts + webhook events |
| `BookingService` | `ConfirmPaymentFromWebhookAsync` — unchanged contract |

**Moyasar webhook remains** at `/api/v1/webhooks/moyasar` — do not remove.

---

# 4. Testing before credentials

All tests use **mocked `HttpClient`** / **Moq** — no network calls to Areeba.

## 4.1 `AreebaPaymentGatewayTests`

| Test | Input | Expected |
|------|-------|----------|
| `CreateSessionAsync_ValidResponse_ReturnsCheckoutUrl` | Mock 201 + session JSON | `PaymentSessionDto` with `SessionId`, `CheckoutUrl`, `Provider=Areeba` |
| `CreateSessionAsync_ApiError_ThrowsOrReturnsFailure` | Mock 4xx/5xx | Appropriate exception or failed result |
| `VerifyAsync_Success_ReturnsSuccessful` | Mock paid status | `IsSuccessful=true`, `TransactionReference` set |
| `VerifyAsync_Failed_ReturnsFailure` | Mock declined status | `IsSuccessful=false`, `FailureReason` set |
| `VerifyAsync_AmountMismatch_ReturnsFailure` | Mock paid with wrong amount | `IsSuccessful=false` |
| `VerifyAsync_CurrencyMismatch_ReturnsFailure` | Mock paid with wrong currency | `IsSuccessful=false` |

## 4.2 `AreebaWebhookServiceTests`

| Test | Scenario | Expected |
|------|----------|----------|
| `ProcessWebhook_SuccessfulPayment_CompletesBooking` | Valid signature + paid event | Attempt Completed; `ConfirmPaymentFromWebhookAsync` called once |
| `ProcessWebhook_FailedPayment_UpdatesAttemptOnly` | Valid signature + failed event | Attempt Failed; booking **not** confirmed |
| `ProcessWebhook_DuplicateEvent_IsIdempotent` | Same `ProviderEventId` twice | Second call returns OK; booking confirm called **once** |
| `ProcessWebhook_InvalidSignature_ThrowsUnauthorized` | Bad signature | 401 / `UnauthorizedException` |
| `ProcessWebhook_AmountMismatch_RejectsCompletion` | Paid event, verify returns amount mismatch | Attempt Failed; booking not confirmed |
| `ProcessWebhook_CurrencyMismatch_RejectsCompletion` | Currency mismatch on verify | Attempt Failed; booking not confirmed |
| `ProcessWebhook_UnknownTransaction_ReturnsNotFound` | Reference not in DB | Graceful failure; no booking mutation |

## 4.3 Existing test regression

| Suite | Action |
|-------|--------|
| `BookingServiceTests` | Ensure payment confirm paths still pass |
| `PaymentWebhookServiceTests` | Moyasar tests unchanged |
| Integration tests | No Areeba live calls; optional webhook controller test with mocked service |

## 4.4 Test fixtures

Store under `src/backend/Khadamati.Tests/Fixtures/Areeba/`:

- `session-create-success.json`
- `verify-paid.json`
- `verify-declined.json`
- `webhook-paid.json`
- `webhook-failed.json`
- `webhook-duplicate.json`

Payload shapes updated when official Areeba API documentation is received.

---

# 5. Stop conditions

## Do not (until explicit approval)

| # | Prohibited action |
|---|-------------------|
| 1 | **Enable Areeba in production** (`Payment:Provider=Areeba` in Production) |
| 2 | **Remove Moyasar** code, config, or webhook endpoint |
| 3 | **Run production migration** for Areeba-specific schema without staging validation |
| 4 | **Start Phase 1C** (API consolidation) |
| 5 | **Start React Native migration** |

## Safe without approval

| Action | Allowed |
|--------|---------|
| Implement adapter + webhook + attempt model behind `Payment:Provider=Areeba` | ✅ |
| Add EF migration for `BookingPaymentAttempts` + `PaymentWebhookEvents` | ✅ (apply to dev/CI only until validated) |
| Unit tests with mocks | ✅ |
| Local `Payment:Provider=Development` unchanged | ✅ |

---

# 6. Implementation order (post-approval)

```
1. DTOs + AreebaOptions configuration
2. BookingPaymentAttempt + PaymentWebhookEvent entities + migration
3. IPaymentAttemptRepository + IPaymentWebhookSignatureValidator
4. AreebaPaymentGateway (mocked HTTP tests)
5. AreebaWebhookService + AreebaWebhookController
6. DI registration + IntegrationReadinessService update
7. Unit test suite (Section 4)
8. STOP — submit for code review
9. (Later, when credentials arrive) Sandbox validation per AREEBA_EXTERNAL_DEPENDENCY_CHECKLIST.md
```

---

# 7. Approval checkpoint

| Deliverable | Status |
|-------------|--------|
| This readiness plan | ✅ Complete |
| Adapter implementation | ✅ Complete — see `AREEBA_IMPLEMENTATION_REPORT.md` |
| Unit test suite | ✅ Complete — see `AREEBA_TEST_REPORT.md` |
| Development verification | ✅ Complete — see `AREEBA_VERIFICATION_REPORT.md` |
| Sandbox validation | ⏸ Awaiting Areeba credentials |
| Production cutover | ⏸ Blocked |

**STOP.** Adapter implementation and development verification are complete. Await approval before sandbox validation or production cutover.
