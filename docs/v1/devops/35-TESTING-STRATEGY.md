# 35. Testing Strategy

**Document ID:** KHAD-V1-QA  
**Status:** Draft for Approval  

---

## 35.1 Test Pyramid

| Layer | Scope | Tools (suggested) |
|-------|-------|-------------------|
| Unit | Domain policies, calculators | JUnit, Dart tests, Vitest |
| Integration | Repositories, Flyway, adapters w/ WireMock | Testcontainers |
| API | HTTP contracts, authz negatives | MockMvc/RestAssured |
| UI component | Critical forms | Testing Library / Flutter widget |
| E2E | Staging journeys | Playwright (web), integration_test/Patrol (mobile) |
| Non-functional | Perf smoke, security scans | k6/JMeter candidate, SAST/DAST |

## 35.2 Mandatory Coverage Areas (P0)

- Auth login/refresh/lockout  
- RBAC deny paths  
- Booking transitions illegal/legal  
- Payment tokenize→debit→webhook idempotency  
- Commission calculation once rules decided  
- Onboarding approve/reject audit  
- GPS/OTP verification pass/fail  
- Notification dispatcher retries  
- Store tenant isolation  

## 35.3 Test Data

- Deterministic fixtures  
- No production PII in lower envs  
- IXOPAY sandbox credentials for staging payment tests  

## 35.4 Definition of Done (QA)

- Automated tests added/updated  
- OpenAPI examples validated  
- Acceptance criteria checked  
- POLICY-GATED cases skipped only if `Q-*` unresolved and tagged  

## 35.5 Environments for Testing

Local → CI → Staging (full E2E) → Production smoke (read-only + synthetic canaries carefully)

## 35.6 Questions

Performance SLOs targets: Q-KPI-001/002; penetration test scope Q-SEC-003
