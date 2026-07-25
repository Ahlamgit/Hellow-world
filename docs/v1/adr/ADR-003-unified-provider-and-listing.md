# ADR-003: Unified Provider and Listing

## Status
Accepted — 2026-07-24 (Master Prompt v1.0) + Architecture Audit

## Context
Separating craftsman vs store marketplace logic causes duplication and search inconsistency.

## Decision
```text
Provider
  ├── CraftsmanProfile
  └── StoreProfile

Listing  → bookable service offering bound to a Provider
```

Search/browse operates on Listings (category, location, availability, rating, subscription status).

Staff/craftsman affiliation to stores is supported via provider affiliation.

## Consequences
Answers Q-REL-001/002 directionally. Schema centers on `providers`, profile extensions, `listings`.
