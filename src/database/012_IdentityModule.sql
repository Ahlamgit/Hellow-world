/* =============================================================================
   KHADAMATI - Enterprise Identity Module V2
   Script: 012_IdentityModule.sql
   Permission-based RBAC, sessions, login history, security logs
   ============================================================================= */

USE Khadamati;
GO

-- Roles
IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(50) NOT NULL,
        NameAr NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsSystemRole BIT NOT NULL DEFAULT 1,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        DeletedBy NVARCHAR(100) NULL
    );
    CREATE UNIQUE INDEX IX_Roles_Name ON dbo.Roles(Name) WHERE IsDeleted = 0;
END
GO

-- UserRoles (many-to-many)
IF OBJECT_ID('dbo.UserRoles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        IsPrimary BIT NOT NULL DEFAULT 0,
        AssignedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        AssignedBy NVARCHAR(100) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
    CREATE UNIQUE INDEX IX_UserRoles_UserRole ON dbo.UserRoles(UserId, RoleId) WHERE IsDeleted = 0;
END
GO

-- UserPermissions (direct grants/denies)
IF OBJECT_ID('dbo.UserPermissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserPermissions (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        PermissionId UNIQUEIDENTIFIER NOT NULL,
        IsGranted BIT NOT NULL DEFAULT 1,
        GrantedBy NVARCHAR(100) NULL,
        GrantedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        ExpiresAt DATETIME2(7) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        CONSTRAINT FK_UserPermissions_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserPermissions_Permission FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(Id)
    );
    CREATE UNIQUE INDEX IX_UserPermissions_UserPerm ON dbo.UserPermissions(UserId, PermissionId) WHERE IsDeleted = 0;
END
GO

-- Extend Users
IF COL_LENGTH('dbo.Users', 'PrimaryRoleId') IS NULL
    ALTER TABLE dbo.Users ADD PrimaryRoleId UNIQUEIDENTIFIER NULL;
GO
IF COL_LENGTH('dbo.Users', 'PasswordChangedAt') IS NULL
    ALTER TABLE dbo.Users ADD PasswordChangedAt DATETIME2(7) NULL;
GO

-- Extend UserProfiles
IF COL_LENGTH('dbo.UserProfiles', 'Timezone') IS NULL ALTER TABLE dbo.UserProfiles ADD Timezone NVARCHAR(50) NOT NULL DEFAULT N'Asia/Riyadh';
IF COL_LENGTH('dbo.UserProfiles', 'Gender') IS NULL ALTER TABLE dbo.UserProfiles ADD Gender NVARCHAR(20) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'BirthDate') IS NULL ALTER TABLE dbo.UserProfiles ADD BirthDate DATE NULL;
IF COL_LENGTH('dbo.UserProfiles', 'Nationality') IS NULL ALTER TABLE dbo.UserProfiles ADD Nationality NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'AddressLine') IS NULL ALTER TABLE dbo.UserProfiles ADD AddressLine NVARCHAR(500) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'Country') IS NULL ALTER TABLE dbo.UserProfiles ADD Country NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'City') IS NULL ALTER TABLE dbo.UserProfiles ADD City NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'Region') IS NULL ALTER TABLE dbo.UserProfiles ADD Region NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'Latitude') IS NULL ALTER TABLE dbo.UserProfiles ADD Latitude DECIMAL(10,7) NULL;
IF COL_LENGTH('dbo.UserProfiles', 'Longitude') IS NULL ALTER TABLE dbo.UserProfiles ADD Longitude DECIMAL(10,7) NULL;
GO

-- Extend RefreshTokens (sessions / devices)
IF COL_LENGTH('dbo.RefreshTokens', 'DeviceId') IS NULL ALTER TABLE dbo.RefreshTokens ADD DeviceId NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.RefreshTokens', 'DeviceName') IS NULL ALTER TABLE dbo.RefreshTokens ADD DeviceName NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.RefreshTokens', 'Platform') IS NULL ALTER TABLE dbo.RefreshTokens ADD Platform NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.RefreshTokens', 'Browser') IS NULL ALTER TABLE dbo.RefreshTokens ADD Browser NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.RefreshTokens', 'UserAgent') IS NULL ALTER TABLE dbo.RefreshTokens ADD UserAgent NVARCHAR(500) NULL;
IF COL_LENGTH('dbo.RefreshTokens', 'LastActivityAt') IS NULL ALTER TABLE dbo.RefreshTokens ADD LastActivityAt DATETIME2(7) NULL;
IF COL_LENGTH('dbo.RefreshTokens', 'LogoutAt') IS NULL ALTER TABLE dbo.RefreshTokens ADD LogoutAt DATETIME2(7) NULL;
GO

-- Extend Permissions
IF COL_LENGTH('dbo.Permissions', 'RequiresEmailVerification') IS NULL
    ALTER TABLE dbo.Permissions ADD RequiresEmailVerification BIT NOT NULL DEFAULT 0;
GO

-- Migrate RolePermissions from string Role to RoleId
IF COL_LENGTH('dbo.RolePermissions', 'RoleId') IS NULL
BEGIN
    ALTER TABLE dbo.RolePermissions ADD RoleId UNIQUEIDENTIFIER NULL;
END
GO

-- Login History
IF OBJECT_ID('dbo.LoginHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoginHistory (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NULL,
        Email NVARCHAR(256) NULL,
        IsSuccessful BIT NOT NULL,
        FailureReason NVARCHAR(200) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        DeviceId NVARCHAR(100) NULL,
        DeviceName NVARCHAR(200) NULL,
        Platform NVARCHAR(50) NULL,
        Browser NVARCHAR(100) NULL,
        SessionId UNIQUEIDENTIFIER NULL,
        LoginAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_LoginHistory_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE SET NULL
    );
    CREATE INDEX IX_LoginHistory_User ON dbo.LoginHistory(UserId, LoginAt DESC);
END
GO

-- Security Logs
IF OBJECT_ID('dbo.SecurityLogs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityLogs (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NULL,
        EventType NVARCHAR(100) NOT NULL,
        Severity NVARCHAR(20) NOT NULL DEFAULT N'Info',
        Description NVARCHAR(1000) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        Metadata NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_SecurityLogs_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE SET NULL
    );
    CREATE INDEX IX_SecurityLogs_Event ON dbo.SecurityLogs(EventType, CreatedAt DESC);
END
GO

-- Password History
IF OBJECT_ID('dbo.PasswordHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordHistory (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        PasswordHash NVARCHAR(512) NOT NULL,
        ChangedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        ChangedByIp NVARCHAR(50) NULL,
        ChangeReason NVARCHAR(50) NOT NULL DEFAULT N'Change',
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_PasswordHistory_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
    CREATE INDEX IX_PasswordHistory_User ON dbo.PasswordHistory(UserId, ChangedAt DESC);
END
GO

PRINT 'Identity Module V2 schema deployed.';
GO
