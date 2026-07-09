# Production deployment

Configure external integrations via environment variables or a secrets manager. The API reads standard `appsettings.Production.json` keys; override any value with the `Section__Key` environment variable pattern (ASP.NET Core configuration).

**Important:** `${PLACEHOLDER}` values in `appsettings.Production.json` are documentation only — ASP.NET Core does not expand them. You must supply real values through environment variables or a secret store.

## Quick start (Docker)

```bash
cp .env.production.example .env.production
# Edit secrets in .env.production

docker compose -f docker-compose.prod.yml --env-file .env.production up -d --build
```

The production compose file sets `ASPNETCORE_ENVIRONMENT=Production`, disables demo seeding by default, and builds the web SPA with `VITE_API_URL=/api/v1` so the browser calls the API through the nginx reverse proxy.

### Database migrations

Production defaults to `Database__MigrateOnStartup=false` to avoid multi-replica migration races. Apply migrations as a one-off job before rolling out new API versions:

```bash
# From the host with .NET SDK and repo checkout:
dotnet ef database update \
  --project src/backend/Khadamati.Infrastructure/Khadamati.Infrastructure.csproj \
  --startup-project src/backend/Khadamati.API/Khadamati.API.csproj \
  --connection "$ConnectionStrings__DefaultConnection"
```

For first-time bootstrap, run migrations once, then start the API. Roles and permissions are seeded automatically; demo users are **not** created when `Database__SeedDemoData=false`.

## Runtime hardening

| Control | Production behavior |
|---------|---------------------|
| Demo seeding | Off when `Database__SeedDemoData=false` (default in Production) |
| JWT secret | Startup fails if missing, under 32 chars, or still a `${...}` placeholder |
| Forwarded headers | Enabled behind reverse proxies (`X-Forwarded-For`, `X-Forwarded-Proto`) |
| HSTS | Enabled outside Development |
| Security headers | `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Permissions-Policy` |
| Rate limiting | Stricter limits in `appsettings.Production.json` |
| Integration gate | Optional `Integrations__RequireProductionReady=true` fails startup if providers are not ready |

## Provider switches

| Integration | Config key | Production value |
|-------------|------------|------------------|
| Payments | `Payment:Provider` | `Moyasar` |
| Push | `Push:Provider` | `firebase` |
| Email | `Email:Provider` | `SendGrid` or `smtp` |
| SMS | `Sms:Provider` | `Twilio` |

## Required secrets

### Database
- `ConnectionStrings__DefaultConnection` — SQL Server connection string

### JWT
- `Jwt__Secret` — minimum 32 characters (validated at startup in Production)

### Moyasar (payments)
- `Payment__Moyasar__SecretKey`
- `Payment__Moyasar__PublishableKey`
- `Payment__Moyasar__WebhookSecret` — validates `X-Moyasar-Signature` on webhooks
- `Payment__Moyasar__CallbackUrl` — `https://<api-host>/api/v1/webhooks/moyasar`
- `Payment__Moyasar__SuccessUrl` — post-checkout redirect

### Firebase / FCM (Android push)
- `Push__Firebase__ServerKey` — FCM legacy server key
- `Push__Firebase__ProjectId`

### APNs (iOS push)
- `Push__Apns__TeamId`
- `Push__Apns__KeyId`
- `Push__Apns__BundleId` — `com.khadamati.app`
- `Push__Apns__PrivateKey` — `.p8` key contents (use `\n` for newlines in env vars)
- `Push__Apns__UseSandbox` — `false` for App Store builds

### Email (SendGrid example)
- `Email__SendGrid__ApiKey`
- `Email__SendGrid__FromAddress`

### SMS (Twilio)
- `Sms__Twilio__AccountSid`
- `Sms__Twilio__AuthToken`
- `Sms__Twilio__FromNumber`

## Mobile clients

### Android
Replace `src/android/app/google-services.json` with the file from Firebase Console (package `com.khadamati.app`).

### iOS
- Enable **Push Notifications** in Xcode Signing & Capabilities
- Set `aps-environment` to `production` in `Khadamati.entitlements` for release builds
- Upload APNs key to Apple Developer portal; mirror credentials in `Push:Apns` above

## Health and readiness checks

| Endpoint | Purpose |
|----------|---------|
| `GET /api/v1/health` | Liveness — process is up |
| `GET /api/v1/health/ready` | Readiness — includes database connectivity |
| `GET /api/v1/health/integrations` | Integration provider status (no auth) |
| `GET /api/v1/admin/system/health` | Admin system health (requires admin JWT) |

```bash
# Liveness
curl https://api.khadamati.com/api/v1/health

# Readiness (use for load balancer / orchestrator probes)
curl https://api.khadamati.com/api/v1/health/ready

# Integration readiness (no auth)
curl https://api.khadamati.com/api/v1/health/integrations

# Admin system health (requires admin JWT)
curl -H "Authorization: Bearer <token>" https://api.khadamati.com/api/v1/admin/system/health
```

`productionReady: true` means all four provider families (payment, push, email, SMS) report **Ready** status.

Startup logs warn when any provider is still on **Development** or **Misconfigured**. Set `Integrations__RequireProductionReady=true` to fail fast instead of only logging warnings.

## Smoke tests

1. **Payment** — create a booking, open Moyasar checkout, complete payment, confirm webhook updates booking status
2. **Push** — sign in on a physical device, confirm `POST /devices/push-token` stores a non-`dev-` token, trigger a booking notification
3. **Email** — register a user and confirm verification email is delivered (not logged only)

See `appsettings.Production.json` and `.env.production.example` for the full production template.
