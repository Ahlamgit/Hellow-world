# 43. Risks

**Document ID:** KHAD-V1-RISKS  
**Status:** Draft for Approval  

| ID | Risk | Impact | Likelihood | Mitigation |
|----|------|--------|------------|------------|
| R-01 | Unresolved business rules delay schema freeze | High | High | Open-questions workshop before Sprint 1 coding of money domains |
| R-02 | Payment.js + mobile WebView 3DS friction | High | Medium | Early spike in Sprint 6; sandbox end-to-end; fallback UX for REQUIRES_ACTION |
| R-03 | Face/OCR provider quality insufficient | High | Medium | Provider PoC; admin override path; do not fully automate approval |
| R-04 | Premature microservices complexity | Medium | Medium | Modular monolith mandate for V1 |
| R-05 | Legacy .NET assumptions contaminate V1 | Medium | High | New docs/stack; explicit “do not reuse” gate |
| R-06 | UI built before designs → rework | High | High | Hard gate: no UI implementation until assets uploaded |
| R-07 | PCI scope creep if card data mishandled | Critical | Low | Payment.js only; security reviews; logging redaction |
| R-08 | Commission/refund disputes without clear policy | High | Medium | Finalize Q-COM/Q-BOOK before enabling payouts |
| R-09 | SMS cost overrun | Medium | Medium | Channel matrix; prefer push/in-app for non-critical |
| R-10 | Multi-region readiness over-engineered | Medium | Medium | Readiness seams only; single-region deploy |
| R-11 | Flutter Payment.js maintenance burden | Medium | Medium | Isolate payment WebView module; adapter tests |
| R-12 | Weak RBAC causes data leakage | Critical | Low | Authz tests mandatory; store scoping reviews |
| R-13 | Flyway/prod migration failure | High | Low | Expand/contract; staging dry runs; backup restore |
| R-14 | Provider outages (SMS/OCR/IXOPAY) | High | Medium | Timeouts, retries, circuit breakers, status pages |
| R-15 | Scope creep into V2 features | High | High | Milestone exit criteria; feature flags |

## Risk Review Cadence

Re-rank risks at each milestone gate; blockers escalate to product owners immediately.
