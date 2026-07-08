/*
================================================================================
 KHADAMATI - 005 Create Stored Procedures
================================================================================
*/
USE KhadamatiDb;
GO

CREATE OR ALTER PROCEDURE audit.sp_InsertAuditLog
    @SchemaName      NVARCHAR(128) = N'dbo',
    @TableName       NVARCHAR(128),
    @Action          NVARCHAR(20),
    @EntityId        NVARCHAR(50) = NULL,
    @OldValues       NVARCHAR(MAX) = NULL,
    @NewValues       NVARCHAR(MAX) = NULL,
    @ChangedColumns  NVARCHAR(MAX) = NULL,
    @UserId          NVARCHAR(50) = NULL,
    @UserEmail       NVARCHAR(256) = NULL,
    @IpAddress       NVARCHAR(50) = NULL,
    @CreatedBy       NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO audit.AuditLogs
        (SchemaName, TableName, Action, EntityId, OldValues, NewValues, ChangedColumns, UserId, UserEmail, IpAddress, CreatedBy, CreatedDate)
    VALUES
        (@SchemaName, @TableName, @Action, @EntityId, @OldValues, @NewValues, @ChangedColumns, @UserId, @UserEmail, @IpAddress, @CreatedBy, SYSUTCDATETIME());
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetNearbyCraftsmen
    @Latitude    DECIMAL(10,7),
    @Longitude   DECIMAL(10,7),
    @RadiusKm    DECIMAL(10,2) = 25,
    @ServiceId   UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH CraftsmanDistances AS
    (
        SELECT
            u.Id AS CraftsmanUserId,
            p.FirstName,
            p.LastName,
            cp.Specialization,
            cp.Rating,
            cp.TotalReviews,
            cp.CompletedJobs,
            cp.IsAvailable,
            a.Latitude,
            a.Longitude,
            CAST(
                6371 * ACOS(
                    COS(RADIANS(@Latitude)) * COS(RADIANS(a.Latitude)) *
                    COS(RADIANS(a.Longitude) - RADIANS(@Longitude)) +
                    SIN(RADIANS(@Latitude)) * SIN(RADIANS(a.Latitude))
                ) AS DECIMAL(10,2)
            ) AS DistanceKm
        FROM dbo.Users u
        INNER JOIN dbo.CraftsmanProfiles cp ON u.Id = cp.UserId AND cp.Deleted = 0
        INNER JOIN dbo.UserProfiles p ON u.Id = p.UserId AND p.Deleted = 0
        INNER JOIN dbo.Addresses a ON u.Id = a.UserId AND a.IsDefault = 1 AND a.Deleted = 0
        INNER JOIN ref.UserStatuses us ON u.StatusId = us.StatusId AND us.StatusCode = N'Active'
        WHERE u.Deleted = 0
          AND u.RoleId = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Craftsman')
          AND cp.IsAvailable = 1
          AND a.Latitude IS NOT NULL
          AND a.Longitude IS NOT NULL
          AND (@ServiceId IS NULL OR EXISTS (
              SELECT 1 FROM dbo.CraftsmanServices cs
              INNER JOIN dbo.CraftsmanProfiles cp2 ON cs.CraftsmanProfileId = cp2.Id
              WHERE cp2.UserId = u.Id AND cs.ServiceId = @ServiceId AND cs.IsAvailable = 1 AND cs.Deleted = 0
          ))
    )
    SELECT *
    FROM CraftsmanDistances
    WHERE DistanceKm <= @RadiusKm
    ORDER BY DistanceKm ASC, Rating DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.vw_DashboardMetrics;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_SoftDeleteUser
    @UserId      UNIQUEIDENTIFIER,
    @DeletedBy   NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Users
        SET Deleted = 1, DeletedDate = SYSUTCDATETIME(), DeletedBy = @DeletedBy, ModifiedDate = SYSUTCDATETIME(), ModifiedBy = @DeletedBy
        WHERE Id = @UserId AND Deleted = 0;

        UPDATE dbo.UserProfiles SET Deleted = 1, DeletedDate = SYSUTCDATETIME(), DeletedBy = @DeletedBy, ModifiedDate = SYSUTCDATETIME(), ModifiedBy = @DeletedBy WHERE UserId = @UserId AND Deleted = 0;
        UPDATE dbo.Addresses SET Deleted = 1, DeletedDate = SYSUTCDATETIME(), DeletedBy = @DeletedBy, ModifiedDate = SYSUTCDATETIME(), ModifiedBy = @DeletedBy WHERE UserId = @UserId AND Deleted = 0;
        UPDATE dbo.RefreshTokens SET Deleted = 1, DeletedDate = SYSUTCDATETIME(), DeletedBy = @DeletedBy, ModifiedDate = SYSUTCDATETIME(), ModifiedBy = @DeletedBy WHERE UserId = @UserId AND Deleted = 0;

        EXEC audit.sp_InsertAuditLog
            @TableName = N'Users', @Action = N'SOFT_DELETE', @EntityId = CAST(@UserId AS NVARCHAR(50)),
            @UserId = CAST(@UserId AS NVARCHAR(50)), @CreatedBy = @DeletedBy;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateServiceRequestStatus
    @ServiceRequestId    UNIQUEIDENTIFIER,
    @NewRequestStatusId  INT,
    @ChangedByUserId     UNIQUEIDENTIFIER = NULL,
    @ChangeReason        NVARCHAR(500) = NULL,
    @ModifiedBy          NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @OldRequestStatusId INT;

    SELECT @OldRequestStatusId = RequestStatusId
    FROM dbo.ServiceRequests
    WHERE Id = @ServiceRequestId AND Deleted = 0;

    IF @OldRequestStatusId IS NULL
    BEGIN
        RAISERROR(N'Service request not found.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.ServiceRequests
        SET RequestStatusId = @NewRequestStatusId,
            ModifiedDate = SYSUTCDATETIME(),
            ModifiedBy = @ModifiedBy,
            CompletedAt = CASE WHEN (SELECT StatusCode FROM ref.ServiceRequestStatuses WHERE RequestStatusId = @NewRequestStatusId) = N'Completed' THEN SYSUTCDATETIME() ELSE CompletedAt END
        WHERE Id = @ServiceRequestId;

        INSERT INTO dbo.ServiceRequestStatusHistory
            (ServiceRequestId, OldRequestStatusId, NewRequestStatusId, ChangedByUserId, ChangeReason, CreatedBy)
        VALUES
            (@ServiceRequestId, @OldRequestStatusId, @NewRequestStatusId, @ChangedByUserId, @ChangeReason, @ModifiedBy);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetUserProfile
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.Id, u.Email, u.Phone,
        r.RoleCode, us.StatusCode AS UserStatus,
        vs.StatusCode AS VerificationStatus,
        ss.StatusCode AS SubscriptionStatus,
        p.FirstName, p.LastName, p.Bio, p.ProfilePictureUrl, p.PreferredLanguage, p.NationalId,
        u.SubscriptionExpiresAt, u.LastLoginAt, u.CreatedDate
    FROM dbo.Users u
    INNER JOIN ref.Roles r ON u.RoleId = r.RoleId
    INNER JOIN ref.UserStatuses us ON u.StatusId = us.StatusId
    INNER JOIN ref.VerificationStatuses vs ON u.VerificationStatusId = vs.VerificationStatusId
    INNER JOIN ref.SubscriptionStatuses ss ON u.SubscriptionStatusId = ss.SubscriptionStatusId
    LEFT JOIN dbo.UserProfiles p ON u.Id = p.UserId AND p.Deleted = 0
    WHERE u.Id = @UserId AND u.Deleted = 0;

    SELECT Id, Label, Street, City, District, PostalCode, Country, Latitude, Longitude, IsDefault
    FROM dbo.Addresses
    WHERE UserId = @UserId AND Deleted = 0
    ORDER BY IsDefault DESC, CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetUnreadNotifications
    @UserId UNIQUEIDENTIFIER,
    @Page       INT = 1,
    @PageSize   INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT Id, TitleAr, TitleEn, MessageAr, MessageEn, NotificationType, ReferenceId, IsRead, ReadAt, CreatedDate
    FROM dbo.Notifications
    WHERE UserId = @UserId AND Deleted = 0 AND IsRead = 0
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalUnread
    FROM dbo.Notifications
    WHERE UserId = @UserId AND Deleted = 0 AND IsRead = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CleanupExpiredRefreshTokens
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RefreshTokens
    SET Deleted = 1, DeletedDate = SYSUTCDATETIME(), DeletedBy = N'SYSTEM', ModifiedDate = SYSUTCDATETIME(), ModifiedBy = N'SYSTEM'
    WHERE Deleted = 0 AND (ExpiresAt < SYSUTCDATETIME() OR RevokedAt IS NOT NULL);

    SELECT @@ROWCOUNT AS TokensCleaned;
END
GO

PRINT 'All stored procedures created successfully.';
GO
