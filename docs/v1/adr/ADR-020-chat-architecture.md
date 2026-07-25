# ADR-020: Chat Architecture (Booking-Scoped)

**Status:** Accepted — 2026-07-24  
**Supersedes / completes:** ADR-009 (scope), ADR-015 (scoping); transport detail below  

---

## Decision

KHADAMATI V1 uses **booking-scoped chat** only.

**Do not** implement open marketplace messaging (no cold contact between strangers).

---

## Chat Rules

- Chat exists **only** when related to a booking  
- Participants: **Customer ↔ Provider** (craftsman or store assignee)  
- Optional: **Admin access** for support (read / intervene with audit)  

### Benefits
Reduces abuse · ties conversations to transactions · simpler moderation · support workflows · marketplace safety  

---

## Requirements

- Real-time messaging  
- Message history  
- Read status  
- Push notifications  
- Booking reference linking  
- Basic moderation (report / admin review)  

---

## Database Entities

| Entity | Purpose |
|--------|---------|
| `conversations` | 1:1 with `booking_id` (UNIQUE booking_id) |
| `messages` | conversation_id, sender_user_id, body, created_at, deleted_at(soft for hide) |
| `message_read_statuses` | message_id or conversation cursor per user, read_at |
| `message_attachments` | Optional media_id FK (if enabled by feature flag) |
| `chat_reports` | reporter, conversation/message, reason, status |

---

## Transport Approach

| Layer | Choice |
|-------|--------|
| System of record | PostgreSQL messages |
| Real-time | WebSocket (or STOMP over WS) to API gateway/chat endpoint |
| Fallback | Short polling on resume/reconnect |
| Fan-out | Push via notification module on new message |

Abstraction: `ChatRealtimePort` so transport can evolve without domain rewrite.

---

## Storage Approach

- Messages in DB (not only ephemeral broker)  
- Attachments via `media` + object storage  
- Indexes: `(conversation_id, created_at)`, unread queries by read cursors  

---

## Notification Integration

Domain event `ChatMessageCreated` → outbox → notification dispatcher → push + in-app for recipient.

---

## Retention Policy

- Configurable per Market (admin/compliance setting)  
- Default stance: retain while booking financial retention applies; anonymize sender display on account deletion (ADR-022)  
- Admin support access audited  

---

## API (excerpt)

`GET/POST /chat/conversations?bookingId=` · `GET/POST /chat/conversations/{id}/messages` · `POST .../read` · `POST .../report`  
Admin: `GET /admin/chat/conversations/{id}` (support permission)

---

## Consequences

- No global inbox social network  
- Conversation created when booking reaches an eligible status (e.g. ACCEPTED or CONFIRMED — config)  
- ADR-015 transport spike reduced to implementing WebSocket+poll fallback as standard  
