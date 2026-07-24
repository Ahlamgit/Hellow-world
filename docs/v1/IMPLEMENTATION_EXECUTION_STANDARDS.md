# KHADAMATI V1 — Implementation Execution Standards

**Document ID:** KHAD-V1-IMPL-EXEC-STANDARDS  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Engineering Enablement Architect  
**Status:** Preparation for post–Gate A execution  

**Sources:**  
Master Prompt v1.0 · [`FINAL_PRE_IMPLEMENTATION_READINESS_REVIEW.md`](./FINAL_PRE_IMPLEMENTATION_READINESS_REVIEW.md) · [`FINAL_IMPLEMENTATION_GATE_REPORT.md`](./FINAL_IMPLEMENTATION_GATE_REPORT.md) · [`FEATURE_TRACEABILITY_MATRIX.md`](./FEATURE_TRACEABILITY_MATRIX.md) · ADR-001…028 · [`standards/18-CODING-STANDARDS.md`](./standards/18-CODING-STANDARDS.md) · [`standards/19-NAMING-CONVENTIONS.md`](./standards/19-NAMING-CONVENTIONS.md) · [`standards/20-SECURITY-ARCHITECTURE.md`](./standards/20-SECURITY-ARCHITECTURE.md) · [`architecture/16-API-DESIGN.md`](./architecture/16-API-DESIGN.md) · [`devops/35-TESTING-STRATEGY.md`](./devops/35-TESTING-STRATEGY.md)

```text
DO NOT write production code.
DO NOT create database migrations.
DO NOT create APIs.
DO NOT implement features.

These standards govern implementation ONLY after Gate A approval.
Until then: Implementation remains BLOCKED.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Architecture | **APPROVED** |
| Scope | **FROZEN** |
| Implementation | **BLOCKED** until readiness blockers close (Gate B) |
| This document | Standards ready for use **after** Gate → **A** |

---

## Purpose

Define engineering standards that will govern KHADAMATI implementation after approval:

- How work is branched, reviewed, and released  
- Environment and secrets discipline  
- Definition of Done and traceability  
- Coding / DB / API / security / testing rules aligned to approved architecture  

Detail specs remain in linked standards docs; this document is the **execution control surface**.

---

# 1. Development Workflow

## 1.1 Repository workflow

| Rule | Expectation |
|------|-------------|
| Source of truth | Approved ADRs + Scope Baseline + Master Prompt + FTM |
| Monorepo / multi-repo | Follow [`standards/17-FOLDER-STRUCTURE.md`](./standards/17-FOLDER-STRUCTURE.md) once coding starts |
| No silent scope | New V1 features require change control (Scope Baseline §10) |
| No invented policy | Unresolved `Q-*` stay open; do not hardcode commercial values |

## 1.2 Branch strategy

| Branch | Purpose |
|--------|---------|
| `main` | Protected; production-ready history only after review |
| `cursor/<descriptive-name>-f98a` | Cloud-agent / feature work (lowercase; see Naming Conventions) |
| Short-lived feature branches | One concern per branch; rebase/merge per team CI policy |

Prefer small PRs tied to a single FTM feature or vertical slice.

## 1.3 Pull request requirements

Every PR **must**:

- [ ] Reference FTM / BR id (or ADR if architecture-only)  
- [ ] Stay within frozen scope  
- [ ] Include tests for changed behaviour (or justify POLICY-GATED skip)  
- [ ] Update OpenAPI if API changed  
- [ ] Include Flyway if schema changed  
- [ ] Pass CI (lint, unit, relevant integration)  
- [ ] No secrets in diff  

## 1.4 Code review process

| Check | Required |
|-------|----------|
| Architecture alignment (ports, provider model, no e-commerce) | Yes |
| AuthZ / audience (admin-web only for admin) | Yes |
| Money paths: idempotency, ledger, audit | Yes |
| Logging: no PAN/PII leakage | Yes |
| Module dependency rules (ArchUnit when enabled) | Yes |

Minimum: Engineering peer review. Security review for auth, payment, KYC, Admin privilege changes. Architect review for ADR-impacting changes.

## 1.5 Documentation updates

Same PR updates:

- OpenAPI / FTM row status when behaviour lands  
- Runbooks if ops behaviour changes  
- README only after stack cutover notes are accurate  

## 1.6 ADR update rules

| Case | Action |
|------|--------|
| Clarifies existing Accepted ADR | Prefer doc note / FTM; no ADR rewrite that changes decision |
| Conflicts with Accepted ADR | New ADR or explicit supersession — **no silent override** |
| New cross-cutting decision | New ADR under `docs/v1/adr/` with status workflow |
| Superseded ADRs | Keep historical file; point to successor (pattern ADR-014→022) |

---

# 2. Environment Strategy

```text
Development
  → Staging
  → Production
