/*
================================================================================
 KHADAMATI - 002 Create Tables
 All business tables include standard audit columns:
   CreatedDate, ModifiedDate, CreatedBy, ModifiedBy, Deleted, DeletedDate, DeletedBy
================================================================================
*/
USE KhadamatiDb;
GO

/* =============================================================================
   REFERENCE / LOOKUP TABLES (INT IDENTITY Primary Keys)
   ============================================================================= */

IF OBJECT_ID(N'ref.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE ref.Roles
    (
        RoleId          INT             NOT NULL IDENTITY(1,1),
        RoleCode        NVARCHAR(50)    NOT NULL,
        RoleNameEn      NVARCHAR(100)   NOT NULL,
        RoleNameAr      NVARCHAR(100)   NOT NULL,
        Description     NVARCHAR(500)   NULL,
        IsActive        BIT             NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT (1),
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_Roles_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_Roles_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId),
        CONSTRAINT UQ_Roles_RoleCode UNIQUE (RoleCode)
    );
END
GO

IF OBJECT_ID(N'ref.UserStatuses', N'U') IS NULL
BEGIN
    CREATE TABLE ref.UserStatuses
    (
        StatusId        INT             NOT NULL IDENTITY(1,1),
        StatusCode      NVARCHAR(50)    NOT NULL,
        StatusNameEn    NVARCHAR(100)   NOT NULL,
        StatusNameAr    NVARCHAR(100)   NOT NULL,
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_UserStatuses_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_UserStatuses_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_UserStatuses PRIMARY KEY CLUSTERED (StatusId),
        CONSTRAINT UQ_UserStatuses_StatusCode UNIQUE (StatusCode)
    );
END
GO

IF OBJECT_ID(N'ref.VerificationStatuses', N'U') IS NULL
BEGIN
    CREATE TABLE ref.VerificationStatuses
    (
        VerificationStatusId INT          NOT NULL IDENTITY(1,1),
        StatusCode      NVARCHAR(50)    NOT NULL,
        StatusNameEn    NVARCHAR(100)   NOT NULL,
        StatusNameAr    NVARCHAR(100)   NOT NULL,
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_VerificationStatuses_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_VerificationStatuses_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_VerificationStatuses PRIMARY KEY CLUSTERED (VerificationStatusId),
        CONSTRAINT UQ_VerificationStatuses_StatusCode UNIQUE (StatusCode)
    );
END
GO

IF OBJECT_ID(N'ref.SubscriptionStatuses', N'U') IS NULL
BEGIN
    CREATE TABLE ref.SubscriptionStatuses
    (
        SubscriptionStatusId INT         NOT NULL IDENTITY(1,1),
        StatusCode      NVARCHAR(50)    NOT NULL,
        StatusNameEn    NVARCHAR(100)   NOT NULL,
        StatusNameAr    NVARCHAR(100)   NOT NULL,
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_SubscriptionStatuses_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_SubscriptionStatuses_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_SubscriptionStatuses PRIMARY KEY CLUSTERED (SubscriptionStatusId),
        CONSTRAINT UQ_SubscriptionStatuses_StatusCode UNIQUE (StatusCode)
    );
END
GO

IF OBJECT_ID(N'ref.ServiceRequestStatuses', N'U') IS NULL
BEGIN
    CREATE TABLE ref.ServiceRequestStatuses
    (
        RequestStatusId INT             NOT NULL IDENTITY(1,1),
        StatusCode      NVARCHAR(50)    NOT NULL,
        StatusNameEn    NVARCHAR(100)   NOT NULL,
        StatusNameAr    NVARCHAR(100)   NOT NULL,
        DisplayOrder    INT             NOT NULL CONSTRAINT DF_ServiceRequestStatuses_DisplayOrder DEFAULT (0),
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_ServiceRequestStatuses_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_ServiceRequestStatuses_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_ServiceRequestStatuses PRIMARY KEY CLUSTERED (RequestStatusId),
        CONSTRAINT UQ_ServiceRequestStatuses_StatusCode UNIQUE (StatusCode)
    );
END
GO

