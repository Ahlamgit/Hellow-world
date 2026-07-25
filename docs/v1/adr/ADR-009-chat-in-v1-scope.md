# ADR-009: Chat in V1 Scope

## Status
Accepted — 2026-07-24 (Master Prompt v1.0)

## Context
Earlier architecture defaulted chat to V2 (Q-COMMS-001).

## Decision
**Customer Chat** is in V1 scope per Master Prompt.

Implementation approach (to be detailed in API/design):
- Booking-scoped messaging between customer and assigned provider preferred (reduces spam/abuse).
- Push + in-app delivery.
- Moderation/report hooks for admin.

Exact realtime transport (WebSocket vs polling) is an engineering ADR at spike time; product scope includes chat.

## Consequences
Answers Q-COMMS-001 = In V1. Adds module `chat` / messaging to module breakdown.
