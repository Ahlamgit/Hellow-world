/*
================================================================================
 KHADAMATI - 004 Create Views
================================================================================
*/
USE KhadamatiDb;
GO

CREATE OR ALTER VIEW dbo.vw_ActiveUsers
AS
SELECT
    u.Id,
    u.Email,
    u.Phone,
    r.RoleCode,
    r.RoleNameEn,
    r.RoleNameAr,
    us.StatusCode AS UserStatus,
    vs.StatusCode AS VerificationStatus,
    ss.StatusCode AS SubscriptionStatus,
    p.FirstName,
    p.LastName,
    p.ProfilePictureUrl,
    p.PreferredLanguage,
    u.SubscriptionExpiresAt,
    u.LastLoginAt,
    u.CreatedDate,
    u.ModifiedDate
FROM dbo.Users u
INNER JOIN ref.Roles r ON u.RoleId = r.RoleId
INNER JOIN ref.UserStatuses us ON u.StatusId = us.StatusId
INNER JOIN ref.VerificationStatuses vs ON u.VerificationStatusId = vs.VerificationStatusId
INNER JOIN ref.SubscriptionStatuses ss ON u.SubscriptionStatusId = ss.SubscriptionStatusId
INNER JOIN dbo.UserProfiles p ON u.Id = p.UserId AND p.Deleted = 0
WHERE u.Deleted = 0 AND us.StatusCode = N'Active';
GO

CREATE OR ALTER VIEW dbo.vw_ServiceRequestSummary
AS
SELECT
    sr.Id AS ServiceRequestId,
    srs.StatusCode AS RequestStatus,
    srs.StatusNameEn,
    srs.StatusNameAr,
    sr.ScheduledAt,
    sr.CompletedAt,
    sr.EstimatedPrice,
    sr.FinalPrice,
    s.NameEn AS ServiceNameEn,
    s.NameAr AS ServiceNameAr,
    cust_p.FirstName + N' ' + cust_p.LastName AS CustomerName,
    ISNULL(craft_p.FirstName + N' ' + craft_p.LastName, N'Unassigned') AS CraftsmanName,
    a.City,
    a.District,
    sr.CustomerRating,
    sr.CreatedDate,
    sr.ModifiedDate
FROM dbo.ServiceRequests sr
INNER JOIN ref.ServiceRequestStatuses srs ON sr.RequestStatusId = srs.RequestStatusId
INNER JOIN dbo.Services s ON sr.ServiceId = s.Id
INNER JOIN dbo.Users cust ON sr.CustomerId = cust.Id
INNER JOIN dbo.UserProfiles cust_p ON cust.Id = cust_p.UserId
LEFT JOIN dbo.Users craft ON sr.CraftsmanId = craft.Id
LEFT JOIN dbo.UserProfiles craft_p ON craft.Id = craft_p.UserId
LEFT JOIN dbo.Addresses a ON sr.AddressId = a.Id
WHERE sr.Deleted = 0;
GO

CREATE OR ALTER VIEW dbo.vw_CraftsmanDirectory
AS
SELECT
    u.Id AS CraftsmanUserId,
    p.FirstName,
    p.LastName,
    p.ProfilePictureUrl,
    cp.Specialization,
    cp.YearsOfExperience,
    cp.Rating,
    cp.TotalReviews,
    cp.CompletedJobs,
    cp.IsAvailable,
    cp.ServiceRadiusKm,
    a.City,
    a.Latitude,
    a.Longitude,
    u.VerificationStatusId,
    vs.StatusCode AS VerificationStatus
FROM dbo.Users u
INNER JOIN dbo.CraftsmanProfiles cp ON u.Id = cp.UserId AND cp.Deleted = 0
INNER JOIN dbo.UserProfiles p ON u.Id = p.UserId AND p.Deleted = 0
INNER JOIN ref.VerificationStatuses vs ON u.VerificationStatusId = vs.VerificationStatusId
LEFT JOIN dbo.Addresses a ON u.Id = a.UserId AND a.IsDefault = 1 AND a.Deleted = 0
WHERE u.Deleted = 0 AND u.RoleId = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Craftsman');
GO

