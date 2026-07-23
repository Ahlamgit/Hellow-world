# KHADAMATI — Areeba Implementation Plan (MPGS Hosted Checkout)

**Status:** Documentation only — **awaiting approval**  
**Date:** 2026-07-23  
**Code / migrations / UI changes:** None in this deliverable  

**Approved inputs:**

| Decision | Source |
|----------|--------|
| Production gateway = **Areeba MPGS Hosted Checkout** | `AREEBA_IMPLEMENTATION_DECISION.md` |
| Keep `IPaymentGateway` unchanged | Architecture approval |
| Keep `DevelopmentPaymentGateway` | Architecture approval |
| Keep Moyasar temporarily during migration | Architecture approval |
| Mobile never marks payment completed | Architecture approval |
| Webhook + API `VerifyAsync` = only completion authority | Architecture approval |
| Phase 1C still blocked | Architecture approval |

**Parent docs:**

- [PAYMENT_GATEWAY_MIGRATION_PLAN.md](./PAYMENT_GATEWAY_MIGRATION_PLAN.md)
- [AREEBA_IMPLEMENTATION_DECISION.md](./AREEBA_IMPLEMENTATION_DECISION.md)

**Stop condition:** Do not implement code, run migrations, or change payment UI until this plan is approved.

---

## Executive summary

This plan defines the **exact** work packages for the first Areeba implementation wave:

1. Backend `AreebaPaymentGateway` + webhook + config + DI (behind `Payment:Provider`)
2. Additive SQL Server / EF migration for four payment correlation columns
3. Web checkout redirect / status display hardening
4. Native mobile checkout correction (no auto-confirm)
5. Security controls, tests, and staged deployment

`IPaymentGateway` method signatures stay as-is. Business/application services continue to call only the abstraction.

---

# 1. IMPLEMENTATION SCOPE

## 1.1 Backend

### New `AreebaPaymentGateway`

| Item | Detail |
|------|--------|
| Path | `Khadamati.Infrastructure/Services/Payments/AreebaPaymentGateway.cs` |
| Implements | `IPaymentGateway` |
| `ProviderName` | `"Areeba"` |
| Product | MPGS only for this wave (`Payment:Areeba:Product` default `Mpgs`) |
| `CreateSessionAsync` | `POST /api/rest/version/{v}/merchant/{merchantId}/session` with `INITIATE_CHECKOUT`; merchant `order.id` = deterministic KHADAMATI order key (e.g. payment id / booking reference); amount + currency from request |
| Returns | `SessionId` = gateway order id (stable); also capture MPGS `session.id` for checkout; `CheckoutUrl` = hosted redirect URL **or** KHADAMATI bridge URL that loads `checkout.min.js` |
| `VerifyAsync` | `GET .../order/{orderId}`; require successful paid/captured equivalent; match amount + currency |
| Auth | HTTP Basic (`merchant.{MerchantId}` + API password) |
| Failure | Map HTTP/API errors to `ApplicationException` / verification failure reasons; never throw raw PSP payloads to clients |
| Fallback | **Do not** silently fall back to Development when Areeba secrets missing in Staging/Production — fail readiness / throw on create |

Supporting types (Infrastructure-private): request/response DTOs for MPGS session and order — **not** exposed to Application/Domain.

### Configuration settings

Add under `Payment:Areeba` in `appsettings.json`, Staging, Production, and `.env.*.example`:

| Key | Purpose |
|-----|---------|
| `Payment:Provider` | Add allowed value `Areeba` (retain `Development`, `Moyasar`) |
| `Payment:Areeba:Product` | `Mpgs` |
| `Payment:Areeba:MerchantId` | Merchant id |
| `Payment:Areeba:ApiUsername` | Typically `merchant.{MerchantId}` |
| `Payment:Areeba:ApiPassword` | API password from ePayment portal |
| `Payment:Areeba:ApiBaseUrl` | Default `https://epayment.areeba.com` |
| `Payment:Areeba:ApiVersion` | Default `100` (confirm with boarding) |
| `Payment:Areeba:WebhookSecret` | Signature secret (fail-closed Staging/Production) |
| `Payment:Areeba:CallbackUrl` | `https://{api-host}/api/v1/webhooks/areeba` |
| `Payment:Areeba:SuccessUrl` | Web/app return URL |
| `Payment:Areeba:CancelUrl` | Cancel/timeout return URL |
| `Payment:Areeba:Currency` | Merchant settlement currency (expect `USD` for Lebanon) |

