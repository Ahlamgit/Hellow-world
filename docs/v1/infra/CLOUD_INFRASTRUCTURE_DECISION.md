# KHADAMATI V1 — Cloud Infrastructure Decision

**Document ID:** KHAD-V1-CLOUD-DECISION  
**Version:** 1.1  
**Date:** 2026-07-24  
**Role:** Cloud Architect  
**BLOCKER-004 status:** **IN PREPARATION**  
**ADR:** ADR-012 · ADR-024  

**Sources:** Master Prompt v1.0 · Decisions Complete · Scope Baseline · Implementation Readiness Plan · ADR-024  

**Related:** [`CLOUD_SIZING_AND_COST_FRAMEWORK.md`](./CLOUD_SIZING_AND_COST_FRAMEWORK.md) · [`PRODUCTION_OPERATIONS_READINESS.md`](./PRODUCTION_OPERATIONS_READINESS.md) · [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md) · [`../vendors/VENDOR_EVALUATION_MATRIX.md`](../vendors/VENDOR_EVALUATION_MATRIX.md) (Storage)

```text
DO NOT write production code.
DO NOT deploy infrastructure.
DO NOT create cloud resources.
DO NOT modify application architecture.

Vendor-neutral until Business/DevOps approve selections.
Unknown decisions: Pending Infrastructure Approval / Pending Infrastructure Decision.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Application architecture | **APPROVED** (do not change) |
| Scope | **FROZEN** |
| Implementation gate | **B) NOT READY — CODING BLOCKED** |
| BLOCKER-004 | **IN PREPARATION** |
| Cloud provider selection | **Pending Infrastructure Approval** |
| RPO / RTO numeric targets | **Pending Infrastructure Decision** |

---

## Portable Target Architecture (Approved — Do Not Change)

```text
Edge (CDN/WAF)
  ├── Admin static
  ├── Store static
  └── API load balancer → API nodes (n)
Worker nodes (schedulers, outbox, notifications, reconcile, settlement)
PostgreSQL primary (+ optional read replica)
Redis
Object storage
Secrets manager
Observability stack
```

---

# 1. Required Platform Components

## 1.1 Application Layer

| Component | Requirement | Vendor lock |
|-----------|-------------|-------------|
| Backend API services | Stateless Spring Boot API behind load balancer; horizontal scale | None — container/VM portable |
| Web portals | Admin + Store static/SPA hosting + CDN | Portable |
| Mobile backend services | Same API platform (no separate mobile-only backend required for V1) | Portable |

Selection of host compute product: **Pending Infrastructure Approval**

## 1.2 Database

| Requirement | Stance |
|-------------|--------|
| Engine | PostgreSQL (approved stack) |
| Transaction consistency | ACID; primary handles OLTP writes |
| Backup strategy | Automated backups + PITR where offered (see §4) |
| Encryption | At rest + in transit (TLS) |
| Scaling approach | Vertical first; read replica for reporting/read-heavy later |
| Migrations | Flyway via dedicated migrate job in Production |
| Hosting product | Managed PostgreSQL preferred — **Pending Infrastructure Approval** |

## 1.3 Cache (Redis)

| Requirement | Stance |
|-------------|--------|
| Session / cache usage | Cache, rate limits, short-lived coordination |
| Queue support | Backing for workers / outbox consumers as designed (ADR-012) |
| Performance optimization | Locks, hot keys, ephemeral data — not system of record |
| Hosting | **Pending Infrastructure Approval** |
| Durability | Treat as ephemeral; rebuild from DB if lost |

## 1.4 Background Workers

Required for:

| Workload |
|----------|
| Notifications |
| Payment processing / reconcile jobs |
| Webhook retries |
| Scheduled tasks |
| Settlement processing |

| Requirement | Stance |
|-------------|--------|
| Scale | Independently from API |
| Hosting | Same cluster or dedicated worker pool — **Pending Infrastructure Approval** |
| Failure handling | Retry + dead-letter / alert; idempotent consumers |

## 1.5 Object Storage

| Content | Requirement |
|---------|-------------|
| Provider documents | Encrypted; access-controlled |
| Identity verification files | Encrypted; restricted access; retention-aware |
| Service images | Encrypted; CDN optional |
| Attachments | Encrypted; access-controlled |

| Control | Stance |
|---------|--------|
| Encryption | At rest (provider-managed or CMEK — **Pending Infrastructure Approval**) |
| Access control | Least privilege; no public buckets for KYC |
| Retention | Align with Compliance BLOCKER-006 / ADR-022 |
| Vendor | Align with BLOCKER-003 Storage — **Pending Infrastructure Approval** |

---

# 2. Environment Strategy

```text
Development
  → Testing / Staging
  → Production
