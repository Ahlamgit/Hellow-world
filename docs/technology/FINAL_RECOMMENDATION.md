# KHADAMATI — Final Architecture Recommendation

**Document type:** Executive decision record  
**Status:** Draft — awaiting approval  
**Audience:** Product, engineering leadership, stakeholders

---

## Decision summary

| Question | Recommendation | Confidence |
|----------|----------------|------------|
| Migrate mobile to React Native? | **Yes — phased, after MVP stabilization** | High |
| Keep ASP.NET Core 8? | **Yes — absolutely** | Very high |
| Keep SQL Server? | **Yes** | High |
| Keep EF Core? | **Yes** | High |
| Keep React Web? | **Yes** | Very high |
| Better mobile alternative than RN? | **Not for this team/product at this stage** | Medium-high |
| Support 50,000 users comfortably? | **Yes — with planned infrastructure (Phase 3–4)** | High |

---

## 1. Should KHADAMATI migrate to React Native?

### Answer: **Yes, with conditions**

**Recommend migration** from Android (Kotlin/Compose) + iOS (Swift/SwiftUI) to **React Native + TypeScript**, using **Expo with development builds**.

### Why yes

| Factor | Evidence |
|--------|----------|
| **Duplicate effort** | ~140 mobile source files implementing the same features twice |
| **Hiring** | TypeScript/React skills are easier to find than Kotlin + Swift pairs — especially relevant for Lebanon/MENA |
| **Web alignment** | Web is already React 19 + TypeScript — shared API types, i18n, validation |
| **Maintenance cost** | Every booking, chat, subscription change is done twice today |
| **Feature gaps** | Verification upload, schedules missing on both — fix once in RN |
| **MVP API ready** | 169+ REST endpoints; mobile is a thin client — migration does not require backend rewrite |

### Why not immediately

| Factor | Evidence |
|--------|----------|
| **Native apps already exist** | ~12,500 LOC with feature parity — shipping native MVP is faster *right now* |
| **Migration cost** | Booking wizard + payments + multi-role = high QA surface |
| **RN is not free** | Still need iOS/Android build pipelines, store accounts, native module expertise |

### Recommended path

1. **Phase 1:** Stabilize backend + ship **native MVP** if launch timeline is critical.
2. **Phase 2:** Start React Native in parallel; freeze new native features.
3. **Phase 4:** Decommission native after RN parity + beta validation.

### Expo vs CLI

**Use Expo with development builds** — not Expo Go alone. KHADAMATI requires FCM, APNs, maps, secure storage, and eventually camera for verification documents. Expo dev builds + EAS provide enterprise CI without maintaining Gradle/Xcode projects for every developer.

---

## 2. Should ASP.NET Core remain?

### Answer: **Yes — do not replace**

| Reason | Detail |
|--------|--------|
| **Investment** | Clean Architecture, MediatR, FluentValidation, 43+ tables, 169+ endpoints already built |
| **Enterprise fit** | RBAC, audit, structured logging, Docker deployment — matches goals |
| **Performance** | ASP.NET Core 8 is among the fastest web frameworks — supports 5K CCU with horizontal scale |
| **Team** | Replacing backend would delay MVP by 12+ months with zero business benefit |
| **Ecosystem** | SQL Server, EF Core, JWT, Serilog are proven enterprise choices |

**No alternative backend** (Node, Java, Go) offers sufficient advantage to justify rewrite.

---

## 3. Should SQL Server remain?

### Answer: **Yes**

| Reason | Detail |
|--------|--------|
| **Current schema** | 43 EF tables, migrations, relationships, admin modules |
| **Scale target** | SQL Server comfortably handles 50K users + 5K CCU with indexes, read replica, pooling |
| **Enterprise features** | Always On, TDE, row-level security, backup/restore — needed for production hardening |
| **Team familiarity** | Existing docs, Docker compose, local dev on SQL Server 2014+ |

### When to reconsider

- **100K+ CCU** with write-heavy global distribution → consider sharding or specialized stores.
- **Full-text geo search at scale** → add Elasticsearch alongside SQL, not instead of it.

PostgreSQL is a viable alternative for *new* projects but **not worth migrating** KHADAMATI's existing EF schema.

---

## 4. Should EF Core remain?

### Answer: **Yes**

| Reason | Detail |
|--------|--------|
| **Primary runtime ORM** | All API data access goes through EF |
| **Migrations** | Production migration pipeline established |
| **Clean Architecture** | Repositories and Unit of Work built on EF |
| **Performance** | Adequate with proper queries, indexes, and read replicas |

### Complement EF with

- **Dapper** for specific hot-path reports (optional, Phase 3)
- **Raw SQL** for admin analytics exports (already partially via views)
- **Never** replace EF wholesale

---

## 5. Should React Web remain?

### Answer: **Yes**

| Reason | Detail |
|--------|--------|
| **Admin portal** | Substantial MUI-based admin UI — no benefit to rewriting |
| **Customer web** | Booking, services, portals already on React |
| **RTL/i18n** | Arabic/English already implemented |
| **Material UI** | No strong reason to replace — mature, accessible, RTL-capable |

### Optional improvements (not replacements)