Secrets must be injectable via env (`Payment__Areeba__ApiPassword`, etc.). Never commit real values.

### Dependency injection registration

Extend `RegisterPaymentProvider` in `DependencyInjection.cs`:

```text
Payment:Provider
  moyasar  → MoyasarPaymentGateway (+ HttpClient)   // keep
  areeba   → AreebaPaymentGateway (+ HttpClient)    // new
  default  → DevelopmentPaymentGateway
```

Also:

- Register webhook handler(s) for Areeba (scoped)
- Extend `IntegrationReadinessService` to treat `Areeba` as a valid production provider and validate required keys
- Update readiness unit tests accordingly

### Webhook handling

| Item | Detail |
|------|--------|
| Endpoint | `POST /api/v1/webhooks/areeba` |
| Controller | Dedicated `AreebaWebhookController` (do not add to locations file) |
| Auth | `[AllowAnonymous]` + signature validation |
| Raw body | Read raw body before deserialize (HMAC/signature input) |
| Application | Prefer provider-agnostic completion path: map event → reference → `ConfirmPaymentFromWebhookAsync` |
| Moyasar | Keep `/webhooks/moyasar` unchanged during coexistence |
| Idempotency | Persist `WebhookEventId`; short-circuit if already processed or payment already `Completed` |

Optional hardening (same wave if low cost): extract shared “confirm from webhook” helper so Moyasar and Areeba do not duplicate booking transition logic.

### Logging and error handling

| Rule | Detail |
|------|--------|
| Serilog | Log payment id, booking id, provider, gateway order/session ids, HTTP status |
| Never log | API passwords, webhook secrets, full card data, CVV, raw authorization headers |
| Truncate | Webhook body logs: structured fields only, or redacted payload |
| Client errors | Map to existing `ConflictException` / `NotFoundException` / `UnauthorizedException` patterns |
| Ops signal | Warn on repeated verify failures; count stuck `AwaitingPayment` |

### Out of scope (backend this wave)

- Refund API
- Escrow / payouts
- Changing `IPaymentGateway` interface shape
- Removing Moyasar
- Phase 1C profile consolidation

---

## 1.2 Database

### Required migrations only

Single additive EF Core migration (name suggestion: `AddAreebaPaymentCorrelationColumns`):

| Change | Type |
|--------|------|
| Add `PaymentProvider` | `nvarchar(50)` NOT NULL, default `'Development'` |
| Add `GatewaySessionId` | `nvarchar(200)` NULL |
| Add `GatewayTransactionId` | `nvarchar(200)` NULL |
| Add `WebhookEventId` | `nvarchar(200)` NULL |
| Indexes | See §2 |
| Backfill | `PaymentProvider` for existing rows |

No new tables. No drops. No renames of existing columns.

### Backward compatibility

| Concern | Approach |
|---------|----------|
| Existing API DTOs | Keep `BookingPaymentDto.sessionId` / `checkoutUrl` / `transactionReference` / `provider` |
| Existing rows | Nullable gateway columns; `PaymentProvider` default + backfill |
| Moyasar path | Continues writing `TransactionReference`; should also populate new columns when touched |
| Development path | `PaymentProvider=Development`; `KHD-*` session ids unchanged |
| Readers | Admin/list queries ignore unknown null gateway fields |

### Rollback plan

| Scenario | Action |
|----------|--------|
| App rollback (code) | Redeploy previous API image; set `Payment:Provider` back to `Moyasar` or `Development` |
| Migration rollback | Prefer **forward fix**; additive columns are safe to leave in place. If mandatory down migration: drop new indexes then drop four columns only after confirming no production code depends on them |
| Data | Do not delete historical `BookingPayments` rows |
| Dual webhook | Leaving Moyasar webhook enabled allows draining in-flight Moyasar payments after app rollback |

---

## 1.3 Web

| Change | Detail |
|--------|--------|
| Checkout redirect | After initiate, prefer **redirect** (or same-tab navigation) to `checkoutUrl` instead of optional new-tab + manual confirm |
| Success/cancel routes | Handle `SuccessUrl` / `CancelUrl` landing; on success **refresh booking** and optionally call confirm (idempotent) |
| Payment status display | Render Pending / Processing / Completed / Failed **only** from API booking/payment status |
| Dev `/pay` simulator | Remain available only when provider is Development; hide or disable for Areeba/Moyasar |
| Copy / i18n | Clarify that payment confirmation comes from the server |