IF OBJECT_ID(N'ref.PaymentStatuses', N'U') IS NULL
BEGIN
    CREATE TABLE ref.PaymentStatuses
    (
        PaymentStatusId INT             NOT NULL IDENTITY(1,1),
        StatusCode      NVARCHAR(50)    NOT NULL,
        StatusNameEn    NVARCHAR(100)   NOT NULL,
        StatusNameAr    NVARCHAR(100)   NOT NULL,
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_PaymentStatuses_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_PaymentStatuses_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_PaymentStatuses PRIMARY KEY CLUSTERED (PaymentStatusId),
        CONSTRAINT UQ_PaymentStatuses_StatusCode UNIQUE (StatusCode)
    );
END
GO

IF OBJECT_ID(N'dbo.SubscriptionPlans', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SubscriptionPlans
    (
        PlanId          INT             NOT NULL IDENTITY(1,1),
        PlanCode        NVARCHAR(50)    NOT NULL,
        PlanNameEn      NVARCHAR(150)   NOT NULL,
        PlanNameAr      NVARCHAR(150)   NOT NULL,
        DescriptionEn   NVARCHAR(1000)  NULL,
        DescriptionAr   NVARCHAR(1000)  NULL,
        Price           DECIMAL(18,2)   NOT NULL,
        DurationDays    INT             NOT NULL,
        TargetRoleId    INT             NOT NULL,
        IsActive        BIT             NOT NULL CONSTRAINT DF_SubscriptionPlans_IsActive DEFAULT (1),
        CreatedDate     DATETIME2(7)    NOT NULL CONSTRAINT DF_SubscriptionPlans_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)    NULL,
        CreatedBy       NVARCHAR(128)   NULL,
        ModifiedBy      NVARCHAR(128)   NULL,
        Deleted         BIT             NOT NULL CONSTRAINT DF_SubscriptionPlans_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)    NULL,
        DeletedBy       NVARCHAR(128)   NULL,
        CONSTRAINT PK_SubscriptionPlans PRIMARY KEY CLUSTERED (PlanId),
        CONSTRAINT UQ_SubscriptionPlans_PlanCode UNIQUE (PlanCode),
        CONSTRAINT FK_SubscriptionPlans_Roles FOREIGN KEY (TargetRoleId) REFERENCES ref.Roles(RoleId)
    );
END
GO

/* =============================================================================
   CORE USER TABLES (UNIQUEIDENTIFIER Primary Keys)
   ============================================================================= */

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id                      UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Users_Id DEFAULT (NEWSEQUENTIALID()),
        Email                   NVARCHAR(256)    NOT NULL,
        Phone                   NVARCHAR(20)     NOT NULL,
        PasswordHash            NVARCHAR(512)    NOT NULL,
        RoleId                  INT              NOT NULL,
        StatusId                INT              NOT NULL,
        VerificationStatusId    INT              NOT NULL,
        SubscriptionStatusId    INT              NOT NULL,
        SubscriptionExpiresAt   DATETIME2(7)     NULL,
        LastLoginAt             DATETIME2(7)     NULL,
        FailedLoginAttempts     INT              NOT NULL CONSTRAINT DF_Users_FailedLoginAttempts DEFAULT (0),
        LockoutEnd              DATETIME2(7)     NULL,
        CreatedDate             DATETIME2(7)     NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate            DATETIME2(7)     NULL,
        CreatedBy               NVARCHAR(128)    NULL,
        ModifiedBy              NVARCHAR(128)    NULL,
        Deleted                 BIT              NOT NULL CONSTRAINT DF_Users_Deleted DEFAULT (0),
        DeletedDate             DATETIME2(7)     NULL,
        DeletedBy               NVARCHAR(128)    NULL,
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT UQ_Users_Phone UNIQUE (Phone),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES ref.Roles(RoleId),
        CONSTRAINT FK_Users_UserStatuses FOREIGN KEY (StatusId) REFERENCES ref.UserStatuses(StatusId),
        CONSTRAINT FK_Users_VerificationStatuses FOREIGN KEY (VerificationStatusId) REFERENCES ref.VerificationStatuses(VerificationStatusId),
        CONSTRAINT FK_Users_SubscriptionStatuses FOREIGN KEY (SubscriptionStatusId) REFERENCES ref.SubscriptionStatuses(SubscriptionStatusId)
    );
END
GO

