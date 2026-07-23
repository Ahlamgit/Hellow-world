# Phase 1B — Database Migration Plan

## Migration

`20260723180606_Phase1BRefreshTokenIndexes`

## Changes

| Table | Index | Purpose |
|-------|-------|---------|
| `RefreshTokens` | `IX_RefreshTokens_ExpiresAt` | Token cleanup / expiry sweeps |
| `RefreshTokens` | `IX_RefreshTokens_UserId_RevokedAt_ExpiresAt` | Active session listing per user |

## Pre-migration checks

```sql
SELECT name FROM sys.indexes
WHERE object_id = OBJECT_ID('RefreshTokens')
  AND name IN ('IX_RefreshTokens_ExpiresAt', 'IX_RefreshTokens_UserId_RevokedAt_ExpiresAt');
```

Expected: no rows (indexes do not exist yet).

## Apply

```bash
cd src/backend
dotnet ef database update --project Khadamati.Infrastructure --startup-project Khadamati.API
```

## Rollback

```bash
dotnet ef database update 20260723144009_Phase1ADataIntegrity \
  --project Khadamati.Infrastructure --startup-project Khadamati.API
```

## Notes

- Additive only — no column changes
- No `HasFilter()` — no `Deleted`/`IsDeleted` risk from Phase 1A
- Safe to apply online (non-clustered indexes)
