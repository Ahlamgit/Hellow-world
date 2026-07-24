# 24. Booking Lifecycle

**Document ID:** KHAD-V1-BOOK  
**Status:** Draft for Approval  

---

## 24.1 Purpose

Define the booking aggregate lifecycle connecting customer demand, craftsman/store fulfillment, payment, verification, and quality follow-up.

## 24.2 Proposed State Machine (Pending Confirmation)

| State | Meaning |
|-------|---------|
| `DRAFT` | Created, not confirmed |
| `PENDING_PAYMENT` | Awaiting successful payment |
| `REQUESTED` | Submitted without immediate capture (if allowed) |
| `CONFIRMED` | Ready to be fulfilled |
| `EN_ROUTE` | Craftsman traveling |
| `ARRIVED` | Arrival verified |
| `IN_PROGRESS` | Job challenge passed / work started |
| `COMPLETED` | Work finished |
| `CANCELLED` | Cancelled per policy |
| `DISPUTED` | Dispute opened |
| `EXPIRED` | Payment/acceptance timeout |

**Questions Requiring Business Decision:** exact states and transitions — Q-BOOK-008, Q-BOOK-009, Q-BOOK-010.

## 24.3 Main Flow (Customer Prepaid Model — Candidate)

```mermaid
stateDiagram-v2
  [*] --> DRAFT
  DRAFT --> PENDING_PAYMENT
  PENDING_PAYMENT --> CONFIRMED: PaymentCaptured
  PENDING_PAYMENT --> CANCELLED: timeout/cancel
  PENDING_PAYMENT --> EXPIRED: pay window elapsed
  CONFIRMED --> EN_ROUTE
  EN_ROUTE --> ARRIVED: GPS/arrival verification
  ARRIVED --> IN_PROGRESS: QR/OTP ok
  IN_PROGRESS --> COMPLETED
  CONFIRMED --> CANCELLED
  COMPLETED --> [*]
```

## 24.4 Side Effects by Transition

| Transition | Side effects |
|------------|--------------|
| → PENDING_PAYMENT | Create payment intent |
| → CONFIRMED | Notify craftsman/store; schedule 24h reminder |
| → ARRIVED | Record verification; notify customer |
| → IN_PROGRESS | Start progress monitoring timers |
| → COMPLETED | Commission calculation event; survey schedule; unlock rating |
| → CANCELLED | Refund policy execution; notifications; release slots |

## 24.5 Scheduling Model (**OPEN**)

Options:

1. Free-text preferred time window  
2. Discrete slot inventory  
3. Immediate “ASAP” jobs  

Decision: Q-BOOK-007.

## 24.6 Assignment Model (**OPEN**)

Options:

1. Customer selects craftsman directly  
2. Platform auto-assigns  
3. Store assigns staff/craftsman  

Decision: Q-BOOK-009, Q-REL-001.

## 24.7 Reminder: 24-hour

Scheduler finds bookings with `scheduled_start` in [now+23h, now+25h] (exact window Q-NTF-004), not yet reminded, status eligible → send notification → mark reminder sent.

## 24.8 Cancellation & Refund

Not invented here. See Q-BOOK-001..006.

## 24.9 Data Integrity Rules

- Status changes only via domain service  
- Every transition appends `booking_status_history`  
- Optimistic locking on booking row  
- Payment capture required before CONFIRMED if prepaid model selected  

## 24.10 Questions Requiring Business Decision

`Q-BOOK-001`..`Q-BOOK-010`, `Q-REL-001`, `Q-NTF-003`, `Q-NTF-004`
