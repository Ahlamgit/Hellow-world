# KHADAMATI — Scalability Plan

**Document type:** Capacity and architecture design  
**Status:** Draft — awaiting approval  
**Target:** 5,000 concurrent users (CCU) without failure; 50,000+ registered users within 2 years

---

## 1. Definitions

| Term | Meaning for KHADAMATI |
|------|------------------------|
| **Registered users** | Accounts in `Users` table |
| **Concurrent users (CCU)** | Simultaneous active sessions making API requests |
| **Peak ratio** | Typical marketplace: 5–15% of DAU concurrent at peak |
| **DAU/MAU** | Assumed 20–30% of registered users monthly active |

### Capacity targets

| Milestone | Registered | Est. peak CCU | Design headroom |
|-----------|------------|---------------|-----------------|
| MVP launch | 5,000 | 250–500 | 2× |
| Growth | 15,000 | 750–1,500 | 2× |
| Scale | 50,000 | 2,500–5,000 | **1.5× → target 5K CCU** |
| Year 2 | 100,000+ | 5,000–10,000 | Phase 4 expansion |

**Primary SLA target:** Sustain **5,000 CCU** with p95 API latency < 500 ms for read endpoints and < 1 s for writes, error rate < 0.1%.

---

## 2. Current baseline (as-is)

| Component | Current state | Scale limit |
|-----------|---------------|-------------|
| API | Single ASP.NET Core 8 instance (Docker) | ~500–1,000 CCU without tuning |
| Database | SQL Server single instance | Bottleneck under write-heavy booking peaks |
| Cache | **None** (no Redis) | Repeated catalog/profile reads hit DB |
| Background jobs | `TokenCleanupService` only | No queue for email/SMS/push batch |
| Search | SQL `LIKE` / EF queries | Degrades with catalog growth |
| Blob storage | URLs in DB; no dedicated object store | Not production-ready at scale |
| Monitoring | Serilog file + console | No APM or centralized metrics |
| Load balancer | None in dev compose | Required for production |

---

## 3. Architecture by scale tier

### 3.1 Tier 1 — MVP (500 CCU, 5K registered)

```
[Mobile/Web] → [Single API instance] → [SQL Server]
                      ↓
                 [Serilog files]
```

**Actions:**
- Connection pooling (`Min Pool Size=5; Max Pool Size=100`)
- EF Core query optimization (no N+1 on booking lists)
- Response compression (gzip/brotli)
- Rate limiting (already via AspNetCoreRateLimit)
- Health checks + Docker restart policy

**Sufficient for:** Internal testing, soft launch.

---

### 3.2 Tier 2 — Growth (1,500 CCU, 15K registered)

```
                    ┌─────────────┐
[Clients] ────────► │   Nginx     │
                    │  (reverse   │
                    │   proxy)    │
                    └──────┬──────┘
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
         [API pod 1]  [API pod 2]  [API pod 3]
              │            │            │
              └────────────┼────────────┘
                           │
              ┌────────────┴────────────┐
              ▼                         ▼
        [SQL Server]              [Redis]
        (primary)            cache + sessions
```

**Add:**
| Component | Purpose |
|-----------|---------|
| **Redis** | Distributed cache (catalog, subscription plans), refresh token/session blacklist, rate limit store |
| **2–3 API replicas** | Horizontal scale behind load balancer |
| **Read optimization** | Indexes on `ServiceRequests(Status, CustomerId)`, `Notifications(UserId, IsRead)` |
| **CDN** | Static web assets |

---

### 3.3 Tier 3 — Scale (5,000 CCU, 50K registered) — **PRIMARY TARGET**

```
                         [CDN]
                           │
[Mobile/Web] ──────► [Load Balancer / WAF]
                           │
         ┌─────────────────┼─────────────────┐
         ▼                 ▼                 ▼
    [API × N]         [API × N]         [API × N]
    (stateless)       (stateless)       (stateless)
         │                 │                 │
         └─────────────────┼─────────────────┘
                           │
         ┌─────────────────┼─────────────────┬──────────────┐
         ▼                 ▼                 ▼              ▼
   [SQL Primary]    [SQL Read Replica]   [Redis Cluster]  [Blob Storage]
                           │                                  │
                           │                            [Azure/AWS S3]
                           ▼
                  [Background Workers]
                  (Hangfire / Azure Functions)
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
          [Email]       [SMS]        [Push FCM/APNs]
```

#### Application layer

| Pattern | Implementation |
|---------|----------------|
| Stateless API | JWT; no in-memory session affinity |
| Horizontal scale | 4–8 API instances (2 vCPU, 4 GB each) |
| Connection limits | Max 100 connections per instance → pool to SQL |
| MediatR handlers | Keep thin; heavy work → background queue |
| Idempotency | Payment confirm, booking create — idempotency keys |

#### Database layer

| Strategy | Detail |
|----------|--------|
| **Indexes** | Geo index on craftsman addresses; filtered indexes on active bookings |
| **Read replica** | Route catalog, service list, public plans to replica |
| **Partitioning** | Consider `Notifications`, `ActivityLogs`, `LoginHistory` by month (year 2) |
| **RowVersion** | Optimistic concurrency on bookings (planned Phase 1A) |
| **Backup** | Full daily + log every 15 min; RPO < 15 min |

