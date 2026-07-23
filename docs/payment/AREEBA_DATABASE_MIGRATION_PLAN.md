# Areeba — Database Migration Plan (Phase A)

**Status:** Implementation input  
**Date:** 2026-07-23  
**Database:** SQL Server (authoritative)  
**Rule:** EF migrations must match physical SQL schema  

## 1. Current state (verified)

### `BookingPayments`

| Column | Type | Notes |
|--------|------|-------|
| `Id` | `uniqueidentifier` | PK |
| `ServiceRequestId` | `uniqueidentifier` | **Unique** (1:0..1 obligation) |
| `PayerUserId` / `PayeeUserId` | `uniqueidentifier` | |
| `Amount` | `decimal(18,2)` | |
| `Currency` | `nvarchar(3)` | |
| `Status` | `int` | `PaymentStatus` |
| `PaymentMethod` | `nvarchar(50)` | |
| `TransactionReference` | `nvarchar(200)` NULL | |
| `PaidAt` | `datetime2` NULL | |
| `FailureReason` | `nvarchar(max)` NULL | |
| Soft-delete / audit | via `BaseEntity` | Includes `CreatedAt`, `UpdatedAt` |

**Gap:** No attempt history; cannot safely model abandon/retry/switch gateway/delayed webhooks.

## 2. Target model

```
ServiceRequests 1 ── 0..1 BookingPayments 1 ── * BookingPaymentAttempts
```

## 3. Additive changes

### 3.1 `PaymentStatus` enum (append only)

| Int | Name | Action |
|-----|------|--------|
| 1–6 | Existing | **Unchanged** |
| 7 | `AwaitingGatewayConfirmation` | Add |
| 8 | `Expired` | Add |

### 3.2 Alter `BookingPayments`

| Column | Type | Null | Default |
|--------|------|------|---------|
| `PaymentProvider` | `nvarchar(50)` | NO | `'Development'` |
| `CurrentAttemptId` | `uniqueidentifier` | YES | NULL |
| `GatewaySessionId` | `nvarchar(200)` | YES | NULL |
| `GatewayTransactionId` | `nvarchar(200)` | YES | NULL |
| `FailedAt` | `datetime2` | YES | NULL |

Keep `PaidAt`, `FailureReason`, `TransactionReference`, `CreatedAt`.

### 3.3 Create `BookingPaymentAttempts`

| Column | Type | Null |
|--------|------|------|
| `Id` | `uniqueidentifier` | NO (PK) |
| `BookingPaymentId` | `uniqueidentifier` | NO (FK → BookingPayments) |
| `PaymentProvider` | `nvarchar(50)` | NO |
| `AttemptNumber` | `int` | NO |
| `GatewaySessionId` | `nvarchar(200)` | YES |
| `GatewayTransactionId` | `nvarchar(200)` | YES |
| `Status` | `int` | NO |
| `Amount` | `decimal(18,2)` | NO |
| `Currency` | `nvarchar(3)` | NO |
| `RequestDate` | `datetime2` | NO |
| `CompletedDate` | `datetime2` | YES |
| `FailedDate` | `datetime2` | YES |
| `FailureReason` | `nvarchar(1000)` | YES |
| `WebhookEventId` | `nvarchar(200)` | YES |
| `CreatedAt` | `datetime2` | NO |
| `UpdatedAt` | `datetime2` | YES |
| Soft-delete columns | as `BaseEntity` | For query-filter consistency |

### 3.4 Indexes

| Name | Keys | Unique | Filter |
|------|------|--------|--------|
| `UX_BookingPaymentAttempts_Payment_AttemptNumber` | `(BookingPaymentId, AttemptNumber)` | Yes | — |
| `UX_BookingPaymentAttempts_WebhookEventId` | `WebhookEventId` | Yes | `WHERE WebhookEventId IS NOT NULL` |
| `IX_BookingPaymentAttempts_GatewayTransactionId` | `GatewayTransactionId` | No | `WHERE GatewayTransactionId IS NOT NULL` |
| `UX_BookingPayments_TransactionReference` | `TransactionReference` | Yes | `WHERE TransactionReference IS NOT NULL` |
| FK index | `BookingPaymentId` | No | — |
| Optional FK | `CurrentAttemptId` → Attempts | No | — |

### 3.5 Business constraints (app-enforced + partial unique where possible)

| Rule | Enforcement |
|------|-------------|
| Only one Completed attempt per payment | App check on finalize; optional filtered unique `(BookingPaymentId)` WHERE `Status = Completed` |
| Completed payment not overwritten | Reject finalize if header already Completed |
| Idempotent webhooks | Unique `WebhookEventId` |

## 4. Data backfill

1. Add header columns with defaults.  
2. Backfill `PaymentProvider`: `Development` if `TransactionReference LIKE 'KHD-%'`; else `Moyasar` when reference present; else `Development`.  
3. For each existing payment, insert attempt `#1` copying amount/currency/status/reference into session/transaction fields as best-effort; set `RequestDate = CreatedAt`.  
4. Set `CurrentAttemptId` to that attempt.  
5. Create indexes after duplicate cleanup on `TransactionReference` / `WebhookEventId`.

## 5. Rollback

| Scenario | Action |
|----------|--------|
| App rollback | Redeploy prior API; leave additive schema |
| Hard down | Drop FK `CurrentAttemptId`, drop `BookingPaymentAttempts`, drop new header columns — only if no dependency |
| Data | Never delete historical obligation rows |

## 6. Migration name

`AddBookingPaymentAttempts`

Apply order: Dev → Staging → Production (with Phase A code that understands attempts).
