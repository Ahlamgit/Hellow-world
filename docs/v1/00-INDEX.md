# KHADAMATI Platform — Version 1 Architecture Pack

**Document Status:** Draft — Aligned to Master Prompt v1.0  
**Version:** 1.1.0-DRAFT  
**Date:** 2026-07-24  
**Classification:** Internal — Architecture & Product Discovery  
**Stack Decision (Fixed):** Spring Boot 3.x / Java 21 / PostgreSQL / Flyway / React+MUI / Flutter / JWT / Areeba IXOPAY Payment.js  
**Controlling brief:** [MASTER_IMPLEMENTATION_PROMPT_v1.0.md](./MASTER_IMPLEMENTATION_PROMPT_v1.0.md)  
**Final scope baseline (frozen):** [FINAL_SCOPE_BASELINE.md](./FINAL_SCOPE_BASELINE.md)  
**Implementation readiness plan:** [IMPLEMENTATION_READINESS_EXECUTION_PLAN.md](./IMPLEMENTATION_READINESS_EXECUTION_PLAN.md) — **B) NOT READY**; tracks BLOCKER-001…007  
**Blocker closure status:** [READINESS_BLOCKER_CLOSURE_STATUS.md](./READINESS_BLOCKER_CLOSURE_STATUS.md) — active closure (0/7)  
**Stakeholder sign-off:** [governance/STAKEHOLDER_SIGN_OFF_PACKAGE.md](./governance/STAKEHOLDER_SIGN_OFF_PACKAGE.md) — BLOCKER-002 **READY FOR APPROVAL**  
**Finance Lebanon config:** [config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md](./config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md) — BLOCKER-005 **IN PREPARATION**  
**Design foundation:** [design/DESIGN_APPROVAL_RECORD.md](./design/DESIGN_APPROVAL_RECORD.md) · [design/BRAND_IDENTITY_SPECIFICATION.md](./design/BRAND_IDENTITY_SPECIFICATION.md) · [design/UI_UX_SPECIFICATION.md](./design/UI_UX_SPECIFICATION.md) · [design/DESIGN_SYSTEM_TOKENS.md](./design/DESIGN_SYSTEM_TOKENS.md) · [design/COLOR_REFERENCE.md](./design/COLOR_REFERENCE.md) · [design/assets/LOGO_ASSET_PACKAGE.md](./design/assets/LOGO_ASSET_PACKAGE.md)  
**Implementation gate:** [FINAL_IMPLEMENTATION_GATE_REPORT.md](./FINAL_IMPLEMENTATION_GATE_REPORT.md)  
**Payment.js validation:** [payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md](./payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md](./payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) — BLOCKER-007 **IN VALIDATION**  
**Feature Traceability:** [FEATURE_TRACEABILITY_MATRIX.md](./FEATURE_TRACEABILITY_MATRIX.md)  
**ADRs:** [adr/README.md](./adr/README.md)  
**Admin configurable finance rules:** [architecture/47-ADMIN-CONFIGURABLE-FINANCIAL-RULES.md](./architecture/47-ADMIN-CONFIGURABLE-FINANCIAL-RULES.md) · [ADR-013](./adr/ADR-013-admin-configurable-financial-business-rules.md)  
**Alignment changelog:** [ALIGNMENT_CHANGELOG_MASTER_PROMPT_v1.md](./ALIGNMENT_CHANGELOG_MASTER_PROMPT_v1.md)  

---

## Purpose

This pack is the **complete discovery and architecture deliverable** for KHADAMATI Version 1 (MVP+).

It is intentionally **implementation-free**. No application code, UI templates, or schema scripts are produced until this pack is approved and design assets (UI video, screenshots, branding, UX) are uploaded.

**This is a new platform version.** Prior KHADAMATI implementations (ASP.NET / SQL Server / native mobile) must not be treated as validated business truth. Any carry-forward assumption is either re-validated here or listed under **Questions Requiring Business Decision**.

---

## Approval Gate

| Gate | Owner | Outcome Required |
|------|-------|------------------|
| Business rules & open questions | Product / Business | Decisions recorded |
| Architecture & NFRs | Solution Architecture | Approved |
| Security & compliance posture | Security / Compliance | Approved |
| Scope & roadmap (V1 vs V2) | Product + Engineering | Approved |
| UI/UX assets | Design | Uploaded (post-approval) |

**Implementation begins only after all gates pass.**

---

## Document Map (45 Deliverables)

