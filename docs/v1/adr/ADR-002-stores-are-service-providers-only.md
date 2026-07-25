# ADR-002: Stores Are Service Providers (No E-Commerce) — Amended

## Status
Accepted — 2026-07-24 · **Amended by ADR-027** (2026-07-24)

## Context
Earlier drafts included full e-commerce (inventory, carts, product orders).

## Decision (Amended)

Stores are:

- **Service providers** (bookable services via Listing)  
- **Service advertisers**  
- **Product catalog advertisers** (promotional content only — see ADR-027)

### Still out of scope (V1 e-commerce)

- Product checkout  
- Shopping cart  
- Online product purchasing / product payment  
- Product delivery workflow  
- Inventory / warehouse management  
- Order fulfillment  

## Consequences
See ADR-027 for promotional catalog entities. No product-order tables.
