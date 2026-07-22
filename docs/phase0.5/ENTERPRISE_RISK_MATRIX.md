# Enterprise Risk Matrix — Phase 0.5 Discovery

**Generated:** 2026-07-22  
**Location:** `docs/phase0.5/`  
**Inputs:** Client feature matrix, test coverage analysis, CI/CD pipeline review, database discovery, Phase 0 security audit

## Risk Scoring

| Score | Likelihood | Impact |
|-------|------------|--------|
| 1 | Rare | Negligible |
| 2 | Unlikely | Minor |
| 3 | Possible | Moderate |
| 4 | Likely | Major |
| 5 | Almost certain | Severe |

**Risk rating** = Likelihood × Impact (max 25). **Priority:** Critical ≥15, High 10–14, Medium 5–9, Low ≤4.

---

## Risk Register

| ID | Category | Risk | L | I | Score | Priority | Evidence | Mitigation direction |
|----|----------|------|---|---|-------|----------|----------|-------------------|
| R-01 | Quality | **BookingService untested** — payment and state-machine bugs reach production undetected | 4 | 5 | **20** | Critical | 0 unit tests; 5 validator tests only; no booking integration beyond nearby craftsmen | BookingService test suite; E2E booking smoke in CI |
| R-02 | Quality | **Payment gateway untested** — Moyasar integration failures or incorrect charges | 3 | 5 | **15** | Critical | `MoyasarPaymentGateway` has 0 tests; webhook tests mock only | Gateway unit tests + sandbox integration tests |
| R-03 | Delivery | **Android CI job incomplete** — broken Android builds not caught | 4 | 4 | **16** | Critical | `ci.yml` android job ends after `chmod +x gradlew` | Add `./gradlew assembleDebug test` |
| R-04 | Delivery | **No iOS CI** — iOS regressions undetected | 4 | 4 | **16** | Critical | No workflow for `src/ios` | Add xcodebuild + XCTest job (or manual gate) |
| R-05 | Parity | **Mobile missing email verify / sessions / change-password** — security UX gap vs web | 4 | 3 | **12** | High | Web has routes; Android/iOS do not | Align mobile identity features or document intentional deferral |
| R-06 | Parity | **Coupon validation missing on mobile** — billing discrepancy vs web | 3 | 4 | **12** | High | Web `supportApi.validateCoupon`; iOS `couponCode: nil` | Implement coupon UI + API on mobile |
| R-07 | Parity | **Admin console web-only** — ops depend on single client | 3 | 4 | **12** | High | No admin on Android/iOS | Accept for now; ensure admin web availability SLA |
| R-08 | Quality | **Controller layer largely untested** — 24 controllers, ~4 endpoints in integration tests | 4 | 3 | **12** | High | ApiIntegrationTests: 7 tests total | Expand integration suite per controller group |
| R-09 | Quality | **Zero client automated tests** — UI regressions on 3 platforms | 4 | 3 | **12** | High | No Jest/Playwright/JUnit/XCTest in repo | Minimum smoke tests per client |
| R-10 | CI/CD | **Frontend CI: build only** — no lint, type-check gate, or unit tests | 3 | 3 | **9** | Medium | `npm run build` only | Add `npm run lint` + test script |
| R-11 | CI/CD | **Staging smoke not in main CI** — compose stack failures on main undetected | 3 | 3 | **9** | Medium | `staging-smoke.yml` separate; `ci.yml` does not call it | Wire smoke as required check on main PRs |
| R-12 | CI/CD | **Smoke tests shallow** — no booking, payment, or subscription flows | 4 | 3 | **12** | High | `smoke-test.sh`: health, SPA shell, optional login | Extend smoke for core API flows |
| R-13 | Security | **No dependency / SAST scanning in CI** | 3 | 4 | **12** | High | No CodeQL, Dependabot gates, or `dotnet list package --vulnerable` | Add security scanning jobs |
| R-14 | Security | **No code coverage gate** — untested code merges freely | 4 | 2 | **8** | Medium | `dotnet test` without coverage threshold | Coverlet + minimum threshold on critical assemblies |
| R-15 | Data | **Android partial offline** — stale catalog shown without freshness indicator | 3 | 2 | **6** | Medium | Room cache without TTL UX | Show last-updated; force refresh on error |
| R-16 | Data | **iOS no offline cache** — poor connectivity = empty app | 3 | 2 | **6** | Medium | Network-only fetches | Optional cache or offline messaging |
| R-17 | i18n | **Admin UI mixed EN/AR** — operator confusion for Arabic admins | 3 | 2 | **6** | Medium | `moduleConfig` bilingual titles; forms often English | Full admin i18n pass |
| R-18 | i18n | **Web location catalog error hardcoded EN** | 2 | 1 | **2** | Low | `useLocationCatalog.ts` | Use `t()` for errors |
| R-19 | Ops | **Docker CI builds but does not run containers** — runtime config errors missed | 2 | 3 | **6** | Medium | `docker build` only | Add compose up + health check in CI |
| R-20 | Compliance | **AuditService untested** — audit trail integrity unverified | 2 | 4 | **8** | Medium | No AuditService tests | Unit tests for audit write paths |
| R-21 | Integrations | **Email/SMS providers untested** — auth flows fail silently in prod | 3 | 4 | **12** | High | EmailService/SmsService 0 tests | Provider abstraction tests + integration readiness gate |
| R-22 | Integrations | **Push notification services untested** | 3 | 3 | **9** | Medium | Firebase/APNs services 0 tests | Mock provider tests |
| R-23 | Business | **Cross-client feature drift** — customers see different capabilities per platform | 4 | 3 | **12** | High | See CLIENT_FEATURE_MATRIX parity table | Feature flags + parity checklist per release |
| R-24 | Availability | **Single integration test fixture** — parallel CI may cause flakiness as suite grows | 2 | 3 | **6** | Medium | One `KhadamatiWebApplicationFactory` | Test isolation review |
| R-25 | Security | **Committed JWT + SQL credentials** — token forgery / DB compromise | 5 | 5 | **25** | Critical | `appsettings.json`, `docker-compose.yml` in git history | Phase 1 secrets migration + rotation |
| R-26 | Data | **EF vs SQL script schema drift** — wrong deploy path corrupts DB | 4 | 5 | **20** | Critical | `000_MasterDeploy.sql` vs EF migrations | Phase 1 archive SQL scripts; EF-only deploy |
| R-27 | Data | **No optimistic concurrency on bookings/slots** — double-booking under load | 4 | 4 | **16** | Critical | No `rowversion` on ServiceRequests, BookingSlotReservations | Phase 1 add rowversion + retry |
| R-28 | Architecture | **AdminService monolith (1,455 LOC)** — change blast radius | 4 | 3 | **12** | High | 52 public methods across 8 domains | Phase 2 service decomposition |
| R-29 | API | **Duplicate profile endpoints** — inconsistent client behavior | 3 | 3 | **9** | Medium | `/users/me` vs `/profile` vs `/auth/me` | Phase 2 API consolidation |
| R-30 | Database | **Missing geo index** — nearby search degrades at scale | 4 | 3 | **12** | High | No lat/long index in EF; integration test flaky | Phase 1 index + query tuning |

