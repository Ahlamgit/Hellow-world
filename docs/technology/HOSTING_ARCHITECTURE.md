# KHADAMATI — Hosting Architecture

**Document type:** Infrastructure and deployment design  
**Status:** Draft — awaiting approval

---

## 1. Overview

KHADAMATI is deployed as a **containerized ASP.NET Core API**, **React SPA** (nginx), and **SQL Server**, with external integrations for payments (Moyasar), push (FCM/APNs), email (SendGrid), and SMS (Twilio).

This document defines recommended hosting for **development**, **testing**, **production**, **high availability**, **backups**, **disaster recovery**, and **cost optimization**.

---

## 2. Environment topology

### 2.1 Development (local)

| Component | Setup |
|-----------|--------|
| API | `dotnet run` on `localhost:5000` or Docker Compose |
| Web | Vite dev server `localhost:5173` |
| SQL Server | Docker `mcr.microsoft.com/mssql/server:2022` or local SQL Server 2014+ |
| Mobile | Emulator → `10.0.2.2:5000` (Android) / LAN IP (device) |
| Redis | Optional — not required for local dev |
| Secrets | `appsettings.Local.json` (gitignored) |

**Cost:** $0 (developer machines)

---

### 2.2 Testing / Staging

Purpose: integration testing, QA, mobile beta, pre-production validation.

```
[staging.khadamati.com]
        │
   [Nginx reverse proxy]
        │
   ┌────┴────┐
   ▼         ▼
[API × 1]  [Web SPA]
   │
   ▼
[SQL Server]     [Redis (small)]
```

| Setting | Value |
|---------|-------|
| API replicas | 1–2 |
| SQL | Managed instance (small tier) or VM |
| Redis | 1 GB cache |
| Seed data | Demo users enabled |
| TLS | Let's Encrypt |
| Integrations | Sandbox keys (Moyasar test, FCM dev) |
| Migrations | Manual or CI job before deploy |

**Reference:** `docs/STAGING.md`, `docker-compose.staging.yml`

**Estimated cost:** $150–300/month (single cloud region)

---

### 2.3 Production

```
                    [Cloudflare / Azure Front Door]
                              │
                         [CDN static]
                              │
                    [WAF + Load Balancer]
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
         [API pod]       [API pod]       [API pod]
              │               │               │
              └───────────────┼───────────────┘
                              │
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
  [SQL Primary]        [SQL Replica RO]         [Redis HA]
  (Always On)                                    │
         │                                         │
         ▼                                         ▼
  [Automated backups]                      [Blob storage]
                                                    │
                                              [CDN for media]
```

| Component | Recommendation |
|-----------|----------------|
| **Cloud** | Azure (strong .NET/SQL affinity) or AWS |
| **API hosting** | Azure App Service (Linux containers) or AKS |
| **Web** | Static hosting on Blob + CDN, or nginx sidecar |
| **SQL** | Azure SQL Managed Instance or SQL Server on VM with Always On |
| **Redis** | Azure Cache for Redis (Standard C1+) |
| **Secrets** | Azure Key Vault / AWS Secrets Manager |
| **DNS** | Cloudflare |

**Estimated cost at 5K CCU:** $900–1,700/month (see `SCALABILITY_PLAN.md`)

---

## 3. High availability

| Layer | HA strategy | Target uptime |
|-------|-------------|---------------|
| API | 3+ instances, health probes, rolling deploy | 99.9% |
| Web (SPA) | CDN multi-edge (inherent HA) | 99.99% |
| SQL Server | Always On Availability Group (1 primary + 1 sync secondary) | 99.95% |
| Redis | Primary + replica, automatic failover | 99.9% |
| Blob storage | Zone-redundant storage (ZRS) | 99.99% |

### Deployment strategy

| Practice | Detail |
|----------|--------|
| Blue/green or rolling | Zero-downtime API deploys |
| Migration job | Run EF migrations **before** traffic shift (not on every replica startup in prod) |
| Feature flags | Gradual mobile/web feature rollout |
| Circuit breakers | On external integrations (Moyasar, SendGrid) |

---

## 4. Backups