```

| Environment | Purpose | Isolation |
|-------------|---------|-----------|
| Development | Engineering | Separate project/account preferred |
| Testing / Staging | Pre-prod, finance dry-run, payment sandbox webhooks | Isolated from Production data |
| Production | Live Lebanon launch | Strict isolation |

Each environment **requires**:

| Control | Confirm |
|---------|---------|
| Isolation (network/data/accounts as practical) | ☑ Required |
| Configuration separation (no shared prod secrets) | ☑ Required |
| Secrets management (dedicated secret store / paths per env) | ☑ Required |

Account/project IDs and regions: **Pending Infrastructure Approval**

---

# 3. Security Requirements

| Area | Requirement |
|------|-------------|
| Network security | Private DB/Redis where offered; security groups/firewall; WAF/CDN at edge |
| TLS | TLS for all external and internal service traffic where supported |
| Secrets management | No secrets in git/images; rotation process — product **Pending Infrastructure Approval** |
| Access control | IAM least privilege; MFA for cloud consoles; Admin app MFA (ADR-006) separate |
| Logging | Centralized, structured, correlation IDs; retention-aware |
| Monitoring | Health + alerts (see §6) |
| Vulnerability management | Image scanning, dependency advisories, patch cadence — process **Pending Infrastructure Approval** |

Security review sign-off: ☐ Pending (checklist §9)

---

# 4. Backup & Disaster Recovery

### Required

| Capability | Stance |
|------------|--------|
| Database backups | Daily full + continuous WAL/PITR if available |
| Storage backups | Versioning; cross-region optional later |
| Restore testing | **Required** before production launch (ADR-024) |
| Recovery objectives | Numeric targets below |

### Recovery objectives

| Metric | Value |
|--------|-------|
| **RPO** | **Pending Infrastructure Decision** |
| **RTO** | **Pending Infrastructure Decision** |

| Asset | Strategy | RPO | RTO | Drill |
|-------|----------|-----|-----|-------|
| PostgreSQL | Managed backup + PITR | Pending Infrastructure Decision | Pending Infrastructure Decision | ☐ |
| Object storage | Versioning / optional CRR | Pending Infrastructure Decision | Pending Infrastructure Decision | ☐ |
| Secrets | Dual custody + runbook | — | Pending Infrastructure Decision | ☐ |
| Redis | Ephemeral rebuild from DB | — | — | N/A |

Failover stance: promote replica / restore DB; redeploy API/worker images. Single primary region for Lebanon V1; future regional deploy via Market model.

---

# 5. Scalability Requirements

Architecture must support:

```text
Lebanon launch
  → Regional expansion
  → International markets
```

| Concern | Approach |
|---------|----------|
| Horizontal scaling | Stateless API replicas |
| Load balancing | Edge / L7 LB in front of API |
| Worker scaling | Independent worker pools |
| Database scaling | Vertical → read replicas for read/report load |
| Object storage | Offload media; CDN for public assets |
| Multi-market | Config/Market-driven; not single-tenant hardcode |

Concrete instance sizes / autoscaling policies: **Pending Infrastructure Approval**

---

# 6. Observability

### Required monitoring

| Domain | Signals |
|--------|---------|
| Application health | Liveness/readiness; process up |
| API performance | Latency, error rate, saturation |
| Database performance | Connections, slow queries, replication lag |
| Queue failures | Outbox lag, consumer errors, DLQ depth |
| Payment failures | Debit/webhook/reconcile failures |
| Security events | Auth failures, webhook auth failures, admin MFA anomalies |

| Capability | Tool / service |
|------------|----------------|
| Metrics | **Pending Infrastructure Approval** |
| Logging | **Pending Infrastructure Approval** |
| Tracing (optional V1) | **Pending Infrastructure Approval** |
| Alerting | Min: 5xx, payment success drop, outbox lag, worker failures, webhook auth failures |

---

# 7. Deployment Model

| Requirement | Stance |
|-------------|--------|
| CI/CD | Automated build/test/deploy pipelines (portable) |
| Automated deployments | Staging continuous or gated; Production gated |
| Rollback strategy | Previous image/version redeploy; DB migrations forward-only with care |
| Version management | Immutable artifact tags; environment promotion |
| IaC | Parameterized templates by env — cloud brand not in domain code |
| Prod migrate | Dedicated Flyway migrate job |

Pipeline vendor/tooling: **Pending Infrastructure Approval**

---

# 8. Vendor Neutrality

| Rule | Confirm |
|------|---------|
| Architecture must **not** depend on one cloud provider | ☑ |
| Domain/application code must not hardcode a cloud brand | ☑ |
| Selection remains pending Business/DevOps approval | ☑ |

Possible future providers may include (non-exhaustive, not selected):

- AWS  
- Azure  
- Google Cloud  
- Other approved providers  

**Selection remains pending** — **Pending Infrastructure Approval**

---

# 9. Approval Checklist

| Area | Status |
|------|--------|
| Cloud provider | Pending |
| Database hosting | Pending |
| Redis hosting | Pending |
| Storage | Pending |
| Backup strategy | Pending |
| DR strategy | Pending |
| Security review | Pending |

Additional pending items: Region · Worker hosting · Monitoring/logging products · CI/CD tooling · RPO/RTO numerics

---

# 10. Environment Decision Matrix (fill on approval)

| Env | Cloud account / project | Region | API | Workers | PostgreSQL | Redis | Object storage | Approved |
|-----|-------------------------|--------|-----|---------|------------|-------|----------------|----------|
| Development | Pending Infrastructure Approval | | | | | | | ☐ |
| Testing / Staging | Pending Infrastructure Approval | | | | | | | ☐ |
| Production | Pending Infrastructure Approval | | | | | | | ☐ |

---

# 11. Explicit Non-Goals

- No change to Provider / Listing / Booking / Ledger application architecture  
- No new V1 product features via infra choices  
- No hardcoding of business financial rates in infra config  
- No production resource creation from this document  

---

# 12. Criteria to mark BLOCKER-004 COMPLETED

- [ ] Approval checklist §9 areas decided (not Pending)  
- [ ] RPO / RTO set (not Pending Infrastructure Decision)  
- [ ] DevOps + Business (+ Security acknowledge) signatures below  
- [ ] Align Storage choice with BLOCKER-003 where applicable  

Until then: keep **IN PREPARATION**.

### Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| DevOps Lead | | | ☐ Approve |
| Business / Product | | | ☐ Approve |
| Security | | | ☐ Acknowledge |
| Solution Architect | | | ☐ Acknowledge (ADR-024 alignment) |

---

**End of Cloud Infrastructure Decision v1.1**
