# Phase 1A — Risk Assessment

## Summary

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Concurrent booking double-book | Medium | High | RowVersion + slot unique index + 409 responses |
| Coupon over-redemption | Medium | Medium | Atomic `ExecuteUpdate` with usage guard |
| Migration index rebuild on large tables | Low | Medium | Indexes are additive; no table rewrite |
| Filtered coupon index — duplicate active codes | Low | High | Pre-migration validation query |
| Client confusion on 409 responses | Low | Low | Messages are user-facing; existing middleware |
| In-memory tests miss SQL concurrency | Medium | Low | Integration tests on SQL Server; unit tests cover logic |

## Technical risks

### RowVersion on existing rows

SQL Server assigns `rowversion` automatically. No manual backfill. Risk: **Low**.

### Optimistic concurrency user experience

Users may see "refresh and try again" on rare concurrent edits. Acceptable for Phase 1A; preferable to silent overwrites.

### Coupon code index change

Dropping and recreating the coupon code index requires no duplicate active codes. Seeded environments are clean; production should run validation query before deploy.

### Slot reservation race window

A race remains between `IsSlotBookedAsync` check and `SaveChanges` until the unique index rejects the second insert. The index is the authoritative guard; application maps violations to 409.

## Deployment risks

- **No production secret changes** in Phase 1A.
- **No API contract breaks**.
- **Backward compatible** with existing web and mobile clients.

## Residual risk after 1A

- Subscription coupon validation at subscribe time only (not at payment webhook) — acceptable; coupons are subscription-scoped.
- Global platform hardcoding (SAR/SA defaults) addressed in Phase 1D, not 1A.
