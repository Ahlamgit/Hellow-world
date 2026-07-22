# KHADAMATI — Deployment Runbook (Phase 0)

**Document version:** 1.0  
**Date:** 2026-07-22  
**Includes:** Phase 0.2 Security Audit & Secrets Migration Plan

---

## 1. Environment Overview

| Environment | API URL | Web URL | Database | Config source |
|-------------|---------|---------|----------|---------------|
| **Development** | `http://localhost:5000` | `http://localhost:5173` | Local SQL Server or Docker | `appsettings.Development.json`, User Secrets, `appsettings.Local.json` |
| **Docker Dev** | `http://localhost:5000` | `http://localhost:3000` | `docker-compose` SQL Server | Environment variables in compose |
| **Staging** | `https://staging-api.khadamati.com` | `https://staging.khadamati.com` | Managed SQL | `.env.staging` + `appsettings.Staging.json` |
| **Production** | `https://api.khadamati.com` | `https://khadamati.com` | Managed SQL | Secret Manager / env vars + `appsettings.Production.json` |

---

## 2. Prerequisites

### 2.1 Development (Windows — primary documented path)

| Tool | Version |
|------|---------|
| .NET SDK | 8.0 LTS |
| Node.js | 18+ |
| SQL Server | 2014+ (or Docker SQL 2022) |
| Git | Latest |

**Project path (user machine):** `C:\Users\Ahlam\Documents\Khadamati`

### 2.2 Development (Linux / Cloud agent)

| Tool | Version |
|------|---------|
| .NET SDK | 8.0 |
| Node.js | 18+ |
| Docker (optional) | For SQL Server container |

---

## 3. Local Development — Two-Terminal Setup

### Terminal 1 — API

```powershell
cd C:\Users\Ahlam\Documents\Khadamati\src\backend
dotnet run --project Khadamati.API
```

Verify:
- `GET http://localhost:5000/api/v1/health` → `healthy`
- Swagger: `http://localhost:5000/swagger`

### Terminal 2 — Web

```powershell
# If ports 5173–5178 are stuck:
taskkill /F /IM node.exe
# Or: .\scripts\stop-web-windows.ps1

cd C:\Users\Ahlam\Documents\Khadamati\src\web
npm install
npm run dev
```

Verify: `http://localhost:5173` loads; network calls go to `http://localhost:5000/api/v1`.

### SQL Connection (Windows)

**Preferred:** copy `appsettings.Local.json.example` → `appsettings.Local.json` (gitignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=KhadamatiDb;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true"
  }
}
```

**Do not** edit tracked `appsettings.json` for local overrides — causes merge conflicts.

See also: [LOCAL_DEV_WINDOWS.md](./LOCAL_DEV_WINDOWS.md)

---

## 4. Docker Development

```bash
docker compose up --build
```

| Service | Port | Notes |
|---------|------|-------|
| SQL Server | 1433 | SA password from compose env |
| API | 5000 → 8080 | `ASPNETCORE_ENVIRONMENT=Development` |
| Web | 3000 → 80 | Nginx static |

Stop: `docker compose down`  
Data persists in volume `sqlserver_data`.

---

## 5. Database Deployment

### 5.1 Canonical Method (All Environments)

```bash
cd src/backend
dotnet ef database update --project Khadamati.Infrastructure --startup-project Khadamati.API
```

Or rely on `Database:MigrateOnStartup: true` at API boot.

### 5.2 Seed Data

| Environment | `SeedDemoData` | Result |
|-------------|----------------|--------|
| Development | `true` (default) | Demo users, services, regions |
| Staging | `false` | Bootstrap admin only if configured |
| Production | `false` | Bootstrap admin only if configured |

**Default dev admin:** `admin@khadamati.com` / `Admin@123456`

### 5.3 Legacy SQL Scripts

`src/database/000_MasterDeploy.sql` deploys scripts 001–007 only. **Do not use for new environments.** See [DATABASE_BASELINE.md](./DATABASE_BASELINE.md).

---

## 6. Staging Deployment

1. Copy `.env.staging.example` → `.env.staging` (never commit real secrets)
2. Set all required variables (see Section 8)
3. Deploy:

```bash
docker compose -f docker-compose.staging.yml --env-file .env.staging up -d --build
```

Reference: [STAGING.md](./STAGING.md)

---

## 7. Production Deployment

1. Provision SQL Server with TLS (`Encrypt=True`)
2. Store secrets in platform secret manager (Azure Key Vault, AWS Secrets Manager, or host env vars)
3. Map secrets to ASP.NET Core environment variables (`Section__Key` format)
4. Deploy:

```bash
docker compose -f docker-compose.prod.yml --env-file .env.production up -d --build
```

Reference: [PRODUCTION.md](./PRODUCTION.md)

### Production Checklist

- [ ] `Jwt__Secret` — unique, ≥32 chars, not in git
- [ ] `ConnectionStrings__DefaultConnection` — dedicated DB user (not SA)
- [ ] `Encrypt=True` on SQL connection
- [ ] `Integrations:RequireProductionReady: true`
- [ ] Email/SMS/Payment/Push production providers configured
- [ ] CORS limited to production domains
- [ ] `App:ExposeAuthLinks: false`
- [ ] HTTPS termination at reverse proxy
- [ ] Backup schedule configured
- [ ] Log aggregation (Serilog sinks)

---

## 8. Configuration Reference

### 8.1 Config File Hierarchy

```
appsettings.json                    # Base (NO secrets in production)
appsettings.{Environment}.json    # Environment overrides
appsettings.Local.json              # Dev machine only (gitignored)
User Secrets (Development)          # dotnet user-secrets
Environment variables               # Staging/Production
```

### 8.2 Key Settings

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection |
| `Jwt:Secret` | JWT signing key (≥32 chars) |
| `Jwt:Issuer` / `Jwt:Audience` | Token validation |
| `Bootstrap:AdminEmail` / `Bootstrap:AdminPassword` | First-run admin (staging/prod) |
| `Database:MigrateOnStartup` | Auto-apply EF migrations |
| `Database:SeedDemoData` | Seed demo data |
| `Database:ContinueOnFailure` | Dev only — boot without DB |
| `Email:Provider` | `Development` \| `SendGrid` |
| `Payment:Provider` | `Development` \| `Moyasar` |
| `Cors:AllowedOrigins` | Web origins |
| `App:WebBaseUrl` | Links in emails |
| `App:ExposeAuthLinks` | Dev: expose reset links in API |

### 8.3 Web Environment

| File | Variable | Default |
|------|----------|---------|
| `.env.development` | `VITE_API_URL` | `http://localhost:5000/api/v1` |

