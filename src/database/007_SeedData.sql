/*
================================================================================
 KHADAMATI - 007 Seed Data
================================================================================
*/
USE KhadamatiDb;
GO

SET NOCOUNT ON;

-- ROLES
MERGE ref.Roles AS t
USING (VALUES
    (N'Customer',      N'Customer',      N'عميل'),
    (N'Craftsman',     N'Craftsman',     N'حرفي'),
    (N'Store',         N'Store',         N'متجر'),
    (N'Administrator', N'Administrator', N'مدير النظام')
) AS s(RoleCode, RoleNameEn, RoleNameAr) ON t.RoleCode = s.RoleCode
WHEN NOT MATCHED THEN INSERT (RoleCode, RoleNameEn, RoleNameAr, CreatedBy) VALUES (s.RoleCode, s.RoleNameEn, s.RoleNameAr, N'SEED');
GO

-- USER STATUSES
MERGE ref.UserStatuses AS t
USING (VALUES
    (N'Pending',   N'Pending',   N'قيد الانتظار'),
    (N'Active',    N'Active',    N'نشط'),
    (N'Suspended', N'Suspended', N'موقوف'),
    (N'Banned',    N'Banned',    N'محظور')
) AS s(StatusCode, StatusNameEn, StatusNameAr) ON t.StatusCode = s.StatusCode
WHEN NOT MATCHED THEN INSERT (StatusCode, StatusNameEn, StatusNameAr, CreatedBy) VALUES (s.StatusCode, s.StatusNameEn, s.StatusNameAr, N'SEED');
GO

-- VERIFICATION STATUSES
MERGE ref.VerificationStatuses AS t
USING (VALUES
    (N'Unverified',    N'Unverified',    N'غير موثق'),
    (N'PendingReview', N'Pending Review', N'قيد المراجعة'),
    (N'Verified',      N'Verified',      N'موثق'),
    (N'Rejected',      N'Rejected',      N'مرفوض')
) AS s(StatusCode, StatusNameEn, StatusNameAr) ON t.StatusCode = s.StatusCode
WHEN NOT MATCHED THEN INSERT (StatusCode, StatusNameEn, StatusNameAr, CreatedBy) VALUES (s.StatusCode, s.StatusNameEn, s.StatusNameAr, N'SEED');
GO

-- SUBSCRIPTION STATUSES
MERGE ref.SubscriptionStatuses AS t
USING (VALUES
    (N'None',      N'None',      N'لا يوجد'),
    (N'Trial',     N'Trial',     N'تجريبي'),
    (N'Active',    N'Active',    N'نشط'),
    (N'Expired',   N'Expired',   N'منتهي'),
    (N'Cancelled', N'Cancelled', N'ملغي')
) AS s(StatusCode, StatusNameEn, StatusNameAr) ON t.StatusCode = s.StatusCode
WHEN NOT MATCHED THEN INSERT (StatusCode, StatusNameEn, StatusNameAr, CreatedBy) VALUES (s.StatusCode, s.StatusNameEn, s.StatusNameAr, N'SEED');
GO

-- SERVICE REQUEST STATUSES
MERGE ref.ServiceRequestStatuses AS t
USING (VALUES
    (N'Draft',      N'Draft',      N'مسودة',      1),
    (N'Pending',    N'Pending',    N'قيد الانتظار', 2),
    (N'Assigned',   N'Assigned',   N'تم التعيين', 3),
    (N'InProgress', N'In Progress', N'قيد التنفيذ', 4),
    (N'Completed',  N'Completed',  N'مكتمل',      5),
    (N'Cancelled',  N'Cancelled',  N'ملغي',       6),
    (N'Disputed',   N'Disputed',   N'متنازع عليه', 7)
) AS s(StatusCode, StatusNameEn, StatusNameAr, DisplayOrder) ON t.StatusCode = s.StatusCode
WHEN NOT MATCHED THEN INSERT (StatusCode, StatusNameEn, StatusNameAr, DisplayOrder, CreatedBy)
    VALUES (s.StatusCode, s.StatusNameEn, s.StatusNameAr, s.DisplayOrder, N'SEED');
GO

-- PAYMENT STATUSES
MERGE ref.PaymentStatuses AS t
USING (VALUES
    (N'Pending',   N'Pending',   N'قيد الانتظار'),
    (N'Processing',N'Processing',N'قيد المعالجة'),
    (N'Completed', N'Completed', N'مكتمل'),
    (N'Failed',    N'Failed',    N'فشل'),
    (N'Refunded',  N'Refunded',  N'مسترد')
) AS s(StatusCode, StatusNameEn, StatusNameAr) ON t.StatusCode = s.StatusCode
WHEN NOT MATCHED THEN INSERT (StatusCode, StatusNameEn, StatusNameAr, CreatedBy) VALUES (s.StatusCode, s.StatusNameEn, s.StatusNameAr, N'SEED');
GO

