# Phase 1B — Test Plan

## Unit tests

| Area | Test file | Cases |
|------|-----------|-------|
| Payment webhook HMAC | `PaymentWebhookServiceTests.cs` | Invalid signature, valid HMAC, dev without secret, pending ignored |
| Admin user validator | `AdminUserValidatorTests.cs` | Mismatched confirm password |
| Existing suites | All `Khadamati.Tests` | Regression — 85 tests |

## Integration tests

| Test | Expected |
|------|----------|
| `AdminAnalytics_WithoutReportsPermission_ReturnsForbidden` | HTTP 403 |
| `Login_WithSeededAdmin_ReturnsTokenWithPermissions` | HTTP 200 (validation pipeline) |
| All 7 integration tests | Pass |

## Manual verification (optional)

1. Admin create user — confirm password mismatch blocked in UI and API
2. Craftsman token on `/admin/analytics/data` — 403
3. Android/iOS profile → Change Password → success message
4. Moyasar webhook with wrong signature in staging — 401

## Commands

```bash
cd src/backend
dotnet test Khadamati.Tests/Khadamati.Tests.csproj
dotnet test Khadamati.IntegrationTests/Khadamati.IntegrationTests.csproj
```
