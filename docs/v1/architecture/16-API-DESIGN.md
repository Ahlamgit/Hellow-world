# 16. API Design

**Document ID:** KHAD-V1-API  
**Status:** Draft for Approval  
**Style:** REST · JSON · OpenAPI 3.1 · JWT Bearer (web cookie mode optional)  

---

## 16.1 Cross-Cutting API Rules

| Rule | Detail |
|------|--------|
| Base URL | `/api/v1` |
| Content type | `application/json` |
| Auth | `Authorization: Bearer <access_token>` (except auth/webhooks) |
| Errors | RFC 7807 Problem Details |
| Pagination | `page`, `size`, `sort` → `{ items, page, size, totalElements, totalPages }` |
| Filtering | explicit query params per resource |
| Idempotency | header `Idempotency-Key` on creates for money/booking |
| Correlation | accept/return `X-Correlation-Id` |
| Time | ISO-8601 UTC timestamps |
| Money | `{ amountMinor: number, currencyCode: string }` |
| Locale | `Accept-Language` |

## 16.2 Error Shape

```json
{
  "type": "https://khadamati.example/problems/validation",
  "title": "Validation failed",
  "status": 400,
  "detail": "scheduledStart must be in the future",
  "code": "BOOKING_INVALID_SCHEDULE",
  "correlationId": "…",
  "errors": [{ "field": "scheduledStart", "message": "…" }]
}
```

