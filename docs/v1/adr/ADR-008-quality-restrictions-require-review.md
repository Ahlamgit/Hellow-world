# ADR-008: Quality Restrictions Require Admin Review

## Status
Accepted — 2026-07-24 (Master Prompt v1.0)

## Decision
Restriction pipeline: warn → flag → restrict visibility if necessary → **admin review**.

System must **not** automatically permanently block a provider without human review.

## Consequences
Answers Q-QUA-003 direction. Auto-permanent-ban is forbidden; temporary visibility restriction may be automated only if still requiring admin review for permanence.
