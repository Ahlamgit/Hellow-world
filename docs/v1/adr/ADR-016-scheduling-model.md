# ADR-016: Scheduling Model

## Status
Open — requires Product decision before booking schema freeze.

## Context
Customers must choose date/time. Slot inventory vs free datetime window vs ASAP is undecided (Q-BOOK-007).

## Options
| Option | Pros | Cons |
|--------|------|------|
| A. Free datetime + provider accept/reject | Simple V1 | Overbooking risk |
| B. Provider availability windows + customer pick within window | Balanced | Needs availability UX |
| C. Discrete slot inventory | Strong concurrency control | Heavier build |

## Interim Architecture Stance (non-assuming final choice)
Schema supports `scheduled_start`/`scheduled_end` + `availability_windows`. Slot table optional behind flag. **Product must pick A/B/C** before implementation of booking create validation.

## Decision
**Not assumed.** Tracked as Blocked in readiness report until Product selects option.
