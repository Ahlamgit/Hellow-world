# KHADAMATI — Final Technology Architecture

**Document type:** Approved technology direction (canonical reference)  
**Status:** Approved — awaiting sign-off on this summary document  
**Version:** 1.0  
**Date:** July 2026  
**Supersedes:** Scale and infrastructure assumptions in earlier `docs/technology/*` drafts where they conflict with this document

---

## 1. Purpose

This document records the **approved technology direction** for the KHADAMATI maintenance and home services marketplace platform. It is the single reference for:

- What technologies are in scope and out of scope
- Why each choice was made
- Realistic scaling assumptions (Year 1 and Year 5)
- Deployment and migration strategy
- Cost discipline and risk controls

**No implementation is authorized by this document alone.** Phase 1 (production stabilization) must complete before React Native work begins.

---

## 2. Approved technology stack

### 2.1 Architecture overview

```
┌──────────────────────────────────────────────────────────────────────┐
│                           CLIENT LAYER                                │
├─────────────────────┬──────────────────────┬─────────────────────────┤
│   React Web (TS)    │  Native Mobile (now) │  React Native (future) │
│   MUI + i18next     │  Kotlin + SwiftUI    │  Expo + TypeScript     │
│   Customer + Admin  │  Customer/Craftsman/ │  Same mobile scope     │
│   Portal            │  Store only          │  (post-stabilization)  │
└──────────┬──────────┴──────────┬───────────┴────────────┬────────────┘
           │                     │                        │
           │         REST API v1 (JWT Bearer)             │
           └─────────────────────┼────────────────────────┘
                                 ▼
┌──────────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core 8 LTS API                             │
│   Clean Architecture · MediatR · FluentValidation · AutoMapper        │
│   Serilog · Permission-based RBAC                                     │
└──────────────────────────────┬───────────────────────────────────────┘
                               │
           ┌───────────────────┼───────────────────┐
           ▼                   ▼                   ▼
   ┌───────────────┐   ┌───────────────┐   ┌───────────────┐
   │  SQL Server   │   │ Object Storage│   │ Background    │
   │  (primary)    │   │ (blob + CDN)  │   │ Jobs          │
   └───────────────┘   └───────────────┘   └───────────────┘

   Future (measurement-driven only):
   ┌───────────────┐   ┌───────────────┐
   │    Redis      │   │  Additional   │
   │   (optional)  │   │  API instances│
   └───────────────┘   └───────────────┘
```

---

### 2.2 Backend — KEEP (no replacement)

| Technology | Role | Decision |
|------------|------|----------|
| **ASP.NET Core 8 LTS** | HTTP API host | **KEEP** |
| **Clean Architecture** | Domain / Application / Infrastructure / API | **KEEP** |
| **EF Core 8** | ORM, migrations, repositories | **KEEP** |
| **SQL Server** | Primary transactional database | **KEEP** |
| **JWT** | Stateless authentication | **KEEP** |
| **FluentValidation** | Request validation | **KEEP** |
| **MediatR** | CQRS command/query dispatch | **KEEP** |
| **Serilog** | Structured logging | **KEEP** |
| **AutoMapper** | DTO mapping | **KEEP** |

**Explicitly excluded:** PHP or any alternative backend rewrite. Replacing the backend would introduce unnecessary cost and risk without improving outcomes for the approved business scale.

**Reason:** The current backend is enterprise-grade, already implements 169+ API endpoints, permission-based RBAC, booking, payments, subscriptions, and admin modules. It aligns with targets for transactional integrity, auditability, and reporting.

---

### 2.3 Database — KEEP SQL Server

| Requirement | Approach |
|-------------|----------|
| Stable transactional booking | ACID via SQL Server + EF Core |
| Payment integrity | Booking payments, webhooks, idempotent handlers |
| Subscription management | Plans, billing options, user subscriptions |
| Reporting | SQL views, admin export, activity logs |
| Auditability | Audit logs, security logs, login history |

**Explicitly excluded (unless performance measurements justify):**

- SQL Server Enterprise tier “by default”
- Read replicas
- Sharding or distributed database architectures
- Migration to PostgreSQL, MySQL, or other engines

