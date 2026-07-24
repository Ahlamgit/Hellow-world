# 36. Deployment Architecture

**Document ID:** KHAD-V1-DEP  
**Status:** Draft for Approval  

---

## 36.1 Principles

- Container-first  
- Immutable images  
- Environment parity  
- Horizontal scale for API  
- Secrets outside images  

## 36.2 Logical Production Topology

```text
                    ┌─────────────┐
                    │  CDN/WAF    │
                    └──────┬──────┘
           ┌───────────────┼────────────────┐
           ▼               ▼                ▼
     admin-web        store-web         API LB
     (static)         (static)            │
                                          ▼
                                   API pods/nodes (n)
                                      │        │
                                      ▼        ▼
                                 PostgreSQL   Object Storage
                                      │
                                 Workers/Cron (same image or dedicated)
```

Mobile apps distribute via App Store / Play Store; talk to API endpoint per environment.

## 36.3 Containers

| Image | Contents |
|-------|----------|
| `khadamati-api` | Spring Boot fat jar |
| `khadamati-worker` | Same image, worker command (schedulers, outbox, reconcile) |
| `khadamati-admin` | Nginx + static admin build |
| `khadamati-store` | Nginx + static store build |
| `postgres` | Official Postgres + init |
| `redis` | **Required** (ADR-012) — cache, rate-limit, locks, queues support |

## 36.4 Runtime Options

| Option | Notes |
|--------|-------|
| Docker Compose | Local + simple staging |
| Kubernetes / ECS / Cloud Run | Production scale — choose with Q-DEP-001 |

Architecture remains portable.

## 36.5 Networking & TLS

- TLS terminate at LB/ingress  
- Private DB subnet  
- Egress allowlist to IXOPAY, email/SMS, OCR, face providers  

## 36.6 High Availability Targets (**OPEN**)

RPO/RTO: Q-DEP-004. Minimum V1 recommendation: automated DB backups + multi-AZ when cloud supports.

## 36.7 Migration Runtime

Flyway on API startup **or** init job before traffic switch. Prefer dedicated migrate job in prod to control rollout (Q-DEP-005).