CREATE OR ALTER VIEW dbo.vw_StoreDirectory
AS
SELECT
    u.Id AS StoreUserId,
    sp.StoreName,
    sp.Description,
    sp.Rating,
    sp.TotalReviews,
    sp.IsOpen,
    sp.OpeningTime,
    sp.ClosingTime,
    sp.CommercialRegistration,
    a.City,
    a.Street,
    (SELECT COUNT(*) FROM dbo.StoreProducts pr WHERE pr.StoreProfileId = sp.Id AND pr.Deleted = 0 AND pr.IsActive = 1) AS ActiveProductCount
FROM dbo.Users u
INNER JOIN dbo.StoreProfiles sp ON u.Id = sp.UserId AND sp.Deleted = 0
LEFT JOIN dbo.Addresses a ON u.Id = a.UserId AND a.IsDefault = 1 AND a.Deleted = 0
WHERE u.Deleted = 0 AND u.RoleId = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Store');
GO

CREATE OR ALTER VIEW dbo.vw_PaymentSummary
AS
SELECT
    p.Id AS PaymentId,
    p.ServiceRequestId,
    ps.StatusCode AS PaymentStatus,
    p.Amount,
    p.CurrencyCode,
    p.PaymentMethod,
    p.TransactionReference,
    p.PaidAt,
    payer_p.FirstName + N' ' + payer_p.LastName AS PayerName,
    payee_p.FirstName + N' ' + payee_p.LastName AS PayeeName,
    p.CreatedDate
FROM dbo.Payments p
INNER JOIN ref.PaymentStatuses ps ON p.PaymentStatusId = ps.PaymentStatusId
INNER JOIN dbo.Users payer ON p.PayerUserId = payer.Id
INNER JOIN dbo.UserProfiles payer_p ON payer.Id = payer_p.UserId
INNER JOIN dbo.Users payee ON p.PayeeUserId = payee.Id
INNER JOIN dbo.UserProfiles payee_p ON payee.Id = payee_p.UserId
WHERE p.Deleted = 0;
GO

CREATE OR ALTER VIEW dbo.vw_DashboardMetrics
AS
SELECT
    (SELECT COUNT(*) FROM dbo.Users WHERE Deleted = 0 AND RoleId = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Customer')) AS TotalCustomers,
    (SELECT COUNT(*) FROM dbo.Users WHERE Deleted = 0 AND RoleId = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Craftsman')) AS TotalCraftsmen,
    (SELECT COUNT(*) FROM dbo.Users WHERE Deleted = 0 AND RoleId = (SELECT RoleId FROM ref.Roles WHERE RoleCode = N'Store')) AS TotalStores,
    (SELECT COUNT(*) FROM dbo.ServiceRequests WHERE Deleted = 0 AND RequestStatusId = (SELECT RequestStatusId FROM ref.ServiceRequestStatuses WHERE StatusCode = N'Pending')) AS PendingRequests,
    (SELECT COUNT(*) FROM dbo.ServiceRequests WHERE Deleted = 0 AND RequestStatusId = (SELECT RequestStatusId FROM ref.ServiceRequestStatuses WHERE StatusCode = N'InProgress')) AS InProgressRequests,
    (SELECT COUNT(*) FROM dbo.ServiceRequests WHERE Deleted = 0 AND RequestStatusId = (SELECT RequestStatusId FROM ref.ServiceRequestStatuses WHERE StatusCode = N'Completed')) AS CompletedRequests,
    (SELECT ISNULL(SUM(FinalPrice), 0) FROM dbo.ServiceRequests WHERE Deleted = 0 AND RequestStatusId = (SELECT RequestStatusId FROM ref.ServiceRequestStatuses WHERE StatusCode = N'Completed')) AS TotalRevenue,
    (SELECT COUNT(*) FROM dbo.Payments WHERE Deleted = 0 AND PaymentStatusId = (SELECT PaymentStatusId FROM ref.PaymentStatuses WHERE StatusCode = N'Completed')) AS CompletedPayments;
GO

PRINT 'All views created successfully.';
GO
