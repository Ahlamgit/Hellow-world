# Phase 1A — Database Migration Plan

## Migration name

`Phase1ADataIntegrity`

## Existing schema impact

| Table | Change | Notes |
|-------|--------|-------|
| `ServiceRequests` | Add `RowVersion` (`rowversion`, not null) | EF optimistic concurrency token |
| `Coupons` | Add `RowVersion` (`rowversion`, not null) | EF optimistic concurrency token |
| `Coupons` | Replace `IX_Coupons_Code` with filtered unique index | Allows re-use of codes after soft-delete |
| `ServiceRequests` | Add `IX_ServiceRequests_CraftsmanId_Status_ScheduledAt` | Slot availability queries |
| `BookingSlotReservations` | Add `IX_BookingSlotReservations_CraftsmanId_IsActive_SlotRange` | Active overlap lookups |

### Tables not modified

- `Users`, `BookingPayments`, `UserSubscriptions` — no schema changes in 1A.

### Data impact

- `RowVersion` columns are auto-populated by SQL Server on insert/update; no data backfill required.
- Index changes are online-friendly metadata operations; no row updates.

## Migration script explanation

### Up

1. **ServiceRequests.RowVersion** — `ALTER TABLE ADD RowVersion rowversion NOT NULL`
2. **Coupons.RowVersion** — same pattern
3. **Coupons index** — drop `IX_Coupons_Code`, create `IX_Coupons_Code_Active` with filter `[IsDeleted] = 0`
4. **ServiceRequests index** — composite on `(CraftsmanId, Status, ScheduledAt)`
5. **BookingSlotReservations index** — composite on `(CraftsmanId, IsActive, SlotStart, SlotEnd)`

### Down

Reverse index changes, drop `RowVersion` columns.

## Pre-migration checklist

- [ ] Backup database (staging/production when deployed)
- [ ] Run `dotnet ef migrations script` and review output
- [ ] Confirm no duplicate active coupon codes exist (`IsDeleted = 0`)
- [ ] Confirm no duplicate active slot reservations exist for same craftsman + slot start

## Validation queries (post-migration)

```sql
-- RowVersion columns exist
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('ServiceRequests', 'Coupons') AND COLUMN_NAME = 'RowVersion';

-- Filtered coupon index
SELECT name, filter_definition FROM sys.indexes
WHERE object_id = OBJECT_ID('Coupons') AND name LIKE '%Code%';

-- No duplicate active slots
SELECT CraftsmanId, SlotStart, COUNT(*)
FROM BookingSlotReservations
WHERE IsActive = 1 AND IsDeleted = 0
GROUP BY CraftsmanId, SlotStart
HAVING COUNT(*) > 1;
```

## Rollback strategy

1. Deploy previous application build (without RowVersion entity properties).
2. Run migration `Down` or restore from backup if Down is unsafe.
3. See [PHASE1A_ROLLBACK_PLAN.md](./PHASE1A_ROLLBACK_PLAN.md).
