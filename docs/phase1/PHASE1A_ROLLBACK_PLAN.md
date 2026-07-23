# Phase 1A — Rollback Plan

## When to rollback

- Migration fails or leaves database in inconsistent state
- Unexpected 409 rate spike on booking or coupon flows after deploy
- Performance regression on slot lookup queries (unlikely — indexes are additive)

## Rollback steps

### 1. Application rollback

Deploy the previous application artifact (pre-Phase-1A build). The old build does not reference `RowVersion` properties; EF will ignore extra columns if migration Up was applied but app is rolled back — **however**, if the new app wrote rows with concurrency tokens, the old app still functions because it does not use RowVersion.

**Recommended:** Roll back app and database together.

### 2. Database rollback

```bash
cd src/backend
dotnet ef database update <PreviousMigrationName> \
  --project Khadamati.Infrastructure \
  --startup-project Khadamati.API
```

Previous migration: `PlatformDefaultsUsdLebanon` (`20260723120000`).

Or run the `Down()` method from `Phase1ADataIntegrity` migration manually:

1. Drop new indexes
2. Recreate `IX_Coupons_Code` (non-filtered)
3. Drop `RowVersion` from `ServiceRequests` and `Coupons`

### 3. Verify rollback

- Health endpoint returns ready
- Create booking flow works
- Coupon validation works
- Integration test suite passes against rolled-back environment

## Data considerations

- Bookings created under 1A remain valid; no data loss from rollback.
- Coupon `UsedCount` increments from atomic redeem are permanent — rollback does not decrement. Acceptable; manual adjustment via admin if needed.

## Communication

Document rollback in deployment notes. No customer-facing schema version change required.
