/*
================================================================================
 KHADAMATI - 001 Create Database
================================================================================
*/
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'KhadamatiDb')
BEGIN
    CREATE DATABASE KhadamatiDb
    COLLATE Arabic_CI_AS; -- Supports Arabic text with case-insensitive comparison
END
GO

USE KhadamatiDb;
GO

-- Ensure required schemas
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'audit')
    EXEC('CREATE SCHEMA audit');
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'ref')
    EXEC('CREATE SCHEMA ref');
GO

PRINT 'Database KhadamatiDb ready.';
GO
