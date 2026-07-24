# 33. Logging Strategy

**Document ID:** KHAD-V1-LOG  
**Status:** Draft for Approval  

---

## 33.1 Principles

- Structured JSON logs  
- Correlation across request → jobs → webhooks  
- No secrets, PAN, CVV, raw tokens, or unnecessary PII  
- Security and audit channels separated from debug noise  

## 33.2 Log Levels

| Level | Usage |
|-------|-------|
| ERROR | Failures needing attention |
| WARN | Recoverable anomalies (retryable provider errors) |
| INFO | Business milestones (booking confirmed, payment captured) |
| DEBUG | Dev/staging diagnostics only |

## 33.3 Required Fields

`timestamp`, `level`, `service`, `env`, `correlationId`, `userId?`, `storeId?`, `bookingId?`, `paymentId?`, `event`, `message`, `errorCode?`

## 33.4 Technology

- Spring Boot: Logback JSON encoder (or Log4j2 JSON)  
- Ship to centralized store (ELK/Loki/CloudWatch — Q-OBS-001)  
- Mobile: crash/analytics tooling TBD Q-OBS-002; do not log tokens  

## 33.5 Audit vs Application Logs

| Type | Store | Mutability |
|------|-------|------------|
| Application logs | Log platform | ephemeral |
| Audit events | `audit_events` table | append-only |
| Payment webhook raw | restricted storage | retention-limited |

## 33.6 Retention

App logs retention Q-OBS-003; audit retention often longer (compliance-driven).