```

| Environment | Purpose | Access rules | Configuration | Secrets |
|-------------|---------|--------------|---------------|---------|
| **Development** | Local/CI feature work | Engineers; no production data | Dev configs / feature flags loose | Local/dev secret store; never commit |
| **Staging** | Integration + E2E + Payment.js sandbox | Eng + QA + selected Product | Prod-like; Market=Lebanon defaults when approved | Staging secrets manager; IXOPAY sandbox only |
| **Production** | Live customers/providers | Break-glass + ops roles; Admin MFA | Strict; migrations via dedicated job | Production secrets manager; rotation required |

### Cross-environment rules

- Configuration separation: no Production credentials in Dev/Staging  
- Secrets handling: secrets manager / env injection; never in git or images  
- Data: no production PII copied to lower envs without approved sanitization  
- Promote **artifacts**, not ad-hoc server edits  

---

# 3. Definition of Done

A feature is **Done** only when **all** apply:

| Criterion | Expectation |
|-----------|-------------|
| Requirements satisfied | Acceptance criteria / BR met |
| Architecture aligned | ADR-001…028 + Scope Baseline; ports for vendors |
| Security reviewed | AuthZ, secrets, OWASP-relevant checks for the change |
| Tests completed | Per Testing Strategy (§8); money/auth paths mandatory |
| Documentation updated | OpenAPI / ops notes as needed |
| Traceability updated | FTM row links feature → module → entity → API → UI |

Also: no PAN in logs; Admin audience enforced where relevant; financial records not casually deleted.

---

# 4. Coding Standards

Detail: [`standards/18-CODING-STANDARDS.md`](./standards/18-CODING-STANDARDS.md) · Naming: [`standards/19-NAMING-CONVENTIONS.md`](./standards/19-NAMING-CONVENTIONS.md)

## 4.1 Backend (Spring Boot / Java 21)

| Topic | Rule |
|-------|------|
| Clean architecture | Controllers thin; application/domain own use cases; adapters at edges |
| Domain separation | No vendor SDKs/DTOs in domain; modules call ports (ADR-025) |
| Provider model | Unified Provider + capabilities (ADR-028); no craftsman/store money forks |
| Error handling | Typed domain exceptions → RFC 7807 Problem Details |
| Logging | SLF4J structured; correlation ids; **no** card data / OTP secrets |

## 4.2 Frontend (React + MUI — Admin & Store)

| Topic | Rule |
|-------|------|
| Component standards | Function components; schema-validated forms; no `any` without exception |
| State management | Server state via TanStack Query (or team-approved equivalent); avoid duplicating server truth |
| Accessibility | Keyboard paths for Admin tables; MUI a11y baselines; RTL/LTR per design tokens |
| Design gate | UI follows approved design assets (ADR-023); no placeholder brand invention |

## 4.3 Mobile (Flutter — Customer & Craftsman)

| Topic | Rule |
|-------|------|
| Performance | Feature-first clean architecture; avoid jank on list/search; paginate feeds |
| Offline considerations | Explicit UX for offline; no silent money mutations offline |
| Permission handling | Request location/notifications with clear purpose; fail gracefully if denied |
| Payment | Payment.js in secured WebView only; no native PAN fields |

---

# 5. Database Standards

Detail: [`architecture/13-DATABASE-ARCHITECTURE.md`](./architecture/13-DATABASE-ARCHITECTURE.md) · Final DB architecture readiness notes

| Topic | Rule |
|-------|------|
| Migration management | Flyway only; deterministic; Production migrate via dedicated job |
| Naming conventions | snake_case tables/columns; see Naming Conventions |
| Index review | Index new FKs and hot filters; review in PR for list/search paths |
| Audit fields | `created_at` / `updated_at` / actor where required; sensitive actions → `audit_events` |
| Soft deletion | Allowed for non-financial master data; **forbidden** as purge for ledger/payments/refunds/commissions (ADR-004) |
| Data integrity | FKs, constraints, server-owned money amounts; legal hold respects retention (BLOCKER-006 when approved) |

Expand/contract for breaking changes. No invented commercial defaults in seed data without Finance approval.

---

# 6. API Standards

Detail: [`architecture/16-API-DESIGN.md`](./architecture/16-API-DESIGN.md)

| Topic | Rule |
|-------|------|
| Versioning | `/api/v1`; backward-compatible by default |
| Authentication | JWT Bearer; audience per client (`admin-web`, `store-web`, `customer-app`, `craftsman-app`) |
| Authorization | Permission + resource scope; Admin only via `admin-web` + MFA (ADR-006) |
| Error format | RFC 7807 Problem Details + `code` + `correlationId` |
| Pagination | Page/size (and cursor where required for mobile feeds) |
| Validation | Bean Validation / schema on inputs; server amounts for payments |
| Documentation | OpenAPI updated in same PR as endpoint changes |
| Idempotency | `Idempotency-Key` on money/booking creates |

Webhooks: verify in adapter; finalize idempotently; never trust client for capture finality.

---

# 7. Security Development Rules

Detail: [`standards/20-SECURITY-ARCHITECTURE.md`](./standards/20-SECURITY-ARCHITECTURE.md)

| Rule | Expectation |
|------|-------------|
| Secret management | Secrets manager; rotation; never in repo/images |
| Dependency review | CI dependency scanning; no critical CVEs untriaged on release |
| OWASP considerations | Map changes to Top 10 controls (access control, injection, auth, logging, components) |
| Authentication protection | Lockout/rate limits; refresh rotation; Admin MFA |
| Authorization testing | Deny-path tests mandatory for new secured endpoints |
| Payment | No PAN/CVV storage; Payment.js hosted fields; webhook signature verify |
| KYC / media | Least privilege; audited access; retention-aware |

Fail closed on authorization.

---

# 8. Testing Strategy

Detail: [`devops/35-TESTING-STRATEGY.md`](./devops/35-TESTING-STRATEGY.md)

| Level | Required focus |
|-------|----------------|
| Unit testing | Domain policies, calculators, state transitions |
| Integration testing | Repositories, Flyway, adapters (WireMock/sandbox) |
| API testing | Contracts + authz negatives + audience rules |
| Mobile testing | Critical Flutter flows; Payment.js WebView scenarios in staging |
| UI testing | Admin/Store critical forms (Testing Library / Playwright as adopted) |
| Regression testing | Staging suite before Production; payment/booking/ledger P0 always |

P0 always covered: auth, RBAC deny, booking legality, payment idempotency, onboarding audit, store tenant isolation.

---

# 9. Release Management

| Topic | Rule |
|-------|------|
| Release approval | Eng Lead + (Product for user-facing) + Security for privileged/payment changes; Prod deploy per ops checklist (BLOCKER-004 when approved) |
| Versioning | Immutable artifact tags; semantic or calendar versioning as team adopts — record in release notes |
| Rollback process | Redeploy previous artifact; DB expand/contract so rollback does not require unsafe down-migrations in Prod |
| Production checklist | Migrations applied; secrets present; health checks green; payment webhook path verified; smoke tests; monitoring alerts armed |

No hot-edit Production config outside approved change process.

---

# 10. Traceability Rules

Every implemented capability must maintain:

```text
Business Requirement
  → Feature
  → Module
  → Database Entity
  → API
  → UI
```

| Link | Tooling / artifact |
|------|--------------------|
| Business Requirement → Feature | [`FEATURE_TRACEABILITY_MATRIX.md`](./FEATURE_TRACEABILITY_MATRIX.md) |
| Feature → Module | FTM module column + package layout |
| Module → Database Entity | FTM / ER mapping / Flyway |
| Entity → API | OpenAPI + FTM API column |
| API → UI | Screen/spec reference after design approval |

PRs that change behaviour must update the FTM status (or equivalent tracker) in the same change set.

Out-of-scope rows (cart, product checkout, inventory, open messaging) must remain **Out** unless Scope Baseline is formally amended.

---

# 11. Relationship to Gate

| Gate state | What this document allows |
|------------|---------------------------|
| **B) NOT READY** | Preparation / reading only — **no** coding, migrations, APIs, features, deploys |
| **A) READY** | These standards become mandatory for all implementation work |

---

# 12. Explicit Non-Goals (now)

- No production code  
- No database migrations  
- No API implementation  
- No feature implementation  
- No deployment  
- No infrastructure creation  

---

**End of Implementation Execution Standards v1.0**
