# ADR-015: Chat Transport and Scoping

## Status
Accepted product scope; transport choice deferred to spike (pre-implementation).

## Context
Chat is in V1 (ADR-009). Realtime mechanism not chosen.

## Decision
1. **Product:** Booking-scoped conversations between Customer and assigned Provider only (reduces spam).  
2. **Persistence:** `conversations`, `messages` in PostgreSQL (system of record).  
3. **Transport:** Decide via spike between (a) WebSocket/STOMP, (b) SSE + poll fallback. Record result as ADR-015a before coding chat.  
4. **Push:** New message → push + in-app.  
5. **Moderation:** User report → Admin review queue.

## Consequences
Chat UI/API contracts can be designed; implementation waits on transport spike ADR-015a.
