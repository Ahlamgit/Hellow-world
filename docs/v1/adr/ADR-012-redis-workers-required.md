# ADR-012: Redis and Background Workers Required

## Status
Accepted — 2026-07-24 (Master Prompt v1.0) + Architecture Audit

## Decision
V1 **requires**:
- Redis (or equivalent) for caching, rate limiting, distributed locks, feature-flag cache  
- Background workers / queue processing for notifications, scheduled tasks, payment reconciliation, long-running IDV jobs  

API nodes must not solely rely on in-process `@Scheduled` without distributed coordination.

## Consequences
Answers Q-DEP-002/003. Updates deployment architecture to include Redis + worker service.