**Reason:** At Year 1 (~2,250 users) and Year 5 (~10,000 users), a well-indexed SQL Server instance on Standard tier is sufficient. Complexity should follow evidence, not speculation.

---

### 2.4 Web — KEEP (no migration)

| Technology | Role | Decision |
|------------|------|----------|
| **React** | Customer portal + Admin portal UI | **KEEP** |
| **TypeScript** | Type safety | **KEEP** |
| **Material UI (MUI)** | Component library | **KEEP** |
| **i18next** | Arabic / English localization | **KEEP** |
| **RTL** | Right-to-left layout support | **KEEP** |
| **Vite** | Build tooling (current) | **KEEP** |

**Reason:** Web client is production-invested, RTL-capable, and hosts the **Admin Portal** — which must remain web-only (no admin on mobile).

---

### 2.5 Mobile — APPROVED FUTURE DIRECTION (do not start yet)

| Current (interim) | Future (approved) |
|-------------------|-------------------|
| Android: Kotlin + Jetpack Compose | React Native + TypeScript |
| iOS: Swift + SwiftUI | Expo Development Build |
| | EAS Build for store binaries |

**Critical constraint:** **Do not start React Native migration until Phase 1 (production stabilization) is complete.** Native apps continue as the mobile clients during stabilization.

**Mobile scope (all clients):**

- **Customer** — browse, book, pay, chat, profile, addresses, reviews
- **Craftsman** — portal, availability, accept/reject bookings, verification docs, subscriptions
- **Store** — portal, products, requests, notifications
- **Admin** — **not on mobile**; Web Admin Portal only

**Reason for future RN migration:** Single TypeScript codebase, alignment with React web, reduced long-term maintenance of duplicate Kotlin + Swift implementations. Timing is deferred deliberately to avoid destabilizing MVP delivery.

---

## 3. Reason for each technology choice

| Layer | Choice | Primary reasons |
|-------|--------|-----------------|
| API | ASP.NET Core 8 | Performance, LTS support, team investment, Clean Architecture fit |
| Data | SQL Server + EF Core | Transactional bookings, mature tooling, existing schema (43+ tables) |
| Auth | JWT + RBAC permissions | Stateless API scaling, fine-grained admin control |
| Web | React + TS + MUI | Admin + customer UIs built, RTL/i18n done, large hiring pool |
| Mobile (now) | Native Kotlin + Swift | Already implemented (~140 files), MVP-ready, no migration risk during Phase 1 |
| Mobile (future) | RN + Expo + EAS | One codebase, shared TS with web, lower long-term cost |
| Jobs | In-process / simple queue first | Sufficient at 10K users; avoid premature distributed systems |
| Storage | Object storage + CDN | Profile images, verification docs, service media — not on API disk |
| Cache | Application memory first; Redis later | Avoid premature infrastructure; add when metrics show read pressure |

---

## 4. Scaling assumptions (approved)

### 4.1 User growth model

| Horizon | Registered users | Notes |
|---------|------------------|-------|
| **Year 1** | ~2,250 | MVP launch, Lebanon market, word-of-mouth + light marketing |
| **Year 5** | ~10,000 | Established marketplace, craftsmen + stores onboarded |

### 4.2 Concurrent usage (derived, not a hard SLA target)

| Metric | Estimate |
|--------|----------|
| Monthly active users (Year 1) | ~30–40% of registered → ~700–900 MAU |
| Monthly active users (Year 5) | ~35–45% of registered → ~3,500–4,500 MAU |
| Peak concurrent users (Year 1) | ~50–150 |
| Peak concurrent users (Year 5) | ~300–800 |

These figures are **order-of-magnitude planning assumptions**, not capacity guarantees. Load testing should validate before marketing spikes.

### 4.3 What “comfortably supports 10,000 users” means

