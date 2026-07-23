# Phase 1A — Data Integrity Implementation Plan

## Objective

Harden booking, slot reservation, and coupon usage against race conditions and concurrent updates before broader Phase 1 security and platform work.

## Scope (approved)

| Item | Approach |
|------|----------|
| RowVersion concurrency | Add `rowversion` column to `ServiceRequests` and `Coupons` |
| Booking concurrency handling | Map `DbUpdateConcurrencyException` to HTTP 409 via `ConflictException` |
| Slot reservation protection | Rely on filtered unique index + catch unique-constraint violations on insert |
| Coupon concurrency protection | Atomic `ExecuteUpdate` increment with `UsedCount < MaxUses` guard |
| Filtered unique indexes | Coupon code unique where `IsDeleted = 0` (soft-delete safe) |
| Missing critical indexes | Composite indexes on `ServiceRequests` and `BookingSlotReservations` for slot queries |
| BookingService tests | New unit test suite |
| Nearby provider search test fixes | Update integration test coordinates to Beirut (seeded data) |

## Out of scope (Phase 1B+)

- Secret rotation
- Profile API consolidation
- Global countries/regions admin model
- React Native migration

## Implementation order

1. Domain entities (`RowVersion` on `ServiceRequest`, `Coupon`)
2. EF Core configurations and migration
3. `DbPersistenceExceptionMapper` helper
4. `BookingService` save-path concurrency handling
5. `CouponService.TryRedeemAsync` + `UserSubscriptionService` integration
6. Unit and integration tests
7. CI validation

## Files changed (expected)

### Backend

- `Khadamati.Domain/Entities/ServiceRequest.cs`
- `Khadamati.Domain/Entities/AdminEntities.cs` (Coupon)
- `Khadamati.Infrastructure/Data/Configurations/EntityConfigurations.cs`
- `Khadamati.Infrastructure/Data/Configurations/AdminEntityConfigurations.cs`
- `Khadamati.Infrastructure/Common/DbPersistenceExceptionMapper.cs` (new)
- `Khadamati.Infrastructure/Services/BookingService.cs`
- `Khadamati.Infrastructure/Services/CouponService.cs`
- `Khadamati.Infrastructure/Services/UserSubscriptionService.cs`
- `Khadamati.Application/Interfaces/ICouponService.cs`
- `Khadamati.Infrastructure/Data/Migrations/*_Phase1ADataIntegrity.cs` (new)
- `Khadamati.Tests/Services/BookingServiceTests.cs` (new)
- `Khadamati.Tests/Services/CouponServiceTests.cs` (new)
- `Khadamati.IntegrationTests/ApiIntegrationTests.cs`

### Documentation

- `docs/phase1/*`

## API impact

- No new endpoints.
- Concurrent booking or coupon conflicts return **409 Conflict** with a clear message (existing middleware behavior).
- No breaking request/response schema changes.

## Web impact

- None in Phase 1A. Clients already handle 409 responses on booking flows.

## Mobile impact

- None in Phase 1A.

## Rollback method

See [PHASE1A_ROLLBACK_PLAN.md](./PHASE1A_ROLLBACK_PLAN.md).
