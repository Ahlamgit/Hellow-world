# Staging deployment

Staging mirrors production runtime (Docker, `ASPNETCORE_ENVIRONMENT=Staging`, no demo seeding) but keeps **Development** integration providers so you can validate deploy mechanics without live Moyasar/SendGrid/Twilio/FCM credentials.

## Quick start

```bash
cp .env.staging.example .env.staging
# Edit Jwt__Secret and Bootstrap__* credentials

chmod +x scripts/*.sh
./scripts/deploy-staging.sh
```

| Service | URL |
|---------|-----|
| Web (nginx + SPA) | http://localhost:8080 |
| API (direct) | http://localhost:5001 |
| Swagger | http://localhost:5001/swagger |
| SQL Server | localhost:1434 |

Default bootstrap admin (from `.env.staging.example`):

- Email: `admin@staging.khadamati.com`
- Password: `StagingAdmin@123456`

Change these before any shared/staging host is reachable from the internet.

## What staging validates

- Docker image builds (API + web)
- EF migrations on startup (`Database__MigrateOnStartup=true`)
- Bootstrap admin creation (no demo `admin@khadamati.com`)
- `/api/v1/health`, `/api/v1/health/ready`, `/api/v1/health/integrations`
- nginx `/api/` proxy and SPA `VITE_API_URL=/api/v1`
- JWT secret validation (same rules as Production)

Integration providers remain in **Development** mode — payments, email, SMS, and push are logged, not sent.

## Manual steps

### Start without smoke tests

```bash
docker compose -f docker-compose.staging.yml --env-file .env.staging up -d --build
```

### Migrations only (host SDK, e.g. before first container start with `MigrateOnStartup=false`)

```bash
./scripts/migrate-database.sh
```

### Smoke tests only

```bash
./scripts/smoke-test.sh http://localhost:8080
```

### Tear down

```bash
docker compose -f docker-compose.staging.yml --env-file .env.staging down
# Add -v to remove volumes
```

## Staging vs production

| Setting | Staging | Production |
|---------|---------|------------|
| `ASPNETCORE_ENVIRONMENT` | `Staging` | `Production` |
| Demo seeding | Off | Off |
| Migrations on startup | On (default) | Off (default) |
| Integration providers | Development | Moyasar, SendGrid, Twilio, firebase |
| Swagger | Enabled | Disabled |
| `Integrations__RequireProductionReady` | false | optional |
| Bootstrap admin | Via `Bootstrap__*` env vars | Via `Bootstrap__*` env vars |

## Merge PR stack into main

Fifteen feature PRs (#17–#31) are stacked as drafts. To land everything on `main`:

```bash
# After PR #32 (merge + staging) is open:
./scripts/merge-pr-stack.sh
```

Or merge PR #32 into `main` directly — it contains the full stack plus staging tooling.

## Promote staging → production

1. Copy integration secrets from your secret manager into `.env.production`
2. Set `Database__MigrateOnStartup=false` and run `./scripts/migrate-database.sh` with `ENV_FILE=.env.production`
3. Set `Integrations__RequireProductionReady=true` once all providers report Ready
4. Deploy with `docker compose -f docker-compose.prod.yml --env-file .env.production up -d --build`
5. Run `./scripts/smoke-test.sh https://<your-prod-web-host>` with `API_DIRECT=https://<your-prod-api-host>`

See [PRODUCTION.md](./PRODUCTION.md) for the full production runbook.
