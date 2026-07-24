# ADR-002: Stores Are Service Providers Only

## Status
Accepted — 2026-07-24 (Master Prompt v1.0)

## Context
Earlier drafts included products, inventory, carts, and product orders for stores.

## Decision
Stores **do not** sell products in V1 (or as current product definition).

**Out of scope:** product inventory, product catalog, shopping cart, product ordering, product sales.

Stores offer **services** via the unified Listing model, manage bookings, affiliations, promotions, and analytics.

## Consequences
Removes store commerce module depth. Answers Q-PRD-001 / Q-PAY-001 (no product orders). Update FRS/scope/API/ERD accordingly. Any future product commerce requires a new ADR.
