# ADR-001: Lebanon as Default Market (Not Hardcoded)

## Status
Accepted — 2026-07-24 (Master Prompt v1.0)

## Context
KHADAMATI launches in Lebanon but must expand to GCC/international without redesign.

## Decision
- Introduce first-class `Market` configuration.
- V1 default market: Lebanon; currency USD; phone +961; locales ar-RTL + en-LTR; timezone Asia/Beirut.
- Application code reads market config; no Lebanon-only conditionals in domain logic.

## Consequences
Answers Q-LOC-001/002/003. Enables multi-country expansion.
