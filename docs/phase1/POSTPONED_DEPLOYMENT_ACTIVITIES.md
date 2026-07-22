# KHADAMATI — Postponed Deployment Activities

**Document version:** 1.0  
**Date:** 2026-07-22  
**When to execute:** **Before pre-production / first public deployment** — not during Phase 1A–1C implementation waves.

---

## Purpose

This checklist captures **deployment and operations activities** intentionally deferred from Phase 1 implementation. These are not code-feature tasks; they require coordination with hosting, secrets management, and release windows.

**Reminder trigger:** Review this document when:
- Staging environment is declared production-ready
- Go-live date is scheduled
- Security audit sign-off is requested
- First real payment provider keys are provisioned

---

## Deferred Items

### DEP-01: JWT Signing Secret Rotation

| Field | Detail |
|-------|--------|
| **Why postponed** | Requires all users to re-authenticate; no benefit until production credentials exist |
| **Current exposure** | JWT secret committed in `appsettings.json`, `docker-compose.yml` (git history) |
| **Pre-production steps** | 1. Generate 64+ char random secret. 2. Store in secret manager (`Jwt__Secret`). 3. Deploy all API instances simultaneously. 4. Invalidate `RefreshTokens` table (force re-login). 5. Verify login/refresh/admin flows. |
| **Rollback** | Restore previous secret in secret manager; redeploy |
| **Owner** | Ops / Security |
| **Reference** | [DEPLOYMENT_RUNBOOK.md](../DEPLOYMENT_RUNBOOK.md) §10.3 |

---

### DEP-02: SQL Server Password Rotation (SA + App User)

| Field | Detail |
|-------|--------|
| **Why postponed** | Dev/staging use known passwords; rotation breaks local clones until updated |
| **Current exposure** | SA password `Khadamati@2024!` in repo and compose files |
| **Pre-production steps** | 1. Create dedicated `khadamati_app` SQL login (least privilege). 2. `ALTER LOGIN sa WITH PASSWORD = '<new-strong>'` OR disable SA. 3. Update `ConnectionStrings__DefaultConnection` in secret manager. 4. Restart API + verify health/ready. 5. Update DBA runbook. |
| **Rollback** | Revert connection string; re-enable SA if disabled |
| **Owner** | Ops / DBA |
| **Reference** | [DEPLOYMENT_RUNBOOK.md](../DEPLOYMENT_RUNBOOK.md) §10.3 SQL hardening |

---

### DEP-03: Git History Secrets Cleanup

| Field | Detail |
|-------|--------|
| **Why postponed** | Disruptive to all clones; must follow credential rotation |
| **Current exposure** | Secrets exist in historical commits even after file scrub |
| **Pre-production steps** | 1. Complete DEP-01 and DEP-02 first (rotated secrets make history less valuable). 2. Option A: `git filter-repo` / BFG to purge secrets. 3. Option B: accept history risk with rotated credentials (document decision). 4. Force-push policy — coordinate team. 5. Enable GitHub secret scanning / push protection. |
| **Rollback** | Restore from backup remote if filter goes wrong |
| **Owner** | Tech lead + Security |
| **Reference** | Phase 0.2 security audit |

---

### DEP-04: Remove Secrets from Tracked Config Files

| Field | Detail |
|-------|--------|
| **Why postponed** | Phase 1 focuses on data integrity; devs still need local bootstrapping |
| **Files to scrub** | `appsettings.json`, `docker-compose.yml` → placeholders + `.env.example` |
| **Pre-production steps** | 1. Replace inline secrets with `${ENV_VAR}` placeholders. 2. Document User Secrets for dev. 3. Verify CI uses injected secrets. 4. Add CI check blocking secret patterns. |
| **Depends on** | DEP-01, DEP-02 (new secrets in secret manager) |
| **Owner** | Backend lead |

---

### DEP-05: Production Secret Manager Wiring

| Field | Detail |
|-------|--------|
| **Why postponed** | No production environment provisioned yet |
| **Scope** | JWT, SQL, SendGrid, Twilio, Moyasar, FCM, APNS keys |
| **Pre-production steps** | 1. Choose provider (Azure Key Vault / AWS Secrets Manager / host `.env`). 2. Map all `Section__Key` env vars. 3. Configure container/host references. 4. Run integration readiness endpoint with `RequireProductionReady: true`. |
| **Owner** | Ops |
| **Reference** | `appsettings.Production.json`, `docker-compose.prod.yml` |

---

### DEP-06: Bootstrap Admin Password Change

