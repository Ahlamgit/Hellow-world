# Areeba — Test Report

**Status:** Complete  
**Date:** 2026-07-23  
**Branch:** `cursor/areeba-adapter-7b80`

---

## Test execution summary

| Suite | Passed | Failed | Skipped | Total |
|-------|--------|--------|---------|-------|
| Unit (`Khadamati.Tests`) | 107 | 0 | 0 | 107 |
| Integration (`Khadamati.IntegrationTests`) | 7 | 0 | 0 | 7 |

Command: `dotnet test` from `src/backend/`

---

## New Areeba-specific tests

### Gateway (`AreebaPaymentGatewayTests`)

| Test | Requirement covered |
|------|-------------------|
| `CreateSessionAsync_SuccessfulResponse_ReturnsSession` | Successful payment response handling |
| `CreateSessionAsync_FailedResponse_Throws` | Failed payment response |
| `CreateSessionAsync_InvalidResponse_Throws` | Invalid response |
| `CreateSessionAsync_Timeout_Throws` | Timeout handling |
| `VerifyAsync_SuccessfulPayment_ReturnsVerified` | Successful verification |
| `VerifyAsync_FailedPayment_ReturnsFailure` | Failed payment response |
| `VerifyAsync_AmountMismatch_ReturnsFailure` | Amount mismatch |
| `VerifyAsync_CurrencyMismatch_ReturnsFailure` | Currency mismatch |
| `VerifyAsync_UnknownTransaction_ReturnsFailure` | Unknown transaction |
| `VerifyAsync_StatusMapping_Works` (Theory) | Status mapping (`paid`, `captured`, `success`, `completed`, `failed`) |

### Webhook (`AreebaWebhookServiceTests`)

| Test | Requirement covered |
|------|-------------------|
| `ProcessWebhookAsync_ValidSignature_ConfirmsPayment` | Valid signature |
| `ProcessWebhookAsync_InvalidSignature_ThrowsUnauthorized` | Invalid signature |
| `ProcessWebhookAsync_DuplicateWebhook_IsIgnored` | Duplicate webhook |
| `ProcessWebhookAsync_UnknownPaymentAttempt_ReturnsNotFound` | Unknown payment attempt |
| `ProcessWebhookAsync_DelayedWebhook_OnCompletedAttempt_IsIgnored` | Delayed webhook |
| `ProcessWebhookAsync_CompletedAttemptCannotBeOverwritten` | Completed payment cannot be overwritten |

### Integration readiness

| Test | Requirement covered |
|------|-------------------|
| `GetReport_AreebaFullyConfigured_IsProductionReadyForPayment` | Areeba config evaluation |
| `GetReport_AreebaMissingMerchant_IsMisconfigured` | Missing config detection |

---

## Regression coverage

Existing suites continue to pass unchanged:
- `BookingServiceTests` (payment confirm flow with Development gateway)
- `PaymentWebhookServiceTests` (Moyasar webhook HMAC)
- `IntegrationReadinessServiceTests` (Moyasar paths preserved)
- All Phase 1A/1B integration tests (7/7 pass)

---

## Test approach

- Gateway tests use a stub `HttpMessageHandler` — no live Areeba API calls.
- Webhook tests use in-memory EF database with real `PaymentAttemptService` and `UnitOfWork`.
- Signature tests use `PaymentWebhookService.ComputeHmacSha256Hex` with known secret/body pairs.
