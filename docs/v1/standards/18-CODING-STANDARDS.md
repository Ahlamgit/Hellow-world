# 18. Coding Standards

**Document ID:** KHAD-V1-CODE  
**Status:** Draft for Approval  

---

## 18.1 General

1. Prefer clarity over cleverness  
2. No secrets in source control  
3. Every money-moving path has tests + audit/outbox  
4. Fail closed on authorization  
5. Do not invent business policy in code comments as truth — link to `Q-*` until decided  

## 18.2 Java / Spring Boot

| Topic | Standard |
|-------|----------|
| Java | 21 LTS |
| Style | Google Java Style or Spotless-enforced format |
| Null | Document non-null invariants; avoid returning null collections |
| Layers | Controllers thin; business in application/domain |
| Transactions | On application services, not controllers |
| Validation | Bean Validation on request DTOs |
| Mapping | Explicit mappers; no entity leakage to API |
| Logging | SLF4J; no PII/card data in logs |
| Exceptions | Map to problem details; use typed domain exceptions |
| Arch tests | ArchUnit module dependency rules in CI |
| Tests | JUnit 5, AssertJ, Testcontainers, Mockito |

## 18.3 TypeScript / React

| Topic | Standard |
|-------|----------|
| TS | `strict: true` |
| Components | Function components |
| Types | No `any` unless justified + eslint exception |
| Hooks | Follow React rules; prefer TanStack Query for server state |
| Forms | Schema-validated |
| Accessibility | MUI a11y baselines; keyboard navigation for admin tables |
| Bundling | Vite; no unused deps |
| Tests | Vitest + Testing Library |

**UI note:** Do not ship placeholder marketing layouts; wait for brand/design assets.

## 18.4 Flutter / Dart

| Topic | Standard |
|-------|----------|
| Effective Dart | Required |
| Architecture | Feature-first clean architecture |
| Immutable state | Prefer immutable models |
| Secrets | secure storage only |
| Lints | `flutter_lints` / stricter custom |
| Tests | unit + widget for critical flows |

## 18.5 SQL / Flyway

- Deterministic migrations  
- Explicit indexes for new FKs  
- No `SELECT *` in hand-written queries used by reporting hotspots  
- Expand/contract for breaking changes  

## 18.6 API Standards

- Consistent naming, pagination, errors  
- OpenAPI updated in same PR as endpoint changes  
- Backward-compatible by default  

## 18.7 Code Review Checklist (Excerpt)

- [ ] Authz annotations/tests present  
- [ ] Idempotency for relevant creates  
- [ ] No cross-module entity coupling  
- [ ] Flyway + rollback/expand plan  
- [ ] Observability fields added  
- [ ] Open questions not silently closed by inventing policy  

## 18.8 Documentation Standards

- Architecture decisions recorded as ADRs under `docs/v1/adr/` when implementation starts  
- Public README reflects Spring/Flutter stack after cutover (not legacy .NET)