| Field | Detail |
|-------|--------|
| **Why postponed** | Dev seed uses known `admin@khadamati.com` / `Admin@123456` |
| **Pre-production steps** | 1. Set `Bootstrap:AdminPassword` via secret manager for first deploy only. 2. Log in and change password. 3. Remove `Bootstrap:*` from production config. 4. Disable demo seed (`SeedDemoData: false`). |
| **Owner** | Ops |

---

### DEP-07: Enable Production Integration Readiness Gate

| Field | Detail |
|-------|--------|
| **Why postponed** | Dev/staging use Development providers |
| **Config** | `Integrations:RequireProductionReady: true` in Production |
| **Pre-production steps** | 1. Provision real SendGrid, Moyasar, FCM, APNS credentials. 2. Set all keys in secret manager. 3. Verify `GET /health/integrations` reports ready. 4. Block deploy if not ready. |
| **Owner** | Ops + Backend |

---

### DEP-08: HTTPS / TLS Hardening

| Field | Detail |
|-------|--------|
| **Why postponed** | Local dev uses HTTP |
| **Pre-production steps** | 1. Enable HTTPS redirect in Production (`Program.cs`). 2. SQL `Encrypt=True;TrustServerCertificate=False`. 3. TLS certificates on reverse proxy. 4. HSTS headers. |
| **Owner** | Ops |

---

### DEP-09: CI/CD Security Gates

| Field | Detail |
|-------|--------|
| **Why postponed** | Phase 1B focuses on app security; pipeline hardening is ops |
| **Items** | Dependency scanning, CodeQL/SAST, `dotnet list package --vulnerable`, secret scanning on PR |
| **Pre-production steps** | Add GitHub Actions jobs; require pass before merge to `main` |
| **Owner** | DevOps |
| **Reference** | [phase0.5/TEST_COVERAGE_ANALYSIS.md](../phase0.5/TEST_COVERAGE_ANALYSIS.md) |

---

### DEP-10: Complete Android / iOS CI Pipelines

| Field | Detail |
|-------|--------|
| **Why postponed** | Not blocking Phase 1 feature work |
| **Current state** | Android CI stops at `chmod gradlew`; no iOS workflow |
| **Pre-production steps** | 1. `./gradlew assembleDebug test` in CI. 2. Add iOS `xcodebuild` job. 3. Require green mobile builds before release tags. |
| **Owner** | DevOps |

---

## Pre-Production Gate Checklist

Execute **in order** before first production deploy:

```
[ ] DEP-04  Scrub tracked config files (placeholders only)
[ ] DEP-05  Secret manager provisioned and mapped
[ ] DEP-02  SQL app user created; SA disabled or rotated
[ ] DEP-01  JWT secret rotated in production
[ ] DEP-06  Bootstrap admin password changed; bootstrap config removed
[ ] DEP-07  Production integrations verified ready
[ ] DEP-08  HTTPS/TLS enabled end-to-end
[ ] DEP-09  CI security gates active
[ ] DEP-10  Mobile CI pipelines green
[ ] DEP-03  Git history cleanup (optional — after rotation)
```

---

## What Phase 1 *Does* Include (Not Postponed)

For clarity, these security items **are** in Phase 1 implementation waves:

| Item | Wave |
|------|------|
| 401 vs 403 handling | 1B |
| HMAC webhook verification | 1B |
| FluentValidation for critical commands | 1B |
| Admin confirm password | 1B |
| Mobile change-password screens | 1B |
| Booking/slot data integrity | 1A |

---

## Notification Schedule (Suggested)

| Milestone | Action |
|-----------|--------|
| Phase 1C complete | Review this document with tech lead |
| Staging sign-off | Begin DEP-04 through DEP-08 |
| Go-live -14 days | Complete DEP-01, DEP-02, DEP-05, DEP-06, DEP-07 |
| Go-live -7 days | DEP-09, DEP-10 |
| Post go-live | DEP-03 if required by compliance |

---

## Related Documents

- [IMPLEMENTATION_PLAN_PHASE1.md](./IMPLEMENTATION_PLAN_PHASE1.md)
- [../DEPLOYMENT_RUNBOOK.md](../DEPLOYMENT_RUNBOOK.md)
- [../phase0.5/ENTERPRISE_RISK_MATRIX.md](../phase0.5/ENTERPRISE_RISK_MATRIX.md) (R-25 secrets)

---

## Sign-Off (Pre-Production)

| Activity | Completed | Date | By |
|----------|-----------|------|-----|
| DEP-01 JWT rotation | | | |
| DEP-02 SQL rotation | | | |
| DEP-03 Git history | | | |
| DEP-04 Config scrub | | | |
| DEP-05 Secret manager | | | |
| DEP-06 Bootstrap admin | | | |
| DEP-07 Integrations | | | |
| DEP-08 TLS | | | |
| DEP-09 CI security | | | |
| DEP-10 Mobile CI | | | |
