/*
================================================================================
 KHADAMATI - 003 Create Indexes
 Filtered indexes exclude soft-deleted rows (Deleted = 0) for performance
================================================================================
*/
USE KhadamatiDb;
GO

-- USERS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email ON dbo.Users (Email) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Phone' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Phone ON dbo.Users (Phone) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_StatusId_RoleId' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE NONCLUSTERED INDEX IX_Users_StatusId_RoleId ON dbo.Users (StatusId, RoleId) INCLUDE (Email, Phone) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_VerificationStatusId' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE NONCLUSTERED INDEX IX_Users_VerificationStatusId ON dbo.Users (VerificationStatusId) WHERE Deleted = 0;

-- USER PROFILES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserProfiles_UserId' AND object_id = OBJECT_ID(N'dbo.UserProfiles'))
    CREATE UNIQUE NONCLUSTERED INDEX IX_UserProfiles_UserId ON dbo.UserProfiles (UserId) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserProfiles_Name' AND object_id = OBJECT_ID(N'dbo.UserProfiles'))
    CREATE NONCLUSTERED INDEX IX_UserProfiles_Name ON dbo.UserProfiles (LastName, FirstName) WHERE Deleted = 0;

-- ADDRESSES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Addresses_UserId' AND object_id = OBJECT_ID(N'dbo.Addresses'))
    CREATE NONCLUSTERED INDEX IX_Addresses_UserId ON dbo.Addresses (UserId) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Addresses_UserId_IsDefault' AND object_id = OBJECT_ID(N'dbo.Addresses'))
    CREATE NONCLUSTERED INDEX IX_Addresses_UserId_IsDefault ON dbo.Addresses (UserId, IsDefault) INCLUDE (Latitude, Longitude) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Addresses_Geo' AND object_id = OBJECT_ID(N'dbo.Addresses'))
    CREATE NONCLUSTERED INDEX IX_Addresses_Geo ON dbo.Addresses (Latitude, Longitude) WHERE Deleted = 0 AND Latitude IS NOT NULL AND Longitude IS NOT NULL;

-- REFRESH TOKENS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
    CREATE NONCLUSTERED INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens (UserId) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_ExpiresAt' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
    CREATE NONCLUSTERED INDEX IX_RefreshTokens_ExpiresAt ON dbo.RefreshTokens (ExpiresAt) WHERE Deleted = 0;

-- CRAFTSMAN PROFILES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CraftsmanProfiles_UserId' AND object_id = OBJECT_ID(N'dbo.CraftsmanProfiles'))
    CREATE UNIQUE NONCLUSTERED INDEX IX_CraftsmanProfiles_UserId ON dbo.CraftsmanProfiles (UserId) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CraftsmanProfiles_Available_Rating' AND object_id = OBJECT_ID(N'dbo.CraftsmanProfiles'))
    CREATE NONCLUSTERED INDEX IX_CraftsmanProfiles_Available_Rating ON dbo.CraftsmanProfiles (IsAvailable, Rating DESC) WHERE Deleted = 0;

-- STORE PROFILES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StoreProfiles_UserId' AND object_id = OBJECT_ID(N'dbo.StoreProfiles'))
    CREATE UNIQUE NONCLUSTERED INDEX IX_StoreProfiles_UserId ON dbo.StoreProfiles (UserId) WHERE Deleted = 0;

-- SERVICE CATEGORIES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceCategories_ParentCategoryId' AND object_id = OBJECT_ID(N'dbo.ServiceCategories'))
    CREATE NONCLUSTERED INDEX IX_ServiceCategories_ParentCategoryId ON dbo.ServiceCategories (ParentCategoryId, DisplayOrder) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceCategories_Active' AND object_id = OBJECT_ID(N'dbo.ServiceCategories'))
    CREATE NONCLUSTERED INDEX IX_ServiceCategories_Active ON dbo.ServiceCategories (IsActive, DisplayOrder) WHERE Deleted = 0;

-- SERVICES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Services_CategoryId' AND object_id = OBJECT_ID(N'dbo.Services'))
    CREATE NONCLUSTERED INDEX IX_Services_CategoryId ON dbo.Services (CategoryId) WHERE Deleted = 0 AND IsActive = 1;

-- CRAFTSMAN SERVICES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CraftsmanServices_ServiceId' AND object_id = OBJECT_ID(N'dbo.CraftsmanServices'))
    CREATE NONCLUSTERED INDEX IX_CraftsmanServices_ServiceId ON dbo.CraftsmanServices (ServiceId) WHERE Deleted = 0 AND IsAvailable = 1;