Do not claim “Paid” from query-string alone.

---

## 1.4 Mobile (Android Kotlin / iOS Swift)

| Change | Detail |
|--------|--------|
| Payment flow correction | Remove initiate → immediate confirm. Open `checkoutUrl` via Custom Tabs (Android) / `SFSafariViewController` or equivalent (iOS) |
| Pending state | Show pending/processing while booking is `AwaitingPayment` and payment not `Completed` |
| Completed state | Only after `GET` booking (or confirm response) shows server-side completion |
| Failed / cancelled | Surface API errors and cancelled return; allow retry per booking rules |
| Deep link / return | Register app links / custom scheme matching `SuccessUrl` / `CancelUrl`; on return, refresh booking from API |
| Authority rule | **Never** mark payment completed locally |

No React Native work in this wave.

---

# 2. DATABASE MIGRATION PLAN

## 2.1 Current `BookingPayments` schema (verified)

From EF configuration + `20260708065052_BookingModule` migration:

| Column | SQL type | Nullable | Notes |
|--------|----------|----------|-------|
| `Id` | `uniqueidentifier` | NO | PK |
| `ServiceRequestId` | `uniqueidentifier` | NO | Unique index `IX_BookingPayments_ServiceRequestId` |
| `PayerUserId` / `PayeeUserId` | `uniqueidentifier` | NO | FK Restrict; indexes present |
| `Amount` | `decimal(18,2)` | NO | |
| `Currency` | `nvarchar(3)` | NO | |
| `Status` | `int` | NO | `PaymentStatus` enum |
| `PaymentMethod` | `nvarchar(50)` | NO | |
| `TransactionReference` | `nvarchar(200)` | YES | No unique index today |
| `PaidAt` | `datetime2` | YES | |
| `FailureReason` | `nvarchar(max)` | YES | |
| Soft-delete / audit | via `BaseEntity` / table columns | | As created by Booking module |

**Not present today:** `PaymentProvider`, `GatewaySessionId`, `GatewayTransactionId`, `WebhookEventId`.

## 2.2 Proposed additions (final)

| Column | CLR type | SQL type | Max length | Nullability | Default | Purpose |
|--------|----------|----------|------------|-------------|---------|---------|
| `PaymentProvider` | `string` | `nvarchar(50)` | 50 | **NOT NULL** | `'Development'` | `Development` \| `Moyasar` \| `Areeba` |
| `GatewaySessionId` | `string?` | `nvarchar(200)` | 200 | **NULL** | — | MPGS `session.id` |
| `GatewayTransactionId` | `string?` | `nvarchar(200)` | 200 | **NULL** | — | MPGS `order.id` (stable) |
| `WebhookEventId` | `string?` | `nvarchar(200)` | 200 | **NULL** | — | Last/processed webhook event id for idempotency |

### Index / uniqueness rules

| Index | Columns | Unique? | Filter | Rationale |
|-------|---------|---------|--------|-----------|
| `UX_BookingPayments_WebhookEventId` | `WebhookEventId` | **Yes** | `WHERE WebhookEventId IS NOT NULL` | Duplicate webhook rejection |
| `IX_BookingPayments_GatewayTransactionId` | `GatewayTransactionId` | No (or unique filtered if 1:1 guaranteed) | `WHERE GatewayTransactionId IS NOT NULL` | Lookup/verify; start **non-unique** unless product guarantees one order id forever |
| `UX_BookingPayments_TransactionReference` | `TransactionReference` | **Yes** | `WHERE TransactionReference IS NOT NULL` | Webhook/confirm correlation; fix current missing index |

**Uniqueness notes:**

- `ServiceRequestId` remains the only booking↔payment uniqueness (1:0..1).
- `WebhookEventId` unique when present — required for replay protection.
- If two historical rows could share blank/duplicate `TransactionReference`, clean or leave nulls before applying unique filtered index; migration script must verify no duplicates first.

### Existing records compatibility

| Step | SQL / EF action |
|------|-----------------|
| 1 | `ADD` columns with defaults/nulls (online-friendly) |
| 2 | Backfill `PaymentProvider`: `Development` where `TransactionReference LIKE 'KHD-%'`; else `Moyasar` for non-null historical gateway refs; else `Development` |
| 3 | Leave `GatewaySessionId` / `GatewayTransactionId` / `WebhookEventId` null for old rows |
| 4 | Optionally copy `TransactionReference` → `GatewayTransactionId` for non-`KHD-` rows to aid support |
| 5 | Create filtered indexes after duplicate check |

