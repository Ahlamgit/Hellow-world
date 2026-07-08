/* =============================================================================
   KHADAMATI - Subscription Management Module
   Script: 009_SubscriptionManagement.sql
   Description: Extended subscription plans with billing options and user subscriptions
   ============================================================================= */

USE Khadamati;
GO

/* Drop legacy simple SubscriptionPlans if migrating from 002 schema */
IF OBJECT_ID(N'dbo.UserSubscriptions', N'U') IS NOT NULL AND COL_LENGTH('dbo.UserSubscriptions', 'PlanId') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_UserSubscriptions_Plans')
        ALTER TABLE dbo.UserSubscriptions DROP CONSTRAINT FK_UserSubscriptions_Plans;
END
GO

IF OBJECT_ID(N'dbo.SubscriptionPlans', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.SubscriptionPlans', 'PlanId') IS NOT NULL
BEGIN
  -- Legacy INT-based table from 002 - rename if present
  EXEC sp_rename 'dbo.SubscriptionPlans', 'SubscriptionPlans_Legacy';
END
GO

IF OBJECT_ID(N'dbo.SubscriptionPlans', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SubscriptionPlans
    (
        Id                          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_SubscriptionPlans_Id DEFAULT (NEWSEQUENTIALID()),
        PlanCode                    NVARCHAR(50)     NOT NULL,
        NameEn                      NVARCHAR(150)    NOT NULL,
        NameAr                      NVARCHAR(150)    NOT NULL,
        DescriptionEn               NVARCHAR(2000)   NULL,
        DescriptionAr               NVARCHAR(2000)   NULL,
        Currency                    NVARCHAR(3)      NOT NULL CONSTRAINT DF_SubscriptionPlans_Currency DEFAULT (N'SAR'),
        TargetRole                  INT              NOT NULL,
        Status                      INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_Status DEFAULT (2),
        DisplayPriority             INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_DisplayPriority DEFAULT (0),
        SearchPriority              INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_SearchPriority DEFAULT (0),
        IsFeatured                  BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_IsFeatured DEFAULT (0),
        HomePageVisible             BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_HomePageVisible DEFAULT (0),
        BannerVisible               BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_BannerVisible DEFAULT (0),
        CategoryVisible             BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_CategoryVisible DEFAULT (0),
        MaxCategories               INT              NULL,
        MaxServices                 INT              NULL,
        MaxPhotos                   INT              NULL,
        MaxVideos                   INT              NULL,
        MaxAdvertisements           INT              NULL,
        AdvertisementCredits        INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_AdCredits DEFAULT (0),
        FeaturedDays                INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_FeaturedDays DEFAULT (0),
        VerificationBadge           BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_VerificationBadge DEFAULT (0),
        PremiumBadge                BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_PremiumBadge DEFAULT (0),
        StatisticsDashboard         BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_Stats DEFAULT (0),
        Analytics                   BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_Analytics DEFAULT (0),
        PriorityCustomerSupport     BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_PrioritySupport DEFAULT (0),
        RenewalReminder             BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_RenewalReminder DEFAULT (1),
        AutoRenewal                 BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_AutoRenewal DEFAULT (0),
        ExpiryNotification          BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_ExpiryNotification DEFAULT (1),
        GracePeriodDays             INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_GracePeriod DEFAULT (0),
        TrialDays                   INT              NOT NULL CONSTRAINT DF_SubscriptionPlans_TrialDays DEFAULT (0),
        DiscountPercentage          DECIMAL(5,2)     NULL,
        CouponSupport               BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_CouponSupport DEFAULT (0),
        TaxRate                     DECIMAL(5,2)     NULL,
        VatRate                     DECIMAL(5,2)     NULL,
        PaymentRequired             BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_PaymentRequired DEFAULT (1),
        PaymentMethods              NVARCHAR(1000)   NULL,
        PlanColor                   NVARCHAR(20)     NULL,
        PlanIcon                    NVARCHAR(500)    NULL,
        ClonedFromPlanId            UNIQUEIDENTIFIER NULL,
        SuspendedAt                 DATETIME2(7)     NULL,
        ArchivedAt                  DATETIME2(7)     NULL,
        CreatedDate                 DATETIME2(7)     NOT NULL CONSTRAINT DF_SubscriptionPlans_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate                DATETIME2(7)     NULL,
        CreatedBy                   NVARCHAR(128)    NULL,
        ModifiedBy                  NVARCHAR(128)    NULL,
        Deleted                     BIT              NOT NULL CONSTRAINT DF_SubscriptionPlans_Deleted DEFAULT (0),
        DeletedDate                 DATETIME2(7)     NULL,
        DeletedBy                   NVARCHAR(128)    NULL,
        CONSTRAINT PK_SubscriptionPlans PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_SubscriptionPlans_PlanCode UNIQUE (PlanCode)
    );
END
GO

IF OBJECT_ID(N'dbo.PlanBillingOptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PlanBillingOptions
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_PlanBillingOptions_Id DEFAULT (NEWSEQUENTIALID()),
        PlanId          UNIQUEIDENTIFIER NOT NULL,
        Cycle           INT              NOT NULL,
        Price           DECIMAL(18,2)    NOT NULL,
        DurationDays    INT              NOT NULL,
        IsActive        BIT              NOT NULL CONSTRAINT DF_PlanBillingOptions_IsActive DEFAULT (1),
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_PlanBillingOptions_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_PlanBillingOptions_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_PlanBillingOptions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_PlanBillingOptions_PlanCycle UNIQUE (PlanId, Cycle),
        CONSTRAINT FK_PlanBillingOptions_Plans FOREIGN KEY (PlanId) REFERENCES dbo.SubscriptionPlans(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.UserSubscriptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserSubscriptions
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_UserSubscriptions_Id DEFAULT (NEWSEQUENTIALID()),
        UserId              UNIQUEIDENTIFIER NOT NULL,
        PlanId              UNIQUEIDENTIFIER NOT NULL,
        BillingOptionId     UNIQUEIDENTIFIER NULL,
        Status              INT              NOT NULL,
        StartDate           DATETIME2(7)     NOT NULL,
        EndDate             DATETIME2(7)     NULL,
        AutoRenew           BIT              NOT NULL CONSTRAINT DF_UserSubscriptions_AutoRenew DEFAULT (0),
        AmountPaid          DECIMAL(18,2)    NULL,
        Currency            NVARCHAR(3)      NULL,
        CouponCode          NVARCHAR(50)     NULL,
        CancelledAt         DATETIME2(7)     NULL,
        CancellationReason  NVARCHAR(500)    NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_UserSubscriptions_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_UserSubscriptions_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_UserSubscriptions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_UserSubscriptions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserSubscriptions_Plans FOREIGN KEY (PlanId) REFERENCES dbo.SubscriptionPlans(Id),
        CONSTRAINT FK_UserSubscriptions_BillingOptions FOREIGN KEY (BillingOptionId) REFERENCES dbo.PlanBillingOptions(Id)
    );
END
GO

CREATE NONCLUSTERED INDEX IX_SubscriptionPlans_Status ON dbo.SubscriptionPlans(Status) WHERE Deleted = 0;
CREATE NONCLUSTERED INDEX IX_SubscriptionPlans_TargetRole ON dbo.SubscriptionPlans(TargetRole) WHERE Deleted = 0;
CREATE NONCLUSTERED INDEX IX_SubscriptionPlans_DisplayPriority ON dbo.SubscriptionPlans(DisplayPriority DESC) WHERE Deleted = 0;
CREATE NONCLUSTERED INDEX IX_PlanBillingOptions_PlanId ON dbo.PlanBillingOptions(PlanId) WHERE Deleted = 0;
CREATE NONCLUSTERED INDEX IX_UserSubscriptions_UserId ON dbo.UserSubscriptions(UserId) WHERE Deleted = 0;
GO

/* Seed sample plans */
DECLARE @CraftsmanBasicId UNIQUEIDENTIFIER = NEWID();
DECLARE @StoreProId UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM dbo.SubscriptionPlans WHERE PlanCode = N'CRAFTSMAN_BASIC')
BEGIN
    INSERT INTO dbo.SubscriptionPlans (Id, PlanCode, NameEn, NameAr, DescriptionEn, DescriptionAr, Currency, TargetRole, Status,
        DisplayPriority, SearchPriority, IsFeatured, HomePageVisible, MaxServices, MaxPhotos, VerificationBadge, PremiumBadge,
        StatisticsDashboard, Analytics, TrialDays, VatRate, PaymentMethods, PlanColor, CreatedBy)
    VALUES (@CraftsmanBasicId, N'CRAFTSMAN_BASIC', N'Craftsman Basic', N'حرفي - أساسي',
        N'Monthly basic plan for craftsmen', N'خطة شهرية أساسية للحرفيين', N'SAR', 2, 1,
        10, 5, 0, 1, 5, 10, 0, 0, 0, 0, 7, 15.00, N'["Card","Mada"]', N'#4CAF50', N'SEED');

    INSERT INTO dbo.PlanBillingOptions (PlanId, Cycle, Price, DurationDays, CreatedBy)
    VALUES
        (@CraftsmanBasicId, 1, 99.00, 30, N'SEED'),
        (@CraftsmanBasicId, 2, 279.00, 90, N'SEED'),
        (@CraftsmanBasicId, 4, 999.00, 365, N'SEED');
END

IF NOT EXISTS (SELECT 1 FROM dbo.SubscriptionPlans WHERE PlanCode = N'STORE_PRO')
BEGIN
    INSERT INTO dbo.SubscriptionPlans (Id, PlanCode, NameEn, NameAr, DescriptionEn, DescriptionAr, Currency, TargetRole, Status,
        DisplayPriority, SearchPriority, IsFeatured, HomePageVisible, BannerVisible, MaxServices, MaxPhotos, MaxVideos,
        MaxAdvertisements, AdvertisementCredits, VerificationBadge, PremiumBadge, StatisticsDashboard, Analytics,
        PriorityCustomerSupport, AutoRenewal, TrialDays, CouponSupport, VatRate, PaymentMethods, PlanColor, PlanIcon, CreatedBy)
    VALUES (@StoreProId, N'STORE_PRO', N'Store Pro', N'متجر - احترافي',
        N'Premium store plan with analytics and priority support', N'خطة متجر مميزة مع التحليلات والدعم الأولوي', N'SAR', 3, 1,
        20, 15, 1, 1, 1, 50, 100, 20, 10, 500, 1, 1, 1, 1, 1, 1, 14, 1, 15.00, N'["Card","Mada","ApplePay"]', N'#FF9800', N'store-pro', N'SEED');

    INSERT INTO dbo.PlanBillingOptions (PlanId, Cycle, Price, DurationDays, CreatedBy)
    VALUES
        (@StoreProId, 1, 199.00, 30, N'SEED'),
        (@StoreProId, 3, 999.00, 180, N'SEED'),
        (@StoreProId, 4, 1799.00, 365, N'SEED'),
        (@StoreProId, 5, 4999.00, 0, N'SEED');
END
GO

PRINT 'Subscription Management module deployed successfully.';
GO
