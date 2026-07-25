# ADR-021: Dispute Management Depth (V1)

**Status:** Accepted — 2026-07-24  
**Supersedes:** ADR-017 open choice  

---

## Decision

V1 supports a **lightweight dispute / complaint** process.

**Do not** build a complex arbitration / multi-party tribunal system in V1.

---

## V1 Capabilities

- Customer complaint  
- Provider complaint  
- Admin review  
- Evidence attachment  
- Resolution notes  
- Status tracking  

### Statuses

```text
Open → Under Review → Resolved → Closed
```

---

## Database Impact

| Entity | Purpose |
|--------|---------|
| `disputes` | booking_id, opened_by_role/user, reason, status, resolution_notes, timestamps |
| `dispute_evidence` | dispute_id, media_id, note |
| `dispute_status_history` | append-only |

---

## Impact on Money (Payment / Escrow / Ledger / Settlement)

| Event | Behavior |
|-------|----------|
| Dispute Open / Under Review | May **hold** escrow release / pause settlement eligibility for that booking (flag on booking + ledger hold) |
| Resolved | Admin selects outcome using **refund rules / manual finance actions** (ADR-013) — e.g. full/partial refund, release to provider, split notes |
| Closed | Holds cleared per resolution; ledger entries immutable for whatever was posted |

Settlement jobs **skip** bookings with active dispute holds.

No automatic complex split-engine in V1 — admin applies configured refund/settlement tools with audit.

---

## API / UI

- Customer/Provider: `POST /bookings/{id}/disputes`, upload evidence, view status  
- Admin: queue, review, resolve, notes  

---

## Consequences

ADR-017 closed. V2 may add structured arbitration if needed.
