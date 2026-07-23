# KHADAMATI — Technology Architecture Documentation

Enterprise architecture analysis for technology evaluation, mobile migration, scalability, hosting, and roadmap planning.

**Status:** Draft — analysis only, no implementation authorized  
**Created:** July 2026

---

## Documents

| # | Document | Description |
|---|----------|-------------|
| 1 | [TECHNOLOGY_EVALUATION.md](./TECHNOLOGY_EVALUATION.md) | Current vs proposed stack comparison across performance, cost, hiring, security, and enterprise readiness |
| 2 | [MOBILE_MIGRATION_PLAN.md](./MOBILE_MIGRATION_PLAN.md) | Kotlin + Swift → React Native migration strategy, risks, and reuse analysis |
| 3 | [SCALABILITY_PLAN.md](./SCALABILITY_PLAN.md) | Architecture for 5K–50K users: API, SQL, Redis, caching, jobs, monitoring |
| 4 | [HOSTING_ARCHITECTURE.md](./HOSTING_ARCHITECTURE.md) | Dev, staging, production, HA, backups, DR, CDN, cost optimization |
| 5 | [ROADMAP.md](./ROADMAP.md) | Five-phase migration and evolution plan |
| 6 | [FINAL_RECOMMENDATION.md](./FINAL_RECOMMENDATION.md) | Executive go/no-go decisions and target architecture |

---

## Key conclusions (summary)

- **Keep:** ASP.NET Core 8, EF Core, SQL Server, React + TypeScript web
- **Migrate (phased):** Android + iOS → React Native + TypeScript (Expo dev builds)
- **Scale:** Add Redis, read replicas, horizontal API scaling, blob storage before 5K CCU
- **Do not:** Rewrite backend, replace SQL, or big-bang mobile migration before MVP stabilization

---

## Rules

- No implementation without approval
- No APK/IPA builds as part of this documentation phase
- API remains the single contract between all clients

---

## Related platform docs

- [PLATFORM_ARCHITECTURE.md](../PLATFORM_ARCHITECTURE.md)
- [PRODUCTION.md](../PRODUCTION.md)
- [AUTHENTICATION.md](../AUTHENTICATION.md)
