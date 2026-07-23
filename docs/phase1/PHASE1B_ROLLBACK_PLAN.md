# Phase 1B — Rollback Plan

## Application rollback

1. Deploy previous API build (pre-1B)
2. Extra indexes remain harmless on older builds
3. Revert mobile/web clients if needed (change-password is additive)

## Database rollback

```bash
dotnet ef database update 20260723144009_Phase1ADataIntegrity \
  --project Khadamati.Infrastructure --startup-project Khadamati.API
```

Drops:

- `IX_RefreshTokens_ExpiresAt`
- `IX_RefreshTokens_UserId_RevokedAt_ExpiresAt`

## Behavioral rollback notes

| Change | Rollback effect |
|--------|-----------------|
| 403 vs 401 | Old builds return 401 for permission denied |
| HMAC webhooks | Old builds used plain secret compare (less secure) |
| `confirmPassword` | Old admin UI can omit field; validator not enforced |
| Nested validation | Old pipeline only validated top-level command validators |

## No data loss

Phase 1B migration is index-only. Rollback does not affect refresh token rows.
