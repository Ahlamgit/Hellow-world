/*
================================================================================
 KHADAMATI - Master Database Deployment Script
 Microsoft SQL Server 2022+
 Run as: sqlcmd -S localhost -U sa -P "YourPassword" -i 000_MasterDeploy.sql
================================================================================
*/
:setvar ScriptDir "."
:r $(ScriptDir)/001_CreateDatabase.sql
:r $(ScriptDir)/002_CreateTables.sql
:r $(ScriptDir)/003_CreateIndexes.sql
:r $(ScriptDir)/004_CreateViews.sql
:r $(ScriptDir)/005_CreateStoredProcedures.sql
:r $(ScriptDir)/006_CreateTriggers.sql
:r $(ScriptDir)/007_SeedData.sql
GO
PRINT 'KHADAMATI database deployment completed successfully.';
GO
