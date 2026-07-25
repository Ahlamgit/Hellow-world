# ADR-032: Notification Delivery Architecture

## Status

**Accepted Architecture Direction** — 2026-07-25  
**Implementation:** **Not authorized**

## Related ADRs

- [ADR-012](./ADR-012-redis-workers-required.md) — Redis + background workers (notification dispatch)
- [ADR-020](./ADR-020-chat-architecture.md) — Booking-scoped chat
- [ADR-025](./ADR-025-external-integration-strategy.md) — Integration ports and adapters
- [ADR-029](./ADR-029-simplified-payment-experience-ledger-control.md) — Simplified customer experience
- [ADR-030](./ADR-030-booking-payment-financial-state-separation.md) — Domain state separation
- [ADR-031](./ADR-031-payment-failure-retry-and-recovery-strategy.md) — Payment recovery (notification coupling for payment status)

## Sources

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FINAL_SCOPE_BASELINE.md`
- `FEATURE_TRACEABILITY_MATRIX.md`

---

## 1. Context

KHADAMATI must communicate timely, relevant information to customers, providers, and administrators without coupling the platform to specific SMS, email, or push vendors (ADR-025). Notifications support marketplace operations — they are **not** a substitute for booking-scoped chat (ADR-020).

### Notification needs by actor

**Customer**

| Category | Examples |
|----------|----------|
| Booking | Confirmation, changes, reminders |
| Payment | Status updates (simple labels per ADR-029/031 — not ledger detail) |
| Service | Progress, completion |
| Engagement | Review prompts |

**Provider**

| Category | Examples |
|----------|----------|
| Booking | New requests, changes, cancellations |
| Payment | Payout-related summaries (not ledger operations) |
| Operations | Availability reminders, schedule alerts |

**Admin**

| Category | Examples |
|----------|----------|
| Security | Suspicious activity, MFA events |
| Financial | Reconciliation alerts, policy anomalies |
| Operations | Disputes, moderation, system health |

### Design goals

| Goal | Description |
|------|-------------|
| **Reliable delivery** | Retries, status tracking, audit without blocking business domains |
| **Vendor independence** | SMS, email, push behind ports/adapters |
| **Simple UX** | Business messages only — no technical delivery failures (ADR-029) |
| **Clear boundaries** | Notifications inform; chat converses (ADR-020) |

**Unchanged:** Approved architecture, frozen scope, Payment.js/ledger models, and chat rules. This ADR defines notification **delivery architecture** only.

---

## 2. Decision

Notifications are **event-driven**.

```text
Business domain event (e.g. BookingConfirmed)
  ↓
Notification event (normalized, role-scoped)
  ↓
Notification orchestration (templates, rules, channel selection)
  ↓
Delivery channels (per user preferences / policy)
  ↓
