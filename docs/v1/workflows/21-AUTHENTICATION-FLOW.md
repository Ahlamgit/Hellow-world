# 21. Authentication Flow

**Document ID:** KHAD-V1-AUTHN  
**Status:** Draft for Approval  

---

## 21.1 Actors & Channels

Customers/Craftsmen via Flutter; Store operators & Admins via React web apps.

## 21.2 Token Model

| Token | Lifetime (suggested) | Storage |
|-------|----------------------|---------|
| Access JWT | 5–15 minutes | Memory (mobile/web) |
| Refresh | Days (Q-AUTH-005) | Secure storage / HttpOnly cookie |

JWT claims (minimum): `sub`, `sid`/`jti`, `roles`, `permissions` (or role only with permission lookup), `typ`, `iat`, `exp`.

Avoid oversized permission lists if claim size becomes an issue — alternative: role in token + permission cache server-side (Q-AUTH-006).

## 21.3 Registration Flow

```mermaid
sequenceDiagram
  participant Client
  participant API
  participant DB
  Client->>API: POST /auth/register/*
  API->>API: validate password policy
  API->>DB: create user + profile
  alt email/phone verification required (Q-AUTH-003)
    API->>Client: verification required
  else immediate session
    API->>DB: persist refresh token hash
    API->>Client: access + refresh
  end
```

## 21.4 Login Flow

1. Client submits identifier + password (+ device metadata)  
2. API checks status/lockout  
3. Verify password hash  
4. Issue access + refresh (new family)  
5. Record successful login audit/security event  

Failure increments attempt counter; threshold locks account temporarily.

## 21.5 Refresh Flow (Rotation)

1. Client sends refresh token  
2. API validates hash, expiry, revocation  
3. Invalidate old refresh  
4. Issue new access + refresh  
5. If old refresh reused → revoke entire family (possible theft)

## 21.6 Logout Flow

Revoke current refresh (or all sessions if “logout everywhere”).

## 21.7 Password Reset (**POLICY-GATED**)

Depends on Q-AUTH-004 (email link vs SMS OTP). Architecture supports both via notification module.

## 21.8 Mobile vs Web Differences

| Topic | Mobile | Web |
|-------|--------|-----|
| Refresh storage | flutter_secure_storage | HttpOnly cookie preferred (Q-FE-002) |
| Biometric unlock | Optional OS biometric to unlock app session | N/A |
| MFA | Optional | Strongly recommended for admin |

## 21.9 Questions Requiring Business Decision

`Q-AUTH-001` identifier, `Q-AUTH-002` MFA, `Q-AUTH-003` verification before session, `Q-AUTH-004` reset channel, `Q-AUTH-005` refresh TTL, `Q-AUTH-006` permissions in JWT vs lookup
