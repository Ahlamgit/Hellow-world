# KHADAMATI — Database Object Inventory (Phase 0.5)

**Document version:** 1.0  
**Date:** 2026-07-22  
**Sources:** EF migrations, `src/database/*.sql`

**Legend:** ✅ Yes · ❌ No · ⚠️ Partial/Drift · — N/A

---

## 1. Tables (44 EF + 10 SQL-only/legacy)

### 1.1 EF Tables (Canonical)

| Table | EF | SQL Scripts | App Used | Duplicate | Obsolete | Requires Migration |
|-------|----|-------------|----------|-----------|----------|-------------------|
| ActivityLogs | ✅ | 011+ | ✅ AdminService | — | — | No |
| Addresses | ✅ | 002+ | ✅ | — | — | No |
| Advertisements | ✅ | 011+ | ✅ | — | — | No |
| AuditLogs | ✅ | 002 (audit.*) | ⚠️ | ⚠️ vs audit.AuditLogs | SQL audit schema obsolete | Reconcile audit strategy |
| BackupJobs | ✅ | 011+ | ✅ | — | — | No |
| BookingPayments | ✅ | ⚠️ as Payments | ✅ | ⚠️ Name mismatch | SQL Payments obsolete | Align SQL views or drop scripts |
| BookingSlotReservations | ✅ | 010+ | ✅ | — | — | No |
| ChatConversations | ✅ | ❌ | ✅ | — | — | Add SQL or EF-only doc |
| ChatMessages | ✅ | ❌ | ✅ | — | — | Add SQL or EF-only doc |
| Cities | ✅ | 002+ | ✅ | — | — | No |
| Complaints | ✅ | 011+ | ✅ | — | — | No |
| Coupons | ✅ | 011+ | ✅ | — | — | No |
| CraftsmanProfiles | ✅ | 002+ | ✅ | — | — | No |
| CraftsmanServices | ✅ | 002+ | ✅ | — | — | No |
| CraftsmanWorkingHours | ✅ | 010+ | ✅ | — | — | No |
| DevicePushTokens | ✅ | ❌ | ✅ | — | — | EF-only |
| EmailVerificationTokens | ✅ | 008+ | ✅ | — | — | No |
| LoginHistory | ✅ | 012+ | ✅ | — | — | No |
| Notifications | ✅ | 002+ | ✅ | — | — | No |
| PasswordHistory | ✅ | 012+ | ✅ | — | — | No |
| PasswordResetTokens | ✅ | 008+ | ✅ | — | — | No |
| Permissions | ✅ | 012+ | ✅ | — | — | No |
| PhoneOtpTokens | ✅ | 008+ | ✅ | — | — | No |
| PlanBillingOptions | ✅ | 009+ | ✅ | — | — | No |
| RefreshTokens | ✅ | 008+ | ✅ | — | — | No |
| Regions | ✅ | 011+ | ✅ | — | — | No |
| RolePermissions | ✅ | 012+ | ✅ | — | — | No |
| Roles | ✅ | ⚠️ ref.Roles | ✅ | ⚠️ Different PK model | ref.Roles obsolete | No (EF canonical) |
| SecurityLogs | ✅ | 012+ | ✅ | — | — | No |
| ServiceCategories | ✅ | 002+ | ✅ | — | — | No |
| ServiceRequests | ✅ | 002+ | ✅ | — | — | No |
| ServiceRequestStatusHistories | ✅ | ⚠️ singular name | ✅ | ⚠️ Plural vs singular | SQL name drift | No |
| Services | ✅ | 002+ | ✅ | — | — | No |
| StoreProducts | ✅ | 002+ | ✅ | — | — | No |
| StoreProfiles | ✅ | 002+ | ✅ | — | — | No |
| SubscriptionPlans | ✅ | 009+ | ✅ | — | — | No |
| SupportTickets | ✅ | 011+ | ✅ | — | — | No |
| SystemSettings | ✅ | 011+ | ✅ | — | — | No |
| UserPermissions | ✅ | 012+ | ✅ | — | — | No |
| UserProfiles | ✅ | 002+ | ✅ | — | — | No |
| UserRoles | ✅ | 012+ | ✅ | — | — | No |
| UserSubscriptions | ✅ | 009+ | ✅ | — | — | No |
| Users | ✅ | 002+ | ✅ | — | — | No |
| VerificationDocuments | ✅ | 002+ | ✅ | — | — | No |