| Capability | Year 1 | Year 5 |
|------------|--------|--------|
| User registration & auth | ✅ Single API + SQL | ✅ Same |
| Booking create/confirm/pay | ✅ With integrity fixes (Phase 1) | ✅ |
| Subscription billing | ✅ | ✅ |
| Admin reporting | ✅ Web admin | ✅ |
| Push notifications | ✅ FCM + APNs | ✅ |
| File uploads | ✅ Object storage required for prod | ✅ |
| p95 API latency (reads) | < 500 ms | < 500 ms |
| Uptime target | 99.5% (MVP) → 99.9% (mature) | 99.9% |

**The architecture does not require** microservices, Kubernetes, event-driven architecture, read replicas, or Redis **at launch or at 10K users by default**.

---

## 5. Initial infrastructure (right-sized)

### 5.1 Phase 1 — Production stabilization stack

| Component | Specification |
|-----------|---------------|
| **API** | 1–2 ASP.NET Core 8 instances (container or App Service) |
| **SQL Server** | Standard tier (Azure SQL or VM); 2–4 vCPU, 8–16 GB RAM |
| **Web** | Static SPA behind nginx or CDN |
| **Object storage** | Azure Blob or S3 for images and documents |
| **Background jobs** | Hangfire (SQL-backed) or hosted `BackgroundService` for email/SMS/push |
| **Secrets** | Key Vault / environment variables — never in git |
| **Logging** | Serilog → file + centralized sink (Seq / CloudWatch / App Insights) |
| **TLS** | Let's Encrypt or managed certificate |
| **Backups** | Daily full + frequent log backups; monthly restore test |

### 5.2 Future additions — only if measurements justify

| Component | Trigger to add |
|-----------|----------------|
| **Redis** | Catalog/read endpoints > 40% SQL CPU; p95 latency breach under load test |
| **Second API instance + load balancer** | Sustained CPU > 70% or error rate spike during peak |
| **Read replica** | Reporting queries impacting OLTP; measurable lock/wait pressure |
| **Search service (Elasticsearch)** | SQL full-text insufficient for craftsman/store discovery |
| **Kubernetes** | **Not planned** unless ops team size and scale exceed single-region App Service |

### 5.3 Explicitly deferred patterns

| Pattern | Status |
|---------|--------|
| Microservices | **Not approved** at current scale |
| Kubernetes | **Not approved** unless ops maturity requires it |
| Event-driven architecture (Service Bus, Kafka) | **Not approved** unless async volume demands it |
| SQL Enterprise | **Not approved** without licensing justification |
| Multi-region active-active | **Deferred** beyond Year 5 |

---

## 6. Deployment strategy

### 6.1 Environments

| Environment | Purpose | Topology |
|-------------|---------|----------|
| **Development** | Local `dotnet run`, Docker Compose SQL, Vite dev server | Single machine |
| **Staging** | QA, integration tests, mobile beta against real API | API + SQL + optional Redis trial |
| **Production** | Live users | API (1–2 instances), SQL Standard, blob storage, CDN |

### 6.2 Release process

1. **CI** — build, unit tests, integration tests (`dotnet test`, `npm run build`)
2. **Staging deploy** — auto or manual on merge to release branch
3. **Database migration** — one-off job before API rollout (not per-replica race in prod)
4. **Smoke tests** — health, auth, booking happy path
5. **Production deploy** — manual approval gate
6. **Rollback** — previous container image + forward-only migrations with compatibility window

### 6.3 Mobile release (current — native)

| Platform | Channel |
|----------|---------|
| Android | Play Internal Testing → Production |
| iOS | TestFlight → App Store |

### 6.4 Mobile release (future — React Native)

| Tool | Role |
|------|------|
| **Expo Development Build** | Dev client with native modules (push, maps, camera) |
| **EAS Build** | CI builds for iOS and Android |
| **EAS Submit** | Store submission automation |

*Not started until Phase 3 approval.*

---

## 7. Migration strategy

### 7.1 Priority order (approved)

```
Phase 1 ──► Phase 2 ──► Phase 3
(stabilize)   (RN docs)   (RN build)
```

| Phase | Name | Scope | Status |
|-------|------|-------|--------|
| **1** | Production stabilization | DB integrity, security, API consolidation, booking reliability, automated tests, globalization | **In progress — current priority** |
| **2** | Prepare React Native migration | Documentation only: `REACT_NATIVE_MIGRATION_ARCHITECTURE.md` | After Phase 1 |
| **3** | React Native implementation | Build RN app, parity with native, beta, decommission native | After Phase 2 approval |