| # | Deliverable | Document |
|---|-------------|----------|
| 1 | Functional Requirements Specification | [requirements/01-FUNCTIONAL-REQUIREMENTS.md](./requirements/01-FUNCTIONAL-REQUIREMENTS.md) |
| 2 | Business Requirements | [requirements/02-BUSINESS-REQUIREMENTS.md](./requirements/02-BUSINESS-REQUIREMENTS.md) |
| 3 | System Scope | [requirements/03-SYSTEM-SCOPE.md](./requirements/03-SYSTEM-SCOPE.md) |
| 4 | Actors | [requirements/04-ACTORS.md](./requirements/04-ACTORS.md) |
| 5 | Use Cases | [requirements/05-USE-CASES.md](./requirements/05-USE-CASES.md) |
| 6 | User Stories | [requirements/06-USER-STORIES.md](./requirements/06-USER-STORIES.md) |
| 7 | Acceptance Criteria | [requirements/07-ACCEPTANCE-CRITERIA.md](./requirements/07-ACCEPTANCE-CRITERIA.md) |
| 8 | Complete Module Breakdown | [architecture/08-MODULE-BREAKDOWN.md](./architecture/08-MODULE-BREAKDOWN.md) |
| 9 | System Architecture | [architecture/09-SYSTEM-ARCHITECTURE.md](./architecture/09-SYSTEM-ARCHITECTURE.md) |
| 10 | Backend Architecture | [architecture/10-BACKEND-ARCHITECTURE.md](./architecture/10-BACKEND-ARCHITECTURE.md) |
| 11 | Frontend Architecture | [architecture/11-FRONTEND-ARCHITECTURE.md](./architecture/11-FRONTEND-ARCHITECTURE.md) |
| 12 | Flutter Architecture | [architecture/12-FLUTTER-ARCHITECTURE.md](./architecture/12-FLUTTER-ARCHITECTURE.md) |
| 13 | Database Architecture | [architecture/13-DATABASE-ARCHITECTURE.md](./architecture/13-DATABASE-ARCHITECTURE.md) |
| 14 | ER Diagram | [architecture/14-ER-DIAGRAM.md](./architecture/14-ER-DIAGRAM.md) |
| 15 | Entity Relationship Mapping | [architecture/15-ENTITY-RELATIONSHIP-MAPPING.md](./architecture/15-ENTITY-RELATIONSHIP-MAPPING.md) |
| 16 | API Design | [architecture/16-API-DESIGN.md](./architecture/16-API-DESIGN.md) |
| 17 | Folder Structure | [standards/17-FOLDER-STRUCTURE.md](./standards/17-FOLDER-STRUCTURE.md) |
| 18 | Coding Standards | [standards/18-CODING-STANDARDS.md](./standards/18-CODING-STANDARDS.md) |
| 19 | Naming Conventions | [standards/19-NAMING-CONVENTIONS.md](./standards/19-NAMING-CONVENTIONS.md) |
| 20 | Security Architecture | [standards/20-SECURITY-ARCHITECTURE.md](./standards/20-SECURITY-ARCHITECTURE.md) |
| 21 | Authentication Flow | [workflows/21-AUTHENTICATION-FLOW.md](./workflows/21-AUTHENTICATION-FLOW.md) |
| 22 | Authorization Flow | [workflows/22-AUTHORIZATION-FLOW.md](./workflows/22-AUTHORIZATION-FLOW.md) |
| 23 | Payment Flow (Areeba IXOPAY Payment.js) | [workflows/23-PAYMENT-FLOW.md](./workflows/23-PAYMENT-FLOW.md) |
| 24 | Booking Lifecycle | [workflows/24-BOOKING-LIFECYCLE.md](./workflows/24-BOOKING-LIFECYCLE.md) |
| 25 | Notification Flow | [workflows/25-NOTIFICATION-FLOW.md](./workflows/25-NOTIFICATION-FLOW.md) |
| 26 | Subscription Workflow | [workflows/26-SUBSCRIPTION-WORKFLOW.md](./workflows/26-SUBSCRIPTION-WORKFLOW.md) |
| 27 | Commission Workflow | [workflows/27-COMMISSION-WORKFLOW.md](./workflows/27-COMMISSION-WORKFLOW.md) |
| 28 | Craftsman Approval Workflow | [workflows/28-CRAFTSMAN-APPROVAL-WORKFLOW.md](./workflows/28-CRAFTSMAN-APPROVAL-WORKFLOW.md) |
| 29 | Identity Verification Workflow | [workflows/29-IDENTITY-VERIFICATION-WORKFLOW.md](./workflows/29-IDENTITY-VERIFICATION-WORKFLOW.md) |
| 30 | Advertisement Workflow | [workflows/30-ADVERTISEMENT-WORKFLOW.md](./workflows/30-ADVERTISEMENT-WORKFLOW.md) |
| 31 | Quality Follow-up Workflow | [workflows/31-QUALITY-FOLLOWUP-WORKFLOW.md](./workflows/31-QUALITY-FOLLOWUP-WORKFLOW.md) |
| 32 | Reporting Architecture | [devops/32-REPORTING-ARCHITECTURE.md](./devops/32-REPORTING-ARCHITECTURE.md) |
| 33 | Logging Strategy | [devops/33-LOGGING-STRATEGY.md](./devops/33-LOGGING-STRATEGY.md) |
| 34 | Error Handling Strategy | [devops/34-ERROR-HANDLING-STRATEGY.md](./devops/34-ERROR-HANDLING-STRATEGY.md) |
| 35 | Testing Strategy | [devops/35-TESTING-STRATEGY.md](./devops/35-TESTING-STRATEGY.md) |
| 36 | Deployment Architecture | [devops/36-DEPLOYMENT-ARCHITECTURE.md](./devops/36-DEPLOYMENT-ARCHITECTURE.md) |
| 37 | CI/CD Pipeline | [devops/37-CICD-PIPELINE.md](./devops/37-CICD-PIPELINE.md) |
| 38 | Environment Configuration | [devops/38-ENVIRONMENT-CONFIGURATION.md](./devops/38-ENVIRONMENT-CONFIGURATION.md) |
| 39 | Configuration Management | [devops/39-CONFIGURATION-MANAGEMENT.md](./devops/39-CONFIGURATION-MANAGEMENT.md) |
| 40 | Development Roadmap | [planning/40-DEVELOPMENT-ROADMAP.md](./planning/40-DEVELOPMENT-ROADMAP.md) |
| 41 | Milestones | [planning/41-MILESTONES.md](./planning/41-MILESTONES.md) |
| 42 | Sprint Breakdown | [planning/42-SPRINT-BREAKDOWN.md](./planning/42-SPRINT-BREAKDOWN.md) |
| 43 | Risks | [planning/43-RISKS.md](./planning/43-RISKS.md) |
| 44 | Technical Recommendations | [planning/44-TECHNICAL-RECOMMENDATIONS.md](./planning/44-TECHNICAL-RECOMMENDATIONS.md) |
| 45 | Future Version 2 Roadmap | [planning/45-FUTURE-V2-ROADMAP.md](./planning/45-FUTURE-V2-ROADMAP.md) |

