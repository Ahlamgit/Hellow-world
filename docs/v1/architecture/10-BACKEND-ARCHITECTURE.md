# 10. Backend Architecture

**Document ID:** KHAD-V1-BE  
**Status:** Draft for Approval  
**Stack:** Spring Boot 3.x · Java 21 · Maven · Spring Security · Spring Data JPA · Flyway · OpenAPI  

---

## 10.1 Style: Clean Layered Modular Monolith

Each module follows:

```text
adapter/in  (REST controllers, webhook endpoints, schedulers)
application (use cases / application services, ports)
domain      (entities, value objects, domain services, domain events)
adapter/out (JPA repos, gateway clients, SMTP/SMS, S3)
```

Cross-module calls prefer **application services/interfaces**, not JPA entities across boundaries.

## 10.2 Recommended Maven Structure

```text
khadamati-backend/
  pom.xml (parent)
  khadamati-api/                 # Spring Boot app, security config, OpenAPI
  khadamati-modules/
    khadamati-platform/
    khadamati-iam/
    khadamati-customer/
    khadamati-craftsman/
    khadamati-store/
    khadamati-catalog/
    khadamati-booking/
    khadamati-payment/
    khadamati-commission/
    khadamati-subscription/
    khadamati-notification/
    khadamati-identity-verification/
    khadamati-ads/
    khadamati-rating/
    khadamati-quality/
    khadamati-reporting/
    khadamati-media/
    khadamati-audit/
  khadamati-commons/             # result types, errors, money, ids
```

Alternative acceptable V1 approach: single Maven module with package-level module boundaries if team size is small — still enforce architectural tests (ArchUnit).

## 10.3 Core Backend Patterns

| Pattern | Usage |
|---------|-------|
| Hexagonal ports/adapters | Payments, notifications, OCR, face, storage |
| Transactional Outbox | Reliable domain event publication |
| Idempotency keys | Payment, booking create, withdrawal request |
| Domain events | Decouple booking → notify/commission/quality |
| Specification / policy objects | Cancellation, entitlement, restriction rules |
| MapStruct (optional) | DTO mapping |
| Bean Validation | Input validation |
| Problem Details (RFC 7807) | Error responses |
| Spring `@Scheduled` / job runner | Reminders, expirations |
| Optimistic locking (`@Version`) | Booking and payment aggregates |

## 10.4 Payment Module Design (Critical)

```text
application/port/PaymentGatewayPort
  - debit(PaymentDebitCommand): GatewayResult
  - refund(...)
  - parseCallback(...)

adapter/out/areeba/IxopayPaymentGatewayAdapter
  - implements PaymentGatewayPort using IXOPAY server API

adapter/in/web/PaymentController
adapter/in/web/IxopayWebhookController
```

Business use cases (`PayBooking`, `PaySubscription`) depend on `PaymentGatewayPort` only.

## 10.5 Security Integration

- Spring Security filter chain  
- JWT decoder/encoder  
- Method security (`@PreAuthorize`) aligned with permission codes  
- Webhook endpoints authenticated via gateway signature/shared secret, not user JWT  

## 10.6 Transaction Boundaries

- One use case ≈ one transaction for core aggregate update + outbox insert  
- External provider I/O outside DB transactions when possible; persist “pending” first  
- Webhook handlers idempotent by gateway transaction id  

## 10.7 API Layer Conventions

- Base path: `/api/v1`  
- Admin: `/api/v1/admin/...`  
- Store: `/api/v1/store/...`  
- Mobile customer: `/api/v1/customer/...` (or role-agnostic resources with authz)  
- Mobile craftsman: `/api/v1/craftsman/...`  
- Public auth: `/api/v1/auth/...`  
- Webhooks: `/api/v1/webhooks/...`  

Exact resource naming in [16-API-DESIGN.md](./16-API-DESIGN.md).

## 10.8 Observability Hooks

- Correlation ID filter (`X-Correlation-Id`)  
- Structured JSON logging  
- Micrometer metrics (Timers for payment gateway, booking create)  
- OpenTelemetry tracing optional for V1 (recommended)

## 10.9 Testing Architecture

- Unit tests for domain policies  
- `@SpringBootTest` / `@WebMvcTest` slice tests  
- Testcontainers PostgreSQL for repository/integration  
- Contract tests for OpenAPI  
- ArchUnit for layer dependency rules  

## 10.10 Java Language Baseline

- Java 21 LTS features allowed: records, sealed types, pattern matching, virtual threads **candidate** for blocking gateway calls (decision Q-BE-001)  
- Nullability: favor clear Optional usage at boundaries; prefer non-null domain invariants  

## 10.11 Questions Requiring Business Decision

- Q-BE-001: Adopt virtual threads in V1?  
- Multi-module Maven vs single module packaging preference (engineering decision; both compliant)
