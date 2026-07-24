# KHADAMATI V1 — Cloud Infrastructure Decision

**Document ID:** KHAD-V1-CLOUD-DECISION  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Pending approval (BLOCKER-004)  
**ADR:** ADR-012 · ADR-024  

**Related:** [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md)

```text
Architecture remains vendor-neutral until this document is approved.
DO NOT modify approved application architecture.
This sheet selects hosting targets only.
```

---

## 1. Portable Target Architecture (Approved — Do Not Change)

```text
Edge (CDN/WAF)
  ├── Admin static
  ├── Store static
  └── API load balancer → API nodes (n)
Worker nodes (schedulers, outbox, notifications, reconcile)
PostgreSQL primary (+ optional read replica)
Redis
Object storage
Secrets manager
Observability stack
```

Environments: Development · Staging · Production (isolated).

---

## 2. Required Decisions

| Decision | Approved value | Notes | State |
|----------|----------------|-------|-------|
| Cloud provider | TBD | e.g. AWS / GCP / Azure / other | ☐ |
| Region | TBD | Prefer Lebanon-proximate / compliance-approved | ☐ |
| Database hosting | TBD | Managed PostgreSQL preferred | ☐ |
| Object storage | TBD | Align with vendor matrix Storage | ☐ |
| Redis hosting | TBD | Required (cache / queues) | ☐ |
| Worker infrastructure | TBD | Same cluster or dedicated workers | ☐ |
| Monitoring | TBD | Metrics + alerting | ☐ |
| Logging | TBD | Centralized, retention-aware | ☐ |
| Backup strategy | TBD | DB PITR + blob versioning outline | ☐ |
| Disaster recovery | TBD | RPO / RTO targets | ☐ |

---

## 3. Environment Matrix

| Env | Cloud account / project | Region | API | Workers | PostgreSQL | Redis | Object storage | Approved |
|-----|-------------------------|--------|-----|---------|------------|-------|----------------|----------|
| Development | TBD | TBD | | | | | | ☐ |
| Staging | TBD | TBD | | | | | | ☐ |
| Production | TBD | TBD | | | | | | ☐ |

---

## 4. Backup & DR Outline

| Asset | Strategy (fill) | RPO | RTO | Drill planned |
|-------|-----------------|-----|-----|---------------|
| PostgreSQL | Daily full + PITR if available | TBD | TBD | ☐ |
| Object storage | Versioning / optional cross-region | TBD | TBD | ☐ |
| Secrets | Dual custody / recovery runbook | — | TBD | ☐ |
| Redis | Ephemeral — rebuild from DB | — | — | N/A |

Restore drill required before production launch (ADR-024).

---

## 5. Observability Outline

| Capability | Tool / service (TBD) | Alerts (min) |
|------------|----------------------|--------------|
| Metrics | TBD | 5xx, latency, payment success, outbox lag, worker failures |
| Logs | TBD | Correlation IDs; webhook auth failures |
| Tracing (optional V1) | TBD | Payment / booking critical paths |

---

## 6. Explicit Non-Goals

- No change to Provider / Listing / Booking / Ledger architecture  
- No new V1 product features via infra choices  
- No hardcoding of business financial rates in infra config  

---

## 7. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| DevOps Lead | | | ☐ Approve |
| Business / Product | | | ☐ Approve |
| Security | | | ☐ Acknowledge |
| Solution Architect | | | ☐ Acknowledge (ADR-024 alignment) |

**BLOCKER-004 closed when §2 decisions are filled and approvals recorded.**

---

**End of Cloud Infrastructure Decision**
