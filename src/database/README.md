# KHADAMATI Database Scripts

SQL Server deployment scripts for the KHADAMATI platform.

See [DATABASE.md](../../docs/DATABASE.md) for full ER diagram, table explanations, and relationships.

## Quick Deploy

```bash
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 001_CreateDatabase.sql
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 002_CreateTables.sql
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 003_CreateIndexes.sql
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 004_CreateViews.sql
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 005_CreateStoredProcedures.sql
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 006_CreateTriggers.sql
sqlcmd -S localhost -U sa -P "Khadamati@2024!" -i 007_SeedData.sql
```

## Conventions

- **Audit columns** on every table: `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`, `Deleted`, `DeletedDate`, `DeletedBy`
- **Soft delete**: `Deleted = 0` filter in all queries; filtered indexes on active rows
- **Identity PKs**: Reference/audit tables use `INT` or `BIGINT IDENTITY`
- **GUID PKs**: Business entities use `UNIQUEIDENTIFIER` with `NEWSEQUENTIALID()`
- **Schemas**: `ref` (lookups), `dbo` (business), `audit` (audit trail)
