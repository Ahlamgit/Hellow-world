# ADR-024: Cloud Infrastructure & Disaster Recovery

**Status:** Accepted (vendor-neutral) — 2026-07-24  

---

## Decision

Document hosting, DB, backups, monitoring, logging, DR, and scaling **without locking vendors** until Business/DevOps approval (Q-DEP-001).

Architecture remains portable across major clouds.

---

## Hosting Architecture

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

Environments: Development · Testing/Staging · Production (isolated).

---

## Database Hosting Strategy

- Managed PostgreSQL preferred  
- Automated backups + point-in-time recovery where available  
- Flyway migrations via dedicated migrate job in prod  

---

## Backups

| Asset | Strategy |
|-------|----------|
| PostgreSQL | Daily full + continuous WAL/PITR if offered |
| Object storage | Versioning / cross-region optional later |
| Secrets | Dual custody / documented recovery |
| Redis | Ephemeral — rebuild from DB |

Restore drill required before production launch.

---

## Monitoring & Logging

- Structured logs + correlation IDs  
- Metrics: latency, error rate, payment success, outbox lag, worker failures  
- Alerts on 5xx, webhook auth failures, reconcile gaps  

---

## Disaster Recovery Approach

| Item | Stance |
|------|--------|
| RPO/RTO | Set numerically by Business (config target); architecture supports backup/restore and multi-AZ when cloud allows |
| Failover | Promote DB replica / restore; redeploy API/worker images |
| Region | Single primary region for Lebanon V1; Market model enables future regional deploy |

**Do not hardcode a cloud brand in domain code.**

---

## Scaling Strategy

- Horizontal scale API (stateless)  
- Scale workers independently  
- Redis for rate-limit/locks  
- DB vertical then read-replica for reporting  
- Object storage for media offload  

---

## Consequences

Implementation uses IaC templates parameterized by env; vendor choice is a deployment decision, not an application rewrite.
