# Phase 0.5 — Enterprise Discovery Index

**Status:** Complete — documentation and analysis only (no code changes)  
**Date:** 2026-07-22  
**Prerequisite:** Phase 0 governance approved  
**Next:** Await approval before Phase 1 implementation

---

## Document Catalog

### Database Discovery

| Document | Purpose |
|----------|---------|
| [DATABASE_ER_MODEL.md](./DATABASE_ER_MODEL.md) | All 44 EF entities: PK, FK, relationships, navigation, cascade, indexes |
| [DATABASE_DEPENDENCY_GRAPH.md](./DATABASE_DEPENDENCY_GRAPH.md) | Table → dependent tables → business modules |
| [DATABASE_OBJECT_INVENTORY.md](./DATABASE_OBJECT_INVENTORY.md) | Tables, views, SPs, triggers — EF vs SQL vs app usage |
| [DATABASE_INDEX_ANALYSIS.md](./DATABASE_INDEX_ANALYSIS.md) | Index inventory, gaps, hot-path query risks |

### Backend Discovery

| Document | Purpose |
|----------|---------|
| [BACKEND_MODULE_ARCHITECTURE.md](./BACKEND_MODULE_ARCHITECTURE.md) | 13 bounded contexts, controllers, commands, services, DTOs |
| [SERVICE_RESPONSIBILITY_REPORT.md](./SERVICE_RESPONSIBILITY_REPORT.md) | SRP analysis: AdminService, BookingService, AuthService, payments, subscriptions |
| [API_DEPENDENCY_MAP.md](./API_DEPENDENCY_MAP.md) | Endpoint → DTO → Handler → Service → Entity chains |

### Client Discovery

| Document | Purpose |
|----------|---------|
| [CLIENT_FEATURE_MATRIX.md](./CLIENT_FEATURE_MATRIX.md) | Web/Android/iOS routes, APIs, i18n, offline, parity gaps |

### Quality & Risk

| Document | Purpose |
|----------|---------|
| [TEST_COVERAGE_ANALYSIS.md](./TEST_COVERAGE_ANALYSIS.md) | Unit/integration/client tests, CI gaps |
| [ENTERPRISE_RISK_MATRIX.md](./ENTERPRISE_RISK_MATRIX.md) | 30 ranked risks with business/technical impact and recommended phase |

---

## Key Findings Summary

| Area | Finding |
|------|---------|
| **Database** | EF migrations canonical; SQL scripts 008–013 not in master deploy; 6 views + 9 SPs obsolete |
| **Backend** | 169 endpoints, MediatR throughout; AdminService 1,455 LOC monolith |
| **API** | Duplicate profile paths; missing FluentValidation on ~60% of commands |
| **Clients** | Web full-featured; mobile missing change-password, coupons, email verify |
| **Tests** | 84 backend tests; BookingService 0 tests; 0 client tests |
| **CI/CD** | Backend test ✅; Android job incomplete; no iOS pipeline |
| **Risk** | 7 Critical risks including secrets, schema drift, booking concurrency |

---

## Phase 1 Readiness Checklist (Pending Approval)

- [ ] Review ENTERPRISE_RISK_MATRIX top 7 critical items
- [ ] Approve secrets migration plan (Phase 0 DEPLOYMENT_RUNBOOK §10)
- [ ] Approve EF-only database deployment policy
- [ ] Prioritize BookingService tests + geo index
- [ ] Sign off on mobile parity deferrals or schedule

---

## Related Phase 0 Documents

- [../BUSINESS_REQUIREMENTS.md](../BUSINESS_REQUIREMENTS.md)
- [../TRACEABILITY_MATRIX.md](../TRACEABILITY_MATRIX.md)
- [../DEPLOYMENT_RUNBOOK.md](../DEPLOYMENT_RUNBOOK.md)