| Asset | Frequency | Retention | RPO | RTO |
|-------|-----------|-----------|-----|-----|
| SQL full backup | Daily | 30 days | 24 h | 4 h |
| SQL log backup | Every 15 min | 7 days | 15 min | 2 h |
| Blob storage | Geo-redundant | 90 days | Near-zero | 1 h |
| Redis | Snapshot daily | 7 days | 24 h | 30 min (cache rebuild acceptable) |
| Config/secrets | Versioned in vault | Indefinite | 0 | 15 min |

### Backup verification

- Monthly restore drill to isolated environment
- Automated alert if backup job fails
- `BackupJobs` admin table tracks job history (already in schema)

---

## 5. Disaster recovery

| Scenario | Response |
|----------|----------|
| Region outage | Failover to secondary region (warm standby SQL replica + DNS switch) — Phase 5 |
| Data corruption | Point-in-time restore from log backups |
| Ransomware | Immutable backup copies (Azure Backup vault) |
| Secret compromise | Rotate JWT secret (forces re-login), rotate API keys |

**DR RTO target:** 4 hours  
**DR RPO target:** 15 minutes (with log backups)

---

## 6. CDN and file storage

| Content type | Storage | Delivery |
|--------------|---------|----------|
| Web SPA (JS/CSS) | Blob / object store | CDN (cache 1 year, hash in filename) |
| Service images | Blob public container | CDN |
| Profile photos | Blob private | SAS URL (short TTL) |
| Verification docs | Blob private | SAS URL (admin + owner only) |
| API responses | No CDN | Dynamic — use Redis instead |

**Current gap:** Images stored as URLs without dedicated blob pipeline — **implement before production scale**.

---

## 7. Network and security

| Control | Implementation |
|---------|----------------|
| TLS 1.2+ | Terminate at load balancer |
| WAF | OWASP rules, rate limit at edge |
| Private SQL | VNet integration; no public SQL endpoint |
| Private Redis | VNet only |
| API rate limiting | AspNetCoreRateLimit + Redis store |
| CORS | Restrict to `khadamati.com`, admin subdomain, mobile deep links |
| Secrets | Never in git; Key Vault references |

---

## 8. CI/CD pipeline

| Stage | Actions |
|-------|---------|
| **Build** | `dotnet build`, `dotnet test`, `npm run build` |
| **Scan** | SAST, dependency audit (Dependabot) |
| **Package** | Docker images → container registry |
| **Migrate** | EF `database update` (staging auto, prod manual gate) |
| **Deploy staging** | Auto on `main` merge |
| **Deploy production** | Manual approval + smoke tests |
| **Mobile (future RN)** | EAS Build → TestFlight / Play Internal |

**Reference:** `.github/workflows/ci.yml` (backend + web + Docker build today)

---

## 9. Cost optimization

| Strategy | Saving |
|----------|--------|
| Reserved instances (1-year) for SQL + API | 30–40% |
| Auto-scale API down at night (non-peak) | 20% on compute |
| CDN for static assets | Reduce API/nginx load |
| Redis cache | Reduce SQL DTU by 30–50% on reads |
| Right-size SQL | Start medium; scale on metrics, not guesses |
| Blob lifecycle policies | Move old verification docs to cool tier |
| Single region (Lebanon/MENA) until expansion | Avoid multi-region cost until needed |

### Cost tiers summary

| Environment | Monthly est. |
|-------------|--------------|
| Development | $0 (local) |
| Staging | $150–300 |
| Production MVP | $400–700 |
| Production (5K CCU) | $900–1,700 |
| Production (10K CCU) | $1,500–3,000 |

---

## 10. Lebanon / regional considerations

| Factor | Recommendation |
|--------|----------------|
| Latency | Host in **UAE North** or **Europe West** for Lebanon users |
| Payments | Moyasar or local PSP with USD/LBP support |
| Data residency | Document where PII is stored; consider UAE/EU compliance |
| Power/connectivity | Cloud HA more reliable than on-prem for MVP |

---

## 11. Related documents

- `SCALABILITY_PLAN.md`
- `docs/PRODUCTION.md`
- `docs/DEPLOYMENT_RUNBOOK.md` (if on governance branch)
- `ROADMAP.md`
