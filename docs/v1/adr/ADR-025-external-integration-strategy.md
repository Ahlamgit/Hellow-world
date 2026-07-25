# ADR-025: External Integration Strategy (Ports & Adapters)

**Status:** Accepted — 2026-07-24  

---

## Decision

All external services use **abstraction layers (ports)**. Business domain must not depend on vendor SDKs/DTOs.

---

## Required Integrations & Ports

| Concern | Port (example) | V1 Adapter |
|---------|----------------|------------|
| Payment gateway | `PaymentGatewayPort` | Areeba IXOPAY |
| SMS | `SmsSenderPort` | TBD vendor |
| Email | `EmailSenderPort` | TBD vendor |
| Maps/GPS | `GeocodingPort` / client GPS validate | TBD maps provider |
| OCR | `OcrPort` | TBD |
| Face verification | `FaceVerificationPort` | TBD |

```text
KHADAMATI Payment Service
        ↓
Payment Adapter
        ↓
Areeba IXOPAY
```

Same pattern for SMS/Email/OCR/Face/Maps.

---

## Rules

- No direct business dependency on vendors  
- Config selects active adapter per Market when needed  
- Timeouts, retries, circuit breakers at adapter boundary  
- Webhooks verified in adapter/controller, mapped to domain events  

---

## Consequences

Vendor swaps (including future payment gateways) do not rewrite booking/ledger/commission logic.
