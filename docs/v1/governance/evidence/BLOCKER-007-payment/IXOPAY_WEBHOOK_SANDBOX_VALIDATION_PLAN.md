# BLOCKER-007 — IXOPAY Webhook Sandbox Validation Plan

| Field | Value |
|-------|-------|
| **Document ID** | EVD-007-WEBHOOK-PLAN-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-007 — Payment.js |
| **Status** | **Design / test plan** — **NO IMPLEMENTATION** until Gate A |
| **Dev environment** | `localhost` + tunnel (see BLOCKER-004 dev decision) |

```text
This document defines HOW webhook sandbox validation will be executed.
It does NOT authorize payment code in the repository before Gate A.
```

---

## 1. Approved business flow (reference)

```text
Booking → Payment initiation → Areeba IXOPAY Payment.js → Payment confirmation
→ Booking confirmation → Ledger recording → Settlement
```

---

## 2. Webhook architecture (design only)

| Component | Responsibility |
|-----------|----------------|
| IXOPAY sandbox | Sends payment status webhooks |
| Webhook endpoint | Receives POST, verifies signature, idempotent processing |
| Booking service | Correlates `bookingId` / `merchantTransactionId` |
| Ledger service | Immutable financial event (ADR-004) — post-Gate A |

**Pattern:** Ports/adapters (ADR-025) — `PaymentWebhookPort` implementation after Gate A.

---

## 3. Sandbox webhook URL strategy (localhost testing)

| Phase | Webhook URL pattern | Tool |
|-------|---------------------|------|
| **Gate B validation** | `https://<tunnel-subdomain>.ngrok.io/api/v1/payments/webhooks/ixopay` | ngrok / Cloudflare Tunnel |
| **Local target** | `http://localhost:<API_PORT>/api/v1/payments/webhooks/ixopay` | Dev machine |
| **Staging** | TBD post–cloud decision | — |

**Configure in IXOPAY sandbox dashboard** after endpoint exists (post–Gate A).

---

## 4. Webhook validation checklist (manual / sandbox)

Execute per `AREEBA_IXOPAY_FINAL_VALIDATION_CHECKLIST.md`:

| # | Test | Method | Pass |
|---|------|--------|------|
| W-01 | Sandbox sends test webhook | IXOPAY dashboard test event | ☐ |
| W-02 | Signature header validated | Reject invalid signature | ☐ |
| W-03 | `payment.success` payload parsed | Log correlation ID | ☐ |
| W-04 | `payment.failed` payload parsed | Log failure reason | ☐ |
| W-05 | Duplicate `eventId` ignored | Send same payload twice | ☐ |
| W-06 | HTTP 200 acknowledgment | IXOPAY retry behavior | ☐ |

**Record results in:** `AREEBA_IXOPAY_VALIDATION_REPORT.md`

---

## 5. Expected webhook payload fields (reference)

Document at validation time from IXOPAY sandbox docs (do not invent):

| Field | Purpose |
|-------|---------|
| Event type | success / failure / cancel |
| Transaction reference | Correlate to booking |
| Amount / currency | Reconcile with booking |
| Signature | HMAC verification |

---

## 6. Implementation gate

| Action | Gate B | Post–Gate A |
|--------|--------|-------------|
| This validation plan | ☑ Allowed | — |
| Webhook route code | ✗ **Forbidden** | Authorized after Gate A |
| Ledger write on webhook | ✗ **Forbidden** | Authorized after Gate A |
| Production webhooks | ✗ **Forbidden** | After staging validation |

**BLOCKER-007 NOT CLOSED** until sandbox tests executed and IL + TA sign `AREEBA_IXOPAY_VALIDATION_REPORT.md`.

---

## 7. Dependencies

| Blocker | Requirement |
|---------|-------------|
| BLOCKER-004 | Localhost dev approved |
| BLOCKER-005 | Ledger/finance model approved |
| BLOCKER-003 | IXOPAY vendor path (sandbox credentials) |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Webhook sandbox validation plan — no implementation |
