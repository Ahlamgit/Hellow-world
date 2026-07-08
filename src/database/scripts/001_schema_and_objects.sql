-- KHADAMATI Database Schema
-- Microsoft SQL Server
-- Version: 1.0.0

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'KhadamatiDb')
BEGIN
    CREATE DATABASE KhadamatiDb;
END
GO

USE KhadamatiDb;
GO

-- Audit trigger template applied via EF migrations for entity tables.
-- Additional database objects below supplement EF Core migrations.

-- View: Active Users with Profiles
IF OBJECT_ID('dbo.vw_ActiveUsers', 'V') IS NOT NULL DROP VIEW dbo.vw_ActiveUsers;
GO
CREATE VIEW dbo.vw_ActiveUsers
AS
SELECT
    u.Id,
    u.Email,
    u.Phone,
    u.Role,
    u.Status,
    u.VerificationStatus,
    u.SubscriptionStatus,
    p.FirstName,
    p.LastName,
    p.ProfilePictureUrl,
    p.PreferredLanguage,
    u.CreatedAt,
    u.LastLoginAt
FROM dbo.Users u
INNER JOIN dbo.UserProfiles p ON u.Id = p.UserId
WHERE u.IsDeleted = 0 AND u.Status = 2; -- Active
GO

-- View: Service Request Summary
IF OBJECT_ID('dbo.vw_ServiceRequestSummary', 'V') IS NOT NULL DROP VIEW dbo.vw_ServiceRequestSummary;
GO
CREATE VIEW dbo.vw_ServiceRequestSummary
AS
SELECT
    sr.Id,
    sr.Status,
    sr.ScheduledAt,
    sr.EstimatedPrice,
    sr.FinalPrice,
    s.NameEn AS ServiceName,
    cp.FirstName + ' ' + cp.LastName AS CustomerName,
    ISNULL(crp.FirstName + ' ' + crp.LastName, 'Unassigned') AS CraftsmanName,
    sr.CreatedAt
FROM dbo.ServiceRequests sr
INNER JOIN dbo.Services s ON sr.ServiceId = s.Id
INNER JOIN dbo.Users cu ON sr.CustomerId = cu.Id
INNER JOIN dbo.UserProfiles cp ON cu.Id = cp.UserId
LEFT JOIN dbo.Users cru ON sr.CraftsmanId = cru.Id
LEFT JOIN dbo.UserProfiles crp ON cru.Id = crp.UserId
WHERE sr.IsDeleted = 0;
GO

-- Stored Procedure: Get Nearby Craftsmen
IF OBJECT_ID('dbo.sp_GetNearbyCraftsmen', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetNearbyCraftsmen;
GO
CREATE PROCEDURE dbo.sp_GetNearbyCraftsmen
    @Latitude DECIMAL(10,7),
    @Longitude DECIMAL(10,7),
    @RadiusKm DECIMAL(10,2) = 25,
    @ServiceId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS CraftsmanId,
        p.FirstName,
        p.LastName,
        cp.Specialization,
        cp.Rating,
        cp.TotalReviews,
        cp.CompletedJobs,
        cp.IsAvailable,
        a.Latitude,
        a.Longitude,
        (6371 * ACOS(
            COS(RADIANS(@Latitude)) * COS(RADIANS(a.Latitude)) *
            COS(RADIANS(a.Longitude) - RADIANS(@Longitude)) +
            SIN(RADIANS(@Latitude)) * SIN(RADIANS(a.Latitude))
        )) AS DistanceKm
    FROM dbo.Users u
    INNER JOIN dbo.CraftsmanProfiles cp ON u.Id = cp.UserId
    INNER JOIN dbo.UserProfiles p ON u.Id = p.UserId
    INNER JOIN dbo.Addresses a ON u.Id = a.UserId AND a.IsDefault = 1
    WHERE u.Role = 2 -- Craftsman
      AND u.IsDeleted = 0
      AND u.Status = 2
      AND cp.IsAvailable = 1
      AND a.Latitude IS NOT NULL
      AND a.Longitude IS NOT NULL
      AND (@ServiceId IS NULL OR EXISTS (
          SELECT 1 FROM dbo.CraftsmanServices cs
          WHERE cs.CraftsmanProfileId = cp.Id
            AND cs.ServiceId = @ServiceId
            AND cs.IsAvailable = 1
            AND cs.IsDeleted = 0
      ))
    HAVING (6371 * ACOS(
        COS(RADIANS(@Latitude)) * COS(RADIANS(a.Latitude)) *
        COS(RADIANS(a.Longitude) - RADIANS(@Longitude)) +
        SIN(RADIANS(@Latitude)) * SIN(RADIANS(a.Latitude))
    )) <= @RadiusKm
    ORDER BY DistanceKm, cp.Rating DESC;
END
GO

-- Stored Procedure: Dashboard Statistics
IF OBJECT_ID('dbo.sp_GetDashboardStats', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetDashboardStats;
GO
CREATE PROCEDURE dbo.sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM dbo.Users WHERE IsDeleted = 0 AND Role = 1) AS TotalCustomers,
        (SELECT COUNT(*) FROM dbo.Users WHERE IsDeleted = 0 AND Role = 2) AS TotalCraftsmen,
        (SELECT COUNT(*) FROM dbo.Users WHERE IsDeleted = 0 AND Role = 3) AS TotalStores,
        (SELECT COUNT(*) FROM dbo.ServiceRequests WHERE IsDeleted = 0 AND Status = 2) AS PendingRequests,
        (SELECT COUNT(*) FROM dbo.ServiceRequests WHERE IsDeleted = 0 AND Status = 5) AS CompletedRequests,
        (SELECT ISNULL(SUM(FinalPrice), 0) FROM dbo.ServiceRequests WHERE IsDeleted = 0 AND Status = 5) AS TotalRevenue;
END
GO

-- Audit log insert procedure
IF OBJECT_ID('dbo.sp_InsertAuditLog', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_InsertAuditLog;
GO
CREATE PROCEDURE dbo.sp_InsertAuditLog
    @TableName NVARCHAR(128),
    @Action NVARCHAR(50),
    @EntityId NVARCHAR(50) = NULL,
    @OldValues NVARCHAR(MAX) = NULL,
    @NewValues NVARCHAR(MAX) = NULL,
    @UserId NVARCHAR(50) = NULL,
    @UserEmail NVARCHAR(256) = NULL,
    @IpAddress NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AuditLogs (TableName, Action, EntityId, OldValues, NewValues, UserId, UserEmail, IpAddress, CreatedAt)
    VALUES (@TableName, @Action, @EntityId, @OldValues, @NewValues, @UserId, @UserEmail, @IpAddress, GETUTCDATE());
END
GO

-- Additional performance indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_Status_Role' AND object_id = OBJECT_ID('dbo.Users'))
    CREATE NONCLUSTERED INDEX IX_Users_Status_Role ON dbo.Users (Status, Role) INCLUDE (Email, Phone);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ServiceRequests_Status_CreatedAt' AND object_id = OBJECT_ID('dbo.ServiceRequests'))
    CREATE NONCLUSTERED INDEX IX_ServiceRequests_Status_CreatedAt ON dbo.ServiceRequests (Status, CreatedAt DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Addresses_UserId_IsDefault' AND object_id = OBJECT_ID('dbo.Addresses'))
    CREATE NONCLUSTERED INDEX IX_Addresses_UserId_IsDefault ON dbo.Addresses (UserId, IsDefault) INCLUDE (Latitude, Longitude);
GO

PRINT 'KHADAMATI database objects created successfully.';
GO
