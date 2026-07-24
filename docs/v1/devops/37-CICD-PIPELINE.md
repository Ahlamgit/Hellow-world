# 37. CI/CD Pipeline

**Document ID:** KHAD-V1-CICD  
**Status:** Draft for Approval  

---

## 37.1 Goals

Every merge to main is buildable, tested, scannable, and deployable to staging; production is gated.

## 37.2 Pipeline Overview

```text
PR opened
  → lint/format
  → unit tests (backend/web/flutter as affected)
  → architecture tests (ArchUnit)
  → dependency + secret scan
  → build artifacts
  → (optional) preview deploy

merge to main
  → full CI
  → publish images/artifacts
  → deploy staging
  → smoke tests

production release tag / manual approval
  → deploy prod
  → migrate
  → smoke
  → rollback plan ready
```

## 37.3 Suggested GitHub Actions Jobs

| Job | Path filters |
|-----|--------------|
| `backend-ci` | `backend/**`, `db/**` |
| `admin-ci` | `apps/admin-portal/**`, `packages/**` |
| `store-ci` | `apps/store-dashboard/**`, `packages/**` |
| `flutter-customer-ci` | `apps/customer_app/**` |
| `flutter-craftsman-ci` | `apps/craftsman_app/**` |
| `docs-lint` | `docs/**` optional |
| `deploy-staging` | main |
| `deploy-prod` | tags `v*` + approval |

## 37.4 Quality Gates

- Tests green  
- No critical CVE in release dependencies (policy threshold Q-CICD-001)  
- OpenAPI generated/validated  
- Docker image signed/scanned (recommended)

## 37.5 Mobile Distribution

- CI builds artifacts  
- Store submission may be manual in V1 (Q-CICD-002)  
- Use flavors: `dev`, `staging`, `prod` with distinct API base URLs  

## 37.6 Rollback

- Previous image redeploy  
- DB migrations must be backward compatible with expand/contract  
- Feature flags to disable risky capabilities quickly  