### 7.2 Phase 1 deliverables (backend stabilization)

| Workstream | Examples |
|------------|----------|
| Database integrity | RowVersion, slot locks, cascade fixes, filtered indexes |
| Security hardening | Mobile admin login block, webhook HMAC, FluentValidation coverage |
| API consolidation | Profile/auth endpoint merge, consistent error responses |
| Booking reliability | Concurrency, payment idempotency, integration tests |
| Automated testing | Unit + integration coverage for booking and auth paths |
| Globalization | USD/LB defaults, timezone, i18n consistency |

**Native mobile continues** throughout Phase 1. Bug fixes on Kotlin/Swift are allowed; no RN project bootstrap.

### 7.3 Phase 2 deliverables (documentation only)

Create `docs/technology/REACT_NATIVE_MIGRATION_ARCHITECTURE.md` covering:

- Project structure
- Authentication flow
- API client design
- State management
- Navigation strategy
- Customer / Craftsman / Store role separation
- File upload and camera integration
- Maps
- Payments
- Push notifications
- Offline strategy
- Release process (EAS)

### 7.4 Phase 3 — React Native implementation

- Parallel run with native apps during beta
- Feature parity checklist before native decommission
- Shared `@khadamati/api-types` package with web (OpenAPI codegen)
- 90-day native freeze after RN production release

### 7.5 What is not migrating

| Asset | Action |
|-------|--------|
| Backend (ASP.NET Core) | **No migration** |
| Database (SQL Server) | **No migration** |
| Web (React) | **No migration** |
| Admin portal | **Stays on web permanently** |
| REST API contract | **Stable** — all clients use `api/v1` |

---

## 8. Cost optimization approach

### 8.1 Principles

1. **Right-size first** — one API instance and Standard SQL until metrics say otherwise.
2. **Reserved capacity** — 1-year reserved instances after 6 months stable usage (30–40% savings).
3. **Blob + CDN** — cheaper than serving media through API.
4. **Avoid unused services** — no Redis, no replica, no K8s “just in case.”
5. **Single region** — UAE or EU West for Lebanon latency; no multi-region until expansion.
6. **Shared TypeScript (future)** — RN + web reduces mobile team cost after Phase 3.

### 8.2 Estimated monthly infrastructure cost

| Stage | Users | Est. monthly (USD) |
|-------|-------|---------------------|
| MVP launch (Year 1) | ~2,250 | $150–350 |
| Growth (Year 3) | ~5,000 | $250–500 |
| Mature (Year 5) | ~10,000 | $400–800 |

*Excludes payment gateway fees, SMS, email volume, and personnel.*

### 8.3 Cost anti-patterns to avoid

| Anti-pattern | Why |
|--------------|-----|
| SQL Enterprise on day one | Licensing cost without benefit at this scale |
| Multi-AZ everything before traffic | Operational cost without measured need |
| Microservices | Network overhead + team burden |
| Maintaining three mobile codebases long-term | Native + RN overlap should be time-boxed |

---

## 9. Risks and mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Booking race conditions (double booking) | Medium | High | Phase 1: RowVersion, slot reservation, tests |
| Admin login on mobile | Medium | Medium | Phase 1: server-side mobile role policy |
| Payment webhook failures | Medium | High | Idempotent handlers, reconciliation job, alerts |
| Premature over-engineering | Medium | Medium | This document: add Redis/replicas only with metrics |
| RN migration delays MVP | High if started early | High | **Defer RN until Phase 1 complete** |
| Dual native codebase maintenance | Ongoing | Medium | Time-box native; plan Phase 3 cutover |
| SQL performance degradation | Low (Year 5) | Medium | Index review, query audit, then replica if needed |
| Lebanon connectivity / hosting | Medium | Medium | Cloud hosting outside Lebanon; CDN; health monitoring |
| Security breach (JWT, secrets) | Low | Critical | Key Vault, rotation runbook, pen test before scale marketing |
| Team skill gap for RN | Medium | Medium | Hire 1 RN lead before Phase 3; Expo reduces native toolchain burden |
| Third-party outage (Moyasar, FCM) | Medium | Medium | Graceful degradation, retry queues, status communication |

