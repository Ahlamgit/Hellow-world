# KHADAMATI Identity & Authentication Module

Enterprise-grade identity module with permission-based RBAC, SQL Server, JWT, and full audit trail.

## Architecture

```
Domain          → Entities (User, Role, Permission, Session, LoginHistory, SecurityLog)
Application     → DTOs, Validators, MediatR Commands, Permission Authorization
Infrastructure  → Repositories, Services, Email/SMS Providers, EF Core
API             → AuthController, SessionsController, ProfileController, LoginHistoryController
```

## Roles (9)

| Role | Self-Register |
|------|---------------|
| SuperAdmin | No |
| Admin | No |
| SupportAgent | No |
| Moderator | No |
| Customer | Yes |
| Craftsman | Yes |
| StoreOwner | Yes |
| StoreEmployee | No |
| Accountant | No |

**Migration:** `Administrator` → `Admin`, `Store` → `StoreOwner`

## Permission-Based Authorization

Every protected endpoint uses `[HasPermission("Permission.Code")]` — no hardcoded role names in controllers.

Example permissions: `Users.Create`, `Bookings.Approve`, `Payments.View`, `Sessions.Revoke`

JWT includes: `roles[]`, `permission[]`, `email_verified`

## Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register Customer/Craftsman/StoreOwner |
| POST | `/api/v1/auth/login` | Login with RememberMe + device info |
| POST | `/api/v1/auth/refresh` | Refresh token rotation |
| POST | `/api/v1/auth/revoke` | Logout current device |
| POST | `/api/v1/auth/forgot-password` | Request reset email |
| POST | `/api/v1/auth/reset-password` | Reset with token |
| POST | `/api/v1/auth/change-password` | Change password (revokes all sessions) |
| POST | `/api/v1/auth/verify-email` | Verify email token |
| POST | `/api/v1/auth/resend-email-verification` | Resend verification |
| POST | `/api/v1/auth/admin/verify-email` | Admin manual verification |
| POST | `/api/v1/auth/phone/send-otp` | Send SMS OTP |
| POST | `/api/v1/auth/phone/verify-otp` | Verify phone OTP |
| GET | `/api/v1/auth/me` | Current user + roles + permissions |
| GET | `/api/v1/auth/permissions` | Current user permissions |

## Session Management

| Method | Endpoint | Permission |
|--------|----------|------------|
| GET | `/api/v1/sessions` | `Sessions.View` |
| DELETE | `/api/v1/sessions/{id}` | Owner or `Sessions.Revoke` |
| DELETE | `/api/v1/sessions/others` | Current user |
| DELETE | `/api/v1/sessions` | `Sessions.RevokeAll` |

Sessions track: device ID, name, platform, browser, IP, login time, last activity, logout time.

## Profile

| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/profile` |
| PUT | `/api/v1/profile` |

Profile fields: name, gender, birth date, nationality, address, GPS, language, timezone, profile picture.

## Login History

| Method | Endpoint | Permission |
|--------|----------|------------|
| GET | `/api/v1/login-history` | `LoginHistory.View` |

## Email Verification Policy

- Login allowed before email verification
- Sensitive actions require verified email (subscriptions, ads, payments, store creation)
- Admin can manually verify via `POST /auth/admin/verify-email`

## Security Features

- BCrypt password hashing (work factor 12)
- Configurable account lockout (`Auth:MaxFailedLoginAttempts`, `Auth:LockoutDurationMinutes`)
- Password policy with history (`Auth:PasswordPolicy`)
- JWT access tokens (15 min) + refresh tokens (7/30 days)
- Refresh token rotation
- Multi-device sessions
- Login history + security logs + audit logs
- Rate limiting on auth endpoints

## Providers (configurable via appsettings)

**Email:** `Development` | `Smtp` | `SendGrid`  
**SMS:** `Development` | `Twilio`

```json
{
  "Email": { "Provider": "Development" },
  "Sms": { "Provider": "Development" },
  "Auth": {
    "MaxFailedLoginAttempts": 5,
    "LockoutDurationMinutes": 15,
    "PasswordPolicy": {
      "MinLength": 8,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequireDigit": true,
      "RequireSpecialChar": true,
      "HistoryCount": 5
    }
  }
}
```

## Database Tables

- `Roles`, `UserRoles`, `Permissions`, `RolePermissions`, `UserPermissions`
- `LoginHistory`, `SecurityLogs`, `PasswordHistory`
- Extended: `Users`, `UserProfiles`, `RefreshTokens`

Deploy: `sqlcmd -i src/database/012_IdentityModule.sql`  
EF Migration: `IdentityModuleV2`

## Default Admin

- Email: `admin@khadamati.com`
- Password: `Admin@123456`
- Role: `SuperAdmin` (all permissions)

## Tests

```bash
cd src/backend && dotnet test
```

52 unit tests covering auth, permissions, password policy, validators, and security services.
