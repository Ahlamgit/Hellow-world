# Security Review Checklist (Final)

**Document ID:** KHAD-V1-SEC-FINAL  

---

## Authentication

| Control | Status |
|---------|--------|
| Admin MFA required | ✓ Required |
| Admin web-only / no mobile admin APIs | ✓ |
| OTP security (hash, TTL, rate limit) | ✓ Designed |
| Password hashing (Argon2id/BCrypt) | ✓ (choose at impl) |
| Refresh rotation + reuse detection | ✓ |
| Account lockout | ✓ |
| Session security (audience claims) | ✓ |

## Authorization

| Control | Status |
|---------|--------|
| RBAC matrix | ✓ Documented |
| Finance-only money policies | ✓ |
| Resource scoping | ✓ |
| API method security | ✓ |

## Data

| Control | Status |
|---------|--------|
| TLS in transit | ✓ |
| PII minimization in logs | ✓ |
| KYC encryption at rest | Required (provider config) |
| Account deletion/anonymization | ADR-014 (retention values blocked) |

## Payments

| Control | Status |
|---------|--------|
| Payment.js only — no PAN/CVV to KHADAMATI | ✓ |
| Server-side amount authority | ✓ |
| Webhook signature + idempotency | ✓ |
| Reconciliation | ✓ Worker required |
| Ledger immutability | ✓ |

## Operations

| Control | Status |
|---------|--------|
| Audit logs (esp. policy changes) | ✓ |
| Rate limiting (Redis) | ✓ |
| Monitoring/alerting | ✓ Designed |
| Dependency/secret scanning in CI | ✓ Required |
| Pen test before launch | Recommended gate |

---

## Residual Risks

- Provider (OCR/Face/SMS) security reviews pending vendor choice  
- Chat abuse without transport spike (ADR-015)  
- Retention durations not numerically set (ADR-014)  
