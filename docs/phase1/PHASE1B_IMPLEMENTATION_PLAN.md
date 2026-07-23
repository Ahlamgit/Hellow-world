# Phase 1B — Security Hardening Implementation Plan

## Objective

Strengthen authentication, authorization, webhook integrity, and input validation without infrastructure changes or secret rotation execution.

## Scope (approved)

| Item | Approach |
|------|----------|
| Refresh token lookup optimization | Remove eager `Include` on token lookup; add composite indexes |
| HTTP 401 vs 403 | `ForbiddenException` for authenticated permission/resource denials |
| HMAC-SHA256 webhook verification | Verify Moyasar signature against raw request body; fail-closed in Production/Staging |
| FluentValidation on critical commands | Nested DTO validation in MediatR pipeline + payment validators |
| Admin create-user password confirmation | `ConfirmPassword` on DTO, validator, and admin web form |
| Android/iOS change password | Wire existing `POST /auth/change-password` API in native apps |
| Secret rotation plan | Documentation only (`docs/security/SECRET_ROTATION_PLAN.md`) |

## Out of scope

- Secret rotation execution
- Infrastructure / deployment changes
- React Native migration
- Broader mobile feature work

## Implementation order

1. `ForbiddenException` + middleware mapping
2. Authorization and service-layer 403 corrections
3. `ValidationBehavior` nested DTO validation
4. Payment webhook HMAC verification
5. Refresh token repository + indexes migration
6. Admin create-user confirm password (API + web)
7. Mobile change-password screens
8. Tests and verification report

## API impact

| Endpoint | Change |
|----------|--------|
| Admin routes (permission denied) | 401 → **403** when bearer token is valid |
| Resource access denied (bookings, chat, etc.) | 401 → **403** when authenticated |
| `POST /webhooks/moyasar` | HMAC-SHA256 required when secret configured (Production/Staging) |
| `POST /admin/users` | `confirmPassword` required; must match `password` |
| `POST /auth/change-password` | No API change; mobile clients now call it |

## Files changed

### Backend

- `Khadamati.Application/Common/Exceptions.cs`
- `Khadamati.API/Middleware/ExceptionHandlingMiddleware.cs`
- `Khadamati.Application/Authorization/AdminAuthorization.cs`
- `Khadamati.Application/Behaviors/ValidationBehavior.cs`
- `Khadamati.Application/Validators/PaymentValidators.cs` (new)
- `Khadamati.Application/Validators/AdminUserValidators.cs`
- `Khadamati.Application/DTOs/Users/AdminUserDtos.cs`
- `Khadamati.Infrastructure/Repositories/SessionRepository.cs`
- `Khadamati.Infrastructure/Data/Configurations/EntityConfigurations.cs`
- `Khadamati.Infrastructure/Services/Payments/PaymentWebhookService.cs`
- Service-layer 403 updates (Booking, Chat, Store, Craftsman, UserManagement, etc.)
- `Khadamati.Infrastructure/Data/Migrations/*_Phase1BRefreshTokenIndexes.cs` (new)

### Web admin

- `src/web/src/admin/adminUsersApi.ts`
- `src/web/src/admin/AdminUsersPage.tsx`

### Mobile

- Android: `ApiService.kt`, `AuthRepository.kt`, `AuthViewModel.kt`, `PasswordRecoveryScreens.kt`, `ProfileScreen.kt`, `NavGraph.kt`
- iOS: `APIEndpoints.swift`, `AuthService.swift`, `PasswordRecoveryView.swift`, `ProfileView.swift`, localization

### Documentation

- `docs/phase1/PHASE1B_*`
- `docs/security/SECRET_ROTATION_PLAN.md`
- `docs/DATABASE_BASELINE.md` (Phase 1B indexes)