Channel adapters (in-app store, push, SMS, email)
```

| Layer | Responsibility |
|-------|----------------|
| **Business modules** | Emit domain events when state changes (booking, payment, service, security) — do not call SMS/email SDKs directly |
| **Notification module** | Subscribe to events; resolve templates; enforce role and consent; dispatch to channels |
| **Adapters** | Vendor-specific I/O behind `SmsSenderPort`, `EmailSenderPort`, `PushSenderPort` (ADR-025) |
| **In-app store** | System of record for user-visible notification inbox (primary channel) |

Business modules **create events**. The notification system **delivers messages**.

---

## 3. Channel separation

| Tier | Channel | Role |
|------|---------|------|
| **Primary** | **In-app notifications** | Authoritative user inbox; always attempted for in-scope events |
| **Secondary** | **Push notifications** | Mobile/web push for timely alerts; deep-link to in-app context |
| **Optional** | **SMS** | High-urgency or OTP-adjacent flows where configured; behind `SmsSenderPort` |
| **Optional** | **Email** | Receipts, summaries, admin reports where configured; behind `EmailSenderPort` |

### Rules

- Channel selection is **policy- and preference-driven** (Admin templates/rules per FTM `BR-ADM-15`–`17`) — not hardcoded per vendor.
- Vendor implementations remain **behind adapters** (ADR-025). Domain code depends on ports only.
- In-app delivery does not require external vendor; push/SMS/email may degrade gracefully if vendor unavailable.
- Payment notifications expose **customer-safe status labels** only (ADR-029, ADR-031) — never card data, webhook refs, or ledger detail.

---

## 4. Failure handling

Notification delivery must be **resilient** without corrupting business domain state.

| Concern | Architecture expectation |
|---------|------------------------|
| **Retry strategy** | Transient adapter failures retried with backoff at worker/adapter boundary (ADR-012); idempotent delivery keys prevent duplicate user-visible spam |
| **Delivery status** | Per-delivery record: pending → sent → delivered / failed / skipped (conceptual — no schema in this ADR) |
| **Failed notification handling** | Failed SMS/email/push logged for Admin ops; in-app may still succeed; business event already committed — notification failure does not roll back booking/payment |
| **Audit logging** | Dispatch attempts, channel, template id, recipient role, outcome — redact sensitive payloads |
| **Dead letter / ops queue** | Persistent failures surfaced to Admin for investigation (not to end users) |

Aligns with event-driven, recoverable patterns in ADR-031 — applied to notification delivery, not payment ledger.

---

## 5. User experience

Users see **relevant business messages**:

| User sees | Examples |
|-----------|----------|
| Booking confirmed | "Your booking with {provider} is confirmed" |
| Payment processing | "We're confirming your payment" (ADR-031) |
| New booking request | Provider: "New booking request for {service}" |
| Chat activity | "New message about your booking" (deep-link to chat — ADR-020) |

Users do **not** see:

- SMTP errors, push token failures, SMS gateway codes
- Adapter timeouts or vendor names
- Internal queue or worker errors

Technical failures map to silent retry, fallback channel (e.g. in-app only), or generic "try again later" — never raw delivery diagnostics.

---

## 6. Security

| Rule | Confirmation |
|------|--------------|
| **No sensitive payment information** | No PAN, CVV, full card tokens, ledger entries, or commission breakdown in notification body |
| **Access based on user role** | Recipients receive only notifications for their actor context (customer, provider, admin) |
| **Admin/security alerts** | Restricted to authorized admin roles; audited |
| **PII minimization** | Message content limited to what the recipient needs for the business action |
| **Adapter credentials** | Secrets in secure configuration — not in notification templates |

OTP flows may use SMS via `SmsSenderPort` but OTP generation/validation remains in IAM domain — not notification-owned business logic.

---

## 7. Chat relationship

| Statement | Confirmation |
|-----------|--------------|
| Chat remains **booking-scoped only** | **Confirmed** (ADR-020) |
| Notifications may inform about chat activity | **Confirmed** — e.g. push/in-app "new message" with deep-link |
| Notifications do **not** create open messaging | **Confirmed** — no notification-initiated stranger contact |
| Chat transport ≠ notification transport | Chat uses `ChatRealtimePort`; notifications use notification dispatcher + channel ports |

Domain flow (per ADR-020): `ChatMessageCreated` → outbox → notification dispatcher → push + in-app for recipient. Notification is **fan-out**, not the chat system of record.

---

## 8. Implementation benefits

When implementation is authorized (post Gate A), this architecture is expected to yield:

| Benefit | Description |
|---------|-------------|
| Vendor swap | Change SMS/email/push provider without rewriting booking/payment modules |
| Testability | Domain events mocked; adapters faked in integration tests |
| Reliability | Retries and delivery status without blocking core transactions |
| UX consistency | Single in-app inbox; optional external channels |
| Operational clarity | Admin sees failed deliveries; users see business messages only |

*Benefits are architectural intent only — no coding authorized by this ADR.*

---

## 9. Traceability

**Principle (FTM):** `BR-NTF-01` — *Notifications are event-driven, vendor-agnostic via ports/adapters; in-app primary; business modules emit events, notification module delivers.*

Maps to existing FTM rows:

- `BR-CUS-16`, `BR-CRF-22` — User in-app inboxes
- `BR-ADM-15`–`17` — Admin templates, channel config, rules
- `BR-QUA-01` — Scheduled reminders via worker + notification
- `BR-IAM-03` — OTP (SMS optional channel via port)
- ADR-020 chat fan-out integration

**Documents updated by this ADR:**

- `docs/v1/adr/README.md` — ADR-032 indexed
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` — notification delivery reference
- `FEATURE_TRACEABILITY_MATRIX.md` — BR-NTF-01 added

---

## 10. Governance rule

| Statement | Confirmation |
|-----------|--------------|
| This ADR authorizes coding | **No** |
| This ADR authorizes schema | **No** |
| This ADR selects SMS/email/push vendors | **No** |
| This ADR modifies frozen scope | **No** |
| This ADR modifies approved architecture (ADR-001–028 core) | **No** — extends delivery pattern only |
| This ADR changes implementation gate | **No** |

Implementation remains **BLOCKED** until Gate A. Vendor selection remains **BLOCKER-003** governance.

---

## Consequences

- Business modules must publish domain events (or outbox) rather than invoking channel SDKs.
- Notification module owns template resolution, channel routing, delivery status, and retry.
- In-app notification store is the primary user-facing channel; push/SMS/email are augmentations.
- Chat and notification remain distinct bounded contexts with explicit integration at `ChatMessageCreated`.
- Admin configures templates and channel rules without code deploy for copy changes where product allows.
