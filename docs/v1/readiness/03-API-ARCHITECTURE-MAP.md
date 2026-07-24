# API Architecture Map (Final)

**Document ID:** KHAD-V1-API-FINAL  
**Base:** `/api/v1` · Auth: JWT Bearer · Errors: RFC 7807 · Money: `{ amountMinor, currencyCode }`  
**Admin APIs:** audience `admin-web` only · MFA session required  

Common headers: `Authorization`, `Idempotency-Key` (money/booking creates), `X-Correlation-Id`, `Accept-Language`.

---

## 1. Authentication APIs

| API | Purpose | Authz | Notes |
|-----|---------|-------|-------|
| `POST /auth/register/customer` | Customer register | Public | OTP may follow |
| `POST /auth/register/craftsman` | Craftsman register | Public | |
| `POST /auth/otp/request` | Send OTP | Public | Rate-limited |
| `POST /auth/otp/verify` | Verify OTP / login | Public | |
| `POST /auth/login` | Password login | Public | Body includes `audience` |
| `POST /auth/mfa/verify` | Admin MFA | Partial | Admin web |
| `POST /auth/refresh` | Rotate tokens | Refresh | Preserves audience |
| `POST /auth/logout` | Revoke | Auth | |
| `POST /auth/password/forgot\|reset` | Reset | Public | Store/Admin primarily |

**Validation:** audience ∈ {`customer-app`,`craftsman-app`,`store-web`,`admin-web`}; admin roles ⇒ audience must be `admin-web` else `AUTH_ADMIN_WEB_ONLY`.

**No mobile admin authentication or admin routes.**

---

## 2. Marketplace APIs

| API | Purpose | Role | Validation |
|-----|---------|------|------------|
| `GET /categories` | Browse categories | Public/Auth | market from header/token |
| `GET /listings` | Search/filter/proximity | Public/Auth | q, category, near, price, sort, cursor |
| `GET /listings/{id}` | Listing detail | Public/Auth | |
| `GET /providers/{id}` | Provider public profile | Public/Auth | |
| `GET /markets/current` | Active market config | Public | |

**Errors:** 400 validation, 404 missing, 429 rate limit.

---

## 3. Booking APIs

| API | Purpose | Role |
|-----|---------|------|
| `POST /bookings` | Create request | Customer |
| `GET /bookings` | List mine | Customer / Provider scoped |
| `GET /bookings/{id}` | Detail | Owner parties / Admin |
| `POST /bookings/{id}/accept` | Provider confirm | Craftsman/Store |
| `POST /bookings/{id}/reject` | Provider reject | Craftsman/Store |
| `POST /bookings/{id}/cancel` | Cancel (policy-evaluated) | Customer/Provider/Admin |
| `GET /bookings/{id}/cancellation-preview` | Show policy outcome | Same |
| `POST /payments/intents` | Start pay after accept | Customer |
| `POST /payments/{id}/debit` | Payment.js token debit | Customer |
| `GET /payments/{id}` | Payment status | Customer |
| `POST /bookings/{id}/progress` | Job progress / verification hooks | Provider |
| `POST /bookings/{id}/complete` | Provider completion | Provider |
| `POST /bookings/{id}/confirm-completion` | Customer confirm | Customer |
| `POST /bookings/{id}/ratings` | Review/rating | Customer |

**Authorization:** resource scoping by customer_id / provider_id / store.  
**Validation:** state machine transitions; Idempotency-Key on create/pay.  
**Errors:** `BOOKING_INVALID_TRANSITION`, `CANCEL_NOT_ALLOWED`, `ENTITLEMENT_*`.

---

## 4. Financial APIs

| API | Purpose | Role |
|-----|---------|------|
| `POST /webhooks/areeba-ixopay` | Gateway callback | Signature auth |
| `GET /craftsmen/me/earnings` | Earnings projection | Craftsman |
| `POST /craftsmen/me/withdrawals` | Withdrawal request | Craftsman |
| `GET /craftsmen/me/withdrawals` | History | Craftsman |
| `GET /admin/ledger/entries` | Ledger query | Finance/Super |
| `POST /admin/withdrawals/{id}/approve\|reject` | Payout decision | Finance/Super |
| `POST /admin/settlements/run` | Settlement processing | Finance/Super |
| Commission calc | Internal on domain events | System |

Internal commission calculation is **not** a public mobile API; triggered by completion events using admin rules.

---

## 5. Provider / Craftsman / Store APIs (excerpt)

Craftsman: profile, listings, availability, onboarding docs, jobs, GPS/face/OTP verify, subscriptions, KPIs, notifications, chat.  
Store: profile, listings, staff affiliations, bookings, promotion status, analytics.  
**No product/order APIs.**

---

## 6. Admin APIs (Web Only)

| Area | Paths | Roles |
|------|-------|-------|
| Users/Customers/Providers | `/admin/customers`, `/craftsmen`, `/stores`, `/users` | Support/Super |
| Onboarding/IDV | `/admin/onboarding/*`, `/verifications/*` | Ops/Super |
| Commission config | `/admin/commission-rules` + history | Finance/Super |
| Cancellation policies | `/admin/cancellation-policies` + history | Finance/Super |
| Refund rules | `/admin/refund-rules` + history | Finance/Super |
| Withdrawal config | `/admin/withdrawal-methods`, `/withdrawal-configs` | Finance/Super |
| Settlement rules | `/admin/settlement-rules` | Finance/Super |
| Subscriptions | `/admin/subscription-plans`, `/subscriptions` | Finance/Super/Content |
| Promotions | `/admin/ads/*`, `/promotions/*` | Content/Super |
| Notifications | `/admin/notification-templates` | Content/Super |
| Reports/Analytics | `/admin/analytics/*`, `/reports/*` | Support+/Finance |
| Audit | `/admin/audit-events` | Super/Support limited |
| Settings | `/admin/settings` | Super |
| Restrictions/Ratings | `/admin/restrictions`, `/ratings` | Ops/Super |

**Every policy write:** MFA session + permission + audit before/after.

### Example contract shape (policy create)

**Request:** rule criteria + outcomes + effective_from/to + active  
**Response:** rule id, version, created_at  
**Authz:** `commission_rules:write`  
**Errors:** 400 validation, 403 forbidden, 409 overlapping priority conflict (if enforced)

---

## 7. Chat APIs (V1 scope)

`GET/POST /chat/conversations`, `GET/POST /chat/conversations/{id}/messages`, `POST .../read` — participants only. Transport per ADR-015.

---

## 8. Error Handling Standard

```json
{
  "type": "https://khadamati.example/problems/...",
  "title": "...",
  "status": 403,
  "detail": "...",
  "code": "AUTH_ADMIN_WEB_ONLY",
  "correlationId": "..."
}
```
