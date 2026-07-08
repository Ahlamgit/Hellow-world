/* =============================================================================
   KHADAMATI - Booking Module
   Script: 010_BookingModule.sql
   ============================================================================= */

USE Khadamati;
GO

-- Update service request statuses reference data
MERGE ref.ServiceRequestStatuses AS t
USING (VALUES
    (N'Pending', N'Pending', N'قيد الانتظار', 1),
    (N'AwaitingPayment', N'Awaiting Payment', N'في انتظار الدفع', 2),
    (N'PaymentConfirmed', N'Payment Confirmed', N'تم تأكيد الدفع', 3),
    (N'PendingCraftsmanConfirmation', N'Pending Craftsman Confirmation', N'في انتظار تأكيد الحرفي', 4),
    (N'Confirmed', N'Confirmed', N'مؤكد', 5),
    (N'Completed', N'Completed', N'مكتمل', 6),
    (N'Cancelled', N'Cancelled', N'ملغي', 7),
    (N'Rejected', N'Rejected', N'مرفوض', 8),
    (N'Expired', N'Expired', N'منتهي', 9),
    (N'NoShow', N'No Show', N'لم يحضر', 10),
    (N'Rescheduled', N'Rescheduled', N'أعيد جدولته', 11)
) AS s(StatusCode, StatusNameEn, StatusNameAr, DisplayOrder)
ON t.StatusCode = s.StatusCode
WHEN NOT MATCHED THEN INSERT (StatusCode, StatusNameEn, StatusNameAr, DisplayOrder, CreatedBy)
    VALUES (s.StatusCode, s.StatusNameEn, s.StatusNameAr, s.DisplayOrder, N'SEED')
WHEN MATCHED THEN UPDATE SET StatusNameEn = s.StatusNameEn, StatusNameAr = s.StatusNameAr, DisplayOrder = s.DisplayOrder;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'BookingReference') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD BookingReference NVARCHAR(30) NULL;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'SlotEnd') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD SlotEnd DATETIME2(7) NULL;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'PaymentDueAt') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD PaymentDueAt DATETIME2(7) NULL;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'ExpiresAt') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD ExpiresAt DATETIME2(7) NULL;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'RejectionReason') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD RejectionReason NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'CancellationReason') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD CancellationReason NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.ServiceRequests', 'RescheduledFromId') IS NULL
    ALTER TABLE dbo.ServiceRequests ADD RescheduledFromId UNIQUEIDENTIFIER NULL;
GO

IF OBJECT_ID(N'dbo.BookingSlotReservations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingSlotReservations
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_BookingSlotReservations_Id DEFAULT (NEWSEQUENTIALID()),
        CraftsmanId     UNIQUEIDENTIFIER NOT NULL,
        ServiceRequestId UNIQUEIDENTIFIER NOT NULL,
        SlotStart       DATETIME2(7)     NOT NULL,
        SlotEnd         DATETIME2(7)     NOT NULL,
        IsActive        BIT              NOT NULL CONSTRAINT DF_BookingSlotReservations_IsActive DEFAULT (1),
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_BookingSlotReservations_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_BookingSlotReservations_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_BookingSlotReservations PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_BookingSlotReservations_Craftsman FOREIGN KEY (CraftsmanId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_BookingSlotReservations_ServiceRequest FOREIGN KEY (ServiceRequestId) REFERENCES dbo.ServiceRequests(Id)
    );
    CREATE UNIQUE NONCLUSTERED INDEX UX_BookingSlotReservations_Craftsman_Slot
        ON dbo.BookingSlotReservations (CraftsmanId, SlotStart) WHERE IsActive = 1 AND Deleted = 0;
END
GO

IF OBJECT_ID(N'dbo.CraftsmanWorkingHours', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CraftsmanWorkingHours
    (
        Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_CraftsmanWorkingHours_Id DEFAULT (NEWSEQUENTIALID()),
        CraftsmanId     UNIQUEIDENTIFIER NOT NULL,
        DayOfWeek       INT              NOT NULL,
        StartTime       TIME(0)          NOT NULL,
        EndTime         TIME(0)          NOT NULL,
        IsActive        BIT              NOT NULL CONSTRAINT DF_CraftsmanWorkingHours_IsActive DEFAULT (1),
        CreatedDate     DATETIME2(7)     NOT NULL CONSTRAINT DF_CraftsmanWorkingHours_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate    DATETIME2(7)     NULL,
        CreatedBy       NVARCHAR(128)    NULL,
        ModifiedBy      NVARCHAR(128)    NULL,
        Deleted         BIT              NOT NULL CONSTRAINT DF_CraftsmanWorkingHours_Deleted DEFAULT (0),
        DeletedDate     DATETIME2(7)     NULL,
        DeletedBy       NVARCHAR(128)    NULL,
        CONSTRAINT PK_CraftsmanWorkingHours PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_CraftsmanWorkingHours_Craftsman FOREIGN KEY (CraftsmanId) REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.BookingPayments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingPayments
    (
        Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_BookingPayments_Id DEFAULT (NEWSEQUENTIALID()),
        ServiceRequestId    UNIQUEIDENTIFIER NOT NULL,
        PayerUserId         UNIQUEIDENTIFIER NOT NULL,
        PayeeUserId         UNIQUEIDENTIFIER NOT NULL,
        Amount              DECIMAL(18,2)    NOT NULL,
        Currency            NVARCHAR(3)      NOT NULL CONSTRAINT DF_BookingPayments_Currency DEFAULT (N'SAR'),
        Status              INT              NOT NULL,
        PaymentMethod       NVARCHAR(50)     NOT NULL,
        TransactionReference NVARCHAR(200)   NULL,
        PaidAt              DATETIME2(7)     NULL,
        FailureReason       NVARCHAR(500)    NULL,
        CreatedDate         DATETIME2(7)     NOT NULL CONSTRAINT DF_BookingPayments_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate        DATETIME2(7)     NULL,
        CreatedBy           NVARCHAR(128)    NULL,
        ModifiedBy          NVARCHAR(128)    NULL,
        Deleted             BIT              NOT NULL CONSTRAINT DF_BookingPayments_Deleted DEFAULT (0),
        DeletedDate         DATETIME2(7)     NULL,
        DeletedBy           NVARCHAR(128)    NULL,
        CONSTRAINT PK_BookingPayments PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_BookingPayments_ServiceRequest UNIQUE (ServiceRequestId),
        CONSTRAINT FK_BookingPayments_ServiceRequest FOREIGN KEY (ServiceRequestId) REFERENCES dbo.ServiceRequests(Id),
        CONSTRAINT FK_BookingPayments_Payer FOREIGN KEY (PayerUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_BookingPayments_Payee FOREIGN KEY (PayeeUserId) REFERENCES dbo.Users(Id)
    );
END
GO

PRINT 'Booking module SQL deployed.';
GO
