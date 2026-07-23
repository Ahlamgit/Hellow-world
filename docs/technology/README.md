# KHADAMATI — Technology Architecture Documentation

Enterprise architecture analysis for technology evaluation, mobile migration, scalability, hosting, and roadmap planning.

**Status:** Technology direction **approved** (July 2026) — see canonical reference below  
**Created:** July 2026

---

## Canonical reference

**[KHADAMATI_FINAL_TECHNOLOGY_ARCHITECTURE.md](./KHADAMATI_FINAL_TECHNOLOGY_ARCHITECTURE.md)** — approved stack, scale assumptions (Year 1: ~2,250 users; Year 5: ~10,000 users), deployment, migration priority, and cost discipline. **This supersedes conflicting figures in earlier drafts below.**

---

## Documents

| # | Document | Description |
|---|----------|-------------|
| **★** | [KHADAMATI_FINAL_TECHNOLOGY_ARCHITECTURE.md](./KHADAMATI_FINAL_TECHNOLOGY_ARCHITECTURE.md) | **Approved technology direction (canonical)** |
| — | [REACT_NATIVE_MIGRATION_ARCHITECTURE.md](./REACT_NATIVE_MIGRATION_ARCHITECTURE.md) | Phase 2 — *to be created after Phase 1 stabilization* |
| 1 | [TECHNOLOGY_EVALUATION.md](./TECHNOLOGY_EVALUATION.md) | Pre-approval stack comparison |
| 2 | [MOBILE_MIGRATION_PLAN.md](./MOBILE_MIGRATION_PLAN.md) | Pre-approval migration inventory |
| 3 | [SCALABILITY_PLAN.md](./SCALABILITY_PLAN.md) | Detailed capacity notes (scale figures superseded by canonical doc) |
| 4 | [HOSTING_ARCHITECTURE.md](./HOSTING_ARCHITECTURE.md) | Environment topology |
| 5 | [ROADMAP.md](./ROADMAP.md) | Phased plan — align with canonical §7 |
| 6 | [FINAL_RECOMMENDATION.md](./FINAL_RECOMMENDATION.md) | Pre-approval recommendation (superseded) |

---

## Approved conclusions (summary)

- **Keep:** ASP.NET Core 8, EF Core, SQL Server, React + TypeScript web
- **Mobile now:** Native Kotlin + Swift until Phase 1 complete
- **Mobile future:** React Native + Expo dev builds + EAS (Phase 3 — not started)
- **Scale:** ~2,250 users Year 1 → ~10,000 Year 5; no over-engineering
- **Defer:** Redis, replicas, K8s, microservices unless metrics justify

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