-- SUBSCRIPTION PLANS
DECLARE @CraftsmanRoleId INT = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Craftsman');
DECLARE @StoreRoleId INT = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Store');

MERGE dbo.SubscriptionPlans AS t
USING (VALUES
    (N'CRAFTSMAN_BASIC',  N'Craftsman Basic',  N'حرفي - أساسي',  N'Monthly basic plan for craftsmen', N'خطة شهرية أساسية للحرفيين', 99.00, 30, @CraftsmanRoleId),
    (N'CRAFTSMAN_PRO',    N'Craftsman Pro',    N'حرفي - احترافي', N'Premium plan with priority listing', N'خطة مميزة مع أولوية الظهور', 199.00, 30, @CraftsmanRoleId),
    (N'STORE_BASIC',      N'Store Basic',      N'متجر - أساسي',  N'Monthly basic plan for stores', N'خطة شهرية أساسية للمتاجر', 149.00, 30, @StoreRoleId)
) AS s(PlanCode, PlanNameEn, PlanNameAr, DescriptionEn, DescriptionAr, Price, DurationDays, TargetRoleId)
ON t.PlanCode = s.PlanCode
WHEN NOT MATCHED THEN INSERT (PlanCode, PlanNameEn, PlanNameAr, DescriptionEn, DescriptionAr, Price, DurationDays, TargetRoleId, CreatedBy)
    VALUES (s.PlanCode, s.PlanNameEn, s.PlanNameAr, s.DescriptionEn, s.DescriptionAr, s.Price, s.DurationDays, s.TargetRoleId, N'SEED');
GO

