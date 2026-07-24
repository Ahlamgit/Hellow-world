# Final Logical Database Architecture

**Document ID:** KHAD-V1-DB-FINAL  
**Status:** Architecture Ready (pending Product decisions ADR-016/017 for optional tables)  
**Engine:** PostgreSQL · Migrations: Flyway  
**Money:** Immutable ledger · Policies: versioned + history (ADR-013)

---

## Global Conventions

| Concern | Rule |
|---------|------|
| PK | `id UUID` |
| Time | `timestamptz` UTC storage; Market timezone for display/scheduling |
| Money | `amount_minor BIGINT` + `currency_code CHAR(3)` |
| Soft delete | `deleted_at` on master/reference data only |
| Optimistic lock | `version BIGINT` on aggregates (booking, payment, policy active row) |
| Audit cols | `created_at`, `created_by`, `updated_at`, `updated_by` |
| Financial immutability | No soft-delete on `ledger_entries`, `payments`, `refunds`, `commission_lines`, `*_history` |
| Market | Most transactional rows carry `market_id` |

---

# A. Core / Identity / Market

## `markets`
| | |
|--|--|
| **Purpose** | Operating country/market configuration |
| **PK** | `id` |
| **Keys** | UNIQUE(`code`) e.g. `LB` |
| **Fields** | code, name, default_currency, phone_country_code, default_locale, supported_locales[], timezone, active |
| **Indexes** | `(active)`, `(code)` |
| **Soft delete** | No — deactivate via `active` |
| **Audit** | Yes |

## `users`
| | |
|--|--|
| **Purpose** | Authentication identity |
| **PK** | `id` |
| **Keys** | UNIQUE(`email`) WHERE email NOT NULL; UNIQUE(`phone_e164`) WHERE phone NOT NULL |
| **Fields** | email, phone_e164, password_hash, status, locale, preferred_currency, market_id, last_login_at |
| **Indexes** | status, market_id |
| **Soft delete** | Prefer status + anonymization workflow (ADR-014) |
| **Audit** | Yes + security events |

## `roles`, `permissions`, `role_permissions`, `user_roles`
RBAC. UNIQUE(`permissions.code`), UNIQUE(`roles.code`). No soft-delete for permissions (deactivate flag optional).

## `refresh_tokens`
token_hash UNIQUE, user_id FK, family_id, expires_at, revoked_at. Index `(user_id, revoked_at)`.

## `otp_challenges`, `mfa_credentials`
OTP: user/phone, code_hash, expires_at, consumed_at. MFA: user_id, secret_encrypted, enabled_at.

## `customers`
1:1 `user_id` UNIQUE FK → users. display_name, default_address_id, marketing_opt_in.

## `providers`
| | |
|--|--|
| **Purpose** | Unified marketplace provider root |
| **PK** | `id` |
| **Keys** | UNIQUE(`user_id`) optional for store org accounts |
| **Fields** | type (`CRAFTSMAN`\|`STORE`), status, market_id, quality_score |
| **Relationships** | 1:0..1 craftsman_profiles / store_profiles; 1:N listings |

## `craftsman_profiles` / `store_profiles`
1:1 provider_id. Onboarding status, skills/bio, legal_name/trade_name, logo_media_id, verification flags.

## `provider_affiliations`
provider_id (craftsman) ↔ store_provider_id, role, active. UNIQUE(craftsman_provider_id, store_provider_id) WHERE active.

## `categories` / `category_translations`
Tree `parent_id`. UNIQUE(category_id, locale).

## `listings`
| | |
|--|--|
| **Purpose** | Bookable service offering |
| **PK** | `id` |
| **FK** | provider_id, category_id, market_id |
| **Fields** | title, description, price_amount_minor, currency_code, active, rating_avg, rating_count |
| **Indexes** | (market_id, category_id, active), (provider_id), geo if lat/lon, gin/trgm for search |
| **Soft delete** | Yes |

## `locations` / `customer_addresses` / `service_areas`
Addresses with lat/lon, country, city, area. service_areas: provider polygon/radius.

## `availability_windows`
provider_id, day/time or date range, market tz reference. Required for scheduling Option B (ADR-016).

---

# B. Booking Domain

