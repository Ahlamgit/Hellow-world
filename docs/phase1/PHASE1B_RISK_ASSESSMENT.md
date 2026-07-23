# Phase 1B — Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Clients expect 401 for permission denied | Medium | Low | Document 403 change; integration test updated |
| Moyasar webhook rejects valid callbacks | Medium | High | HMAC on raw body; dev environment skips when secret unset |
| Nested validation breaks MediatR commands | Low | High | Skip primitives/strings; use `IValidator` dispatch; integration tests |
| Refresh token lookup without Include | Low | Low | Auth refresh loads user separately via `GetUserByIdWithRolesAsync` |
| Admin form breaking change (`confirmPassword`) | Low | Low | Web form updated; API backward-compatible if field omitted (validation fails) |
| Mobile change-password UX gaps | Low | Low | Minimal screen; reuses existing API validation |

## Security improvements

- Fail-closed webhook verification in Production/Staging
- Correct authorization semantics (403 vs 401)
- Password confirmation on admin user creation
- Command/DTO validation for payment webhooks

## Residual risks (accepted for Phase 1B)

- Secret rotation not executed (documented only)
- No rate-limit changes on webhook endpoint
- Native mobile apps remain until React Native parity