### EF / SQL Server discipline

1. Generate EF migration from entity + `BookingPaymentConfiguration` updates.
2. Review generated SQL; adjust filtered indexes if EF does not emit them correctly (Phase 1A lesson).
3. Apply against SQL Server in Dev → Staging before Production.
4. Physical schema must match EF model snapshot.

### Explicitly not in this migration

- `GatewayStatus`
- `PaymentAttemptNumber`
- Refund / escrow tables
- Dropping Moyasar-related artifacts (none as columns)

---

# 3. PAYMENT SECURITY REVIEW

## 3.1 Areeba API key storage

| Control | Requirement |
|---------|-------------|
| Storage | Environment variables / secret store only (`Payment__Areeba__ApiPassword`, webhook secret) |
| Repo | Empty placeholders in `appsettings*.json`; real values only in `.env` / host secrets |
| Access | Restrict to API process identity; no client apps receive API password |
| Rotation | Document in security runbook; dual-password support if Areeba portal allows standby password |
| Readiness | Staging/Production fail or warn via `IntegrationReadinessService` when Provider=Areeba and secrets missing |

## 3.2 Webhook signature validation

| Control | Requirement |
|---------|-------------|
| Algorithm | Confirm with Areeba boarding (expect HMAC over raw body or documented header scheme); implement exactly as specified |
| Input | **Raw request body** bytes/string before JSON deserialize |
| Comparison | Fixed-time equals (Phase 1B pattern) |
| Fail-closed | Staging + Production: missing secret or bad signature → `Unauthorized`; **no** booking mutation |
| Development | May allow missing secret only in Development environment |
| Moyasar | Keep existing HMAC validation path during coexistence |

## 3.3 Idempotency handling

| Layer | Behavior |
|-------|----------|
| Webhook | If `WebhookEventId` already stored → return processed/no-op |
| Payment | If `PaymentStatus.Completed` → return current booking DTO; do not re-reserve slot |
| Confirm API | `VerifyAsync` + same finalize path; safe to call after webhook |
| Slot reservation | Rely on existing unique slot constraints (Phase 1A) |

## 3.4 Replay attack protection

| Control | Requirement |
|---------|-------------|
| Signature | Reject unsigned / invalid signatures |
| Event id | Persist unique `WebhookEventId` |
| Verify | Always re-check amount/currency with Areeba Retrieve Order — do not trust webhook body alone for money fields |
| Freshness | Optionally reject events older than configured window if timestamp provided by Areeba (confirm field availability) |

## 3.5 Duplicate webhook handling

| Case | Response |
|------|----------|
| Same event id twice | 200 + idempotent result; log info |
| Same payment, different event, already Completed | 200 + no-op |
| Concurrent webhooks | DB uniqueness / row update concurrency; one winner; other no-op or retry-safe |

## 3.6 Sensitive data logging rules

| May log | Must not log |
|---------|--------------|
| Payment id, booking id, provider | `ApiPassword`, `WebhookSecret`, Basic auth header |
| Gateway order/session ids | PAN, CVV, full track data |
| HTTP status, high-level error codes | Full raw webhook body if it contains masked-but-sensitive card metadata — prefer allowlisted fields |
| Correlation / trace ids | Customer email in debug unless already permitted by privacy policy |

Serilog enrichers must not dump entire configuration sections containing secrets.

---

# 4. TEST IMPLEMENTATION PLAN

## 4.1 Unit tests

| Test | Expectation |
|------|-------------|
| **Create payment session** | Mock HTTP: success JSON → `PaymentSessionDto` with order/session + checkout URL; provider `Areeba` |
| **Gateway failure** | Non-success HTTP / network → application error; payment not marked Completed |
| **Invalid response** | Missing session/order fields → fail create; no partial success returned to caller |
| **Webhook verification — valid** | Valid signature + paid mapping → calls confirm path |
| **Webhook verification — invalid** | Bad signature → unauthorized; booking service not called |
| **VerifyAsync mismatch** | Wrong amount/currency/status → `IsSuccessful=false` |
| **Idempotent completed payment** | Second confirm/webhook → no double transition |
| **Readiness** | Areeba misconfigured vs ready |

Use `HttpMessageHandler` mocks; no real network in unit tests.

## 4.2 Integration tests

