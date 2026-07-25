# ADR-028: Unified Provider Capability Model

**Status:** Accepted — 2026-07-24  
**Related:** ADR-003 (Provider + Listing) · ADR-019 (availability) · ADR-027 (catalog ads) · ADR-005 (booking)  

---

## Decision

KHADAMATI uses a **unified Provider marketplace** with a **capability-based** authorization model.

### Customer experience is **service-first**

Customers must **not** be required to choose Craftsman vs Store before searching.

They search by need (e.g. AC repair, plumbing, solar, painting, appliance repair). The system returns suitable **providers** (and their listings/services).

Provider type may appear as **trust information** only (e.g. “Verified Professional” / “Verified Company”), or as an optional filter — **never** as the mandatory entry point.

### Hierarchy

```text
Provider
  ↓
Provider Type (CRAFTSMAN | STORE | future types)
  ↓
Provider Capabilities (flags / grants)
  ↓
Listings / Services
  ↓
Bookings
```

### Capability model (not type-only)

Capabilities are first-class and evaluated at runtime. Examples:

| Capability | Meaning |
|------------|---------|
| `CanCreateServices` | Create/manage bookable listings |
| `CanAcceptBookings` | Accept/reject and fulfill bookings |
| `CanReceivePayments` | Receive payouts via ledger/withdrawals |
| `CanAdvertiseProducts` | Promotional catalog (ADR-027) |
| `CanManageTeam` | Staff / affiliations |
| `CanCreatePromotions` | Request/manage promotions (within admin rules) |
| `CanManageAvailability` | Availability calendar (ADR-019) |

**Booking engine depends on `CanAcceptBookings`, not on provider type.**  
**Payments/ledger/settlement/withdrawals use unified Provider accounts** — no separate craftsman vs store money pipelines.

Default capability sets may be seeded **by type** (and/or subscription plan) but remain **data-driven** for GCC/franchise evolution without redesign.

---

## Reason

- Avoid forcing customers into supply-side taxonomy  
- Enable craftsman→company evolution, store technicians, franchises, market variations  
- Prevent duplicate booking/payment engines per type  
- Align search with service intent  

---

## Database Impact

| Entity / field | Purpose |
|----------------|---------|
| `providers` | Unified root (`id`, `type`, `status`, `market_id`, verification flags) |
| `provider_capabilities` | `provider_id`, `capability_code`, `enabled`, optional `source` (TYPE_DEFAULT / SUBSCRIPTION / ADMIN_OVERRIDE) |
| `capability_definitions` | Catalog of capability codes |
| `listings` / services | Belong to `provider_id`; bookable if provider has `CanCreateServices` + listing active |
| `catalog_items` | Only if `CanAdvertiseProducts` |
| Ledger accounts | Owner = `provider_id` (unified) |

**No** separate `craftsman_bookings` / `store_bookings` tables.  
**No** separate payment flows by type.

Indexes: listings search by category/geo/market; join providers + capabilities for filters.

---

## API Impact

### Customer (service-first)

| API | Notes |
|-----|-------|
| `GET /categories`, `GET /listings` | Primary discovery — **no required providerType** |
| `GET /listings?q=&category=&near=&availableFrom=` | Service/need search |
| Optional `providerType` filter | Non-mandatory |
| `GET /providers/{id}` | Trust badges from type + verification |
| Booking APIs | Target `provider_id` / `listing_id`; server checks `CanAcceptBookings` |

### Provider apps / store

Capability-gated endpoints (403 `CAPABILITY_DENIED` if missing).

### Admin

Grant/revoke capability overrides; manage type defaults and subscription entitlement → capability mapping.

---

## UI/UX Impact

### Customer

```text
Search Service → View Providers → Compare
(rating, reviews, availability, distance, price, verification)
→ Select Provider → Select Service → Book → Confirm → Pay → Completion
```

- No mandatory Craftsman/Store chooser  
- Trust labels: Individual = “Verified Professional”; Company = “Verified Company”  

### Craftsman / Store

- Capability-driven menus (hide catalog if `CanAdvertiseProducts` false)  
- Shared booking UX patterns  

---

## Search Architecture

Primary dimensions: **Service / Category / Location / Availability / Rating / Capabilities** (and price).  
Provider type = optional facet only.

---

## Payment & Ledger

Single path: Payment → Escrow → Ledger (provider payable) → Commission → Withdrawal / Settlement.  
Owner key = `provider_id`.

---

## Store Confirmation (unchanged e-commerce ban)

Stores: service providers + service advertisers + product catalog advertisers.  
Catalog promotional only — no cart/checkout/product pay/orders/inventory/delivery.

---

## Future Scalability Impact

New provider types or hybrid roles = new type defaults + capability grants, not new booking/payment modules.

---

## Consequences

- Supersedes any UX that forces type-first navigation  
- Extends ADR-003 with explicit capabilities table  
- FTM/Master Prompt/search/booking authz updated accordingly  
- Implementation still **not authorized** (gate B) until design/vendors/sign-off  