---

## Detailed Risk Profiles (Top Critical & High)

### R-01 — BookingService Untested
| Field | Detail |
|-------|--------|
| **Description** | Core booking lifecycle (653 LOC) has zero dedicated unit tests |
| **Business impact** | Failed bookings, payment errors, customer churn |
| **Technical impact** | State machine regressions undetected; refactor blocked |
| **Dependencies** | BookingService, PaymentWebhookService, MoyasarPaymentGateway |
| **Recommended phase** | **Phase 1** |

### R-25 — Committed Secrets
| Field | Detail |
|-------|--------|
| **Description** | JWT secret and SQL SA password in tracked config and compose files |
| **Business impact** | Full platform compromise; regulatory breach |
| **Technical impact** | Credential rotation required; git history exposure |
| **Dependencies** | All environments, CI/CD secret injection |
| **Recommended phase** | **Phase 1** (per DEPLOYMENT_RUNBOOK §10) |

### R-26 — Schema Drift
| Field | Detail |
|-------|--------|
| **Description** | Legacy SQL scripts use ref.* tables; EF uses enum ints and different names |
| **Business impact** | Failed deployments, data loss on wrong script execution |
| **Technical impact** | Views/SPs reference obsolete Payments/Reviews tables |
| **Dependencies** | DATABASE_OBJECT_INVENTORY, deployment runbook |
| **Recommended phase** | **Phase 1** |

### R-27 — Booking Concurrency
| Field | Detail |
|-------|--------|
| **Description** | No rowversion on hot booking tables |
| **Business impact** | Double bookings, craftsman schedule conflicts |
| **Technical impact** | Race conditions under concurrent slot reservation |
| **Dependencies** | BookingService, BookingSlotReservations filtered unique index |
| **Recommended phase** | **Phase 1** |

