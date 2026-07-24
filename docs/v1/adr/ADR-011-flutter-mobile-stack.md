# ADR-011: Flutter Is Approved Mobile Stack

## Status
Accepted — 2026-07-24

## Context
Master Prompt §17 requires following approved KHADAMATI architecture and “native mobile direction.” Prior fixed stack specified Flutter for Customer and Craftsman apps.

## Decision
Customer and Craftsman V1 apps are **Flutter**.

“Native mobile direction” means dedicated mobile applications (not mobile web wrappers as primary), implemented with the approved Flutter stack — not a return to separate Kotlin/Swift codebases unless a future ADR overturns this.

## Consequences
Clarifies technology choice; legacy Android/iOS folders remain non-authoritative.
