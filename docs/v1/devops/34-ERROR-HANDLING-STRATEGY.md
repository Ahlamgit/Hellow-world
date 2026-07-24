# 34. Error Handling Strategy

**Document ID:** KHAD-V1-ERR  
**Status:** Draft for Approval  

---

## 34.1 API Errors

All errors return **RFC 7807 Problem Details** with stable `code`.

| HTTP | When |
|------|------|
| 400 | Validation / domain rule violation |
| 401 | Unauthenticated |
| 403 | Unauthorized / entitlement denied |
| 404 | Resource not found (or hidden) |
| 409 | Conflict / invalid state transition / idempotency mismatch |
| 422 | Semantic persistence issues if distinguished from 400 |
| 429 | Rate limited |
| 500 | Unexpected |
| 502/503 | Upstream provider failure surfaced carefully |

## 34.2 Exception Taxonomy

```text
DomainException (mapped to 4xx + code)
  ├─ NotFoundException
  ├─ ConflictException
  ├─ ValidationException
  ├─ EntitlementException
  └─ PaymentException
InfrastructureException (5xx or 502)
Unexpected Exception → 500 generic detail
```

## 34.3 Client Guidance

- Mobile/web map `code` to localized messages  
- Never show raw stack traces  
- Payment `REQUIRES_ACTION` is not an error — it’s a state  

## 34.4 Idempotency & Partial Failures

- Prefer state machine recovery over compensating blindly  
- Notification failures don’t fail primary transaction after commit  
- Webhook processing must tolerate duplicates  

## 34.5 Worker Errors

- Retryable vs non-retryable classification  
- Dead-letter + admin alert for poison messages  

## 34.6 Questions

Which 404 vs 403 policy for cross-tenant reads — recommend hide-as-404 for customer data; document final choice in security ADR at kickoff.
