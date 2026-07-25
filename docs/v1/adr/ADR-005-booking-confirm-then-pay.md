# ADR-005: Booking Confirm-Then-Pay

## Status
Accepted — 2026-07-24 (Master Prompt v1.0)

## Context
Earlier drafts considered mandatory prepayment before provider confirmation.

## Decision
Canonical V1 booking money sequence:

```text
Booking request → Provider confirmation (accept) → Payment → Service execution → …
```

Provider may accept/reject before payment capture.

Exact cancellation/refund windows remain open (Q-BOOK-001..004) but must not contradict this sequence without a new ADR.

## Consequences
Answers Q-BOOK-008 (prepay-before-confirm = No). Answers Q-BOOK-009 (customer selects provider). Payment intent created after accept (or at accept).