## 16.3 Auth APIs

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/v1/auth/register/customer` | Customer registration |
| POST | `/api/v1/auth/register/craftsman` | Craftsman registration |
| POST | `/api/v1/auth/login` | Login (**requires `clientId`/`audience`; admin roles allowed only for `admin-web`**) |
| POST | `/api/v1/auth/refresh` | Refresh tokens (preserves/validates audience) |
| POST | `/api/v1/auth/logout` | Revoke refresh |
| POST | `/api/v1/auth/password/forgot` | Start reset (**policy Q-AUTH-004**) |
| POST | `/api/v1/auth/password/reset` | Complete reset |

**Audience enforcement:** Login body (or header) must include client audience: `admin-web` | `store-web` | `customer-app` | `craftsman-app`. If the authenticated user has admin roles and audience ≠ `admin-web`, respond `403` with code `AUTH_ADMIN_WEB_ONLY`.

## 16.4 Customer APIs (representative)

| Method | Path | Description |
|--------|------|-------------|
| GET/PUT | `/api/v1/customers/me` | Profile |
| CRUD | `/api/v1/customers/me/addresses` | Addresses |
| GET | `/api/v1/categories` | Browse categories |
| GET | `/api/v1/services` | Search services |
| GET | `/api/v1/stores` | Browse stores |
| GET | `/api/v1/stores/{id}` | Store detail |
| POST | `/api/v1/bookings` | Create booking |
| GET | `/api/v1/bookings` | My bookings |
| GET | `/api/v1/bookings/{id}` | Booking detail |
| POST | `/api/v1/bookings/{id}/cancel` | Cancel |
| POST | `/api/v1/payments/intents` | Create payable intent |
| POST | `/api/v1/payments/{id}/debit` | Debit with Payment.js token |
| GET | `/api/v1/payments/{id}` | Payment status |
| POST | `/api/v1/bookings/{id}/ratings` | Rate/review |
| GET | `/api/v1/notifications/in-app` | Inbox |
| POST | `/api/v1/notifications/in-app/{id}/read` | Mark read |

## 16.5 Craftsman APIs (representative)

| Method | Path | Description |
|--------|------|-------------|
| GET/PUT | `/api/v1/craftsmen/me` | Profile |
| CRUD | `/api/v1/craftsmen/me/services` | Catalog |
| GET | `/api/v1/craftsmen/me/onboarding` | Onboarding state |
| POST | `/api/v1/craftsmen/me/onboarding/documents` | Upload metadata + media |
| POST | `/api/v1/craftsmen/me/onboarding/submit` | Submit application |
| GET | `/api/v1/craftsmen/me/jobs` | Assigned/accepted jobs |
| POST | `/api/v1/craftsmen/me/jobs/{bookingId}/gps-checks` | GPS verification |
| POST | `/api/v1/craftsmen/me/jobs/{bookingId}/face-checks` | Selfie verification |
| POST | `/api/v1/craftsmen/me/jobs/{bookingId}/challenges/verify` | QR/OTP |
| POST | `/api/v1/craftsmen/me/jobs/{bookingId}/progress` | Progress update |
| GET | `/api/v1/subscription-plans` | Plans |
| POST | `/api/v1/craftsmen/me/subscriptions` | Subscribe |
| GET | `/api/v1/craftsmen/me/earnings` | Earnings |
| GET | `/api/v1/craftsmen/me/kpis` | KPIs |
| POST | `/api/v1/craftsmen/me/withdrawals` | Withdrawal request |
| GET | `/api/v1/craftsmen/me/withdrawals` | Withdrawal history |

## 16.6 Store APIs (representative)

| Method | Path | Description |
|--------|------|-------------|
| GET/PUT | `/api/v1/store/me` | Store profile |
| CRUD | `/api/v1/store/listings` | Service listings (**no products**) |
| CRUD | `/api/v1/store/staff` | Provider/staff affiliations |
| GET/PATCH | `/api/v1/store/bookings` | Bookings |
| GET | `/api/v1/store/promotions` | Promotion / featured status (admin-managed) |
| GET | `/api/v1/store/reports/*` | Analytics |

> Product/order commerce endpoints are **out of scope** (ADR-002).

## 16.7 Admin APIs (representative)

Prefix `/api/v1/admin`

| Area | Paths |
|------|-------|
| Users | `/users`, `/users/{id}/roles`, `/users/{id}/status` |
| Customers/Craftsmen/Stores | `/customers`, `/craftsmen`, `/stores` |
| Onboarding | `/craftsmen/onboarding`, `/craftsmen/onboarding/{id}/approve\|reject\|request-info` |
| Commissions | `/commission-rules`, history, activate |
| Cancellation policies | `/cancellation-policies`, history |
| Refund rules | `/refund-rules`, history |
| Withdrawals config | `/withdrawal-methods`, `/withdrawal-configs`, history |
| Withdrawals ops | `/withdrawals/{id}/approve\|reject` |
| Settlements | `/settlement-rules`, `/settlements/overview`, batches |
| Subscriptions | `/subscription-plans` |
| Notifications | `/notification-templates`, `/notification-deliveries` |
| Ads | `/ads/campaigns`, `/ads/placements` |
| Ratings | `/ratings`, moderation actions |
| Settings | `/settings`, `/feature-flags` |
| Audit | `/audit-events` |
| Analytics/Reports | `/analytics/*`, `/reports/*` |
| Quality | `/quality/rules`, `/quality/scores`, `/restrictions` |

## 16.8 Webhooks

| Method | Path | Auth |
|--------|------|------|
| POST | `/api/v1/webhooks/areeba-ixopay` | Gateway signature/shared secret |

Must be idempotent on gateway transaction/event id.

## 16.9 Media

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/v1/media/upload-requests` | Returns presigned upload URL |
| GET | `/api/v1/media/{id}` | Metadata / authorized download redirect |

## 16.10 OpenAPI Delivery

- Springdoc OpenAPI UI at `/swagger-ui` (non-prod or protected in prod — Q-API-001)  
- Published `openapi.yaml` artifact for client generation  

## 16.11 Versioning & Compatibility

- URL version `/v1`  
- Additive changes preferred  
- Breaking changes require `/v2`  

## 16.12 Questions Requiring Business Decision

- Q-AUTH-004 password reset channels  
- Q-API-001 public swagger in production?  
- Resource grouping style (role-prefixed vs shared resources) can be finalized at implementation kickoff without changing domain model
