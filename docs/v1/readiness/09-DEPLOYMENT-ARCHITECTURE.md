# Deployment Architecture (Production Preparation)

**Document ID:** KHAD-V1-DEPLOY-FINAL  

---

## Environments

| Env | Purpose |
|-----|---------|
| Development | Local/compose; sandbox IXOPAY |
| Testing / Staging | Integration, QA, UAT |
| Production | Live Lebanon market |

Isolated: DB, Redis, object storage, secrets, IXOPAY credentials, push projects.

---

## Runtime Topology

```text
CDN/WAF → Admin Web (static) + Store Web (static)
        → API LB → API nodes (stateless)
                 → Worker nodes (schedulers, outbox, reconcile, notifications)
PostgreSQL (primary + backups; read replica optional)
Redis (cache, rate-limit, locks, queues support)
Object Storage (media/KYC)
Observability (logs, metrics, traces)
```

---

## Components

| Component | Tech |
|-----------|------|
| Backend API | Spring Boot containers |
| Workers | Same image, worker command |
| Admin/Store | Nginx static |
| Mobile | Flutter apps → API |
| DB | Managed PostgreSQL preferred |
| Cache | Redis **required** |
| Queue | Redis/broker via workers + outbox |
| Storage | S3-compatible |
| Payments | Areeba IXOPAY |

Cloud vendor: **Blocked** pending Q-DEP-001 (portable architecture ready).

---

## Monitoring & Logging

- Structured JSON logs + correlation IDs  
- Metrics: API latency, payment success, outbox lag, worker failures  
- Alerts: webhook auth failures, payment reconcile gaps, 5xx spikes  

---

## Backup & DR

| Item | Requirement |
|------|-------------|
| DB backups | Automated daily + PITR if available |
| Backup test | Restore drill before prod launch |
| RPO/RTO | **Blocked** — set Q-DEP-004 |
| Secrets | Secret manager; rotation runbooks |
| Redis | Ephemeral OK; not system of record |

---

## CI/CD

PR checks → staging deploy → manual prod approval → Flyway migrate job → smoke.
