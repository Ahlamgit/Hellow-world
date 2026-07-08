/* =============================================================================
   KHADAMATI - Enterprise Administration Dashboard
   Script: 011_AdminDashboard.sql
   ============================================================================= */

USE Khadamati;
GO

-- Advertisements
IF OBJECT_ID('dbo.Advertisements', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Advertisements (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        TitleEn NVARCHAR(200) NOT NULL,
        TitleAr NVARCHAR(200) NOT NULL,
        DescriptionEn NVARCHAR(MAX) NULL,
        ImageUrl NVARCHAR(500) NULL,
        Placement NVARCHAR(50) NOT NULL DEFAULT N'HomePage',
        TargetUserId UNIQUEIDENTIFIER NULL,
        StartDate DATETIME2(7) NOT NULL,
        EndDate DATETIME2(7) NULL,
        Impressions INT NOT NULL DEFAULT 0,
        Clicks INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
END
GO

-- Coupons
IF OBJECT_ID('dbo.Coupons', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Coupons (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Code NVARCHAR(50) NOT NULL,
        DescriptionEn NVARCHAR(500) NOT NULL,
        DescriptionAr NVARCHAR(500) NOT NULL,
        DiscountPercentage DECIMAL(5,2) NOT NULL,
        MaxDiscountAmount DECIMAL(18,2) NULL,
        MaxUses INT NOT NULL DEFAULT 0,
        UsedCount INT NOT NULL DEFAULT 0,
        ValidFrom DATETIME2(7) NOT NULL,
        ValidTo DATETIME2(7) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
    CREATE UNIQUE INDEX IX_Coupons_Code ON dbo.Coupons(Code) WHERE IsDeleted = 0;
END
GO

-- Complaints
IF OBJECT_ID('dbo.Complaints', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Complaints (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        ComplainantUserId UNIQUEIDENTIFIER NOT NULL,
        AgainstUserId UNIQUEIDENTIFIER NULL,
        BookingId UNIQUEIDENTIFIER NULL,
        Subject NVARCHAR(300) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT N'Open',
        Priority NVARCHAR(20) NOT NULL DEFAULT N'Normal',
        Resolution NVARCHAR(MAX) NULL,
        ResolvedAt DATETIME2(7) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        CONSTRAINT FK_Complaints_Complainant FOREIGN KEY (ComplainantUserId) REFERENCES dbo.Users(Id)
    );
END
GO

-- Support Tickets
IF OBJECT_ID('dbo.SupportTickets', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupportTickets (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        TicketNumber NVARCHAR(30) NOT NULL,
        Subject NVARCHAR(300) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT N'Open',
        Priority NVARCHAR(20) NOT NULL DEFAULT N'Normal',
        Category NVARCHAR(50) NOT NULL DEFAULT N'General',
        AssignedToUserId UNIQUEIDENTIFIER NULL,
        ClosedAt DATETIME2(7) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        CONSTRAINT FK_SupportTickets_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
    CREATE UNIQUE INDEX IX_SupportTickets_TicketNumber ON dbo.SupportTickets(TicketNumber) WHERE IsDeleted = 0;
END
GO

-- Regions & Cities
IF OBJECT_ID('dbo.Regions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Regions (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        NameEn NVARCHAR(100) NOT NULL,
        NameAr NVARCHAR(100) NOT NULL,
        Code NVARCHAR(20) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
END
GO

IF OBJECT_ID('dbo.Cities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cities (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        RegionId UNIQUEIDENTIFIER NOT NULL,
        NameEn NVARCHAR(100) NOT NULL,
        NameAr NVARCHAR(100) NOT NULL,
        Code NVARCHAR(20) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        CONSTRAINT FK_Cities_Region FOREIGN KEY (RegionId) REFERENCES dbo.Regions(Id)
    );
END
GO

-- System Settings
IF OBJECT_ID('dbo.SystemSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemSettings (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        SettingKey NVARCHAR(100) NOT NULL,
        SettingValue NVARCHAR(MAX) NOT NULL,
        Category NVARCHAR(50) NOT NULL DEFAULT N'General',
        Description NVARCHAR(500) NULL,
        IsEncrypted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
    CREATE UNIQUE INDEX IX_SystemSettings_Key ON dbo.SystemSettings(SettingKey) WHERE IsDeleted = 0;
END
GO

-- Permissions
IF OBJECT_ID('dbo.Permissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permissions (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Code NVARCHAR(100) NOT NULL,
        NameEn NVARCHAR(200) NOT NULL,
        NameAr NVARCHAR(200) NOT NULL,
        Module NVARCHAR(50) NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
    CREATE UNIQUE INDEX IX_Permissions_Code ON dbo.Permissions(Code) WHERE IsDeleted = 0;
END
GO

-- Role Permissions
IF OBJECT_ID('dbo.RolePermissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RolePermissions (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Role NVARCHAR(50) NOT NULL,
        PermissionId UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL,
        CONSTRAINT FK_RolePermissions_Permission FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(Id)
    );
END
GO

-- Activity Logs
IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ActivityLogs (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NULL,
        Action NVARCHAR(100) NOT NULL,
        Module NVARCHAR(50) NOT NULL,
        EntityId NVARCHAR(100) NULL,
        Details NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
    CREATE INDEX IX_ActivityLogs_Module ON dbo.ActivityLogs(Module, CreatedAt DESC);
END
GO

-- Backup Jobs
IF OBJECT_ID('dbo.BackupJobs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BackupJobs (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(200) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT N'Pending',
        SizeBytes BIGINT NOT NULL DEFAULT 0,
        FilePath NVARCHAR(500) NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        CompletedAt DATETIME2(7) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(7) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2(7) NULL
    );
END
GO

PRINT 'Admin Dashboard schema deployed successfully.';
GO
