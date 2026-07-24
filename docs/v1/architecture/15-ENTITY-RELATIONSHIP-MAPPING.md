# 15. Entity Relationship Mapping

**Document ID:** KHAD-V1-ERM  
**Status:** Draft for Approval  

Logical entities → suggested tables/fields. Field lists are **architectural**, not final DDL. Business-coded enums awaiting decisions are noted.

---

## 15.1 IAM

### `users`
| Field | Type | Notes |
|-------|------|-------|
| id | UUID | PK |
| email | citext NULL | unique if used |
| phone_e164 | varchar NULL | unique if used |
| password_hash | varchar | |
| status | enum | ACTIVE, LOCKED, DISABLED |
| locale | varchar | |
| preferred_currency | char(3) | readiness |
| region_code | varchar | readiness |
| last_login_at | timestamptz | |
| ...audit cols | | |

### `roles`, `permissions`, `user_roles`, `role_permissions`
Standard RBAC mapping. Permission codes like `commission:write`, `onboarding:approve`.

### `refresh_tokens`
| Field | Notes |
|-------|-------|
| token_hash | store hash only |
| user_id | FK |
| family_id | rotation theft detection |
| expires_at, revoked_at | |

---

## 15.2 Party Profiles

### `customers` (1:1 user)
Profile fields: display_name, default_address_id, marketing_opt_in (Q-CMP-002)

### `customer_addresses`
geo: line1/line2/city/area/country/postal/lat/lon

### `craftsmen` (1:1 user)
| Field | Notes |
|-------|-------|
| onboarding_status | DRAFT, SUBMITTED, NEEDS_INFO, PENDING_APPROVAL, APPROVED, REJECTED, SUSPENDED |
| quality_score | numeric |
| subscription_required | derived via entitlements |

### `stores`
legal_name, trade_name, status, geo, contact, logo_media_id

### `store_users` *
Only if Q-STR-001 = multi-user stores.

---

## 15.3 Catalog

### `categories`
tree via `parent_id`, sort_order, active

### `category_translations`
category_id + locale + name + description

### `craftsman_services` / `store_services` / `products`
pricing as `amount_minor` + `currency_code`; inventory fields for products depend on Q-PRD-001

---

## 15.4 Booking

### `bookings`
| Field | Notes |
|-------|-------|
| customer_id | FK |
| craftsman_id | FK NULL |
| store_id | FK NULL |
| service_ref_type/id | polymorphic or explicit FKs |
| status | state machine |
| scheduled_start/end | timestamptz |
| location snapshot | JSON/cols |
| price_amount_minor/currency | |
| cancellation_reason | |

### `booking_status_history`
booking_id, from_status, to_status, actor_id, reason, at

---

## 15.5 Payments

### `payments`
| Field | Notes |
|-------|-------|
| payable_type | BOOKING, SUBSCRIPTION, AD*, ORDER* |
| payable_id | UUID |
| status | CREATED, PENDING, REQUIRES_ACTION, CAPTURED, FAILED, CANCELLED, REFUNDED, PARTIALLY_REFUNDED |
| amount_minor/currency | |
| gateway | AREEBA_IXOPAY |
| gateway_transaction_id | unique w/ gateway |
| transaction_token_last4_ref | never full token storage beyond need |
| customer_id / user_id | |
| idempotency_key | |

### `payment_attempts`, `payment_webhooks`, `refunds`
Raw gateway payloads stored encrypted/minimized per compliance (Q-CMP-003).

---

## 15.6 Commissions & Withdrawals

### `commission_rules`
scope (global/category/store/craftsman*), percent or fixed, effective dates — structure TBD Q-COM-001

### `commission_lines`
source_payment_id/booking_id, rule_id, basis_amount, commission_amount, status

### `withdrawal_requests`
craftsman_id, amount, currency, status, destination snapshot (rail TBD Q-SET-002)

### `settlement_periods` *
If settlement is periodic rather than per-withdrawal.

---

## 15.7 Subscriptions

### `subscription_plans`
code, billing_period, price, entitlements JSON, active

### `subscriptions`
holder_type/id (craftsman/store*), plan_id, status, current_period_start/end, auto_renew

---

## 15.8 Notifications

### `notification_templates`
channel, event_key, locale, subject/body templates, version

### `notification_deliveries`
user_id, channel, status, provider_ref, error, sent_at

### `in_app_notifications`
user_id, title, body, data JSON, read_at

---

## 15.9 Identity Verification

### `verification_cases`
subject_type/id, purpose (ONBOARDING/JOB), status

### `verification_documents`
media_id, doc_type, status

### `ocr_results`, `face_checks`, `gps_checks`
provider, score, raw summary, pass/fail

### `job_challenges`
booking_id, type (QR/OTP), secret_hash, expires_at, consumed_at

---

## 15.10 Ads / Ratings / Quality / Audit

### Ads
`ad_campaigns`, `ad_creatives`, `ad_placements` (slot, start/end, priority)

### Ratings
`ratings` (score), `reviews` (text, moderation_status)

### Quality
`surveys`, `survey_responses`, `quality_scores`, `restriction_rules`, `actor_restrictions`

### Audit
`audit_events`: actor_id, action, entity_type, entity_id, before_json, after_json, ip, correlation_id, at

### Platform
`global_settings(key, value_json)`, `feature_flags`, `outbox_events`, `idempotency_keys`

---

## 15.11 Relationship Cardinalities (Summary)

| From | To | Cardinality |
|------|----|-------------|
| User | Customer | 0..1 |
| User | Craftsman | 0..1 |
| Customer | Booking | 1..* |
| Booking | Payment | 0..* |
| Booking | Rating | 0..1 |
| Craftsman | Subscription | 0..* |
| Payment | CommissionLine | 0..* |
| Craftsman | VerificationCase | 0..* |

## 15.12 Questions Requiring Business Decision

Affects schema shape materially: `Q-COM-001`, `Q-SET-002`, `Q-PRD-001`, `Q-STR-001`, `Q-SUB-001`, `Q-DB-001`, `Q-CMP-002/003`, payable types for ads/orders in V1 depth.
