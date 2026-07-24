# KHADAMATI Platform

Enterprise marketplace connecting **Customers**, **Craftsmen**, **Stores**, and **Administrators**.

## Version 1 Status

KHADAMATI V1 is in **architecture & discovery**. Implementation has not started for the new stack.

**Architecture pack (for approval):** [`docs/v1/00-INDEX.md`](./docs/v1/00-INDEX.md)

### V1 Target Stack (Fixed)

| Layer | Technology |
|-------|------------|
| Backend | Spring Boot 3.x, Java 21, Spring Security, Spring Data JPA, Maven |
| API | REST, JWT, OpenAPI |
| Database | PostgreSQL, Flyway |
| Administration Portal | React, TypeScript, Material UI |
| Store Dashboard | React, TypeScript, Material UI |
| Customer App | Flutter |
| Craftsman App | Flutter |
| Payments | Areeba IXOPAY Payment.js only (gateway abstraction required) |

### Process Gate

1. Approve architecture pack + disposition open questions  
2. Upload UI design video, screenshots, branding, colors, UX requirements  
3. **Only then** begin implementation (UI must follow provided designs)

Open questions: [`docs/v1/QUESTIONS-REQUIRING-BUSINESS-DECISION.md`](./docs/v1/QUESTIONS-REQUIRING-BUSINESS-DECISION.md)

## Legacy Codebase Notice

Existing `/src` content and older docs under `/docs/*.md` reflect a **previous** direction (ASP.NET / SQL Server / native mobile). They are **not** authoritative for V1 without re-validation.

## License

Proprietary — KHADAMATI Platform
