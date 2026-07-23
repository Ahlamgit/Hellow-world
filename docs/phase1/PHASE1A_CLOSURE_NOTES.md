# Phase 1A — Closure Notes

**Status:** Closed and approved for merge  
**Date:** 2026-07-23  
**Branch:** `cursor/phase1a-data-integrity-7b80`

---

## 1. Migration lessons learned

### EF model vs physical SQL schema alignment

Phase 1A exposed a gap between **C# property names** and **physical SQL column names** when defining filtered indexes.

| Layer | Name used |
|-------|-----------|
| C# entity (`BaseEntity`) | `IsDeleted` |
| SQL column (`AuditColumnConfiguration`) | `Deleted` |
| EF query filters | `!e.IsDeleted` (translated correctly) |
| Raw `HasFilter()` strings | Must use **`[Deleted]`** — not `[IsDeleted]` |

**Root cause:** `HasFilter()` accepts a raw SQL fragment. EF does not translate C# property names inside filter strings. The global `AuditColumnConfiguration` maps `IsDeleted` → `Deleted`, but that mapping does not apply to literal filter text.

**First migration apply failed** with:

```
Invalid column name 'IsDeleted'.
```

**Fix applied:** Updated `AdminEntityConfigurations.cs`, `EntityConfigurations.cs`, migration `20260723144009_Phase1ADataIntegrity`, and model snapshot to use `[Deleted] = 0`.

**Validated on SQL Server 2022** — migration applied successfully; data preserved.

### Deleted / IsDeleted naming inconsistency

This is **system-wide** for all `BaseEntity`-derived tables:

```csharp
// Khadamati.Infrastructure/Data/AuditColumnConfiguration.cs
builder.Property(e => e.IsDeleted).HasColumnName("Deleted");
```

| Context | Correct reference |
|---------|-------------------|
| C# / LINQ / EF queries | `IsDeleted` |
| SQL scripts / `HasFilter()` / raw SQL | `Deleted` |
| `INFORMATION_SCHEMA` / `sys.indexes` | `Deleted` |

### Other entities with similar naming differences

| C# property | SQL column | Scope |
|-------------|------------|-------|
| `CreatedAt` | `CreatedDate` | All `BaseEntity` tables |
| `UpdatedAt` | `ModifiedDate` | All `BaseEntity` tables |
| `UpdatedBy` | `ModifiedBy` | All `BaseEntity` tables |
| `DeletedAt` | `DeletedDate` | All `BaseEntity` tables |
| `IsDeleted` | `Deleted` | All `BaseEntity` tables |

No other property/column mismatches were found beyond the standard audit mapping. Enum properties are stored as `int` with matching column names.

### Process improvements for future migrations

1. **Always use physical column names** in `HasFilter()`, raw SQL, and migration `filter:` arguments.
2. **Run `dotnet ef database update` against SQL Server** before merge — in-memory EF tests do not catch filter column mismatches.
3. **Reference `docs/DATABASE_BASELINE.md`** as the authoritative column naming guide.
4. **Add migration review checklist:** grep new migrations for `[IsDeleted]` and replace with `[Deleted]`.

---

## 2. Migration quality check

### Correct naming conventions

| Item | Assessment |
|------|------------|
| Index names | ✅ Descriptive (`IX_Coupons_Code_Active`, `IX_ServiceRequests_CraftsmanId_Status_ScheduledAt`) |
| Filter predicates | ✅ Fixed to `[Deleted] = 0` |
| RowVersion columns | ✅ Standard SQL Server `rowversion` type |
| Rename vs recreate (slot index) | ✅ `RenameIndex` preserves existing filtered definition |

### Environment-specific assumptions

| Assumption | Risk | Mitigation |
|------------|------|------------|
| SQL Server filtered indexes | Low | Project targets SQL Server; documented in baseline |
| `sp_rename` for index rename | Low | Standard SQL Server operation |
| No duplicate active coupon codes | Medium | Pre-migration validation query documented |

No hardcoded hostnames, file paths, or environment-specific credentials in the migration.

### Safe rollback behavior

| Down step | Safety |
|-----------|--------|
| Drop composite indexes | ✅ Metadata only |
| Drop `RowVersion` columns | ✅ No business data loss |
| Recreate `IX_Coupons_Code` (non-filtered) | ⚠️ Fails if duplicate codes exist across soft-deleted rows — document pre-check |
| Rename slot index back | ✅ Metadata only |

**Down() not executed live** — reviewed in source. Rollback procedure in `PHASE1A_ROLLBACK_PLAN.md`.

### Production compatibility

| Concern | Status |
|---------|--------|
| Existing rows preserved | ✅ Validated with pre-migration test data |
| Additive schema changes | ✅ No column drops |
| Online index creation | ✅ Non-clustered indexes; no table rebuild |
| Client API impact | ✅ None |
| Backward-compatible app rollback | ✅ Extra columns/indexes ignored by pre-1A app builds |

---

## 3. DATABASE_BASELINE.md

Created `docs/DATABASE_BASELINE.md` documenting:

- EF property → SQL column mapping for audit fields
- `Deleted` vs `IsDeleted` rule
- Phase 1A schema additions
- SQL Server as authoritative runtime schema
- Validation queries

`docs/DATABASE.md` (legacy script documentation) remains for reference; **`DATABASE_BASELINE.md` is authoritative for the EF Core / API stack**.

---

## 4. Phase 1A deliverables (closed)

| Deliverable | Status |
|-------------|--------|
| RowVersion concurrency | ✅ |
| Booking/slot/coupon integrity | ✅ |
| Indexes + filtered uniqueness | ✅ |
| Unit tests (83) | ✅ |
| Integration tests (7) | ✅ |
| SQL Server live validation | ✅ |
| Verification report | ✅ |
| Closure notes | ✅ |
| Database baseline | ✅ |

**Phase 1B may proceed.**