| Test | Expectation |
|------|-------------|
| **Sandbox payment** | Against Areeba sandbox (or recorded contract test if secrets unavailable in CI): initiate → paid → booking advances |
| **Successful webhook** | Signed payload → payment Completed + craftsman pending confirmation |
| **Failed payment** | Declined/failed status → not Completed; booking stays awaiting payment (or Failed per rule) |
| **Duplicate webhook** | Second delivery does not create second reservation / does not error critically |
| **Migration apply** | EF migration applies cleanly on SQL Server test DB; backfill + indexes exist |

CI strategy: unit tests always; sandbox integration behind secret-gated job or manual staging checklist if sandbox credentials are not in CI.

## 4.3 Regression

| Area | Expectation |
|------|-------------|
| **Existing booking flow** | Create → confirm → await payment → (dev) pay → craftsman accept path unchanged |
| **Existing Moyasar flow** | With `Payment:Provider=Moyasar`, invoice create + webhook still pass tests |
| **Development provider** | Default local/CI path unchanged (`KHD-*`) |
| **Phase 1A** | Slot concurrency / coupon / RowVersion scenarios still green |
| **Phase 1B** | Moyasar HMAC fail-closed tests still green |

---

# 5. DEPLOYMENT STRATEGY

## Stage 1 — Development environment

| Action | Detail |
|--------|--------|
| Merge implementation behind config | Default `Payment:Provider=Development` |
| Apply DB migration | Dev SQL Server |
| Manual smoke | Development checkout still works |
| Areeba optional | Engineers may point local Provider to Areeba with sandbox secrets |

**Exit:** CI green; Dev booking pay works on Development gateway.

## Stage 2 — Sandbox validation

| Action | Detail |
|--------|--------|
| Staging config | `Payment:Provider=Areeba` + sandbox secrets |
| Webhooks | Public staging callback URL registered in Areeba |
| Clients | Web + Android + iOS against staging API |
| Evidence | Record successful sandbox pay, invalid signature rejection, duplicate webhook, cancel path |
| Moyasar | Code + webhook still deployed; not creating new sessions if Provider=Areeba |

**Exit:** Signed sandbox validation checklist (backend + web + mobile).

## Stage 3 — Production with provider configuration

| Action | Detail |
|--------|--------|
| Feature switch | **Config-only**: `Payment:Provider=Areeba` (no separate code flag required if DI switch is sufficient) |
| Secrets | Production MPGS merchant credentials + webhook secret in vault/host env |
| Moyasar webhook | Remains enabled for in-flight Moyasar payments |
| Monitor | Payment success rate, webhook 401s, verify failures, stuck `AwaitingPayment` |
| Rollback | Set `Payment:Provider=Moyasar` (or Development only if emergency and acceptable) and redeploy config; Moyasar code still present |

**Exit:** New production payments created on Areeba; no critical error budget breach for agreed soak window.

## Stage 4 — Moyasar deprecation

| Gate | Detail |
|------|--------|
| Drain | No Moyasar `Processing` in-flight beyond TTL |
| Soak | Successful production period (recommend ≥ 30 days) |
| Remove | Moyasar gateway, webhook, config keys, readiness branches, docs references |
| Secrets | Remove/rotate Moyasar secrets |

**Hard rule:** No Moyasar deletion in Stages 1–3.

---

## Suggested implementation work order (after approval)

1. Database migration + entity/config update  
2. `AreebaPaymentGateway` + DI + readiness  
3. Areeba webhook endpoint + security  
4. `BookingService` persistence of new columns (still via existing initiate/finalize)  
5. Unit/integration tests  
6. Web checkout/status UX  
7. Mobile flow correction + deep links  
8. Staging sandbox validation  
9. Production provider switch  

---

## Approval checklist

- [ ] Approve §1 scope (backend / DB / web / mobile)  
- [ ] Approve §2 column types, nullability, indexes, backfill  
- [ ] Approve §3 security controls  
- [ ] Approve §4 test matrix  
- [ ] Approve §5 four-stage deployment  

**Still blocked until approval:**

- Database migration  
- Areeba coding  
- Payment UI changes  
- Phase 1C  

**Sign-off:**

| Role | Name | Date | Decision |
|------|------|------|----------|
| Product | | | Approve / Changes requested |
| Engineering | | | Approve / Changes requested |
| Ops / Security | | | Approve / Changes requested |

---

*End of document — documentation only; STOP and wait for approval.*
