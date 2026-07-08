# KHADAMATI Authentication Module

Complete SQL Server + JWT authentication with BCrypt password hashing.

## Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/v1/auth/register` | Anonymous | Register Customer, Craftsman, or Store |
| POST | `/api/v1/auth/login` | Anonymous | Login with optional `rememberMe` |
| POST | `/api/v1/auth/refresh` | Anonymous | Refresh JWT access token |
| POST | `/api/v1/auth/revoke` | Bearer | Logout / revoke refresh token |
| POST | `/api/v1/auth/forgot-password` | Anonymous | Request password reset email |
| POST | `/api/v1/auth/reset-password` | Anonymous | Reset password with token |
| POST | `/api/v1/auth/change-password` | Bearer | Change password (revokes sessions) |
| POST | `/api/v1/auth/verify-email` | Anonymous | Verify email with token |
| POST | `/api/v1/auth/resend-email-verification` | Anonymous | Resend verification email |
| POST | `/api/v1/auth/phone/send-otp` | Bearer | Send 6-digit SMS OTP |
| POST | `/api/v1/auth/phone/verify-otp` | Bearer | Verify phone with OTP |
| GET | `/api/v1/auth/me` | Bearer | Current user from JWT |

## Security Features

- **BCrypt** password hashing (work factor 12)
- **JWT** access tokens (15 min default)
- **Refresh tokens** stored in SQL Server (7 days, 30 days with Remember Me)
- **Email verification** tokens (24h expiry, BCrypt-hashed)
- **Password reset** tokens (1h expiry, BCrypt-hashed)
- **Phone OTP** (6 digits, 10 min expiry, BCrypt-hashed, rate limited)
- **Account lockout** after 5 failed login attempts (15 min)
- **Role-based authorization** policies: `CustomerOnly`, `CraftsmanOnly`, `StoreOnly`, `AdminOnly`

## Roles

| Role | Self-Registration | Policy |
|------|-------------------|--------|
| Customer | Yes | `CustomerOnly` |
| Craftsman | Yes | `CraftsmanOnly` |
| Store | Yes | `StoreOnly` |
| Administrator | No (seed only) | `AdminOnly` |

## Configuration (`appsettings.json`)

```json
{
  "Jwt": {
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7,
    "RememberMeRefreshTokenExpirationDays": 30
  },
  "Auth": {
    "EmailVerificationExpirationHours": 24,
    "PasswordResetExpirationHours": 1,
    "OtpExpirationMinutes": 10,
    "MaxOtpAttempts": 5,
    "MaxOtpRequestsPerHour": 5
  }
}
```

## Database Tables

- `EmailVerificationTokens` — email verification workflow
- `PasswordResetTokens` — forgot/reset password workflow
- `PhoneOtpTokens` — phone OTP verification
- `RefreshTokens` — JWT refresh token storage (+ `RememberMe` flag)
- `Users` — `EmailVerifiedAt`, `PhoneVerifiedAt` columns

Deploy: `sqlcmd -i src/database/008_AuthModule.sql`

## Unit Tests

```bash
cd src/backend
dotnet test Khadamati.Tests/Khadamati.Tests.csproj
```

Tests cover validators, BCrypt hashing, OTP service, token service, and AuthService integration.