### 1.2 SQL-Only Tables (Not in EF)

| Table | EF | SQL | App Used | Status |
|-------|----|-----|----------|--------|
| ref.Roles | ❌ | ✅ | ❌ | **Obsolete** — EF uses dbo.Roles |
| ref.UserStatuses | ❌ | ✅ | ❌ | **Obsolete** — enum on Users |
| ref.VerificationStatuses | ❌ | ✅ | ❌ | **Obsolete** |
| ref.SubscriptionStatuses | ❌ | ✅ | ❌ | **Obsolete** |
| ref.ServiceRequestStatuses | ❌ | ✅ | ❌ | **Obsolete** |
| ref.PaymentStatuses | ❌ | ✅ | ❌ | **Obsolete** |
| dbo.Payments | ⚠️ | ✅ | ❌ | **Obsolete** — renamed BookingPayments |
| dbo.Reviews | ❌ | ✅ | ❌ | **Obsolete** — reviews on ServiceRequests |
| audit.AuditLogs | ⚠️ | ✅ | ❌ | **Obsolete** — EF uses dbo.AuditLogs |

---

## 2. Views

| View | SQL Script | EF | App Used | Status |
|------|------------|----|----------|--------|
| dbo.vw_ActiveUsers | 004 | ❌ | ❌ | **Obsolete** — AdminService uses LINQ |
| dbo.vw_ServiceRequestSummary | 004 | ❌ | ❌ | **Obsolete** |
| dbo.vw_CraftsmanDirectory | 004 | ❌ | ❌ | **Obsolete** — BookingService C# |
| dbo.vw_StoreDirectory | 004 | ❌ | ❌ | **Obsolete** |
| dbo.vw_PaymentSummary | 004 | ❌ | ❌ | **Broken** — references Payments table |
| dbo.vw_DashboardMetrics | 004 | ❌ | ❌ | **Broken** — references Payments/Reviews |

**Recommendation:** Archive views or rewrite against EF schema if raw SQL reporting is needed.

---

## 3. Stored Procedures

| Procedure | SQL Script | EF | App Used | Status |
|-----------|------------|----|----------|--------|
| audit.sp_InsertAuditLog | 005 | ❌ | ❌ | **Obsolete** — AuditService C# |
| dbo.sp_GetNearbyCraftsmen | 005 | ❌ | ❌ | **Obsolete** — BookingService geo query |
| dbo.sp_GetDashboardStats | 005 | ❌ | ❌ | **Obsolete** — AdminService LINQ |
| dbo.sp_SoftDeleteUser | 005 | ❌ | ❌ | **Obsolete** — soft-delete via EF |
| dbo.sp_UpdateServiceRequestStatus | 005 | ❌ | ❌ | **Obsolete** — BookingService |
| dbo.sp_GetUserProfile | 005 | ❌ | ❌ | **Obsolete** — UserQueries/ProfileService |
| dbo.sp_GetUnreadNotifications | 005 | ❌ | ❌ | **Obsolete** — BookingService notifications |
| dbo.sp_CleanupExpiredRefreshTokens | 005 | ❌ | ⚠️ | **Partial** — TokenCleanupService background job |
| dbo.sp_InvalidateUserTokens | 008 | ❌ | ❌ | **Obsolete** — AuthService |

---

## 4. Functions

| Function | SQL Script | EF | App Used | Status |
|----------|------------|----|----------|--------|
| *(none defined)* | — | — | — | All logic in C# application layer |

---

## 5. Triggers

