# 13. Database Architecture

**Document ID:** KHAD-V1-DB  
**Status:** Draft for Approval  
**Engine:** PostgreSQL  
**Migrations:** Flyway  

---

## 13.1 Goals

- Strong relational integrity for money and booking lifecycles  
- Module-aligned schemas/table prefixes  
- Multi-currency ready (amount + currency)  
- Multi-language ready (translation tables or JSONB locale maps — decision Q-DB-001)  
- Multi-region ready (`region_code` / `country_code` columns where relevant)  
- Auditable, soft-deletable master data  

## 13.2 Database Topology

| Environment | Pattern |
|-------------|---------|
| Local/Dev | Single Postgres instance |
| Staging | Single instance (+ backups) |
| Production | Primary + automated backups; read replica for reporting if needed |

Logical database: `khadamati`  
Optional schemas per module: `iam`, `booking`, `payment`, … **or** single `public` schema with prefixed table names. Recommendation: **schemas per module** for clarity.

## 13.3 Flyway Conventions

```text
db/migration/
  V202607241000__iam_init.sql
  V202607241010__booking_init.sql
  ...
  R__reporting_views.sql   # repeatable views if used
```

Rules:

- Versioned migrations are immutable after merge  
- No business data seeds in prod migrations except reference data explicitly approved  
- Use `uuid` PKs (UUIDv7 recommended if available ops-wise; else UUIDv4) — Q-DB-002  

## 13.4 Cross-Cutting Columns

Standard columns on mutable entities:

| Column | Type | Notes |
|--------|------|-------|
| `id` | UUID | PK |
| `created_at` | timestamptz | required |
| `created_by` | UUID NULL | actor if known |
| `updated_at` | timestamptz | required |
| `updated_by` | UUID NULL | |
| `deleted_at` | timestamptz NULL | soft delete |
| `version` | bigint | optimistic lock |

Money columns:

| Column | Type |
|--------|------|
| `amount_minor` | bigint |
| `currency_code` | char(3) |

## 13.5 Core Table Groups (Logical)

### IAM
`users`, `roles`, `permissions`, `role_permissions`, `user_roles`, `refresh_tokens`, `login_attempts`

### Customer / Craftsman / Store
`customers`, `customer_addresses`, `craftsmen`, `craftsman_services`, `stores`, `store_users` (if Q-STR-001), `products`, `store_services`, `orders`, `order_items`

### Catalog
`categories`, `category_translations`, `service_offerings` (projection or source)

### Booking
`bookings`, `booking_status_history`, `booking_reminders`

### Payment
`payments`, `payment_attempts`, `payment_webhooks`, `refunds`

### Commission / Settlement
`commission_rules`, `commission_lines`, `settlement_periods`, `withdrawal_requests`

### Subscription
`subscription_plans`, `subscription_plan_translations`, `subscriptions`, `subscription_payments`

### Notification
`notification_templates`, `notification_deliveries`, `in_app_notifications`

### Identity Verification
`verification_cases`, `verification_documents`, `ocr_results`, `face_checks`, `gps_checks`, `job_challenges`

### Ads
`ad_campaigns`, `ad_placements`, `ad_creatives`

### Ratings / Quality
`ratings`, `reviews`, `quality_scores`, `restriction_rules`, `actor_restrictions`, `surveys`, `survey_responses`

### Platform
`global_settings`, `feature_flags`, `outbox_events`, `idempotency_keys`

### Audit
`audit_events` (append-only, partitioned by month recommended)

## 13.6 Indexing Strategy

- FK indexes on all foreign keys  
- Partial indexes for active records (`deleted_at IS NULL`)  
- Booking: `(customer_id, created_at desc)`, `(craftsman_id, scheduled_start)`, `(status, scheduled_start)`  
- Payment: unique `(gateway, gateway_transaction_id)`  
- Idempotency: unique `(actor_id, key)`  
- Outbox: `(published_at nulls first, created_at)`  

## 13.7 Partitioning Candidates

- `audit_events` by month  
- `notification_deliveries` by month  
- `payment_webhooks` by month  

## 13.8 Data Retention

Retention periods TBD (Q-DB-003): PII, biometrics, audit, payment logs.

## 13.9 Backup & Migration Safety

- Expand-contract migrations for breaking changes  
- Never drop columns in same release as code removal  
- Backup verification in staging before prod migrate  

## 13.10 Questions Requiring Business Decision

- Q-DB-001: translation storage model  
- Q-DB-002: UUIDv4 vs v7  
- Q-DB-003: retention policy  
- Q-STR-001: store_users table needed?