- Share `@khadamati/api-types` with React Native
- Share Zod schemas for client-side validation
- Keep MUI for admin; RN uses its own component library

**Do not migrate web to Next.js or Blazor** unless SEO becomes a primary channel — marketplace apps are mobile-first.

---

## 6. Would another mobile technology be better?

### Alternatives considered

| Technology | Verdict | Why |
|------------|---------|-----|
| **Keep native (Kotlin + Swift)** | Valid for MVP only | Best performance but 2× cost forever |
| **React Native + Expo** | **Recommended** | Best balance of cost, hiring, web synergy |
| **Flutter** | Not recommended | Would not share code with React web; team learns Dart instead of TS |
| **.NET MAUI** | Not recommended | Smaller community, weaker third-party ecosystem for maps/push/payments |
| **Ionic/Capacitor** | Not recommended | WebView-based — inferior UX for maps, chat, native feel |
| **PWA only** | Insufficient | Push, GPS, app store presence critical for marketplace in Lebanon |

### Conclusion

**React Native is the best fit** for KHADAMATI given React web, TypeScript hiring pool, and API-first backend. **Flutter** is the only serious alternative if the team strongly prefers Dart and rejects JavaScript — but it forfeits web code sharing.

---

## 7. Will this architecture support 50,000 users?

### Answer: **Yes — comfortably, with infrastructure investment**

| Metric | Supported? | Condition |
|--------|------------|-----------|
| 50,000 registered users | ✅ Yes | Standard SQL sizing |
| 5,000 concurrent users | ✅ Yes | 4+ API instances, Redis, read replica, load test proof |
| 100,000 registered (year 2) | ✅ Yes | Tier 4 additions (search cluster, more replicas) |
| 10,000 CCU | ⚠️ Requires Tier 4 | Kubernetes, event-driven workers, search cluster |

### What does NOT scale by itself

| Gap today | Required action |
|-----------|-----------------|
| No Redis | Add in Phase 3 |
| Single API instance | Horizontal scale in Phase 4 |
| No read replica | Add before 5K CCU |
| No blob storage | Move media to S3/Azure Blob |
| No background queue | Offload email/SMS/push batch |
| No APM | Add before production launch |

### Mobile technology impact on scale

**None.** 50K users and 5K CCU are **backend/infrastructure** challenges. Whether clients are native or React Native does not change API load characteristics meaningfully.

---

## 8. Risk-adjusted recommendation matrix

| Strategy | MVP speed | Long-term cost | Scale readiness | Overall |
|----------|-----------|----------------|-----------------|---------|
| Keep native forever | ⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐ | Good for short-term only |
| Big-bang RN now | ⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | **Not recommended** |
| Native MVP → phased RN | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | **Recommended** |
| RN only (skip native ship) | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | OK if MVP can wait |

---

## 9. Approved technology stack (target state)

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENTS                               │
├──────────────┬──────────────────────┬───────────────────────┤
│  React Web   │   React Native       │   (No admin mobile)   │
│  (Customer + │   Expo + TypeScript  │                       │
│   Admin)     │   iOS + Android      │                       │
└──────┬───────┴──────────┬───────────┴───────────────────────┘
       │                  │
       │    REST API v1 (JWT)
       ▼                  ▼
┌─────────────────────────────────────────────────────────────┐
│              ASP.NET Core 8 LTS (Clean Architecture)         │
│         MediatR · FluentValidation · AutoMapper · Serilog    │
└──────────────────────────┬──────────────────────────────────┘
                           │
       ┌───────────────────┼───────────────────┐
       ▼                   ▼                   ▼
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ SQL Server  │    │    Redis    │    │ Blob Store  │
│ + replica   │    │   (cache)   │    │  + CDN      │
└─────────────┘    └─────────────┘    └─────────────┘
```

---

## 10. Actions requiring approval

| # | Action | Phase |
|---|--------|-------|
| 1 | Merge .NET 8 to `main` | 1 |
| 2 | Implement mobile login role policy (block admin) | 1 |
| 3 | Ship native MVP OR defer for RN | 1 — **business decision** |
| 4 | Approve React Native migration budget and hiring | 2 |
| 5 | Provision Redis + staging load tests | 3 |
| 6 | Production HA topology | 4–5 |

**No code, migration, or package installation should begin until this document and supporting plans are approved.**

---

## 11. Document index

| Document | Purpose |
|----------|---------|
| `TECHNOLOGY_EVALUATION.md` | Stack comparison |
| `MOBILE_MIGRATION_PLAN.md` | RN migration detail |
| `SCALABILITY_PLAN.md` | 5K–50K user design |
| `HOSTING_ARCHITECTURE.md` | Environments and HA |
| `ROADMAP.md` | Phased execution plan |
| `FINAL_RECOMMENDATION.md` | This document |

---

## 12. Approval record

| Role | Name | Decision | Date |
|------|------|----------|------|
| Product owner | | ☐ Approved ☐ Rejected | |
| Tech lead | | ☐ Approved ☐ Rejected | |
| Engineering | | ☐ Approved ☐ Rejected | |

**Upon approval:** Proceed to Phase 1 implementation per `ROADMAP.md`. React Native work authorized only after Phase 1 exit criteria or explicit waiver.