| Trigger | Table | SQL Script | EF | App Used | Status |
|---------|-------|------------|----|----------|--------|
| TR_Users_SetModifiedDate | Users | 006 | ❌ | ❌ | **Obsolete** — EF SaveChanges |
| TR_UserProfiles_SetModifiedDate | UserProfiles | 006 | ❌ | ❌ | **Obsolete** |
| TR_ServiceRequests_SetModifiedDate | ServiceRequests | 006 | ❌ | ❌ | **Obsolete** |
| TR_Users_Audit | Users | 006 | ❌ | ❌ | **Obsolete** — AuditService |
| TR_ServiceRequests_StatusHistory | ServiceRequests | 006 | ❌ | ❌ | **Duplicate** — BookingService writes history |
| TR_Users_SoftDeleteAudit | Users | 006 | ❌ | ❌ | **Obsolete** |

---

## 6. Indexes

See [DATABASE_INDEX_ANALYSIS.md](./DATABASE_INDEX_ANALYSIS.md) for full EF vs SQL index comparison.

**Summary:**

| Source | Count | App relies on |
|--------|-------|---------------|
| EF migrations | ~70 index definitions | ✅ Yes (runtime) |
| SQL 003_CreateIndexes.sql | 28+ indexes | ❌ No (unless SQL path deployed) |

---

## 7. Constraints

### 7.1 EF Unique Constraints

`Users.Email`, `Users.Phone`, `UserProfiles.UserId`, `RefreshTokens.Token`, `CraftsmanProfiles.UserId`, `StoreProfiles.UserId`, `CraftsmanServices.(CraftsmanProfileId, ServiceId)`, `BookingPayments.ServiceRequestId`, `BookingSlotReservations.ServiceRequestId`, filtered slot unique, `ChatConversations.BookingId`, `Coupons.Code`, `Permissions.Code`, `Regions.Code`, `Roles.Name`, `SubscriptionPlans.PlanCode`, `SupportTickets.TicketNumber`, `SystemSettings.SettingKey`, `DevicePushTokens.(UserId, Token)`, `PlanBillingOptions.(PlanId, Cycle)`, `RolePermissions.(RoleId, PermissionId)`, `UserPermissions.(UserId, PermissionId)`, `UserRoles.(UserId, RoleId)`, `ServiceRequests.BookingReference`

### 7.2 SQL CHECK Constraints (Not in EF)

| Constraint | Table | Rule | Requires Migration |
|------------|-------|------|-------------------|
| CK_ServiceRequests_CustomerRating | ServiceRequests | Rating 1–5 or NULL | **Yes** — add to EF if enforcing |
| CK_Reviews_Rating | Reviews | Rating 1–5 | N/A — Reviews table obsolete |
| CK_AuditLogs_Action | audit.AuditLogs | Action enum | N/A — audit schema obsolete |

---

## 8. Master Deploy Script Coverage

| Script | In 000_MasterDeploy | EF Coverage |
|--------|---------------------|-------------|
| 001–007 | ✅ | Partial / drifted |
| 008–013 | ❌ | Covered by EF migrations |

---

## 9. Action Summary

| Action | Objects | Phase |
|--------|---------|-------|
| **Archive** | ref.* tables, dbo.Payments, dbo.Reviews, all views/SPs/triggers | Phase 1 |
| **Document EF-only** | Chat*, DevicePushTokens | Phase 0.5 ✅ |
| **Add EF constraints** | CustomerRating check | Phase 2 |
| **Reconcile audit** | audit.AuditLogs vs dbo.AuditLogs | Phase 2 |
| **Do not deploy** | 000_MasterDeploy.sql on EF databases | Immediate |

---

## 10. Related Documents

- [DATABASE_ER_MODEL.md](./DATABASE_ER_MODEL.md)
- [DATABASE_DEPENDENCY_GRAPH.md](./DATABASE_DEPENDENCY_GRAPH.md)
- [DATABASE_INDEX_ANALYSIS.md](./DATABASE_INDEX_ANALYSIS.md)
