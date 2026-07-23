# Areeba — Sandbox Validation Report

**Status:** Validation phase executed in agent environment — **incomplete for go-live**  
**Date:** 2026-07-23  
**Branch:** `cursor/payment-gateway-migration-plan-4876`  
**Commit under test:** `bc6a611` (docs) + implementation `6e0f632`  
**Decision doc:** [AREEBA_GO_LIVE_DECISION.md](./AREEBA_GO_LIVE_DECISION.md)

**Hard stop observed:** Production `Payment:Provider` was **not** switched to Areeba. Moyasar was **not** removed. Phase 1C / React Native were **not** started.

---

## Validation environment constraints

| Capability | Available in this cloud agent? | Impact |
|------------|--------------------------------|--------|
| Repo / config / unit tests | Yes | Environment + automated payment tests executed |
| Docker SQL Server 2022 | **No** — image extract failed (`overlayfs` whiteout not permitted) | Physical staging migration **not applied here** |
| Areeba sandbox merchant credentials | **No** — not present in env / secrets | Live checkout + live webhook **not executed** |
| Device lab (Android/iOS) | **No** | Client checks are **code-path + static review** only |

Where a required scenario could not be executed live, status is **BLOCKED** (not PASS).

---

# 1. Environment validation

| Check | Result | Evidence |
|-------|--------|----------|
| Areeba sandbox credentials configuration | **BLOCKED** | No `Payment__Areeba__*` / merchant secrets in process env; cannot bind Staging Provider=Areeba against live MPGS |
| Secret keys are environment variables only | **PASS** | Production uses `${AREEBA_API_PASSWORD}`, `${AREEBA_WEBHOOK_SECRET}`, `${AREEBA_MERCHANT_ID}`, etc. Dev `appsettings.json` uses empty strings |
| No secrets committed | **PASS** | Tracked configs contain placeholders only; secret-pattern scan found no live `sk_live` / embedded Areeba passwords |
| Production configuration remains unchanged (provider) | **PASS** | `appsettings.Production.json` → `"Payment": { "Provider": "Moyasar" }` (Areeba section present for future cutover only) |

### Production payment snippet (unchanged cutover switch)

```json
"Payment": {
  "Provider": "Moyasar",
  ...
  "Areeba": { "...": "${AREEBA_*}" }
}
```

**Conclusion §1:** Config hygiene **PASS**. Live sandbox credential binding **BLOCKED** until ops provides Staging secrets.

---

# 2. SQL Server staging validation

## 2.1 Attempted execution

| Step | Result |
|------|--------|
| Start Docker daemon | Partial (manual `dockerd`) |
| Pull `mcr.microsoft.com/mssql/server:2022-latest` | **FAIL** — `failed to convert whiteout file ... operation not permitted` |
| `dotnet ef database update` against Staging SQL | **NOT RUN** — no reachable SQL Server |

## 2.2 Migration artifact review (static — PASS as design)

Migration `20260723210419_AddBookingPaymentAttempts` includes:

| Expectation | In migration? |
|-------------|----------------|
| `BookingPaymentAttempts` table | Yes |
| Header columns on `BookingPayments` | Yes (`PaymentProvider`, `CurrentAttemptId`, gateway ids, `FailedAt`) |
| Indexes: attempt number, `GatewayTransactionId`, `WebhookEventId`, one-Completed | Yes |
| Existing `BookingPayment` preservation | Yes (additive + backfill; no obligation deletes) |
| Payment → attempts relationship | Yes (FK cascade from payment to attempts) |

## 2.3 Required Staging ops script (must be run outside this agent)

```bash
# On Staging host with connection string
dotnet ef database update \
  --project src/backend/Khadamati.Infrastructure \
  --startup-project src/backend/Khadamati.API \
  --connection "$STAGING_CONNECTION_STRING"
```

```sql
-- Verify table
SELECT COUNT(*) AS AttemptRows FROM BookingPaymentAttempts;
SELECT COUNT(*) AS PaymentRows FROM BookingPayments WHERE Deleted = 0;

-- Every payment should have >= 1 attempt after backfill
SELECT p.Id
FROM BookingPayments p
WHERE p.Deleted = 0
  AND NOT EXISTS (
    SELECT 1 FROM BookingPaymentAttempts a
    WHERE a.BookingPaymentId = p.Id AND a.Deleted = 0);

-- Indexes
SELECT name FROM sys.indexes
WHERE object_id = OBJECT_ID('BookingPaymentAttempts');
```

| Staging physical check | Status |
|------------------------|--------|
| Migration applied | **BLOCKED** (pending ops) |
| Table / indexes / data preserved | **BLOCKED** (pending ops) |

---

# 3. Sandbox payment scenarios

## 3.1 Automated evidence (unit tests)