---

## 9. Phase 0.2 — Security Audit

### 9.1 Executive Summary

**Risk level: HIGH** — Production-grade secrets and default credentials are committed to the repository and git history. Immediate rotation and migration to secret stores is required before any public deployment.

### 9.2 Secrets Inventory

| # | Secret Type | Location | Committed? | Severity | Value Pattern (redacted) |
|---|-------------|----------|------------|----------|--------------------------|
| S-01 | SQL SA password | `src/backend/Khadamati.API/appsettings.json` | ✅ Yes | **Critical** | `Password=Khadamati@2024!` |
| S-02 | JWT signing secret | `src/backend/Khadamati.API/appsettings.json` | ✅ Yes | **Critical** | `Khadamati-Super-Secret-Key-...` |
| S-03 | SQL SA password | `docker-compose.yml` | ✅ Yes | **Critical** | `MSSQL_SA_PASSWORD`, healthcheck |
| S-04 | JWT secret | `docker-compose.yml` | ✅ Yes | **Critical** | `Jwt__Secret` env var |
| S-05 | SQL connection | `docker-compose.yml` | ✅ Yes | **Critical** | API `ConnectionStrings__DefaultConnection` |
| S-06 | Dev SQL connection | `appsettings.Development.json` | ✅ Yes | Medium | May contain local Windows auth or SA |
| S-07 | Example SA password | `appsettings.Development.example.json` | ✅ Yes | Low | Example only — acceptable if clearly fake |
| S-08 | Staging example secrets | `.env.staging.example` | ✅ Yes | Low | Placeholder passwords in examples |
| S-09 | Production example secrets | `.env.production.example` | ✅ Yes | Low | `change-me` placeholders |
| S-10 | Bootstrap admin password | `.env.staging.example` | ✅ Yes | Medium | `StagingAdmin@123456` example |
| S-11 | Seeded admin password | `DatabaseSeeder.cs` | ✅ Yes | **High** | `Admin@123456` hardcoded dev seed |
| S-12 | Seeded craftsman password | `DatabaseSeeder.cs` | ✅ Yes | Medium | `Craftsman@123` |
| S-13 | Integration test passwords | `ApiIntegrationTests.cs` | ✅ Yes | Low | Test-only credentials |
| S-14 | SendGrid API key | `appsettings.json` | Empty | — | Placeholder `""` |
| S-15 | Twilio credentials | `appsettings.json` | Empty | — | Placeholder `""` |
| S-16 | Moyasar keys | `appsettings.json` | Empty | — | Placeholder `""` |
| S-17 | FCM server key | `appsettings.json` | Empty | — | Placeholder `""` |
| S-18 | APNS private key | `appsettings.json` | Empty | — | Placeholder `""` |
| S-19 | Firebase API key | `src/android/app/google-services.json` | ✅ Yes | Low | Placeholder `AIzaSyPlaceholder-...` |
| S-20 | SMTP password | `appsettings.json` | Empty | — | `smtp.example.com` host |
| S-21 | Production templates | `appsettings.Production.json` | ✅ Yes | Low | `${JWT_SECRET}` placeholders — correct pattern |
| S-22 | Staging compose secrets | `docker-compose.staging.yml` | ✅ Yes | Low | `${DB_PASSWORD}` env references — correct pattern |
| S-23 | Web API URL | `src/web/.env.development` | ✅ Yes | Low | Public localhost URL — acceptable |