## `bookings`
| | |
|--|--|
| **Purpose** | Booking aggregate |
| **FK** | customer_id, provider_id, listing_id, market_id, store_provider_id NULL |
| **Fields** | status, scheduled_start/end, location snapshot, price_*, notes, version |
| **Indexes** | (customer_id, created_at DESC), (provider_id, scheduled_start), (status, scheduled_start), market_id |
| **Soft delete** | No — terminal statuses |
| **Audit** | status history + audit on sensitive admin actions |

## `booking_status_history`
booking_id, from_status, to_status, actor_id, reason, at. Append-only.

## `booking_cancellations`
booking_id, requested_by_role, reason, policy_id, policy_version, penalty_amount_*, refund_rule_id, approved_by, at.

## `completion_records`
booking_id UNIQUE, provider_completed_at, customer_confirmed_at, notes.

## `booking_reminders`
booking_id, type (`H24`), scheduled_for, sent_at. UNIQUE(booking_id, type).

---

# C. Financial Domain

## `payments`
payable_type/id, status, amounts, gateway, gateway_transaction_id UNIQUE(gateway, gateway_tx), idempotency_key, user_id, market_id. **Immutable history via attempts.**

## `payment_attempts`, `payment_webhooks`
Append-only; webhooks UNIQUE(gateway_event_id) for idempotency.

## `refunds`
payment_id, booking_id, refund_rule_id+version, amount, status, reason. Ledger-linked.

## `ledger_accounts`
owner_type/id, account_type (`PLATFORM`, `PROVIDER_PAYABLE`, `ESCROW`, `GATEWAY_CLEARING`, …), currency, market_id. UNIQUE(owner, type, currency, market).

## `ledger_entries`
| | |
|--|--|
| **Purpose** | Immutable double-entry lines |
| **Fields** | txn_id, booking_id NULL, user/provider refs, debit_account_id, credit_account_id, amount_minor, currency, type, status, created_at, created_by |
| **Constraints** | amount > 0; debit ≠ credit |
| **Indexes** | (booking_id), (debit_account_id, created_at), (txn_id) |
| **Soft delete** | **Forbidden** |

## `escrow_holds` (or ESCROW account movements only)
If separate: booking_id, payment_id, amount, status (HELD/RELEASED/REFUNDED), timestamps.

## `commission_rules` + `commission_rule_history`
Criteria + percent/fixed + effective dates + priority + active + market_id. History append-only.

## `commission_lines`
booking_id, rule_id, rule_version, amounts, status. FK to booking/payment.

## `refund_rules` + history · `cancellation_policies` + history · `settlement_rules` + history · `withdrawal_methods` · `withdrawal_configs` + history

## `withdrawal_requests`
provider_id, method_id, amount, status, admin decisions, ledger_txn_id.

## `settlement_batches`
rule_id/version, period, totals, status.

---

# D. Configuration / Engagement

## `subscription_plans` + translations · `subscriptions`
## `notification_templates` · `notification_deliveries` · `in_app_notifications`
## `ad_packages` · `ad_placements` · `promotions` (admin-managed)
## `global_settings` · `feature_flags`
## `outbox_events` · `idempotency_keys`
## `media_objects`
## `verification_cases` · documents · ocr/face/gps · `job_challenges`
## `ratings` · `reviews` · `surveys` · `survey_responses` · `actor_restrictions`
## `conversations` · `messages` (chat)
## `audit_events` (append-only, partition by month)

---

# E. Relationship Overview

```text
markets 1—N users|providers|listings|bookings|policies
users 1—0..1 customers
users 1—0..1 → providers (craftsman) OR store_users N—1 store provider
providers 1—N listings 1—N bookings
bookings 1—N payments|status_history|cancellations|ledger_entries
commission_rules 1—N commission_lines
```

---

# F. Database Quality Checklist

| Requirement | Covered |
|-------------|---------|
| Foreign keys | Yes on all relationships |
| Indexes on FKs + hot filters | Yes |
| Unique constraints (email/phone/gateway tx/idempotency) | Yes |
| Integrity on money (immutable entries) | Yes |
| Audit tables / cols | Yes |
| created/updated timestamps | Yes |
| Policy versioning + history | Yes |
| Soft delete strategy explicit | Yes |
| User deletion anonymization path | ADR-014 |
| Partition readiness (audit, deliveries, webhooks) | Yes |

---

## Open Schema Forks (Blocked)

- Slot inventory table — only if ADR-016 Option C  
- Full dispute tables — only if ADR-017 Option 2  