**SQL Server sizing (50K users, 5K CCU):**
- Primary: 8 vCPU, 32 GB RAM, Premium SSD
- Replica: 4 vCPU, 16 GB RAM (read-only)
- Estimated DTU/vCore utilization at peak: 60–70%

#### Redis layer

| Use case | TTL | Key pattern |
|----------|-----|-------------|
| Service catalog | 5–15 min | `catalog:services:{categoryId}` |
| Subscription plans | 30 min | `plans:{role}` |
| Rate limiting | 1 min sliding | AspNetCoreRateLimit Redis store |
| Refresh token rotation lock | 30 s | `refresh:{userId}` |
| Nearby craftsmen (optional) | 2 min | `geo:{lat}:{lng}:{serviceId}` |

**Redis sizing:** 2 GB single node → 6 GB cluster at 5K CCU.

#### Background jobs

| Job | Trigger | Queue |
|-----|---------|-------|
| Email verification | Registration | High priority |
| SMS OTP | Phone verify | High priority |
| Push notification dispatch | Booking status change | Medium |
| Subscription renewal reminders | Cron daily | Low |
| Report generation | Admin request | Low |
| Token/session cleanup | Cron hourly | Low |

**Technology:** Hangfire (SQL-backed, simple) or Azure Service Bus + Functions (cloud-native).

#### Caching strategy

| Data | Cache | Invalidation |
|------|-------|--------------|
| Service categories | Redis + CDN | Admin catalog change event |
| Craftsman directory (public) | Redis | Profile update webhook |
| User profile | Short TTL (60s) | On PUT `/users/me` |
| Bookings list | **No cache** (must be fresh) | — |

#### Search

| Phase | Approach |
|-------|----------|
| MVP | SQL full-text on `Services.NameEn/NameAr`, craftsman specialization |
| Scale (50K+) | **Elasticsearch** or Azure Cognitive Search for craftsmen/stores/geo |

#### Blob storage

| Content | Store |
|---------|-------|
| Profile pictures | S3 / Azure Blob + CDN |
| Verification documents | Private blob + SAS URLs |
| Service images | Public blob + CDN |

---

### 3.4 Tier 4 — Year 2 (10,000 CCU, 100K registered)

- Multi-region read replicas (if expanding beyond Lebanon)
- Kubernetes (AKS/EKS) with HPA on CPU + request rate
- Event-driven architecture (booking events → Service Bus → notification workers)
- Database sharding **not required** at 100K users for this domain
- Dedicated search cluster

---

## 4. Load estimates (5,000 CCU)

### Request profile (assumed)

| Endpoint group | % of traffic | RPS @ 5K CCU |
|----------------|--------------|--------------|
| Auth (login/refresh) | 5% | ~25 |
| Services/catalog (read) | 30% | ~150 |
| Bookings (read/write) | 25% | ~125 |
| Notifications/chat | 20% | ~100 |
| Profile/addresses | 10% | ~50 |
| Other | 10% | ~50 |
| **Total** | 100% | **~500 RPS** |

ASP.NET Core 8 on 4× (2 vCPU) instances: comfortable at ~500 RPS with caching.

### Database connections

- 8 instances × 50 pool = 400 max connections
- SQL Server handles 400–500 with tuning
- Use **read replica** to offload 40% of reads

---

## 5. Monitoring and logging

| Layer | Tool (recommended) |
|-------|-------------------|
| APM | Application Insights or Datadog |
| Logs | Serilog → Seq / ELK / CloudWatch |
| Metrics | Prometheus + Grafana |
| Uptime | Health endpoint `/api/v1/health` + synthetic checks |
| Alerts | p95 latency, 5xx rate, SQL CPU, Redis memory, queue depth |

### Key dashboards

- Requests per second by endpoint
- Booking create success/failure rate
- Payment webhook processing lag
- Push notification delivery rate
- Active SQL connections
- Redis hit ratio (target > 80% for catalog)

---

## 6. Failure modes and mitigations

| Failure | Impact | Mitigation |
|---------|--------|------------|
| SQL primary down | Total outage | Failover to secondary (Always On AG) |
| Redis down | Cache miss storm | Circuit breaker; degrade to DB; Redis HA |
| API instance crash | Partial | LB removes unhealthy instance |
| Payment webhook delay | Stuck bookings | Idempotent webhook handler + reconciliation job |
| Push provider outage | No notifications | Queue + retry; in-app notifications still work |
| Booking race (double slot) | Data integrity | RowVersion + slot reservation table (Phase 1A) |

---

## 7. Capacity checklist for 5K CCU

- [ ] 4+ stateless API instances behind load balancer
- [ ] Redis for cache + distributed rate limiting
- [ ] SQL Server with read replica + proper indexes
- [ ] Blob storage for media (not local disk)
- [ ] Background job queue for async work
- [ ] APM + centralized logging
- [ ] Automated backups + tested restore
- [ ] WAF / DDoS protection on public endpoints
- [ ] Load test validating 5K CCU / 500 RPS

---

## 8. Related documents

- `HOSTING_ARCHITECTURE.md` — environment topology
- `ROADMAP.md` — when to implement each tier
- `FINAL_RECOMMENDATION.md` — technology decisions