### 9.3 Environment Files & Gitignore Gaps

| File | Gitignored? | Risk |
|------|-------------|------|
| `.env` | ✅ Yes | — |
| `.env.local` | ✅ Yes | — |
| `.env.*.local` | ✅ Yes | — |
| `appsettings.Local.json` | ✅ Yes | — |
| `.env.development` (web) | ❌ **Tracked** | Low (localhost only) |
| `.env.staging` | ❌ Not present (example only) | Would be HIGH if committed with real values |
| `.env.production` | ❌ Not present (example only) | Would be CRITICAL if committed |

**Recommendation:** Add `.env.staging` and `.env.production` to `.gitignore` before first real env file is created.

### 9.4 Hardcoded Credentials in Documentation

| Location | Credential | Action |
|----------|------------|--------|
| This runbook / seed docs | `admin@khadamati.com` / `Admin@123456` | Dev-only; rotate for staging/prod |
| User conversation / LOCAL_DEV_WINDOWS | Same | Document as dev-only |

### 9.5 Attack Surface Notes

| Area | Finding |
|------|---------|
| JWT | Symmetric HS256; secret compromise = full impersonation |
| Rate limiting | Enabled; login 10/min, register 5/min |
| CORS | Permissive in dev (multiple localhost ports) |
| HTTPS | Redirect skipped in Development |
| SQL | `TrustServerCertificate=True` in dev/docker |
| Auth links | `ExposeAuthLinks` can leak reset URLs in dev API responses |
| Webhooks | Moyasar webhook — verify `WebhookSecret` in production |

---

## 10. Secrets Migration Plan

### 10.1 Development — .NET User Secrets

**Goal:** Remove secrets from tracked `appsettings.json` and `appsettings.Development.json`.

#### Step 1 — Initialize User Secrets

```bash
cd src/backend/Khadamati.API
dotnet user-secrets init
```

#### Step 2 — Set secrets locally

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=KhadamatiDb;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true"

dotnet user-secrets set "Jwt:Secret" "<NEW-DEV-JWT-SECRET-MIN-32-CHARS>"

# Optional — override seed admin in dev:
dotnet user-secrets set "Bootstrap:AdminPassword" "<your-local-admin-password>"
```

#### Step 3 — Scrub tracked files (Phase 1 implementation)

Replace `appsettings.json` secrets with placeholders:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Jwt": {
    "Secret": ""
  }
}
```

Keep `appsettings.Development.example.json` as template only.

#### Step 4 — Docker dev

Use `.env` file (gitignored) with `docker compose`:

```bash
# .env (gitignored)
DB_PASSWORD=<new-docker-sa-password>
JWT_SECRET=<new-docker-jwt-secret>
```

Update `docker-compose.yml` to reference `${DB_PASSWORD}` and `${JWT_SECRET}` — **do not hardcode**.

### 10.2 Production — Secret Manager / Environment Variables

**Goal:** Zero secrets in source control; inject at deploy time.

#### ASP.NET Core environment variable mapping

| Secret | Environment Variable |
|--------|---------------------|
| SQL connection | `ConnectionStrings__DefaultConnection` |
| JWT secret | `Jwt__Secret` |
| SendGrid | `Email__SendGrid__ApiKey` |
| Twilio | `Sms__Twilio__AccountSid`, `Sms__Twilio__AuthToken` |
| Moyasar | `Payment__Moyasar__SecretKey`, `Payment__Moyasar__WebhookSecret` |
| FCM | `Push__Firebase__ServerKey` |
| APNS | `Push__Apns__PrivateKey`, `Push__Apns__KeyId` |
| Bootstrap admin | `Bootstrap__AdminPassword` |

`appsettings.Production.json` already uses `${JWT_SECRET}` style for some values — ensure deployment pipeline substitutes or override entirely via env vars (env vars take precedence).

#### Platform options (choose one)

| Platform | Service |
|----------|---------|
| Azure | Azure Key Vault + App Service / Container Apps references |
| AWS | AWS Secrets Manager + ECS task definitions |
| Docker / VPS | `.env.production` on host (chmod 600), never in git |
| GitHub Actions | Repository secrets → inject at deploy |

