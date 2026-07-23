# KHADAMATI — Database Baseline (Authoritative)

This document is the **canonical reference** for the live SQL Server schema used by the ASP.NET Core API (EF Core). Legacy scripts under `src/database/` may differ and are not authoritative for the running application.

**Last validated:** 2026-07-23 (Phase 1A SQL Server apply on SQL Server 2022 Developer, Linux)

---

## Source of truth

| Layer | Authority |
|-------|-----------|
| **Runtime schema** | SQL Server database `KhadamatiDb` |
| **Schema definition** | EF Core migrations in `src/backend/Khadamati.Infrastructure/Data/Migrations/` |
| **Column naming** | `AuditColumnConfiguration` + entity configurations |
| **Legacy SQL scripts** | `src/database/` — historical reference only |

Apply migrations:

```bash
cd src/backend
dotnet ef database update --project Khadamati.Infrastructure --startup-project Khadamati.API
```

---

## EF property → SQL column mapping (audit fields)

All entities inheriting `BaseEntity` use this mapping globally (`AuditColumnConfiguration`):

| C# property (`BaseEntity`) | SQL column | Type |
|----------------------------|------------|------|
| `CreatedAt` | `CreatedDate` | `datetime2` |
| `UpdatedAt` | `ModifiedDate` | `datetime2` |
| `UpdatedBy` | `ModifiedBy` | `nvarchar(max)` |
| `IsDeleted` | **`Deleted`** | `bit` |
| `DeletedAt` | `DeletedDate` | `datetime2` |
| `DeletedBy` | `DeletedBy` | `nvarchar(max)` |

**Critical rule:** In raw SQL, filtered indexes, and migration scripts, always use the **physical column name** (`Deleted`), not the C# property name (`IsDeleted`).

---

## Soft-delete convention

- C# query filters: `!entity.IsDeleted` (EF translates to `Deleted = 0`)
- SQL filters: `WHERE Deleted = 0`
- Filtered unique indexes: `WHERE [Deleted] = 0` (and additional predicates as needed)

---

## Phase 1A schema additions (migration `20260723144009_Phase1ADataIntegrity`)

| Table | Column / index | Notes |
|-------|----------------|-------|
| `ServiceRequests` | `RowVersion` | `rowversion`, optimistic concurrency |
| `Coupons` | `RowVersion` | `rowversion`, optimistic concurrency |
| `Coupons` | `IX_Coupons_Code_Active` | Unique on `Code` WHERE `[Deleted] = 0` |
| `ServiceRequests` | `IX_ServiceRequests_CraftsmanId_Status_ScheduledAt` | Composite index |
| `BookingSlotReservations` | `IX_BookingSlotReservations_CraftsmanId_IsActive_SlotRange` | Composite index |
| `BookingSlotReservations` | `IX_BookingSlotReservations_ActiveSlot` | Renamed filtered unique slot index |

---

## Other naming differences (C# vs SQL)

| Area | C# / EF | SQL column | Notes |
|------|---------|------------|-------|
| Soft delete | `IsDeleted` | `Deleted` | **All `BaseEntity` tables** |
| Created timestamp | `CreatedAt` | `CreatedDate` | All `BaseEntity` tables |
| Updated timestamp | `UpdatedAt` | `ModifiedDate` | All `BaseEntity` tables |
| Primary keys | `Id` (`Guid`) | `Id` (`uniqueidentifier`) | Business entities |
| Enum storage | `Status`, `Role`, etc. | `int` | Via `HasConversion<int>()` |

No tables use a physical column named `IsDeleted`. Any `HasFilter("[IsDeleted] = …")` in EF configuration is **incorrect** and will fail on SQL Server apply.

---

## Validation queries (post-migration)

```sql
-- Audit column names on a sample table
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Coupons'
  AND COLUMN_NAME IN ('Deleted', 'CreatedDate', 'ModifiedDate', 'RowVersion');

-- Filtered indexes must reference Deleted, not IsDeleted
SELECT i.name, i.filter_definition
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'Coupons' AND i.filter_definition IS NOT NULL;
```

---

## Related documentation

- `docs/DATABASE.md` — legacy enterprise SQL script documentation
- `docs/phase1/PHASE1A_CLOSURE_NOTES.md` — Phase 1A migration lessons
- `docs/phase1/PHASE1A_VERIFICATION_REPORT.md` — live SQL validation evidence
