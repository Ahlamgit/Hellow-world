# ADR-019: Scheduling & Availability Calendar Model

**Status:** Accepted — 2026-07-24  
**Supersedes:** ADR-016 (scheduling model open)

---

## Decision

KHADAMATI uses a **Provider Availability Calendar** model.

Scheduling must **not** rely only on unconstrained customer-selected times. **The provider controls availability.** Customers request bookings only within (or against) that availability; providers confirm; then payment.

---

## Provider Availability Capabilities

Providers define:

- Working days  
- Working hours / available time periods  
- Service areas  
- Holidays  
- Exceptions  
- Temporary unavailable periods  

Example:

```text
Monday:  08:00–12:00, 14:00–18:00
Tuesday: Unavailable
```

---

## Customer Booking Flow

1. Select service  
2. Select provider/listing  
3. **View available provider times**  
4. Request booking time  
5. Provider confirms  
6. Payment required  
7. Service execution  

```text
Customer Request
    ↓
Provider Availability Check
    ↓
Provider Confirmation
    ↓
Payment
    ↓
Service
```

Aligns with ADR-005 (confirm → pay) and extends it with availability enforcement.

---

## Scheduling Requirements

- Booking conflict prevention  
- Calendar updates  
- Provider exceptions  
- Time zones via Market (`Asia/Beirut` default)  
- Future multi-market support  
- Multiple bookings per day  
- Service duration (from listing / booking)  

---

## Database Impact

| Entity | Purpose |
|--------|---------|
| `provider_availability` / working-hours rows | Recurring weekly patterns |
| `working_hours` | Day-of-week + start/end (market-local → stored UTC bounds or local+tz) |
| `calendar_exceptions` | Holidays, time-off, one-off open/close overrides |
| `booking_schedule` | Materialized scheduled interval on booking (start/end, duration) |
| `service_duration` | Duration minutes on listing (or override on booking) |
| `service_areas` | Geo eligibility (existing) |

**Constraints:** Overlapping confirmed bookings for same provider conflict-checked; requests may soft-hold optional (config) — default: conflict checked at confirm.

**Indexes:** `(provider_id, day_of_week)`, `(provider_id, exception_date)`, `(provider_id, scheduled_start)` on bookings.

---

## API Impact

| API | Purpose |
|-----|---------|
| `GET/PUT /providers/me/availability` | Manage recurring hours |
| `CRUD /providers/me/calendar-exceptions` | Exceptions |
| `GET /listings/{id}/availability` | Customer-visible slots/windows |
| `POST /bookings` | Validate requested time against availability + duration |
| `POST /bookings/{id}/accept` | Re-validate no conflict before confirm |

---

## UI Impact

| Surface | Screens |
|---------|---------|
| Craftsman / Store | Availability calendar management, exceptions |
| Customer | Available time selection (not free-form only) |

---

## Consequences

- ADR-016 closed in favor of this model  
- Booking create without availability check is invalid  
- Slot granularity (e.g. 30/60 min) is **admin/listing configurable**, not hardcoded forever  
