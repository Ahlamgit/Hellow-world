/*
================================================================================
 KHADAMATI - 006 Create Triggers (Audit + ModifiedDate)
================================================================================
*/
USE KhadamatiDb;
GO

-- Helper: auto-set ModifiedDate on UPDATE for all audited tables
CREATE OR ALTER TRIGGER dbo.TR_Users_SetModifiedDate ON dbo.Users
AFTER UPDATE AS BEGIN SET NOCOUNT ON;
    IF NOT UPDATE(ModifiedDate) UPDATE u SET ModifiedDate = SYSUTCDATETIME() FROM dbo.Users u INNER JOIN inserted i ON u.Id = i.Id;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_UserProfiles_SetModifiedDate ON dbo.UserProfiles
AFTER UPDATE AS BEGIN SET NOCOUNT ON;
    IF NOT UPDATE(ModifiedDate) UPDATE t SET ModifiedDate = SYSUTCDATETIME() FROM dbo.UserProfiles t INNER JOIN inserted i ON t.Id = i.Id;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_ServiceRequests_SetModifiedDate ON dbo.ServiceRequests
AFTER UPDATE AS BEGIN SET NOCOUNT ON;
    IF NOT UPDATE(ModifiedDate) UPDATE t SET ModifiedDate = SYSUTCDATETIME() FROM dbo.ServiceRequests t INNER JOIN inserted i ON t.Id = i.Id;
END
GO

-- Audit trigger template for Users
CREATE OR ALTER TRIGGER dbo.TR_Users_Audit ON dbo.Users
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO audit.AuditLogs (SchemaName, TableName, Action, EntityId, OldValues, NewValues, CreatedDate)
        SELECT N'dbo', N'Users', N'UPDATE', CAST(i.Id AS NVARCHAR(50)),
            (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            SYSUTCDATETIME()
        FROM inserted i INNER JOIN deleted d ON i.Id = d.Id;
    END
    ELSE IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        INSERT INTO audit.AuditLogs (SchemaName, TableName, Action, EntityId, NewValues, CreatedDate)
        SELECT N'dbo', N'Users', N'INSERT', CAST(i.Id AS NVARCHAR(50)),
            (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSUTCDATETIME()
        FROM inserted i;
    END
    ELSE
    BEGIN
        INSERT INTO audit.AuditLogs (SchemaName, TableName, Action, EntityId, OldValues, CreatedDate)
        SELECT N'dbo', N'Users', N'DELETE', CAST(d.Id AS NVARCHAR(50)),
            (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSUTCDATETIME()
        FROM deleted d;
    END
END
GO

-- Auto-log status changes on ServiceRequests
CREATE OR ALTER TRIGGER dbo.TR_ServiceRequests_StatusHistory ON dbo.ServiceRequests
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(RequestStatusId)
    BEGIN
        INSERT INTO dbo.ServiceRequestStatusHistory
            (ServiceRequestId, OldRequestStatusId, NewRequestStatusId, ChangedByUserId, ChangeReason, CreatedBy)
        SELECT i.Id, d.RequestStatusId, i.RequestStatusId, NULL, N'Trigger: automatic status change', i.ModifiedBy
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.RequestStatusId <> d.RequestStatusId;
    END
END
GO

-- Soft delete cascade audit
CREATE OR ALTER TRIGGER dbo.TR_Users_SoftDeleteAudit ON dbo.Users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Deleted)
    BEGIN
        INSERT INTO audit.AuditLogs (SchemaName, TableName, Action, EntityId, NewValues, CreatedBy, CreatedDate)
        SELECT N'dbo', N'Users', N'SOFT_DELETE', CAST(i.Id AS NVARCHAR(50)),
            N'{"Deleted":true,"DeletedDate":"' + CONVERT(NVARCHAR(30), i.DeletedDate, 126) + N'"}',
            i.DeletedBy, SYSUTCDATETIME()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.Deleted = 1 AND d.Deleted = 0;
    END
END
GO

PRINT 'All triggers created successfully.';
GO