-- STORE PRODUCTS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StoreProducts_StoreProfileId' AND object_id = OBJECT_ID(N'dbo.StoreProducts'))
    CREATE NONCLUSTERED INDEX IX_StoreProducts_StoreProfileId ON dbo.StoreProducts (StoreProfileId) WHERE Deleted = 0 AND IsActive = 1;

-- SERVICE REQUESTS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceRequests_CustomerId' AND object_id = OBJECT_ID(N'dbo.ServiceRequests'))
    CREATE NONCLUSTERED INDEX IX_ServiceRequests_CustomerId ON dbo.ServiceRequests (CustomerId, CreatedDate DESC) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceRequests_CraftsmanId' AND object_id = OBJECT_ID(N'dbo.ServiceRequests'))
    CREATE NONCLUSTERED INDEX IX_ServiceRequests_CraftsmanId ON dbo.ServiceRequests (CraftsmanId, CreatedDate DESC) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceRequests_RequestStatusId' AND object_id = OBJECT_ID(N'dbo.ServiceRequests'))
    CREATE NONCLUSTERED INDEX IX_ServiceRequests_RequestStatusId ON dbo.ServiceRequests (RequestStatusId, ScheduledAt) WHERE Deleted = 0;

-- SERVICE REQUEST STATUS HISTORY
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceRequestStatusHistory_ServiceRequestId' AND object_id = OBJECT_ID(N'dbo.ServiceRequestStatusHistory'))
    CREATE NONCLUSTERED INDEX IX_ServiceRequestStatusHistory_ServiceRequestId ON dbo.ServiceRequestStatusHistory (ServiceRequestId, CreatedDate DESC) WHERE Deleted = 0;

-- REVIEWS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reviews_ReviewedUserId' AND object_id = OBJECT_ID(N'dbo.Reviews'))
    CREATE NONCLUSTERED INDEX IX_Reviews_ReviewedUserId ON dbo.Reviews (ReviewedUserId, CreatedDate DESC) WHERE Deleted = 0;

-- PAYMENTS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Payments_ServiceRequestId' AND object_id = OBJECT_ID(N'dbo.Payments'))
    CREATE NONCLUSTERED INDEX IX_Payments_ServiceRequestId ON dbo.Payments (ServiceRequestId) WHERE Deleted = 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Payments_PaymentStatusId' AND object_id = OBJECT_ID(N'dbo.Payments'))
    CREATE NONCLUSTERED INDEX IX_Payments_PaymentStatusId ON dbo.Payments (PaymentStatusId, CreatedDate DESC) WHERE Deleted = 0;

-- NOTIFICATIONS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Notifications_UserId_IsRead' AND object_id = OBJECT_ID(N'dbo.Notifications'))
    CREATE NONCLUSTERED INDEX IX_Notifications_UserId_IsRead ON dbo.Notifications (UserId, IsRead, CreatedDate DESC) WHERE Deleted = 0;

-- USER SUBSCRIPTIONS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserSubscriptions_UserId' AND object_id = OBJECT_ID(N'dbo.UserSubscriptions'))
    CREATE NONCLUSTERED INDEX IX_UserSubscriptions_UserId ON dbo.UserSubscriptions (UserId, EndDate DESC) WHERE Deleted = 0;

-- VERIFICATION DOCUMENTS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_VerificationDocuments_UserId' AND object_id = OBJECT_ID(N'dbo.VerificationDocuments'))
    CREATE NONCLUSTERED INDEX IX_VerificationDocuments_UserId ON dbo.VerificationDocuments (UserId, VerificationStatusId) WHERE Deleted = 0;

-- AUDIT LOGS
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLogs_CreatedDate' AND object_id = OBJECT_ID(N'audit.AuditLogs'))
    CREATE NONCLUSTERED INDEX IX_AuditLogs_CreatedDate ON audit.AuditLogs (CreatedDate DESC);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLogs_TableName_EntityId' AND object_id = OBJECT_ID(N'audit.AuditLogs'))
    CREATE NONCLUSTERED INDEX IX_AuditLogs_TableName_EntityId ON audit.AuditLogs (SchemaName, TableName, EntityId, CreatedDate DESC);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLogs_UserId' AND object_id = OBJECT_ID(N'audit.AuditLogs'))
    CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId ON audit.AuditLogs (UserId, CreatedDate DESC);

PRINT 'All indexes created successfully.';
GO