Payment-related filter run: **18 passed** (`AreebaPaymentGateway*`, `PaymentWebhook*`, `IntegrationReadiness*`).

| Scenario | Live Areeba sandbox | Automated / code evidence | Status |
|----------|---------------------|---------------------------|--------|
| **Successful payment** (booking → initiate → checkout → webhook → verify → complete) | Not executed | Create session + HMAC webhook confirm + verify captured matching unit tests | **BLOCKED** (E2E) / partial unit |
| **Failed payment** (rejected card / verify fail) | Not executed | `VerifyAsync` non-success path; finalize marks Failed | **BLOCKED** (E2E) / partial unit |
| **Abandoned payment** (leave checkout, remains pending/processing) | Not executed | Clients do not confirm; status stays Processing/AwaitingGatewayConfirmation until webhook | **BLOCKED** (E2E) / code-path PASS |
| **Duplicate webhook** | Not executed | Unique `WebhookEventId` + webhook service duplicate test | **BLOCKED** (E2E) / unit PASS |
| **Invalid webhook** | Not executed | Invalid signature → `UnauthorizedException` unit tests | **BLOCKED** (E2E) / unit PASS |
| **Wrong amount** | Not executed | `VerifyAsync_WrongAmount_Fails` | **BLOCKED** (E2E) / unit PASS |
| **Wrong currency** | Not executed | `VerifyAsync_WrongCurrency_Fails` | **BLOCKED** (E2E) / unit PASS |

## 3.2 Live sandbox procedure (for Staging when credentials exist)

1. Set Staging `Payment__Provider=Areeba` + MPGS sandbox merchant/password/webhook secret.  
2. Register `https://{staging-api}/api/v1/webhooks/areeba`.  
3. Create customer booking → confirm → initiate payment.  
4. Complete Areeba hosted checkout with test card.  
5. Confirm webhook received; booking leaves `AwaitingPayment`; attempt `Status=Completed`.  
6. Repeat failure / abandon / duplicate / bad signature / amount mismatch cases.

---

# 4. Client validation

## 4.1 Web (`BookingPaymentPage`)

| Check | Result | Notes |
|-------|--------|-------|
| Redirect to checkout | **PASS (code)** | `window.location.assign(checkoutUrl)` after initiate |
| Return / pending handling | **PASS (code)** | Refresh status; stays on page while `AwaitingPayment` |
| Completed state | **PASS (code)** | Navigates when booking status leaves `AwaitingPayment` |
| Failed / waiting messaging | **PASS (code)** | Non-Development providers blocked from client confirm |
| Live sandbox UI pass | **BLOCKED** | No Staging URL + secrets in agent |

Development-only `confirmPayment` button remains for local/CI (`provider === 'Development'`).

## 4.2 Android

| Check | Result | Notes |
|-------|--------|-------|
| No initiate → immediate confirm | **PASS (code)** | Removed from `BookingRepository.pay` |
| Open checkout URL | **PASS (code)** | `Intent.ACTION_VIEW` when `checkoutUrl` set |
| Return to app / refresh status | **PASS (code)** | Refresh payment status control + reload booking |
| Live device sandbox | **BLOCKED** | No device lab |

## 4.3 iOS

| Check | Result | Notes |
|-------|--------|-------|
| No initiate → immediate confirm | **PASS (code)** | Removed from `BookingViewModel.pay` |
| Open checkout URL | **PASS (code)** | `UIApplication.shared.open` |
| Refresh payment status | **PASS (code)** | Detail refresh + `loadBooking` after pay |
| Live device sandbox | **BLOCKED** | No device lab |

---

# 5. Summary scoreboard

| Area | PASS | BLOCKED | FAIL |
|------|------|---------|------|
| §1 Environment / secrets / prod provider | 3 | 1 (sandbox credentials) | 0 |
| §2 SQL Staging apply | 0 (design review only) | 1 | 0 (infra limitation) |
| §3 Sandbox payment E2E | 0 | 7 | 0 |
| §3 Automated payment unit coverage | 18 tests | — | 0 |
| §4 Client code paths | Web/Android/iOS code PASS | Live UI | 0 |

---

# 6. Remaining work before go-live

1. Apply `AddBookingPaymentAttempts` on **Staging SQL Server** and run §2.3 verification SQL.  
2. Configure Areeba **sandbox** secrets on Staging; set Staging `Payment:Provider=Areeba` only (not Production).  
3. Execute full §3 live matrix + §4 device/browser QA.  
4. Attach evidence (screenshots, webhook logs, SQL counts) to an updated copy of this report.  
5. Only then re-evaluate [AREEBA_GO_LIVE_DECISION.md](./AREEBA_GO_LIVE_DECISION.md).

---

*End of sandbox validation report.*