-- ADMIN USER (BCrypt hash for Admin@123456, work factor 12)
DECLARE @AdminId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'admin@khadamati.com')
BEGIN
    INSERT INTO dbo.Users (Id, Email, Phone, PasswordHash, RoleId, StatusId, VerificationStatusId, SubscriptionStatusId, CreatedBy)
    VALUES (
        @AdminId,
        N'admin@khadamati.com',
        N'+966500000001',
        N'$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4.G2oX1Y5qJGmOeS', -- Admin@123456
        (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Administrator'),
        (SELECT StatusId FROM ref.UserStatuses WHERE StatusCode = N'Active'),
        (SELECT VerificationStatusId FROM ref.VerificationStatuses WHERE StatusCode = N'Verified'),
        (SELECT SubscriptionStatusId FROM ref.SubscriptionStatuses WHERE StatusCode = N'Active'),
        N'SEED'
    );

    INSERT INTO dbo.UserProfiles (UserId, FirstName, LastName, PreferredLanguage, CreatedBy)
    VALUES (@AdminId, N'System', N'Administrator', N'en', N'SEED');
END
GO

-- SERVICE CATEGORIES
DECLARE @CatPlumbing UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222201';
DECLARE @CatElectrical UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222202';
DECLARE @CatHVAC UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222203';
DECLARE @CatPainting UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222204';
DECLARE @CatCleaning UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222205';

MERGE dbo.ServiceCategories AS t
USING (VALUES
    (@CatPlumbing,   N'سباكة',  N'Plumbing',   N'خدمات السباكة والصرف الصحي', N'Plumbing and drainage services', 1),
    (@CatElectrical, N'كهرباء', N'Electrical', N'خدمات الكهرباء والتمديدات', N'Electrical services and wiring', 2),
    (@CatHVAC,       N'تكييف',  N'HVAC',       N'صيانة وتركيب التكييف', N'AC maintenance and installation', 3),
    (@CatPainting,   N'دهان',   N'Painting',   N'خدمات الدهان والديكور', N'Painting and decoration', 4),
    (@CatCleaning,   N'تنظيف',  N'Cleaning',   N'خدمات التنظيف المنزلي', N'Home cleaning services', 5)
) AS s(Id, NameAr, NameEn, DescriptionAr, DescriptionEn, DisplayOrder)
ON t.Id = s.Id
WHEN NOT MATCHED THEN INSERT (Id, NameAr, NameEn, DescriptionAr, DescriptionEn, DisplayOrder, CreatedBy)
    VALUES (s.Id, s.NameAr, s.NameEn, s.DescriptionAr, s.DescriptionEn, s.DisplayOrder, N'SEED');
GO

-- SERVICES
MERGE dbo.Services AS t
USING (VALUES
    ('33333333-3333-3333-3333-333333333301', '22222222-2222-2222-2222-222222222201', N'إصلاح تسرب',      N'Leak Repair',           150.00, 60),
    ('33333333-3333-3333-3333-333333333302', '22222222-2222-2222-2222-222222222201', N'تركيب صنبور',     N'Faucet Installation',   100.00, 45),
    ('33333333-3333-3333-3333-333333333303', '22222222-2222-2222-2222-222222222202', N'إصلاح كهربائي',   N'Electrical Repair',     200.00, 90),
    ('33333333-3333-3333-3333-333333333304', '22222222-2222-2222-2222-222222222203', N'صيانة مكيف',       N'AC Maintenance',        250.00, 120),
    ('33333333-3333-3333-3333-333333333305', '22222222-2222-2222-2222-222222222205', N'تنظيف منزل',      N'Home Cleaning',         300.00, 180),
    ('33333333-3333-3333-3333-333333333306', '22222222-2222-2222-2222-222222222204', N'دهان غرفة',       N'Room Painting',         400.00, 240)
) AS s(Id, CategoryId, NameAr, NameEn, BasePrice, Duration)
ON t.Id = s.Id
WHEN NOT MATCHED THEN INSERT (Id, CategoryId, NameAr, NameEn, BasePrice, EstimatedDurationMinutes, CreatedBy)
    VALUES (s.Id, s.CategoryId, s.NameAr, s.NameEn, s.BasePrice, s.Duration, N'SEED');
GO

-- SAMPLE CUSTOMER
DECLARE @CustomerId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444401';
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'customer@khadamati.com')
BEGIN
    INSERT INTO dbo.Users (Id, Email, Phone, PasswordHash, RoleId, StatusId, VerificationStatusId, SubscriptionStatusId, CreatedBy)
    VALUES (
        @CustomerId, N'customer@khadamati.com', N'+966500000002',
        N'$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4.G2oX1Y5qJGmOeS',
        (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Customer'),
        (SELECT StatusId FROM ref.UserStatuses WHERE StatusCode = N'Active'),
        (SELECT VerificationStatusId FROM ref.VerificationStatuses WHERE StatusCode = N'Verified'),
        (SELECT SubscriptionStatusId FROM ref.SubscriptionStatuses WHERE StatusCode = N'None'),
        N'SEED'
    );
    INSERT INTO dbo.UserProfiles (UserId, FirstName, LastName, PreferredLanguage, CreatedBy)
    VALUES (@CustomerId, N'Ahmed', N'Al-Rashid', N'ar', N'SEED');
    INSERT INTO dbo.Addresses (UserId, Label, Street, City, District, Latitude, Longitude, IsDefault, CreatedBy)
    VALUES (@CustomerId, N'Home', N'King Fahd Road', N'Riyadh', N'Al Olaya', 24.7136000, 46.6753000, 1, N'SEED');
END
GO

-- SAMPLE CRAFTSMAN
DECLARE @CraftsmanId UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555501';
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'craftsman@khadamati.com')
BEGIN
    INSERT INTO dbo.Users (Id, Email, Phone, PasswordHash, RoleId, StatusId, VerificationStatusId, SubscriptionStatusId, CreatedBy)
    VALUES (
        @CraftsmanId, N'craftsman@khadamati.com', N'+966500000003',
        N'$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4.G2oX1Y5qJGmOeS',
        (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Craftsman'),
        (SELECT StatusId FROM ref.UserStatuses WHERE StatusCode = N'Active'),
        (SELECT VerificationStatusId FROM ref.VerificationStatuses WHERE StatusCode = N'Verified'),
        (SELECT SubscriptionStatusId FROM ref.SubscriptionStatuses WHERE StatusCode = N'Active'),
        N'SEED'
    );
    INSERT INTO dbo.UserProfiles (UserId, FirstName, LastName, PreferredLanguage, CreatedBy)
    VALUES (@CraftsmanId, N'Mohammed', N'Al-Saud', N'ar', N'SEED');

    DECLARE @CraftsmanProfileId UNIQUEIDENTIFIER = NEWID();
    INSERT INTO dbo.CraftsmanProfiles (Id, UserId, Specialization, YearsOfExperience, Rating, TotalReviews, CompletedJobs, IsAvailable, ServiceRadiusKm, CreatedBy)
    VALUES (@CraftsmanProfileId, @CraftsmanId, N'Plumbing & Electrical', 8, 4.75, 120, 95, 1, 30.00, N'SEED');

    INSERT INTO dbo.Addresses (UserId, Label, Street, City, District, Latitude, Longitude, IsDefault, CreatedBy)
    VALUES (@CraftsmanId, N'Workshop', N'Industrial Area', N'Riyadh', N'Al Malaz', 24.6877000, 46.7219000, 1, N'SEED');
END
GO

PRINT 'Seed data inserted successfully.';
GO