**Cross-cutting:** [QUESTIONS-REQUIRING-BUSINESS-DECISION.md](./QUESTIONS-REQUIRING-BUSINESS-DECISION.md)  
**Final architecture audit:** [ARCHITECTURE_AUDIT_FINAL.md](./ARCHITECTURE_AUDIT_FINAL.md)  
**Final readiness report:** [FINAL_ARCHITECTURE_READINESS_REPORT.md](./FINAL_ARCHITECTURE_READINESS_REPORT.md)  
**Implementation gate report:** [FINAL_IMPLEMENTATION_GATE_REPORT.md](./FINAL_IMPLEMENTATION_GATE_REPORT.md)  
**Decisions complete:** [FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md](./FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md)  
**Readiness pack:** [readiness/](./readiness/)  
**Master prompt v1.0 (controlling):** [MASTER_IMPLEMENTATION_PROMPT_v1.0.md](./MASTER_IMPLEMENTATION_PROMPT_v1.0.md)  
**Feature Traceability Matrix:** [FEATURE_TRACEABILITY_MATRIX.md](./FEATURE_TRACEABILITY_MATRIX.md)  
**ADRs:** [adr/README.md](./adr/README.md)  
**Full prompt (earlier composite):** [FULL_PROMPT_INCLUDING_DESIGN.md](./FULL_PROMPT_INCLUDING_DESIGN.md)

---

## Fixed Technology Constraints (Non-Negotiable for V1)

| Area | Choice |
|------|--------|
| Backend | Spring Boot 3.x, Java 21, Maven |
| Security | Spring Security, JWT |
| Persistence | Spring Data JPA, PostgreSQL, Flyway |
| API | REST, OpenAPI |
| Admin Portal | React + TypeScript + Material UI |
| Store Dashboard | React + TypeScript + Material UI |
| Customer App | Flutter |
| Craftsman App | Flutter |
| Payments | Areeba IXOPAY Payment.js **only** (abstraction layer required) |

---

## How Ambiguity Is Handled

Wherever a business rule is not explicitly provided or cannot be derived from industry-standard marketplace patterns without inventing product policy, the document:

1. States what is **known / required**
2. States what is **out of inventable scope**
3. Adds an entry to **Questions Requiring Business Decision** with an ID (`Q-xxx`)

No invented commission rates, fee models, KYC thresholds, settlement cadences, or locale defaults are presented as final.

---

## Suggested Review Order

1. Scope + Actors + Open Questions  
2. Business & Functional Requirements  
3. Workflows (Booking, Payment, Approval, Identity, Commission)  
4. System / Backend / DB Architecture  
5. Security & Authz  
6. Roadmap / Milestones / Risks  
7. Frontend & Flutter (after UI assets arrive — architecture only until then)