IF OBJECT_ID(N'dbo.UserProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserProfiles
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_UserProfiles_Id DEFAULT (NEWSEQUENTIALID()),
        UserId              UNIQUEIDENTIFIER NOT NULL,
        FirstName           NVARCHAR(100)    NOT NULL,
        LastName            NVARCHAR(100)    NOT NULL,
        Bio                 NVARCHAR(2000)   NULL,
        ProfilePictureUrl   NVARCHAR(1000)   NULL,
        PreferredLanguage   NVARCHAR(5)      NOT NULL CONSTRAINT DF_UserProfiles_PreferredLanguage DEFAULT (N'ar'),
        NationalId          NVARCHAR(50)     NULL,
        DateOfBirth         DATE             NULL,
        Gender              NVARCHAR(20)     NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_UserProfiles_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_UserProfiles_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_UserProfiles PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_UserProfiles_UserId UNIQUE (UserId),
        CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Addresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Addresses
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Addresses_Id DEFAULT (NEWSEQUENTIALID()),
        UserId          UNIQUEIDENTIFIER NOT NULL,
        Label           NVARCHAR(100)    NOT NULL,
        Street          NVARCHAR(300)    NOT NULL,
        City            NVARCHAR(100)    NOT NULL,
        District        NVARCHAR(100)    NULL,
        PostalCode      NVARCHAR(20)     NULL,
        Country         NVARCHAR(3)      NOT NULL CONSTRAINT DF_Addresses_Country DEFAULT (N'SA'),
        Latitude        DECIMAL(10,7)    NULL,
        Longitude       DECIMAL(10,7)    NULL,
        IsDefault       BIT              NOT NULL CONSTRAINT DF_Addresses_IsDefault DEFAULT (0),
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_Addresses_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_Addresses_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_Addresses PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Addresses_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_RefreshTokens_Id DEFAULT (NEWSEQUENTIALID()),
        UserId              UNIQUEIDENTIFIER NOT NULL,
        Token               NVARCHAR(512)    NOT NULL,
        JwtId               NVARCHAR(128)    NOT NULL,
        ExpiresAt           DATETIME2(7)     NOT NULL,
        RevokedAt           DATETIME2(7)     NULL,
        ReplacedByToken     NVARCHAR(512)    NULL,
        CreatedByIp         NVARCHAR(50)     NULL,
        RevokedByIp         NVARCHAR(50)     NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_RefreshTokens_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_RefreshTokens_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_RefreshTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_RefreshTokens_Token UNIQUE (Token),
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.CraftsmanProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CraftsmanProfiles
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_CraftsmanProfiles_Id DEFAULT (NEWSEQUENTIALID()),
        UserId              UNIQUEIDENTIFIER NOT NULL,
        Specialization      NVARCHAR(200)    NULL,
        YearsOfExperience   INT              NOT NULL CONSTRAINT DF_CraftsmanProfiles_YearsOfExperience DEFAULT (0),
        Rating              DECIMAL(3,2)     NOT NULL CONSTRAINT DF_CraftsmanProfiles_Rating DEFAULT (0),
        TotalReviews        INT              NOT NULL CONSTRAINT DF_CraftsmanProfiles_TotalReviews DEFAULT (0),
        CompletedJobs       INT              NOT NULL CONSTRAINT DF_CraftsmanProfiles_CompletedJobs DEFAULT (0),
        IsAvailable         BIT              NOT NULL CONSTRAINT DF_CraftsmanProfiles_IsAvailable DEFAULT (1),
        ServiceRadiusKm     DECIMAL(10,2)    NULL,
        LicenseNumber       NVARCHAR(100)    NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_CraftsmanProfiles_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_CraftsmanProfiles_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_CraftsmanProfiles PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_CraftsmanProfiles_UserId UNIQUE (UserId),
        CONSTRAINT FK_CraftsmanProfiles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.StoreProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StoreProfiles
    (
        Id                          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_StoreProfiles_Id DEFAULT (NEWSEQUENTIALID()),
        UserId                      UNIQUEIDENTIFIER NOT NULL,
        StoreName                   NVARCHAR(200)    NOT NULL,
        CommercialRegistration      NVARCHAR(100)    NULL,
        Description                 NVARCHAR(2000)   NULL,
        Rating                      DECIMAL(3,2)     NOT NULL CONSTRAINT DF_StoreProfiles_Rating DEFAULT (0),
        TotalReviews                INT              NOT NULL CONSTRAINT DF_StoreProfiles_TotalReviews DEFAULT (0),
        IsOpen                      BIT              NOT NULL CONSTRAINT DF_StoreProfiles_IsOpen DEFAULT (1),
        OpeningTime                 TIME(0)          NULL,
        ClosingTime                 TIME(0)          NULL,
        CreatedDate                 DATETIME2(7)     NOT NULL CONSTRAINT DF_StoreProfiles_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate                DATETIME2(7)     NULL,
        CreatedBy                   NVARCHAR(128)    NULL,
        ModifiedBy                  NVARCHAR(128)    NULL,
        Deleted                     BIT              NOT NULL CONSTRAINT DF_StoreProfiles_Deleted DEFAULT (0),
        DeletedDate                 DATETIME2(7)     NULL,
        DeletedBy                   NVARCHAR(128)    NULL,
        CONSTRAINT PK_StoreProfiles PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_StoreProfiles_UserId UNIQUE (UserId),
        CONSTRAINT FK_StoreProfiles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.VerificationDocuments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.VerificationDocuments
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_VerificationDocuments_Id DEFAULT (NEWSEQUENTIALID()),
        UserId              UNIQUEIDENTIFIER NOT NULL,
        DocumentType        NVARCHAR(100)    NOT NULL,
        DocumentUrl         NVARCHAR(1000)   NOT NULL,
        VerificationStatusId INT             NOT NULL,
        ReviewedByUserId    UNIQUEIDENTIFIER NULL,
        ReviewedAt          DATETIME2(7)     NULL,
        RejectionReason     NVARCHAR(500)    NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_VerificationDocuments_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_VerificationDocuments_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_VerificationDocuments PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_VerificationDocuments_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_VerificationDocuments_VerificationStatuses FOREIGN KEY (VerificationStatusId) REFERENCES ref.VerificationStatuses(VerificationStatusId),
        CONSTRAINT FK_VerificationDocuments_ReviewedBy FOREIGN KEY (ReviewedByUserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.UserSubscriptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserSubscriptions
    (
        Id                      UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_UserSubscriptions_Id DEFAULT (NEWSEQUENTIALID()),
        UserId                  UNIQUEIDENTIFIER NOT NULL,
        PlanId                  INT              NOT NULL,
        SubscriptionStatusId    INT              NOT NULL,
        StartDate               DATETIME2(7)     NOT NULL,
        EndDate                 DATETIME2(7)     NOT NULL,
        AutoRenew               BIT              NOT NULL CONSTRAINT DF_UserSubscriptions_AutoRenew DEFAULT (0),
        CreatedDate             DATETIME2(7)     NOT NULL CONSTRAINT DF_UserSubscriptions_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate            DATETIME2(7)     NULL,
        CreatedBy               NVARCHAR(128)    NULL,
        ModifiedBy              NVARCHAR(128)    NULL,
        Deleted                 BIT              NOT NULL CONSTRAINT DF_UserSubscriptions_Deleted DEFAULT (0),
        DeletedDate             DATETIME2(7)     NULL,
        DeletedBy               NVARCHAR(128)    NULL,
        CONSTRAINT PK_UserSubscriptions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_UserSubscriptions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserSubscriptions_Plans FOREIGN KEY (PlanId) REFERENCES dbo.SubscriptionPlans(PlanId),
        CONSTRAINT FK_UserSubscriptions_SubscriptionStatuses FOREIGN KEY (SubscriptionStatusId) REFERENCES ref.SubscriptionStatuses(SubscriptionStatusId)
    );
END
GO

/* =============================================================================
   SERVICE CATALOG TABLES
   ============================================================================= */

IF OBJECT_ID(N'dbo.ServiceCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceCategories
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ServiceCategories_Id DEFAULT (NEWSEQUENTIALID()),
        NameAr              NVARCHAR(200)    NOT NULL,
        NameEn              NVARCHAR(200)    NOT NULL,
        DescriptionAr       NVARCHAR(1000)   NULL,
        DescriptionEn       NVARCHAR(1000)   NULL,
        IconUrl             NVARCHAR(1000)   NULL,
        DisplayOrder        INT              NOT NULL CONSTRAINT DF_ServiceCategories_DisplayOrder DEFAULT (0),
        IsActive            BIT              NOT NULL CONSTRAINT DF_ServiceCategories_IsActive DEFAULT (1),
        ParentCategoryId    UNIQUEIDENTIFIER NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_ServiceCategories_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_ServiceCategories_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_ServiceCategories PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_ServiceCategories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES dbo.ServiceCategories(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Services', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Services
    (
        Id                          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Services_Id DEFAULT (NEWSEQUENTIALID()),
        CategoryId                  UNIQUEIDENTIFIER NOT NULL,
        NameAr                      NVARCHAR(200)    NOT NULL,
        NameEn                      NVARCHAR(200)    NOT NULL,
        DescriptionAr               NVARCHAR(2000)   NULL,
        DescriptionEn               NVARCHAR(2000)   NULL,
        BasePrice                   DECIMAL(18,2)    NOT NULL,
        ImageUrl                    NVARCHAR(1000)   NULL,
        IsActive                    BIT              NOT NULL CONSTRAINT DF_Services_IsActive DEFAULT (1),
        EstimatedDurationMinutes    INT              NOT NULL CONSTRAINT DF_Services_EstimatedDurationMinutes DEFAULT (60),
        CreatedDate                 DATETIME2(7)     NOT NULL CONSTRAINT DF_Services_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate                DATETIME2(7)     NULL,
        CreatedBy                   NVARCHAR(128)    NULL,
        ModifiedBy                  NVARCHAR(128)    NULL,
        Deleted                     BIT              NOT NULL CONSTRAINT DF_Services_Deleted DEFAULT (0),
        DeletedDate                 DATETIME2(7)     NULL,
        DeletedBy                   NVARCHAR(128)    NULL,
        CONSTRAINT PK_Services PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Services_ServiceCategories FOREIGN KEY (CategoryId) REFERENCES dbo.ServiceCategories(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.CraftsmanServices', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CraftsmanServices
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_CraftsmanServices_Id DEFAULT (NEWSEQUENTIALID()),
        CraftsmanProfileId  UNIQUEIDENTIFIER NOT NULL,
        ServiceId           UNIQUEIDENTIFIER NOT NULL,
        CustomPrice         DECIMAL(18,2)    NOT NULL,
        IsAvailable         BIT              NOT NULL CONSTRAINT DF_CraftsmanServices_IsAvailable DEFAULT (1),
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_CraftsmanServices_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_CraftsmanServices_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_CraftsmanServices PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_CraftsmanServices_Craftsman_Service UNIQUE (CraftsmanProfileId, ServiceId),
        CONSTRAINT FK_CraftsmanServices_CraftsmanProfiles FOREIGN KEY (CraftsmanProfileId) REFERENCES dbo.CraftsmanProfiles(Id),
        CONSTRAINT FK_CraftsmanServices_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.StoreProducts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StoreProducts
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_StoreProducts_Id DEFAULT (NEWSEQUENTIALID()),
        StoreProfileId      UNIQUEIDENTIFIER NOT NULL,
        NameAr              NVARCHAR(200)    NOT NULL,
        NameEn              NVARCHAR(200)    NOT NULL,
        DescriptionAr       NVARCHAR(2000)   NULL,
        DescriptionEn       NVARCHAR(2000)   NULL,
        Price               DECIMAL(18,2)    NOT NULL,
        StockQuantity       INT              NOT NULL CONSTRAINT DF_StoreProducts_StockQuantity DEFAULT (0),
        Sku                 NVARCHAR(100)    NULL,
        ImageUrl            NVARCHAR(1000)   NULL,
        IsActive            BIT              NOT NULL CONSTRAINT DF_StoreProducts_IsActive DEFAULT (1),
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_StoreProducts_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_StoreProducts_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_StoreProducts PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_StoreProducts_StoreProfiles FOREIGN KEY (StoreProfileId) REFERENCES dbo.StoreProfiles(Id)
    );
END
GO

/* =============================================================================
   SERVICE REQUEST / ORDER TABLES
   ============================================================================= */

IF OBJECT_ID(N'dbo.ServiceRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceRequests
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ServiceRequests_Id DEFAULT (NEWSEQUENTIALID()),
        CustomerId          UNIQUEIDENTIFIER NOT NULL,
        ServiceId           UNIQUEIDENTIFIER NOT NULL,
        CraftsmanId         UNIQUEIDENTIFIER NULL,
        AddressId           UNIQUEIDENTIFIER NULL,
        RequestStatusId     INT              NOT NULL,
        Description         NVARCHAR(2000)   NULL,
        ScheduledAt         DATETIME2(7)     NULL,
        CompletedAt         DATETIME2(7)     NULL,
        EstimatedPrice      DECIMAL(18,2)    NOT NULL,
        FinalPrice          DECIMAL(18,2)    NULL,
        Notes               NVARCHAR(2000)   NULL,
        CustomerRating      INT              NULL,
        CustomerReview      NVARCHAR(2000)   NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_ServiceRequests_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_ServiceRequests_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_ServiceRequests PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_ServiceRequests_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_ServiceRequests_Craftsmen FOREIGN KEY (CraftsmanId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_ServiceRequests_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id),
        CONSTRAINT FK_ServiceRequests_Addresses FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(Id),
        CONSTRAINT FK_ServiceRequests_RequestStatuses FOREIGN KEY (RequestStatusId) REFERENCES ref.ServiceRequestStatuses(RequestStatusId),
        CONSTRAINT CK_ServiceRequests_CustomerRating CHECK (CustomerRating IS NULL OR (CustomerRating BETWEEN 1 AND 5))
    );
END
GO

IF OBJECT_ID(N'dbo.ServiceRequestStatusHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceRequestStatusHistory
    (
        HistoryId           BIGINT           NOT NULL IDENTITY(1,1),
        ServiceRequestId    UNIQUEIDENTIFIER NOT NULL,
        OldRequestStatusId  INT              NULL,
        NewRequestStatusId  INT              NOT NULL,
        ChangedByUserId     UNIQUEIDENTIFIER NULL,
        ChangeReason        NVARCHAR(500)    NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_ServiceRequestStatusHistory_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_ServiceRequestStatusHistory_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_ServiceRequestStatusHistory PRIMARY KEY CLUSTERED (HistoryId),
        CONSTRAINT FK_ServiceRequestStatusHistory_ServiceRequests FOREIGN KEY (ServiceRequestId) REFERENCES dbo.ServiceRequests(Id),
        CONSTRAINT FK_ServiceRequestStatusHistory_OldStatus FOREIGN KEY (OldRequestStatusId) REFERENCES ref.ServiceRequestStatuses(RequestStatusId),
        CONSTRAINT FK_ServiceRequestStatusHistory_NewStatus FOREIGN KEY (NewRequestStatusId) REFERENCES ref.ServiceRequestStatuses(RequestStatusId),
        CONSTRAINT FK_ServiceRequestStatusHistory_ChangedBy FOREIGN KEY (ChangedByUserId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Reviews', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reviews
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Reviews_Id DEFAULT (NEWSEQUENTIALID()),
        ServiceRequestId    UNIQUEIDENTIFIER NOT NULL,
        ReviewerUserId      UNIQUEIDENTIFIER NOT NULL,
        ReviewedUserId      UNIQUEIDENTIFIER NOT NULL,
        Rating              INT              NOT NULL,
        Comment             NVARCHAR(2000)   NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_Reviews_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_Reviews_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_Reviews PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Reviews_ServiceRequest_Reviewer UNIQUE (ServiceRequestId, ReviewerUserId),
        CONSTRAINT FK_Reviews_ServiceRequests FOREIGN KEY (ServiceRequestId) REFERENCES dbo.ServiceRequests(Id),
        CONSTRAINT FK_Reviews_Reviewer FOREIGN KEY (ReviewerUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Reviews_Reviewed FOREIGN KEY (ReviewedUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5)
    );
END
GO

IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Payments_Id DEFAULT (NEWSEQUENTIALID()),
        ServiceRequestId    UNIQUEIDENTIFIER NOT NULL,
        PayerUserId         UNIQUEIDENTIFIER NOT NULL,
        PayeeUserId         UNIQUEIDENTIFIER NOT NULL,
        Amount              DECIMAL(18,2)    NOT NULL,
        CurrencyCode        NVARCHAR(3)      NOT NULL CONSTRAINT DF_Payments_CurrencyCode DEFAULT (N'SAR'),
        PaymentStatusId     INT              NOT NULL,
        PaymentMethod       NVARCHAR(50)     NOT NULL,
        TransactionReference NVARCHAR(200)   NULL,
        PaidAt              DATETIME2(7)     NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_Payments_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_Payments_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_Payments PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Payments_ServiceRequests FOREIGN KEY (ServiceRequestId) REFERENCES dbo.ServiceRequests(Id),
        CONSTRAINT FK_Payments_Payer FOREIGN KEY (PayerUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Payments_Payee FOREIGN KEY (PayeeUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Payments_PaymentStatuses FOREIGN KEY (PaymentStatusId) REFERENCES ref.PaymentStatuses(PaymentStatusId)
    );
END
GO

IF OBJECT_ID(N'dbo.Notifications', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notifications
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Notifications_Id DEFAULT (NEWSEQUENTIALID()),
        UserId              UNIQUEIDENTIFIER NOT NULL,
        TitleAr             NVARCHAR(200)    NOT NULL,
        TitleEn             NVARCHAR(200)    NOT NULL,
        MessageAr           NVARCHAR(2000)   NOT NULL,
        MessageEn           NVARCHAR(2000)   NOT NULL,
        NotificationType    NVARCHAR(50)     NOT NULL,
        ReferenceId         UNIQUEIDENTIFIER NULL,
        IsRead              BIT              NOT NULL CONSTRAINT DF_Notifications_IsRead DEFAULT (0),
        ReadAt              DATETIME2(7)     NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_Notifications_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_Notifications_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_Notifications PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

/* =============================================================================
   AUDIT TABLES (BIGINT IDENTITY Primary Keys)
   ============================================================================= */

IF OBJECT_ID(N'audit.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE audit.AuditLogs
    (
        AuditLogId      BIGINT           NOT NULL IDENTITY(1,1),
        TableName       NVARCHAR(128)    NOT NULL,
        SchemaName      NVARCHAR(128)    NOT NULL CONSTRAINT DF_AuditLogs_SchemaName DEFAULT (N'dbo'),
        Action          NVARCHAR(20)     NOT NULL, -- INSERT, UPDATE, DELETE, SOFT_DELETE
        EntityId        NVARCHAR(50)     NULL,
        OldValues       NVARCHAR(MAX)    NULL,
        NewValues       NVARCHAR(MAX)    NULL,
        ChangedColumns  NVARCHAR(MAX)    NULL,
        UserId          NVARCHAR(50)     NULL,
        UserEmail       NVARCHAR(256)    NULL,
        IpAddress       NVARCHAR(50)     NULL,
        ApplicationName NVARCHAR(100)    NULL CONSTRAINT DF_AuditLogs_ApplicationName DEFAULT (N'KHADAMATI'),
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_AuditLogs_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_AuditLogs_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_AuditLogs PRIMARY KEY CLUSTERED (AuditLogId),
        CONSTRAINT CK_AuditLogs_Action CHECK (Action IN (N'INSERT', N'UPDATE', N'DELETE', N'SOFT_DELETE'))
    );
END
GO

-- Legacy compatibility view for EF/API expecting dbo.AuditLogs
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs
    (
        Id              BIGINT           NOT NULL IDENTITY(1,1),
        TableName       NVARCHAR(128)    NOT NULL,
        Action          NVARCHAR(50)     NOT NULL,
        EntityId        NVARCHAR(50)     NULL,
        OldValues       NVARCHAR(MAX)    NULL,
        NewValues       NVARCHAR(MAX)    NULL,
        UserId          NVARCHAR(50)     NULL,
        UserEmail       NVARCHAR(256)    NULL,
        IpAddress       NVARCHAR(50)     NULL,
        CreatedAt       DATETIME2(7)     NOT NULL CONSTRAINT DF_dbo_AuditLogs_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    ALTER TABLE dbo.AuditLogs ADD CONSTRAINT PK_dbo_AuditLogs PRIMARY KEY CLUSTERED (Id);
END
GO

PRINT 'All tables created successfully.';
GO