### R-28 — AdminService Monolith
| Field | Detail |
|-------|--------|
| **Description** | Single service handles dashboard, catalog, locations, marketing, RBAC, backup |
| **Business impact** | Slow feature delivery; admin regressions span modules |
| **Technical impact** | SRP violation; difficult testing |
| **Dependencies** | 40+ admin API endpoints |
| **Recommended phase** | **Phase 2** |

---

## Risk by Recommended Phase

| Phase | Risk IDs |
|-------|----------|
| **Phase 1** (Hardening) | R-01, R-02, R-03, R-12, R-25, R-26, R-27, R-30 |
| **Phase 2** (Architecture) | R-05, R-06, R-08, R-28, R-29, R-13, R-21 |
| **Phase 3** (Scale) | R-15, R-16, R-19, R-24 |
| **Accept / Defer** | R-07 (admin web-only), R-18 (low i18n) |

---

## CI/CD Pipeline Analysis

### `.github/workflows/ci.yml`

| Job | Stages | Status |
|-----|--------|--------|
| `backend` | checkout → setup-dotnet 8 → restore → build → **test** | ✅ Functional |
| `frontend` | checkout → setup-node 22 → npm ci → **build** | ⚠️ No lint/test |
| `docker` | checkout → build API image → build web image | ⚠️ No run/scan |
| `android` | checkout → setup-java 17 → chmod gradlew | ❌ **Incomplete** |

**Missing gates:**
- iOS build/test
- Code coverage reporting / thresholds
- Lint (ESLint, dotnet format, ktlint, SwiftLint)
- Security scanning (SAST, dependency audit)
- Staging smoke (not triggered from ci.yml)
- E2E / contract tests
- Artifact signing / SBOM
- Parallel job failure aggregation (jobs are independent; no deploy gate)

### `.github/workflows/staging-smoke.yml`

| Stage | Action |
|-------|--------|
| Trigger | push/PR to `main`, workflow_dispatch |
| Prepare | Copy `.env.staging.example`, chmod scripts |
| Start | `docker compose -f docker-compose.staging.yml up -d --build` |
| Wait | Poll `GET /api/v1/health/ready` (240s timeout) |
| Smoke | `./scripts/smoke-test.sh http://localhost:8080` |
| Teardown | `docker compose down -v` (always) |

**Smoke script coverage (`scripts/smoke-test.sh`):**
- `GET /api/v1/health` (via proxy or direct)
- `GET /api/v1/health/ready`
- `GET /api/v1/health/integrations`
- `GET /` SPA shell
- Optional `POST /auth/login` (bootstrap admin from `.env.staging`)

**Missing smoke gates:**
- Authenticated booking flow
- Payment webhook simulation
- Subscription listing
- Web admin login
- Database migration verification
- Mobile API contract checks

---

## Risk Heatmap (by category)

```
Category          Critical  High  Medium  Low
─────────────────────────────────────────────
Quality               2      4      2     0
Delivery              2      0      0     0
Parity                0      4      0     0
CI/CD                 0      1      4     0
Security              0      1      1     0
Data                  0      0      2     0
i18n                  0      0      1     1
Ops                   0      0      1     0
Integrations          0      1      1     0
Business              0      1      0     0
Availability          0      0      1     0
Compliance            0      0      1     0
```

---

## Top 5 Immediate Actions

1. **R-01 / R-02:** Add BookingService and payment path tests before next release.
2. **R-03 / R-04:** Complete Android CI and add iOS build job.
3. **R-11 / R-12:** Promote staging smoke to required PR check; extend smoke script.
4. **R-06 / R-23:** Document and close mobile/web billing parity gap (coupon flow).
5. **R-13:** Add dependency vulnerability scanning to `ci.yml`.

---

## Traceability

| Document | Purpose |
|----------|---------|
| [CLIENT_FEATURE_MATRIX.md](./CLIENT_FEATURE_MATRIX.md) | Route/API/parity baseline |
| [TEST_COVERAGE_ANALYSIS.md](./TEST_COVERAGE_ANALYSIS.md) | Test inventory and gaps |
| [API_INVENTORY.md](./API_INVENTORY.md) | Backend endpoint reference |
| [TRACEABILITY_MATRIX.md](./TRACEABILITY_MATRIX.md) | Requirements mapping |