### 10.3 Credential Rotation Plan (Compromised Secrets)

Because S-01 through S-05 are in git history, **rotation is mandatory** even after scrubbing files.

| Priority | Credential | Rotation Steps |
|----------|------------|------------------|
| **P0** | JWT signing secret | 1. Generate new 64-char random secret. 2. Deploy to all API instances. 3. All users must re-login (invalidate refresh tokens). |
| **P0** | SQL SA password | 1. `ALTER LOGIN sa WITH PASSWORD = '<new>'`. 2. Update secret store / User Secrets. 3. Restart API. 4. **Prefer:** disable SA; create `khadamati_app` SQL user with least privilege. |
| **P1** | Docker compose passwords | Update `.env` + recreate containers |
| **P1** | Bootstrap admin | Change via admin UI after first login; remove `Bootstrap:*` from prod config |
| **P2** | Dev seed passwords | Change in `DatabaseSeeder` to random per-environment or env-driven |
| **P2** | Moyasar/SendGrid/etc. | Rotate when real keys are first provisioned |

#### JWT rotation procedure

```
1. Generate: openssl rand -base64 48
2. Set Jwt__Secret in secret store
3. Deploy API (rolling restart)
4. Optional: truncate RefreshTokens table to force re-auth
5. Verify: login flow, admin dashboard, mobile apps
```

#### SQL hardening procedure

```
1. CREATE LOGIN khadamati_app WITH PASSWORD = '<strong>';
2. CREATE USER khadamati_app FOR LOGIN khadamati_app;
3. GRANT SELECT, INSERT, UPDATE, DELETE on schema dbo;
4. DENY access to sys tables
5. Update connection string to use khadamati_app
6. Disable or rename SA account
```

### 10.4 Migration Timeline (Recommended)

| Phase | Action | Code change? |
|-------|--------|--------------|
| **0.2 (now)** | Document secrets (this runbook) | Docs only ✅ |
| **1.0** | User Secrets for dev; scrub `appsettings.json` | Yes |
| **1.1** | `.gitignore` for `.env.staging`, `.env.production` | Yes |
| **1.2** | Docker compose env-var only | Yes |
| **2.0** | Rotate JWT + SQL passwords in all deployed envs | Ops |
| **2.1** | Dedicated SQL app user; disable SA | Ops |
| **3.0** | Secret Manager integration in CI/CD | Ops + IaC |

---

## 11. Monitoring & Troubleshooting

### 11.1 Common Issues

| Symptom | Cause | Fix |
|---------|-------|-----|
| `ERR_CONNECTION_REFUSED` on register | API not running | Start `dotnet run --project Khadamati.API` |
| Swagger won't load | SQL connection failed | Check connection string; set `ContinueOnFailure: true` in dev |
| Vite ports all in use | Multiple `npm run dev` | `taskkill /F /IM node.exe` |
| Git merge on `appsettings.json` | Local edits to tracked file | `git checkout -- appsettings.json`; use `appsettings.Local.json` |
| CORS errors | Origin not in allow list | Add port to `Cors:AllowedOrigins` |
| 401 on admin | Missing/expired token | Re-login as admin |
| Integration test nearby fails | Seed data / geo data | Verify craftsman addresses in DB |

### 11.2 Health Endpoints

```bash
curl http://localhost:5000/api/v1/health
curl http://localhost:5000/api/v1/health/ready
curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/v1/health/integrations
```

### 11.3 Logs

- API: Serilog → console + `logs/` directory
- Docker: `docker logs khadamati-api`

---

## 12. Rollback Procedure

1. Stop API containers / service
2. Restore database from latest `BackupJobs` entry or SQL backup
3. Deploy previous API image / build artifact
4. Verify `/api/v1/health/ready`
5. Smoke test: login, list services, admin dashboard

---

## 13. Related Documents

- [BUSINESS_REQUIREMENTS.md](./BUSINESS_REQUIREMENTS.md)
- [DATABASE_BASELINE.md](./DATABASE_BASELINE.md)
- [API_INVENTORY.md](./API_INVENTORY.md)
- [LOCAL_DEV_WINDOWS.md](./LOCAL_DEV_WINDOWS.md)
- [STAGING.md](./STAGING.md)
- [PRODUCTION.md](./PRODUCTION.md)

---

## 14. Approval & Next Steps

| Item | Owner | Status |
|------|-------|--------|
| Security audit reviewed | Security / Tech Lead | Pending |
| Rotation executed (JWT, SQL) | Ops | Pending |
| User Secrets adopted (dev) | Developers | Pending |
| Phase 1 implementation approved | Product Owner | Pending |

**No application code changes until Phase 0 documents are approved.**