---

## 10. Governance and approval chain

| Decision | Authority | This document |
|----------|-----------|---------------|
| Technology stack | Product + engineering leadership | **Approved** (user sign-off July 2026) |
| Phase 1 implementation | Engineering | Authorized (stabilization work) |
| Phase 2 RN architecture doc | Engineering | Pending Phase 1 completion |
| Phase 3 RN implementation | Product + engineering | **Not authorized** |
| Infrastructure tier upgrades | Engineering + ops | Requires performance evidence |

### Document hierarchy

| Priority | Document |
|----------|----------|
| **1 (canonical)** | `KHADAMATI_FINAL_TECHNOLOGY_ARCHITECTURE.md` (this file) |
| 2 | `docs/technology/REACT_NATIVE_MIGRATION_ARCHITECTURE.md` (Phase 2 — not yet created) |
| 3 | Earlier analysis docs (`TECHNOLOGY_EVALUATION.md`, etc.) — reference only; scale figures superseded by §4 |

---

## 11. Next steps

| Step | Owner | Blocked by |
|------|-------|------------|
| Complete Phase 1 stabilization | Engineering | — |
| Sign off this summary document | Stakeholders | This review |
| Create `REACT_NATIVE_MIGRATION_ARCHITECTURE.md` | Engineering | Phase 1 complete |
| Load test at projected Year 1 peak | Engineering | Staging environment |
| Production launch (native mobile) | Product | Phase 1 exit criteria |
| Begin React Native implementation | Engineering | Phase 2 doc + explicit Phase 3 approval |

---

## 12. Approval record

| Role | Decision | Name | Date |
|------|----------|------|------|
| Product / business | ☑ Technology direction approved | | July 2026 |
| Engineering | ☐ This summary document approved | | |
| Stakeholders | ☐ Proceed to Phase 1 execution per §7.2 | | |

**Upon approval of this document:** Execute Phase 1. Do not begin React Native implementation. Phase 2 documentation (`REACT_NATIVE_MIGRATION_ARCHITECTURE.md`) is authorized after Phase 1 stabilization is complete.

---

## Appendix A — Technology decision log

| Date | Decision | Rationale |
|------|----------|-----------|
| Jul 2026 | Keep ASP.NET Core 8 + SQL Server | Sunk investment, enterprise fit, scale adequate |
| Jul 2026 | Reject PHP backend migration | Unnecessary cost and risk |
| Jul 2026 | Keep React web + MUI | Admin + customer portals complete |
| Jul 2026 | Approve future RN migration | Long-term maintenance; defer until stable |
| Jul 2026 | Revise scale target to 10K @ Year 5 | Right-size infrastructure; avoid over-engineering |
| Jul 2026 | Defer Redis, replicas, K8s, microservices | Add only when measurements justify |

## Appendix B — Related documents

- [README.md](./README.md) — technology doc index
- [TECHNOLOGY_EVALUATION.md](./TECHNOLOGY_EVALUATION.md) — detailed comparison (pre-approval analysis)
- [MOBILE_MIGRATION_PLAN.md](./MOBILE_MIGRATION_PLAN.md) — migration inventory (pre-approval analysis)
- [SCALABILITY_PLAN.md](./SCALABILITY_PLAN.md) — detailed capacity notes (superseded scale in §4)
- [HOSTING_ARCHITECTURE.md](./HOSTING_ARCHITECTURE.md) — environment topology
- [ROADMAP.md](./ROADMAP.md) — phased plan (align with §7 priority order)
- [FINAL_RECOMMENDATION.md](./FINAL_RECOMMENDATION.md) — pre-approval recommendation (superseded by this doc)
- [../PLATFORM_ARCHITECTURE.md](../PLATFORM_ARCHITECTURE.md) — platform reference
- [../PRODUCTION.md](../PRODUCTION.md) — production deployment
- [../AUTHENTICATION.md](../AUTHENTICATION.md) — auth and RBAC
