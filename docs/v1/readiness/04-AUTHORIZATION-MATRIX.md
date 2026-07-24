# Authorization Matrix (RBAC Final)

**Document ID:** KHAD-V1-RBAC-FINAL  
**Status:** Architecture Ready  
**Special:** Admin = Web Portal only + MFA + Audit  

---

## Roles

| Role | Channel | MFA |
|------|---------|-----|
| Customer | Customer Flutter app | No (OTP auth) |
| Craftsman | Craftsman Flutter app | No (OTP; step-up later optional) |
| Store Operator | Store Dashboard web | Recommended |
| Support Admin | Admin Portal web | **Required** |
| Content Admin | Admin Portal web | **Required** |
| Finance Admin | Admin Portal web | **Required** |
| Ops Admin (onboarding/quality) | Admin Portal web | **Required** |
| Super Admin | Admin Portal web | **Required** |

---

## Matrix (Selected Actions)

| Action | Customer | Craftsman | Store | Support | Content | Finance | Ops | Super |
|--------|:--------:|:---------:|:-----:|:-------:|:-------:|:-------:|:---:|:-----:|
| Register/login own channel | ✓ | ✓ | ✓ | | | | | |
| Browse listings/search | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Create booking | ✓ | | | | | | | |
| Accept/reject booking | | ✓ | ✓ | | | | | |
| Pay for booking | ✓ | | | | | | | |
| Field verification (GPS/selfie/QR) | | ✓ | ✓* | | | | | |
| Complete / confirm completion | ✓/✓ | ✓ | ✓ | | | | | |
| Rate/review | ✓ | | | | | | | |
| Chat on booking | ✓ | ✓ | ✓ | | | | | |
| Manage own listings | | ✓ | ✓ | | | | | |
| Request withdrawal | | ✓ | ✓** | | | | | |
| Manage promotions config | | | read | | ✓ | | | ✓ |
| Approve onboarding | | | | | | | ✓ | ✓ |
| Moderate ratings | | | | ✓ | | | ✓ | ✓ |
| Notification templates | | | | | ✓ | | | ✓ |
| **Commission rules CRUD** | | | | | | ✓ | | ✓ |
| **Cancellation policies CRUD** | | | | | | ✓ | | ✓ |
| **Refund rules CRUD** | | | | | | ✓ | | ✓ |
| **Withdrawal config CRUD** | | | | | | ✓ | | ✓ |
| **Settlement rules CRUD** | | | | | | ✓ | | ✓ |
| Approve withdrawals | | | | | | ✓ | | ✓ |
| Run settlements | | | | | | ✓ | | ✓ |
| View ledger | | | | limited | | ✓ | | ✓ |
| Global settings | | | | | | | | ✓ |
| Audit logs | | | | limited | | ✓ | ✓ | ✓ |
| Admin mobile login | ✗ | ✗ | ✗ | ✗ | ✗ | ✗ | ✗ | ✗ |

\* If store-affiliated staff performs jobs.  
\** If store payouts enabled by config.

---

## Data Visibility

| Role | Sees |
|------|------|
| Customer | Own profile, bookings, payments, chats, notifications |
| Craftsman | Own profile, jobs, earnings, subscriptions |
| Store | Own store listings, bookings, staff, analytics |
| Support | User/provider profiles (PII minimization), bookings read |
| Finance | Financial configs, ledger, withdrawals, settlements, reports |
| Content | Templates, promotions |
| Ops | Onboarding dossiers, IDV, restrictions |
| Super | All |

---

## Restricted Actions (All Non-Finance)

Cannot change commission/cancel/refund/withdrawal/settlement policies without Finance or Super role.

---

## Enforcement

1. JWT audience + roles/permissions  
2. Method security on admin routes  
3. Resource scoping in application services  
4. Audit every policy and money approval mutation  
