/*
================================================================================
 KHADAMATI - 008 Authentication Module Tables
================================================================================
*/
USE KhadamatiDb;
GO

-- Extend Users table
IF COL_LENGTH('dbo.Users', 'EmailVerifiedAt') IS NULL
    ALTER TABLE dbo.Users ADD EmailVerifiedAt DATETIME2(7) NULL;
GO
IF COL_LENGTH('dbo.Users', 'PhoneVerifiedAt') IS NULL
    ALTER TABLE dbo.Users ADD PhoneVerifiedAt DATETIME2(7) NULL;
GO

-- Extend RefreshTokens
IF COL_LENGTH('dbo.RefreshTokens', 'RememberMe') IS NULL
    ALTER TABLE dbo.RefreshTokens ADD RememberMe BIT NOT NULL CONSTRAINT DF_RefreshTokens_RememberMe DEFAULT (0);
GO

IF OBJECT_ID(N'dbo.EmailVerificationTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmailVerificationTokens
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_EmailVerificationTokens_Id DEFAULT (NEWSEQUENTIALID()),
        UserId          UNIQUEIDENTIFIER NOT NULL,
        TokenHash       NVARCHAR(512)    NOT NULL,
        ExpiresAt       DATETIME2(7)     NOT NULL,
        VerifiedAt      DATETIME2(7)     NULL,
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_EmailVerificationTokens_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_EmailVerificationTokens_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_EmailVerificationTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_EmailVerificationTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_EmailVerificationTokens_UserId ON dbo.EmailVerificationTokens (UserId) WHERE Deleted = 0;
    CREATE NONCLUSTERED INDEX IX_EmailVerificationTokens_ExpiresAt ON dbo.EmailVerificationTokens (ExpiresAt) WHERE Deleted = 0;
END
GO

IF OBJECT_ID(N'dbo.PasswordResetTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordResetTokens
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_PasswordResetTokens_Id DEFAULT (NEWSEQUENTIALID()),
        UserId          UNIQUEIDENTIFIER NOT NULL,
        TokenHash       NVARCHAR(512)    NOT NULL,
        ExpiresAt       DATETIME2(7)     NOT NULL,
        UsedAt          DATETIME2(7)     NULL,
        RequestedFromIp NVARCHAR(50)     NULL,
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_PasswordResetTokens_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_PasswordResetTokens_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_PasswordResetTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_UserId ON dbo.PasswordResetTokens (UserId) WHERE Deleted = 0;
END
GO

IF OBJECT_ID(N'dbo.PhoneOtpTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhoneOtpTokens
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_PhoneOtpTokens_Id DEFAULT (NEWSEQUENTIALID()),
        UserId          UNIQUEIDENTIFIER NOT NULL,
        Phone           NVARCHAR(20)     NOT NULL,
        OtpHash         NVARCHAR(512)    NOT NULL,
        ExpiresAt       DATETIME2(7)     NOT NULL,
        VerifiedAt      DATETIME2(7)     NULL,
        AttemptCount    INT              NOT NULL CONSTRAINT DF_PhoneOtpTokens_AttemptCount DEFAULT (0),
        RequestedFromIp NVARCHAR(50)     NULL,
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_PhoneOtpTokens_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_PhoneOtpTokens_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_PhoneOtpTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_PhoneOtpTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_PhoneOtpTokens_UserId_Phone ON dbo.PhoneOtpTokens (UserId, Phone) WHERE Deleted = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InvalidateUserTokens
    @UserId UNIQUEIDENTIFIER,
    @IpAddress NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RefreshTokens
    SET RevokedAt = SYSUTCDATETIME(), RevokedByIp = @IpAddress, ModifiedDate = SYSUTCDATETIME(), ModifiedBy = N'SYSTEM'
    WHERE UserId = @UserId AND RevokedAt IS NULL AND Deleted = 0;
END
GO

PRINT 'Authentication module database objects created.';
GO
