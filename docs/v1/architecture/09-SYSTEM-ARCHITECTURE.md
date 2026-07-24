# 9. System Architecture

**Document ID:** KHAD-V1-SYSARCH  
**Status:** Draft for Approval  

---

## 9.1 Architectural Style

**Modular Monolith + Clean Layered Architecture**, exposed as versioned REST APIs, consumed by four clients.

Rationale for V1:

- Faster consistency for booking/payment/commission transactions
- Simpler ops than premature microservices
- Module boundaries preserve future extraction

## 9.2 Logical Context Diagram

```text
┌──────────────────┐  ┌──────────────────┐
│ Admin Portal     │  │ Store Dashboard  │
│ React + MUI      │  │ React + MUI      │
└────────┬─────────┘  └────────┬─────────┘
         │ REST/JWT            │ REST/JWT
         ▼                     ▼
┌────────────────────────────────────────────┐
│              API Edge (TLS LB)             │
└────────────────────┬───────────────────────┘
                     │
         ┌───────────▼───────────┐
         │  KHADAMATI API        │
         │  Spring Boot 3 / J21  │
         │  Modular Monolith     │
         └───────────┬───────────┘
                     │
     ┌───────────────┼────────────────┐
     ▼               ▼                ▼
┌─────────┐   ┌────────────┐   ┌──────────────┐
│PostgreSQL│  │ Object     │   │ External     │
│+ Flyway  │  │ Storage    │   │ Providers    │
└─────────┘   └────────────┘   └──────────────┘
                                      │
                ┌─────────────────────┼─────────────────────┐
                ▼                     ▼                     ▼
        Areeba IXOPAY          Email/SMS/Push         OCR/Face/Maps
        Payment.js+API
```

```text
┌──────────────────┐  ┌──────────────────┐
│ Customer App     │  │ Craftsman App    │
│ Flutter          │  │ Flutter          │
└────────┬─────────┘  └────────┬─────────┘
         │ REST/JWT + Payment.js (WebView/JS bridge where needed)
         └──────────────┬──────┘
                        ▼
                 KHADAMATI API
```

## 9.3 C4 Container View (Containers)

| Container | Technology | Responsibility |
|-----------|------------|----------------|
| `admin-web` | React/TS/MUI | Administration Portal |
| `store-web` | React/TS/MUI | Store Dashboard |
| `customer-app` | Flutter | Customer mobile |
| `craftsman-app` | Flutter | Craftsman mobile |
| `api` | Spring Boot | Business APIs, auth, workflows |
| `worker` (optional same artifact, separate process) | Spring Boot | Scheduled jobs, async consumers |
| `db` | PostgreSQL | System of record |
| `object-store` | S3-compatible TBD | Media/documents |
| `observability` | TBD stack | Logs/metrics/traces |

> V1 may run API+worker in one deployable with profiles, or two replicas of same image with different command — see Q-DEP-002.

## 9.4 Key Architectural Principles

1. **Domain purity:** business rules in domain services; adapters at edges  
2. **API-first contracts:** OpenAPI is source of truth for client generation where practical  
3. **Idempotent money APIs**  
4. **Event-driven side effects** via transactional outbox  
5. **Least privilege RBAC**  
6. **PII minimization** and PCI scope reduction via Payment.js  
7. **Config over code** for rates, templates, thresholds  
8. **Readiness for i18n / multi-currency / multi-region** without implementing all markets on day one  

## 9.5 Synchronous vs Asynchronous

| Interaction | Mode |
|-------------|------|
| CRUD, booking create, auth | Synchronous REST |
| Payment debit initiate | Synchronous to gateway + async callback finalization |
| Notifications | Async via outbox + dispatcher |
| OCR / face matching | Async job with status polling/push |
| Reminders / surveys / scoring | Scheduled jobs |
| Reporting heavy aggregates | Async materialization or on-read with caching |

## 9.6 Scalability Model

- Stateless API instances horizontally scaled  
- PostgreSQL primary + optional read replica for reporting  
- Connection pooling (HikariCP)  
- Cache (Redis) — **required** for rate-limits, distributed locks, feature flags (ADR-012)  
- Object storage for binary load off DB  
- Background **worker** process required for notifications, schedules, payment reconciliation  

## 9.7 Resilience

- Timeouts/retries with jitter on provider calls  
- Circuit breakers for OCR/SMS/email/face providers  
- Webhook signature validation + replay protection  
- Graceful degradation: notification channel failure must not roll back completed booking payment  

## 9.8 Security High Level

See [20-SECURITY-ARCHITECTURE.md](../standards/20-SECURITY-ARCHITECTURE.md). TLS everywhere, JWT auth, RBAC, secrets in env/secret manager, OWASP controls.

## 9.9 Environment Topology

`local` → `dev` (optional) → `staging` → `production`

Each environment has isolated DB, keys, and IXOPAY credentials.

## 9.10 Questions Requiring Business Decision

- Q-DEP-001 cloud provider  
- Q-DEP-002 worker process strategy  
- Q-DEP-003 Redis (or equivalent) in V1?
