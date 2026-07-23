# Phase 1A — Test Plan

## Automated tests

### Unit tests — `Khadamati.Tests`

| Test class | Cases |
|------------|-------|
| `BookingServiceTests` | Create booking success; slot conflict on create; confirm payment success; slot conflict on finalize; nearby craftsmen distance filter; accept booking |
| `CouponServiceTests` | Validate valid/invalid coupon; redeem increments UsedCount; redeem fails at max uses |

### Integration tests — `Khadamati.IntegrationTests`

| Test | Change |
|------|--------|
| `NearbyCraftsmen_WithCoordinates_ReturnsOrderedResults` | Coordinates updated to Beirut (33.8938, 35.5018) matching seeder |

### Regression

- All existing `Khadamati.Tests` and `Khadamati.IntegrationTests` must pass.

## Commands

```bash
cd src/backend
dotnet test Khadamati.Tests/Khadamati.Tests.csproj
dotnet test Khadamati.IntegrationTests/Khadamati.IntegrationTests.csproj
dotnet ef migrations script --project Khadamati.Infrastructure --startup-project Khadamati.API --idempotent
```

## Manual validation (optional)

1. Create two bookings for same craftsman/slot from two sessions — second should fail with 409.
2. Apply same coupon concurrently on subscribe — only allowed redemptions succeed.

## Acceptance criteria

- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Migration applies cleanly on empty and seeded database
- [ ] No new linter errors in changed files
