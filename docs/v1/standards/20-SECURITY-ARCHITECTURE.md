# 20. Security Architecture

**Document ID:** KHAD-V1-SEC  
**Status:** Draft for Approval  

---

## 20.1 Security Goals

- Protect authenticity and integrity of users and transactions  
- Minimize PCI scope via Payment.js hosted fields  
- Enforce least-privilege RBAC  
- Provide auditability for sensitive actions  
- Align controls with OWASP Top 10  

## 20.2 Trust Boundaries

```text
Internet Clients → TLS LB → API → DB/Providers
Webhook Providers → TLS LB → Webhook Controllers (signature auth)
Admins → Admin Portal → API (privileged permissions)
```

## 20.3 Authentication

- Password hashing: Argon2id or BCrypt (cost calibrated) — choose Q-SEC-001  
- JWT access tokens (short-lived)  
- Refresh tokens (rotated, hashed at rest)  
- Optional admin MFA (Q-AUTH-002)  
- Account lockout + rate limiting on auth endpoints  
- **Administrators authenticate only via Administration Portal (web). Mobile app admin login is forbidden (BR-008 / Q-AUTH-007 decided).**  
- Login requests must declare client audience; admin roles rejected for `customer-app` / `craftsman-app` / `store-web` audiences  
- Admin API routes additionally require admin-web audience claim (defense in depth)  

## 20.4 Authorization

- Role-based + permission-based checks  
- Resource scoping: store operators limited to own store; customers to own bookings  
- Deny by default  

## 20.5 OWASP Top 10 Mapping (High Level)

| Risk | Control |
|------|---------|
| Broken Access Control | Method security + integration tests |
| Cryptographic Failures | TLS, hashed passwords, no PAN storage |
| Injection | JPA/parameterized SQL, validation |
| Insecure Design | Threat modeling on booking/payment/IDV |
| Security Misconfiguration | Hardened headers, secure defaults, no public swagger in prod without auth |
| Vulnerable Components | Dependency scanning in CI |
| Auth Failures | Lockout, refresh rotation, MFA option |
| Software/Data Integrity | Signed webhooks, immutable audit, CI signing |
| Logging Failures | Structured security event logs without secrets |
| SSRF | Allowlists for provider URLs; no user-controlled fetch to internal nets |

## 20.6 Data Protection

| Data Class | Handling |
|------------|----------|
| Card PAN/CVV | Never touches KHADAMATI servers; Payment.js only |
| Password | Hash only |
| Refresh tokens | Hash only |
| National ID docs | Object storage encrypted; access audited; retention Q-IDV-006 |
| Biometric/selfie | Encrypted storage; retention Q-IDV-006 |
| PII | Minimize in logs; encrypt sensitive fields if required by regulation Q-CMP-001 |

## 20.7 Application Security Controls

- Input validation & output encoding  
- CSRF strategy for cookie-based browser auth (if cookies chosen)  
- CORS allowlist per environment  
- Security headers (CSP, HSTS, X-Content-Type-Options, Referrer-Policy)  
- File upload constraints (type/size) + malware scan candidate Q-SEC-002  
- Secrets via env/secret manager; never in images  

## 20.8 Payment Security

- Public integration key only in client Payment.js init  
- API username/password or gateway credentials only on server  
- Callback authenticity verification  
- Idempotent processing  
- Amount verification server-side (client amount not trusted)

## 20.9 Audit & Monitoring

- Audit privileged admin actions and money movements  
- Alert on repeated auth failures, webhook signature failures, privilege denials spikes  

## 20.10 Secure SDLC

- PR reviews  
- SAST/dependency scan  
- Secret scan  
- Staging penetration test before production launch (Q-SEC-003 scope)

## 20.11 Questions Requiring Business Decision

`Q-SEC-001`, `Q-SEC-002`, `Q-SEC-003`, `Q-AUTH-002`, `Q-CMP-001`, `Q-IDV-006`  

**Decided:** Administrators are web-only (Administration Portal); no Flutter admin login.
